using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Entities.Hierarchy;
using LearningHub.Nhs.OpenApi.Models.Exceptions;
using LearningHub.Nhs.OpenApi.Models.ServiceModels.Role;
using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    /// <summary>
    /// The role repository.
    /// </summary>
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager) { }

        public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct) =>
            await DbContext.Set<Role>()
                .AsNoTracking()
                .Where(x => x.RemoveDate == null)
                .OrderBy(x => x.Name)
                .ToListAsync(ct);

        public Task<Role?> GetByIdAsync(int id, CancellationToken ct) =>
            DbContext.Set<Role>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.RemoveDate == null, ct);

        public Task<Role?> GetTrackedByIdAsync(int id, CancellationToken ct) =>
            DbContext.Set<Role>()
                .FirstOrDefaultAsync(x => x.Id == id && x.RemoveDate == null, ct);

        public Task<bool> CodeExistsAsync(string normalizedCode, CancellationToken ct) =>
            DbContext.Set<Role>().AnyAsync(x =>
                x.RemoveDate == null && x.Code != null &&
                x.Code.ToLower() == normalizedCode, ct);

        public Task<bool> UserExistsAsync(int userId, CancellationToken ct) =>
            DbContext.Set<User>().AnyAsync(x => x.Id == userId, ct);

        public async Task<bool> ScopeTargetExistsAsync(RoleScope scope, CancellationToken ct)
        {
            if (scope.OrganisationId is int orgId)
                return await DbContext.Set<Nhs.Models.Entities.Organisation>().AnyAsync(x => x.Id == orgId, ct);

            if (scope.CatalogueId is int catalogueId)
                return await DbContext.Set<CatalogueNodeVersion>()
                    .AnyAsync(x => x.Id == catalogueId, ct);

            if (scope.CategoryId is int categoryId)
                return await DbContext.Set<CatalogueNodeVersionCategory>()
                    .AnyAsync(x => x.Id == categoryId, ct);

            return true; // no scope IDs = system-wide
        }

        public async Task<Role> CreateUniqueAsync(Role entity, CancellationToken ct)
        {
            // Serializable keeps concurrent callers from creating two active roles with the same code.
            await using var tx = await DbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var code = entity.Code!.ToLowerInvariant();
            if (await CodeExistsAsync(code, ct))
                throw new HttpResponseException($"An active role with code '{code}' already exists.", HttpStatusCode.Conflict);

            await DbContext.Set<Role>().AddAsync(entity, ct);
            await DbContext.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return entity;
        }

        public async Task SaveChangesAsync(CancellationToken ct) =>
            await DbContext.SaveChangesAsync(ct);

        public async Task<T> InSerializableTransactionAsync<T>(Func<Task<T>> operation, CancellationToken ct)
        {
            await using var tx = await DbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var value = await operation();
            await tx.CommitAsync(ct);
            return value;
        }
    }
}
