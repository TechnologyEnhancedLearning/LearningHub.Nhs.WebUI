namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
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
        Task<List<RoleUserGroupViewModel>> GetRoleUserGroupViewModelsByUserGroupId(int userGroupId);

        /// <summary>
        /// Get list of RoleUserGroupViewModel for a supplied User Group.
        /// </summary>
        /// <param name="userId">The userGroupId.</param>
        /// <returns>A list of RoleUserGroupViewModel.</returns>
        Task<List<RoleUserGroupViewModel>> GetRoleUserGroupViewModelsByUserId(int userId);

        /// <summary>
        /// The get all for search.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<RoleUserGroup>> GetAllforSearch(int catalogueNodeId, int userId);
    }
}
