// <copyright file="PermissionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The permission repository.
    /// </summary>
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public PermissionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Permission> GetByIdAsync(int id)
        {
            return await this.DbContext.Permission.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Permission> GetByIdAsync(int id, bool includeRoles)
        {
            if (includeRoles)
            {
                return await this.DbContext.Permission
                    .Where(p => !p.Deleted)
                    .Include(pr => pr.PermissionRole)
                    .ThenInclude(pr => pr.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
            }
            else
            {
                return await this.DbContext.Permission.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
            }
        }

        /// <summary>
        /// The get by name async.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Permission> GetByNameAsync(string name)
        {
            return await this.DbContext.Permission.AsNoTracking().FirstOrDefaultAsync(n => n.Name == name && !n.Deleted);
        }

        /// <summary>
        /// The get available for role async.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<Permission>> GetAvailableForRoleAsync(int roleId)
        {
            var result = from permission in this.DbContext.Permission.Where(p => !p.Deleted)
                         join pr in this.DbContext.PermissionRole.Where(pr1 => pr1.RoleId == roleId && !pr1.Deleted)
                             on permission.Id
                             equals pr.PermissionId
                             into permissionsWithRole
                         from p in permissionsWithRole.DefaultIfEmpty()
                         where p == null
                         select permission;

            return await result.ToListAsync();
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="permissionId">The permission id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int userId, int permissionId)
        {
            var permissionUpdate = this.DbContext.Permission
                    .Where(p => p.Id == permissionId)
                    .Include(pr => pr.PermissionRole)
                    .SingleOrDefault();

            this.SetAuditFieldsForDelete(userId, permissionUpdate);

            foreach (var pr in permissionUpdate.PermissionRole)
            {
                this.SetAuditFieldsForDelete(userId, pr);
            }

            await this.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="pagesReturned">The pages returned.</param>
        /// <returns>The Permission list.</returns>
        public List<Permission> Search(string searchText, int page, int pageSize, out int pagesReturned)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = page };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = pageSize };
            var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = searchText };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var permissions = this.DbContext.Permission.FromSqlRaw("hub.PermissionSearch @p0, @p1, @p2, @p3 output", param0, param1, param2, param3).ToList();

            pagesReturned = (int)param3.Value;

            return permissions;
        }
    }
}
