using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Entities.Hierarchy;
using LearningHub.Nhs.Models.Role;
using LearningHub.Nhs.OpenApi.Models.Exceptions;
using LearningHub.Nhs.OpenApi.Models.ServiceModels.Role;
using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
using LearningHub.Nhs.OpenApi.Repositories.Helpers;
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
    /// <inheritdoc/>
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        /// <inheritdoc/>
        public UserRoleRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager) { }

        private IQueryable<UserRoleViewModel> ActiveAssignments() =>
            from ur in DbContext.Set<UserRole>().AsNoTracking()
            join role in DbContext.Set<Role>().AsNoTracking() on ur.RoleId equals role.Id
            where ur.RemoveDate == null && role.RemoveDate == null
            select new UserRoleViewModel
            {
                Id = ur.Id,
                UserId = ur.UserId,
                RoleId = ur.RoleId,
                RoleCode = role.Code,
                RoleName = role.Name,
                RoleScopeType = role.ScopeType,
                ScopeOrganisationId = ur.ScopeOrganisationId,
                ScopeCatalogueId = ur.ScopeCatalogueId,
                ScopeCategoryId = ur.ScopeCategoryId,
                ScopeSelfAssessmentId = ur.ScopeSelfAssessmentId,
                CreateDate = ur.CreateDate,
                AmendDate = ur.AmendDate
            };

        /// <inheritdoc/>
        public async Task<IReadOnlyList<UserRoleViewModel>> GetByUserIdAsync(int userId, CancellationToken ct) =>
            await ActiveAssignments().Where(x => x.UserId == userId)
                .OrderBy(x => x.RoleName).ThenBy(x => x.Id).ToListAsync(ct);
        /// <inheritdoc/>
        public async Task<IReadOnlyList<UserRoleViewModel>> GetByRoleIdAsync(int roleId, CancellationToken ct) =>
            await ActiveAssignments().Where(x => x.RoleId == roleId)
                .OrderBy(x => x.UserId).ThenBy(x => x.Id).ToListAsync(ct);
        /// <inheritdoc/>
        public Task<UserRoleViewModel?> GetByIdAsync(int userId, int assignmentId, CancellationToken ct) =>
            ActiveAssignments().FirstOrDefaultAsync(
                x => x.UserId == userId && x.Id == assignmentId, ct);
        /// <inheritdoc/>
        public async Task<IReadOnlyList<UserRole>> GetActiveByRoleIdAsync(int roleId, CancellationToken ct) =>
            await DbContext.Set<UserRole>().AsNoTracking()
                .Where(x => x.RoleId == roleId && x.RemoveDate == null).ToListAsync(ct);
        /// <inheritdoc/>
        public Task<bool> HasActiveAssignmentsAsync(int roleId, CancellationToken ct) =>
            DbContext.Set<UserRole>().AnyAsync(
                x => x.RoleId == roleId && x.RemoveDate == null, ct);

        private Task<bool> DuplicateExistsAsync(UserRole candidate, int? excludeId, CancellationToken ct) =>
            DbContext.Set<UserRole>().AnyAsync(x =>
                x.RemoveDate == null &&
                x.UserId == candidate.UserId &&
                x.RoleId == candidate.RoleId &&
                x.ScopeOrganisationId == candidate.ScopeOrganisationId &&
                x.ScopeCatalogueId == candidate.ScopeCatalogueId &&
                x.ScopeCategoryId == candidate.ScopeCategoryId &&
                x.ScopeSelfAssessmentId == candidate.ScopeSelfAssessmentId &&
                (!excludeId.HasValue || x.Id != excludeId.Value), ct);
        /// <inheritdoc/>
        public async Task<UserRole> CreateUniqueAsync(UserRole entity, CancellationToken ct)
        {
            await using var tx = await DbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            // Recheck the role INSIDE the transaction, to avoid assigning a concurrently
            // deleted or scope-changed role after service validation.
            var role = await DbContext.Set<Role>().AsNoTracking().FirstOrDefaultAsync(
                r => r.Id == entity.RoleId && r.RemoveDate == null, ct)
                ?? throw new HttpResponseException("Role is no longer active.", HttpStatusCode.Conflict);
            RoleScopeRules.Validate(role.ScopeType, RoleScope.From(entity));

            if (await DuplicateExistsAsync(entity, excludeId: null, ct))
               throw new HttpResponseException("This user already has that role at the specified scope.", HttpStatusCode.Conflict);

            await DbContext.Set<UserRole>().AddAsync(entity, ct);
            await DbContext.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<UserRole> UpdateScopeUniqueAsync(
            int userId, int assignmentId, RoleScope scope, int actorId, CancellationToken ct)
        {
            await using var tx = await DbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var entity = await DbContext.Set<UserRole>().FirstOrDefaultAsync(
                x => x.Id == assignmentId && x.UserId == userId && x.RemoveDate == null, ct)
                ?? throw new HttpResponseException("User role assignment was not found.", HttpStatusCode.NotFound);

            var role = await DbContext.Set<Role>().AsNoTracking().FirstOrDefaultAsync(
                r => r.Id == entity.RoleId && r.RemoveDate == null, ct)
                ?? throw new HttpResponseException("Role is no longer active.", HttpStatusCode.Conflict);

            // Use a candidate rather than modifying the tracked entity before duplicate validation.
            var candidate = new UserRole
            {
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                ScopeOrganisationId = scope.OrganisationId,
                ScopeCatalogueId = scope.CatalogueId,
                ScopeCategoryId = scope.CategoryId,
                ScopeSelfAssessmentId = scope.SelfAssessmentId
            };

            RoleScopeRules.Validate(role.ScopeType, scope);
            if (await DuplicateExistsAsync(candidate, assignmentId, ct))
                throw new HttpResponseException("This user already has that role at the specified scope.", HttpStatusCode.Conflict);

            entity.ScopeOrganisationId = scope.OrganisationId;
            entity.ScopeCatalogueId = scope.CatalogueId;
            entity.ScopeCategoryId = scope.CategoryId;
            entity.ScopeSelfAssessmentId = scope.SelfAssessmentId;
            entity.AmendDate = DateTimeOffset.UtcNow;
            entity.AmendUserId = actorId;

            await DbContext.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> SoftDeleteAsync(int userId, int assignmentId, int actorId, CancellationToken ct)
        {
            var entity = await DbContext.Set<UserRole>().FirstOrDefaultAsync(
                x => x.Id == assignmentId && x.UserId == userId && x.RemoveDate == null, ct);
            if (entity is null) return false;

            entity.RemoveDate = DateTimeOffset.UtcNow;
            entity.RemoveUserId = actorId;
            await DbContext.SaveChangesAsync(ct);
            return true;
        }
    }
}
