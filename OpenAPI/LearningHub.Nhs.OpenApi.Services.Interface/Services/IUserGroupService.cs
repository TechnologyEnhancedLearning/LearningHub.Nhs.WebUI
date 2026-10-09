namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.UserGroup;
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
        Task<List<RoleUserGroupViewModel>> GetUserGroupRoleDetailByUserGroupId(int userGroupId);

        /// <summary>
        /// Returns a list of role user group detail view models for the supplied user id.
        /// </summary>
        /// <param name="userId">The user group id.</param>
        /// <returns>The list of <see cref="RoleUserGroupViewModel"/>.</returns>
        Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailByUserId(int userId);

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
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<bool> UserHasCatalogueContributionPermission(int userId);


        ////////////////////////////////////////////

            /// <summary>
            /// Retrieves all user groups.
            /// </summary>
            /// <returns>A collection of user groups.</returns>
            Task<IReadOnlyCollection<UserGroupViewModel>> GetAllAsync();

            /// <summary>
            /// Retrieves a user group by its identifier.
            /// </summary>
            /// <param name="id">The identifier of the user group.</param>
            /// <returns>The matching user group.</returns>
            Task<UserGroupViewModel> GetByIdAsync(int id);

            /// <summary>
            /// Retrieves all user groups associated with a specified user.
            /// </summary>
            /// <param name="userId">The identifier of the user.</param>
            /// <returns>A collection of user groups.</returns>
            Task<IReadOnlyCollection<UserGroupViewModel>> GetByUserIdAsync(int userId);

            /// <summary>
            /// Creates a new user group.
            /// </summary>
            /// <param name="request">The user group creation request.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns>The newly created user group.</returns>
            Task<UserGroupViewModel> CreateAsync(CreateUserGroupRequest request, int currentUserId);

            /// <summary>
            /// Updates a user group using the supplied patch request.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="request">The patch request containing the updated values.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns>The updated user group.</returns>
            Task<UserGroupViewModel> PatchAsync(int userGroupId, UpdateUserGroupRequest request, int currentUserId);

            /// <summary>
            /// Soft deletes a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns><c>true</c> if the user group was deleted; otherwise, <c>false</c>.</returns>
            Task<bool> SoftDeleteAsync(int userGroupId, int currentUserId);

            /// <summary>
            /// Retrieves all users assigned to a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <returns>A collection of user group memberships.</returns>
            Task<IReadOnlyCollection<UserGroupMembershipViewModel>> GetUsersAsync(int userGroupId);

            /// <summary>
            /// Adds users to a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="request">The request containing the users to add.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns>The updated collection of user group memberships.</returns>
            Task<IReadOnlyCollection<UserGroupMembershipViewModel>> AddUsersAsync(int userGroupId, AddUserGroupUsersRequest request, int currentUserId);

            /// <summary>
            /// Removes a user from a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="userId">The identifier of the user to remove.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns><c>true</c> if the user was removed; otherwise, <c>false</c>.</returns>
            Task<bool> RemoveUserAsync(int userGroupId, int userId, int currentUserId);

            /// <summary>
            /// Retrieves all reporters assigned to a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <returns>A collection of reporter assignments.</returns>
            Task<IReadOnlyCollection<UserGroupReporterViewModel>> GetReportersAsync(int userGroupId);

            /// <summary>
            /// Adds reporters to a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="request">The request containing the reporters to add.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns>The updated collection of reporter assignments.</returns>
            Task<IReadOnlyCollection<UserGroupReporterViewModel>> AddReportersAsync(int userGroupId, AddUserGroupReportersRequest request, int currentUserId);

            /// <summary>
            /// Removes a reporter from a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="userId">The identifier of the reporter to remove.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns><c>true</c> if the reporter was removed; otherwise, <c>false</c>.</returns>
            Task<bool> RemoveReporterAsync(int userGroupId, int userId, int currentUserId);

            /// <summary>
            /// Retrieves all catalogue links associated with a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <returns>A collection of catalogue links.</returns>
            Task<IReadOnlyCollection<UserGroupCatalogueViewModel>> GetCataloguesAsync(int userGroupId);

            /// <summary>
            /// Links a catalogue node and role to a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="request">The catalogue link request.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns>The newly created catalogue link.</returns>
            Task<UserGroupCatalogueViewModel> LinkCatalogueAsync(int userGroupId, LinkUserGroupCatalogueRequest request, int currentUserId);

            /// <summary>
            /// Removes a catalogue link from a user group.
            /// </summary>
            /// <param name="userGroupId">The identifier of the user group.</param>
            /// <param name="catalogueNodeId">The identifier of the catalogue node.</param>
            /// <param name="roleId">The identifier of the role.</param>
            /// <param name="currentUserId">The identifier of the user performing the operation.</param>
            /// <returns><c>true</c> if the catalogue link was removed; otherwise, <c>false</c>.</returns>
            Task<bool> UnlinkCatalogueAsync(int userGroupId, int catalogueNodeId, int roleId, int currentUserId);
        

    }
}
