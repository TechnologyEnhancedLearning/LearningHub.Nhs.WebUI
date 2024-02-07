// <copyright file="IRoleUserGroupRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.User;

    /// <summary>
    /// The IRoleUserGroupRepository interface.
    /// </summary>
    public interface IRoleUserGroupRepository : IGenericRepository<RoleUserGroup>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<RoleUserGroup> GetByIdAsync(int id);

        /// <summary>
        /// The get by catalogueNodeId async.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="userGroupId">The userGroup id.</param>
        /// <param name="scopeId">The scope id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<RoleUserGroup> GetByRoleIdUserGroupIdScopeIdAsync(int roleId, int userGroupId, int scopeId);

        /// <summary>
        /// The get by role id and catalogue id.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="catalogueNodeId">The catalogue node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<RoleUserGroup>> GetByRoleIdCatalogueId(int roleId, int catalogueNodeId);

        /// <summary>
        /// The get by role id and catalogue id that has users.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="catalogueNodeId">The catalogue node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<RoleUserGroup>> GetByRoleIdCatalogueIdWithUsers(int roleId, int catalogueNodeId);

        /// <summary>
        /// Get list of RoleUserGroupViewModel for a supplied User Group.
        /// </summary>
        /// <param name="userGroupId">The userGroupId.</param>
        /// <returns>A list of RoleUserGroupViewModel.</returns>
        List<RoleUserGroupViewModel> GetRoleUserGroupViewModelsByUserGroupId(int userGroupId);

        /// <summary>
        /// Get list of RoleUserGroupViewModel for a supplied User Group.
        /// </summary>
        /// <param name="userId">The userGroupId.</param>
        /// <returns>A list of RoleUserGroupViewModel.</returns>
        List<RoleUserGroupViewModel> GetRoleUserGroupViewModelsByUserId(int userId);
    }
}
