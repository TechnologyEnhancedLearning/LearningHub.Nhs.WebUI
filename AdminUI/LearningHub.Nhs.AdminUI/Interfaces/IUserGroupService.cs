// <copyright file="IUserGroupService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>
namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// Defines the <see cref="IUserGroupService" />.
    /// </summary>
    public interface IUserGroupService
    {
        /// <summary>
        /// The GetRoleUserGroupDetailAsync.
        /// </summary>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailAsync();

        /// <summary>
        /// The GetUserGroupAdminBasicPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserAdminBasicViewModel}"/>.</returns>
        Task<PagedResultSet<UserGroupAdminBasicViewModel>> GetUserGroupAdminBasicPageAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// The GetUserGroupAdminDetailbyIdAsync.
        /// </summary>
        /// <param name="id">The id.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserGroupAdminDetailViewModel}"/>.</returns>
        Task<UserGroupAdminDetailViewModel> GetUserGroupAdminDetailbyIdAsync(int id);

        /// <summary>
        /// The CreateUserGroup.
        /// </summary>
        /// <param name="userGroup">The user<see cref="UserGroupAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateUserGroup(UserGroupAdminDetailViewModel userGroup);

        /// <summary>
        /// The UpdateUserGroup.
        /// </summary>
        /// <param name="userGroup">The user<see cref="UserGroupAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserGroup(UserGroupAdminDetailViewModel userGroup);

        /// <summary>
        /// The DeleteUserGroup.
        /// </summary>
        /// <param name="userGroupId">The userGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteUserGroup(int userGroupId);

        /// <summary>
        /// The AddUserGroupsToUser.
        /// </summary>
        /// <param name="userGroupId">The userId<see cref="int"/>.</param>
        /// <param name="userIdList">The userIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task<LearningHubValidationResult> AddUsersToUserGroup(int userGroupId, string userIdList);

        /// <summary>
        /// The DeleteUserUserGroup.
        /// </summary>
        /// <param name="userUserGroupId">The userUserGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteUserUserGroup(int userUserGroupId);

        /// <summary>
        /// The DeleteRoleUserGroup.
        /// </summary>
        /// <param name="roleUserGroupId">The roleUserGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteRoleUserGroup(int roleUserGroupId);

        /// <summary>
        /// The GetUserUserGroupPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserUserGroupViewModel}"/>.</returns>
        Task<PagedResultSet<UserUserGroupViewModel>> GetUserUserGroupPageAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// The GetUserGroupAdminRoleDetailByIdAsync.
        /// </summary>
        /// <param name="id">The id.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<RoleUserGroupViewModel>> GetUserGroupAdminRoleDetailByIdAsync(int id);

        /// <summary>
        /// The AddUserGroupsToCatalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId<see cref="int"/>.</param>
        /// <param name="roleId">The id<see cref="int"/>.</param>
        /// <param name="userGroupIdList">The userGroupIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task<LearningHubValidationResult> AddUserGroupsToCatalogue(int catalogueNodeId, int roleId, string userGroupIdList);

        /// <summary>
        /// The GetRoleUserGroupPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{RoleUserGroupViewModel}"/>.</returns>
        Task<PagedResultSet<RoleUserGroupViewModel>> GetRoleUserGroupPageAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// Check if user has given permission.
        /// </summary>
        /// <param name="permissionCode">To check against permission code.</param>
        /// <returns>Success or not.</returns>
        Task<bool> UserHasPermissionAsync(string permissionCode);
    }
}
