// <copyright file="IRoleService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The RoleService interface.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Get Roles.
        /// </summary>
        /// <returns>List of role including permissions.</returns>
        Task<IEnumerable<RolePermissionsViewModel>> GetAllRoles();

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="loadPermissions">The load permissions.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<Role> GetByIdAsync(int id, bool loadPermissions);

        /// <summary>
        /// The get available for user group async.
        /// </summary>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<List<Role>> GetAvailableForUserGroupAsync(int userGroupId);

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="role">The role.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<LearningHubValidationResult> CreateAsync(int userId, Role role);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="role">The role.</param>
        /// <param name="updatePermissions">The update permissions.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<LearningHubValidationResult> UpdateAsync(int userId, Role role, bool updatePermissions);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task DeleteAsync(int userId, int roleId);

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>
        /// The <see cref="SearchResult"/>.
        /// </returns>
        SearchResult Search(string searchText, int page, int pageSize);
    }
}
