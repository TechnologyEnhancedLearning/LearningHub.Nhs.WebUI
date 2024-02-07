// <copyright file="RoleRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The role repository.
    /// </summary>
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public RoleRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <inheritdoc />
        public async Task<Role> GetByIdAsync(int id)
        {
            return await this.DbContext.Role.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
        }

        /// <inheritdoc />
        public async Task<Role> GetByIdAsync(int id, bool includePermissions)
        {
            if (includePermissions)
            {
                return await this.DbContext.Role
                    .Where(r => !r.Deleted)
                    .Include(r => r.PermissionRole)
                    .ThenInclude(pr => pr.Permission)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
            }
            else
            {
                return await this.DbContext.Role.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
            }
        }

        /// <inheritdoc />
        public async Task<List<Role>> GetAvailableForUserGroupAsync(int userGroupId)
        {
            var result = from role in this.DbContext.Role.Where(r => !r.Deleted)
                         join rug in this.DbContext.RoleUserGroup.Where(rug => rug.UserGroupId == userGroupId && !rug.Deleted)
                             on role.Id
                             equals rug.RoleId
                             into rolesWithUserGroup
                         from r in rolesWithUserGroup.DefaultIfEmpty()
                         where r == null
                         select role;

            return await result.ToListAsync();
        }

        /// <inheritdoc />
        public override async Task<int> CreateAsync(int userId, Role role)
        {
            foreach (var pr in role.PermissionRole)
            {
                pr.Permission = null;
                this.SetAuditFieldsForCreate(userId, pr);
            }

            return await base.CreateAsync(userId, role);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(int userId, Role role, bool updatePermissions)
        {
            var amendDate = DateTimeOffset.Now;
            if (updatePermissions)
            {
                var roleUpdate = this.DbContext.Role
                    .Where(r => r.Id == role.Id)
                    .Include(r => r.PermissionRole)
                    .SingleOrDefault();

                if (roleUpdate != null)
                {
                    // Update Role
                    this.DbContext.Entry(roleUpdate).CurrentValues.SetValues(role);
                    this.SetAuditFieldsForUpdate(userId, roleUpdate);

                    if (updatePermissions)
                    {
                        // Delete PermissionRole
                        foreach (var existingChild in roleUpdate.PermissionRole.Where(pr => !pr.Deleted))
                        {
                            var permissionRole = role.PermissionRole
                                                        .Where(pr => pr.Equals(existingChild) && !pr.Deleted)
                                                        .SingleOrDefault();
                            if (permissionRole == null)
                            {
                                this.SetAuditFieldsForDelete(userId, existingChild);
                            }
                        }

                        // Insert PermissionRole
                        foreach (var permissionRole in role.PermissionRole)
                        {
                            var existingPermissionRole = roleUpdate.PermissionRole
                                .Where(pr => pr.Equals(permissionRole)
                                            && !pr.Deleted)
                                .SingleOrDefault();

                            if (existingPermissionRole == null)
                            {
                                var newPermissionRole = new PermissionRole()
                                {
                                    RoleId = permissionRole.RoleId,
                                    PermissionId = permissionRole.PermissionId,
                                };
                                this.SetAuditFieldsForCreate(userId, newPermissionRole);
                                roleUpdate.PermissionRole.Add(newPermissionRole);
                            }
                        }
                    }
                }
            }
            else
            {
                // Simple update - no child collection
                var roleUpdate = this.DbContext.Role
                            .Where(r => r.Id == role.Id)
                            .SingleOrDefault();

                if (roleUpdate != null)
                {
                    // Update Role
                    this.DbContext.Entry(roleUpdate).CurrentValues.SetValues(role);
                    this.SetAuditFieldsForUpdate(userId, roleUpdate);
                }
            }

            await this.DbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int userId, int roleId)
        {
            var amendDate = DateTimeOffset.Now;

            var roleUpdate = this.DbContext.Role
                    .Where(r => r.Id == roleId)
                    .Include(r => r.PermissionRole)
                    .SingleOrDefault();

            this.SetAuditFieldsForDelete(userId, roleUpdate);

            foreach (var pr in roleUpdate.PermissionRole)
            {
                this.SetAuditFieldsForDelete(userId, pr);
            }

            await this.DbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public List<Role> Search(string searchText, int page, int pageSize, out int pagesReturned)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = page };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = pageSize };
            var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = searchText };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var roles = this.DbContext.Role.FromSqlRaw("hub.RoleSearch @p0, @p1, @p2, @p3 output", param0, param1, param2, param3).ToList();

            pagesReturned = (int)param3.Value;

            return roles;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            return await this.DbContext.Role
                    .Include(r => r.PermissionRole)
                    .ThenInclude(p => p.Permission)
                    .ToListAsync();
        }
    }
}
