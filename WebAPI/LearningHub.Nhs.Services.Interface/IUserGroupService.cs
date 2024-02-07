// <copyright file="IUserGroupService.cs" company="NHS England">
// Copyright (c) NHS England.
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
    /// The UserGroupService interface.
    /// </summary>
    public interface IUserGroupService
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserGroup> GetByIdAsync(int id, bool includeRoles);

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userGroup">The user group.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAsync(int userId, UserGroup userGroup);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> DeleteAsync(int userId, int userGroupId);

        /// <summary>
        /// Returns a user group detail view model for the supplied id.
        /// </summary>
        /// <param name="id">The user group id.</param>
        /// <returns>The <see cref="UserGroupAdminDetailViewModel"/>.</returns>
        Task<UserGroupAdminDetailViewModel> GetUserGroupAdminDetailByIdAsync(int id);

        /// <summary>
        /// Returns a list of role user group detail view models for the supplied user group id.
        /// </summary>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The list of <see cref="RoleUserGroupViewModel"/>.</returns>
        List<RoleUserGroupViewModel> GetUserGroupRoleDetailByUserGroupId(int userGroupId);

        /// <summary>
        /// Returns a list of role user group detail view models for the supplied user id.
        /// </summary>
        /// <param name="userId">The user group id.</param>
        /// <returns>The list of <see cref="RoleUserGroupViewModel"/>.</returns>
        List<RoleUserGroupViewModel> GetRoleUserGroupDetailByUserId(int userId);

        /// <summary>
        /// Create a user group.
        /// </summary>
        /// <param name="userGroupAdminDetailViewModel">The user group admin detail view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateUserGroupAsync(UserGroupAdminDetailViewModel userGroupAdminDetailViewModel, int currentUserId);

        /// <summary>
        /// Updates a user group.
        /// </summary>
        /// <param name="userGroupAdminDetailViewModel">The user group admin detail view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserGroupAsync(UserGroupAdminDetailViewModel userGroupAdminDetailViewModel, int currentUserId);

        /// <summary>
        /// Returns a list of "role - user group" info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The presetFilter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<RoleUserGroupViewModel>> GetRoleUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "");

        /// <summary>
        /// Returns a list of "user - user group" info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The presetFilter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<UserUserGroupViewModel>> GetUserUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "");

        /// <summary>
        /// Returns a list of basic user group info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The preset filter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<UserGroupAdminBasicViewModel>> GetUserGroupAdminBasicPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "");

        /// <summary>
        /// Adds a list of users to a user group.
        /// </summary>
        /// <param name="userUserGroups">The user user group view model list.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> AddUserUserGroups(List<UserUserGroupViewModel> userUserGroups, int currentUserId);

        /// <summary>
        /// Removes user from a user group.
        /// </summary>
        /// <param name="userUserGroupViewModel">The user user group view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> DeleteUserUserGroupAsync(UserUserGroupViewModel userUserGroupViewModel, int currentUserId);

        /// <summary>
        /// Removes a role - user group.
        /// </summary>
        /// <param name="roleUserGroupViewModel">The role user group view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> DeleteRoleUserGroupAsync(RoleUserGroupUpdateViewModel roleUserGroupViewModel, int currentUserId);

        /// <summary>
        /// Adds a list of role user groups.
        /// </summary>
        /// <param name="roleUserGroups">The role user group view model list.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> AddRoleUserGroups(List<RoleUserGroupUpdateViewModel> roleUserGroups, int currentUserId);

        /// <summary>
        /// Adds a user group attribute.
        /// </summary>
        /// <param name="userGroupAttribute">The user group attribute view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> AddUserGroupAttribute(UserGroupAttributeViewModel userGroupAttribute, int currentUserId);

        /// <summary>
        /// Removes a user group attribute.
        /// </summary>
        /// <param name="userGroupAttribute">The user group attribute view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> DeleteUserGroupAttributeAsync(UserGroupAttributeViewModel userGroupAttribute, int currentUserId);
    }
}
