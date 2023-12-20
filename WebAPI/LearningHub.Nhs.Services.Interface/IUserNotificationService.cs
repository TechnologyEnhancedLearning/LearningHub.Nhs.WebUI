// <copyright file="IUserNotificationService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The UserNotificationService interface.
    /// </summary>
    public interface IUserNotificationService
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserNotification> GetByIdAsync(int id);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="notificationId">The notificationId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserNotification> GetByIdAndUserIdAsync(int notificationId, int userId);

        /// <summary>
        /// The get page async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="priorityType">Notification priority type.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<UserNotificationViewModel>> GetPageAsync(int userId, NotificationPriorityEnum priorityType, int page, int pageSize, string sortColumn = "", string sortDirection = "");

        /// <summary>
        /// The get user unread notification count async.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> GetUserUnreadNotificationCountAsync(int userid);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateAsync(int userId, UserNotification notification);

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAsync(int userId, UserNotification notification);
    }
}
