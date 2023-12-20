// <copyright file="IUserService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// Defines the <see cref="IUserService" />.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// The GetCurrentUser.
        /// </summary>
        /// <returns>The <see cref="Task{UserViewModel}"/>.</returns>
        Task<UserViewModel> GetCurrentUser();

        /// <summary>
        /// The GetEmailAddressRegistrationStatusAsync.
        /// </summary>
        /// <param name="emailAddress">The emailAddress<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{EmailRegistrationStatus}"/>.</returns>
        Task<EmailRegistrationStatus> GetEmailAddressRegistrationStatusAsync(string emailAddress);

        /// <summary>
        /// The GetUserAdminBasicPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserAdminBasicViewModel}"/>.</returns>
        Task<PagedResultSet<UserAdminBasicViewModel>> GetUserAdminBasicPageAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// The GetLHUserAdminBasicPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserAdminBasicViewModel}"/>.</returns>
        Task<PagedResultSet<LearningHub.Nhs.Models.User.UserAdminBasicViewModel>> GetLHUserAdminBasicPageAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// The GetUserAdminDetailbyIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserAdminDetailViewModel}"/>.</returns>
        Task<UserAdminDetailViewModel> GetUserAdminDetailbyIdAsync(int id);

        /// <summary>
        /// The GetUserByUserId.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserViewModel}"/>.</returns>
        Task<UserViewModel> GetUserByUserId(int id);

        /// <summary>
        /// The GetUserContributionsAsync.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="PagedResultSet{ResourceAdminSearchResultViewModel}"/>.</returns>
        Task<PagedResultSet<ResourceAdminSearchResultViewModel>> GetUserContributionsAsync(int userId);

        /// <summary>
        /// The GetUserHistoryPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserHistoryViewModel}"/>.</returns>
        Task<PagedResultSet<UserHistoryViewModel>> GetUserHistoryPageAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// The GetUserLearningRecordsAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="Task{PagedResultSet{UserLearningRecordViewModel}}"/>.</returns>
        Task<PagedResultSet<MyLearningDetailedItemViewModel>> GetUserLearningRecordsAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// The SendAdminPasswordResetEmail.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SendAdminPasswordResetEmail(int userId);

        /// <summary>
        /// The ClearUserCachedPermissions.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> ClearUserCachedPermissions(int userId);

        /// <summary>
        /// The SendEmailToUserAsync.
        /// </summary>
        /// <param name="emailAddress">The emailAddress<see cref="string"/>.</param>
        /// <param name="subject">The subject<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="string"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task SendEmailToUserAsync(string emailAddress, string subject, string body, int userId);

        /// <summary>
        /// The StoreUserHistory.
        /// </summary>
        /// <param name="userHistory">The userHistory<see cref="UserHistoryViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task StoreUserHistory(UserHistoryViewModel userHistory);

        /// <summary>
        /// The UpdateUser.
        /// </summary>
        /// <param name="user">The user<see cref="UserAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateUser(UserAdminDetailViewModel user);

        /// <summary>
        /// The AddUserGroupsToUser.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="userGroupIdList">The userGroupIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task<LearningHubValidationResult> AddUserGroupsToUser(int userId, string userGroupIdList);
    }
}
