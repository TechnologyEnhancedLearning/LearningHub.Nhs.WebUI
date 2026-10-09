namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The role user group repository.
    /// </summary>
    public class RoleUserGroupRepository : GenericRepository<RoleUserGroup>, IRoleUserGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleUserGroupRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public RoleUserGroupRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<RoleUserGroup> GetByIdAsync(int id)
        {
            return await this.DbContext.RoleUserGroup
                                        .Include(n => n.UserGroup).ThenInclude(u => u.UserGroupAttribute)
                                        .Include(n => n.Role)
                                        .Include(n => n.Scope).AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// The get by catalogueNodeId async.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="userGroupId">The userGroup id.</param>
        /// <param name="scopeId">The scope id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<RoleUserGroup> GetByRoleIdUserGroupIdScopeIdAsync(int roleId, int userGroupId, int scopeId)
        {
            return await this.DbContext.RoleUserGroup.AsNoTracking().FirstOrDefaultAsync(n => n.RoleId == roleId && n.UserGroupId == userGroupId && n.ScopeId == scopeId);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public new IQueryable<RoleUserGroup> GetAll()
        {
            return this.DbContext.Set<RoleUserGroup>()
                .Include(n => n.UserGroup).ThenInclude(u => u.UserGroupAttribute)
                .Include(n => n.Role)
                .Include(n => n.Scope)
                .Where(n => n.Deleted == false);
        }

        /// <summary>
        /// The get all for Search.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<RoleUserGroup>> GetAllforSearch(int catalogueNodeId, int userId)
        {
            return await this.DbContext.RoleUserGroup.Where(rug => rug.Scope.CatalogueNodeId == catalogueNodeId)
                                        .Include(n => n.UserGroup).ThenInclude(u => u.UserUserGroup.Where(p => p.UserId == userId))
                                        .Include(n => n.Scope).AsNoTracking()
                                        .ToListAsync();
        }

        ///// <summary>
        ///// The get by role id and catalogue id.
        ///// </summary>
        ///// <param name="roleId">The role id.</param>
        ///// <param name="catalogueNodeId">The catalogue node id.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //public async Task<List<RoleUserGroup>> GetByRoleIdCatalogueId(int roleId, int catalogueNodeId)
        //{
        //    return await this.DbContext.RoleUserGroup.Where(rug => rug.RoleId == roleId && rug.Scope.CatalogueNodeId == catalogueNodeId)
        //                                .Include(n => n.UserGroup).ThenInclude(u => u.UserGroupAttribute)
        //                                .Include(n => n.Role)
        //                                .Include(n => n.Scope).AsNoTracking().OrderByDescending(rug => rug.RoleId).ToListAsync();
        //}

        /// <summary>
        /// Retrieves all active role assignments for a specified role and catalogue node.
        /// </summary>
        /// <param name="roleId">The identifier of the role.</param>
        /// <param name="catalogueNodeId">The identifier of the catalogue node.</param>
        /// <returns>A collection of matching role-user-group assignments.</returns>
        public async Task<List<RoleUserGroup>> GetByRoleIdCatalogueId(int roleId, int catalogueNodeId)
        {
            return await DbContext.RoleUserGroup
                .Where(
                    rug =>
                        rug.RoleId == roleId &&
                        !rug.Deleted &&
                        !rug.UserGroup.Deleted &&
                        rug.Scope.CatalogueNodeId == catalogueNodeId)
                .Include(x => x.UserGroup)
                    .ThenInclude(x => x.UserGroupAttribute)
                .Include(x => x.Role)
                .Include(x => x.Scope)
                .AsNoTracking()
                .OrderByDescending(x => x.RoleId)
                .ToListAsync();
        }

        /// <summary>
        /// The get by role id and catalogue id that has users.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="catalogueNodeId">The catalogue node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<RoleUserGroup>> GetByRoleIdCatalogueIdWithUsers(int roleId, int catalogueNodeId)
        {
            return await this.DbContext.RoleUserGroup.Where(rug => rug.RoleId == roleId && rug.Scope.CatalogueNodeId == catalogueNodeId)
                                        .Include(n => n.UserGroup).ThenInclude(u => u.UserUserGroup).Where(u => u.UserGroup != null && u.UserGroup.UserUserGroup.Count() > 0)
                                        .Include(n => n.Role)
                                        .Include(n => n.Scope).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Get list of RoleUserGroupViewModel for a supplied User Group.
        /// </summary>
        /// <param name="userGroupId">The userGroupId.</param>
        /// <returns>A list of RoleUserGroupViewModel.</returns>
        public async Task<List<RoleUserGroupViewModel>> GetRoleUserGroupViewModelsByUserGroupId(int userGroupId)
        {
            var param0 = new SqlParameter("@userGroupId", SqlDbType.Int) { Value = userGroupId };

            var vm = await this.DbContext.RoleUserGroupViewModel.FromSqlRaw("hub.RoleUserGroupGetByUserGroupId @userGroupId", param0).AsNoTracking().ToListAsync();
            return vm;
        }

        /// <summary>
        /// Get list of RoleUserGroupViewModel for a supplied User Group.
        /// </summary>
        /// <param name="userId">The userGroupId.</param>
        /// <returns>A list of RoleUserGroupViewModel.</returns>
        public async Task<List<RoleUserGroupViewModel>> GetRoleUserGroupViewModelsByUserId(int userId)
        {
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };

            var vm = await this.DbContext.RoleUserGroupViewModel.FromSqlRaw("hub.RoleUserGroupGetByUserId @userId", param0).AsNoTracking().ToListAsync();
            return vm;
        }

        /// <summary>
        /// Retrieves an active role-to-user-group assignment for the specified role, user group, and scope.
        /// </summary>
        /// <param name="roleId">The identifier of the role.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <param name="scopeId">The identifier of the scope.</param>
        /// <returns>The matching active role-user-group assignment if found; otherwise, <c>null</c>.</returns>
        public async Task<RoleUserGroup> GetActiveAsync(int roleId, int userGroupId, int scopeId)
        {
            return await DbContext.RoleUserGroup
                .Include(x => x.UserGroup)
                    .ThenInclude(x => x.UserGroupAttribute)
                .Include(x => x.Role)
                .Include(x => x.Scope)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.RoleId == roleId &&
                         x.UserGroupId == userGroupId &&
                         x.ScopeId == scopeId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted);
        }

        /// <summary>
        /// Retrieves a role-to-user-group assignment for update operations, including records that have been deleted.
        /// </summary>
        /// <param name="roleId">The identifier of the role.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <param name="scopeId">The identifier of the scope.</param>
        /// <returns>The most recent matching role-user-group assignment if found; otherwise, <c>null</c>.</returns>
        public async Task<RoleUserGroup> GetIncludingDeletedForUpdateAsync(int roleId, int userGroupId, int scopeId)
        {
            return await DbContext.RoleUserGroup
                .Include(x => x.UserGroup)
                    .ThenInclude(x => x.UserGroupAttribute)
                .Include(x => x.Role)
                .Include(x => x.Scope)
                .Where(
                    x => x.RoleId == roleId &&
                         x.UserGroupId == userGroupId &&
                         x.ScopeId == scopeId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active catalogue-based role assignments associated with a specified user group.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>A collection of active role-user-group assignments.</returns>
        public async Task<List<RoleUserGroup>> GetActiveByUserGroupIdAsync(int userGroupId)
        {
            return await DbContext.RoleUserGroup
                .Include(x => x.UserGroup)
                    .ThenInclude(x => x.UserGroupAttribute)
                .Include(x => x.Role)
                .Include(x => x.Scope)
                .AsNoTracking()
                .Where(
                    x => x.UserGroupId == userGroupId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted &&
                         x.Scope.ScopeType == ScopeTypeEnum.Catalogue &&
                         x.Scope.CatalogueNodeId.HasValue)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all active role assignments for a specified role and catalogue node.
        /// </summary>
        /// <param name="roleId">The identifier of the role.</param>
        /// <param name="catalogueNodeId">The identifier of the catalogue node.</param>
        /// <returns>A collection of active role-user-group assignments.</returns>
        public async Task<List<RoleUserGroup>> GetActiveByRoleIdCatalogueIdAsync(int roleId, int catalogueNodeId)
        {
            return await DbContext.RoleUserGroup
                .Include(x => x.UserGroup)
                    .ThenInclude(x => x.UserGroupAttribute)
                .Include(x => x.Role)
                .Include(x => x.Scope)
                .AsNoTracking()
                .Where(
                    x => x.RoleId == roleId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted &&
                         x.Scope.ScopeType == ScopeTypeEnum.Catalogue &&
                         x.Scope.CatalogueNodeId == catalogueNodeId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all active role assignments for a specified role and catalogue node.
        /// </summary>
        /// <param name="roleId">The identifier of the role.</param>
        /// <param name="catalogueNodeId">The identifier of the catalogue node.</param>
        /// <returns>A collection of matching role-user-group assignments.</returns>
        //public async Task<List<RoleUserGroup>> GetByRoleIdCatalogueId(int roleId, int catalogueNodeId)
        //{
        //    return await DbContext.RoleUserGroup
        //        .Where(
        //            rug =>
        //                rug.RoleId == roleId &&
        //                !rug.Deleted &&
        //                !rug.UserGroup.Deleted &&
        //                rug.Scope.CatalogueNodeId == catalogueNodeId)
        //        .Include(x => x.UserGroup)
        //            .ThenInclude(x => x.UserGroupAttribute)
        //        .Include(x => x.Role)
        //        .Include(x => x.Scope)
        //        .AsNoTracking()
        //        .OrderByDescending(x => x.RoleId)
        //        .ToListAsync();
        //}


    }
}