// <copyright file="INotificationService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The NotificationService interface.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<NotificationViewModel> GetByIdAsync(int id);

        /// <summary>
        /// The get page async.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<NotificationViewModel>> GetPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string filter = "");

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAsync(int userId, NotificationViewModel notification);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateAsync(int userId, NotificationViewModel notification);

        /// <summary>
        /// Creates  resource access permisssion notification.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="readOnlyUser">Is read only user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> CreatePermisssionNotificationAsync(int userId, bool readOnlyUser);

        /// <summary>
        /// Creates resource published notification.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="resourceTitle">Resource title.</param>
        /// <param name="resourceReferenceId">Resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> CreateResourcePublishedNotificationAsync(int userId, string resourceTitle, int resourceReferenceId);

        /// <summary>
        /// Creates resource publish failed notification.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="resourceTitle">Resource title.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> CreatePublishFailedNotificationAsync(int userId, string resourceTitle, string errorMessage = "");
    }
}