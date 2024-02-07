// <copyright file="RoleUserGroupRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

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
        /// The get by role id and catalogue id.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="catalogueNodeId">The catalogue node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<RoleUserGroup>> GetByRoleIdCatalogueId(int roleId, int catalogueNodeId)
        {
            return await this.DbContext.RoleUserGroup.Where(rug => rug.RoleId == roleId && rug.Scope.CatalogueNodeId == catalogueNodeId)
                                        .Include(n => n.UserGroup).ThenInclude(u => u.UserGroupAttribute)
                                        .Include(n => n.Role)
                                        .Include(n => n.Scope).AsNoTracking().OrderByDescending(rug => rug.RoleId).ToListAsync();
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
                                        .Include(n => n.UserGroup).ThenInclude(u => u.UserUserGroup).Where(u => (u.UserGroup != null && u.UserGroup.UserUserGroup.Count() > 0))
                                        .Include(n => n.Role)
                                        .Include(n => n.Scope).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Get list of RoleUserGroupViewModel for a supplied User Group.
        /// </summary>
        /// <param name="userGroupId">The userGroupId.</param>
        /// <returns>A list of RoleUserGroupViewModel.</returns>
        public List<RoleUserGroupViewModel> GetRoleUserGroupViewModelsByUserGroupId(int userGroupId)
        {
            var param0 = new SqlParameter("@userGroupId", SqlDbType.Int) { Value = userGroupId };

            var vm = this.DbContext.RoleUserGroupViewModel.FromSqlRaw("hub.RoleUserGroupGetByUserGroupId @userGroupId", param0).ToList();
            return vm;
        }

        /// <summary>
        /// Get list of RoleUserGroupViewModel for a supplied User Group.
        /// </summary>
        /// <param name="userId">The userGroupId.</param>
        /// <returns>A list of RoleUserGroupViewModel.</returns>
        public List<RoleUserGroupViewModel> GetRoleUserGroupViewModelsByUserId(int userId)
        {
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };

            var vm = this.DbContext.RoleUserGroupViewModel.FromSqlRaw("hub.RoleUserGroupGetByUserId @userId", param0).ToList();
            return vm;
        }
    }
}