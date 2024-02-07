// <copyright file="UserNotificationService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user notification service.
    /// </summary>
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IUserNotificationRepository usernotificationRepository;
        private readonly ITimezoneOffsetManager timezoneOffsetManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationService"/> class.
        /// </summary>
        /// <param name="usernotificationRepository">The usernotification repository.</param>
        /// <param name="timezoneOffsetManager">The timezoneOffsetManager.</param>
        public UserNotificationService(IUserNotificationRepository usernotificationRepository, ITimezoneOffsetManager timezoneOffsetManager)
        {
            this.usernotificationRepository = usernotificationRepository;
            this.timezoneOffsetManager = timezoneOffsetManager;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserNotification> GetByIdAsync(int id)
        {
            return await this.usernotificationRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="notificationId">The notificationId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserNotification> GetByIdAndUserIdAsync(int notificationId, int userId)
        {
            return await this.usernotificationRepository.GetByNotificationAndUserIdAsync(userId, notificationId);
        }

        /// <summary>
        /// The get user unread notification count async.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> GetUserUnreadNotificationCountAsync(int userid)
        {
            return await this.usernotificationRepository.GetUserUnreadNotificationCountAsync(userid);
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateAsync(int userId, UserNotification notification)
        {
            notification.Notification = null;
            notification.User = null;
            notification.ReadOnDate = notification.ReadOnDate.HasValue ? this.timezoneOffsetManager.ConvertToUserTimezone(notification.ReadOnDate.Value) : null;
            await this.usernotificationRepository.UpdateAsync(userId, notification);
            return new LearningHubValidationResult { IsValid = true };
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAsync(int userId, UserNotification notification)
        {
            var notificationUserId = notification.UserId == 0
                ? userId
                : notification.UserId;
            var result = new LearningHubValidationResult();
            var currentNotification = await this.usernotificationRepository.GetByNotificationAndUserIdAsync(notificationUserId, notification.NotificationId);
            if (currentNotification != null)
            {
                result.IsValid = false;
                return result;
            }

            notification.UserId = notificationUserId;
            await this.usernotificationRepository.CreateAsync(userId, notification);
            result.IsValid = true;
            return result;
        }

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
        public async Task<PagedResultSet<UserNotificationViewModel>> GetPageAsync(
            int userId, NotificationPriorityEnum priorityType, int page, int pageSize, string sortColumn = "", string sortDirection = "")
        {
            PagedResultSet<UserNotificationViewModel> result = new PagedResultSet<UserNotificationViewModel>();

            // Get existing user notifications
            var items = this.usernotificationRepository.GetAllNonDismissed(userId, priorityType, sortColumn, sortDirection);

            //// Join and map
            var viewmodelitems = await items.Select(n => MapNotificationToDummyUserNotification(n)).ToListAsync();
            result.TotalItemCount = viewmodelitems.Count();

            int totalPages = (int)Math.Ceiling((double)result.TotalItemCount / pageSize);

            if (totalPages < page)
            {
                page = totalPages;
            }

            var pageditems = viewmodelitems.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            result.Items = pageditems;

            return result;
        }

        /// <summary>
        /// The map notification to dummy user notification.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns>The <see cref="UserNotificationViewModel"/>.</returns>
        private static UserNotificationViewModel MapNotificationToDummyUserNotification(UserSpecificNotification n)
        {
            var userNotification = new UserNotificationViewModel()
            {
                Body = n.Notification.Message,
                Title = n.Notification.Title,
                NotificationId = n.Notification.Id,
                Date = n.Notification.StartDate,
                UserDismissable = n.Notification.UserDismissable,
                ReadOnDate = n.UserNotification?.ReadOnDate,
                Id = n.UserNotification != null ? n.UserNotification.Id : -1,
                NotificationType = n.Notification.NotificationTypeEnum,
                NotificationPriority = n.Notification.NotificationPriorityEnum,
            };
            return userNotification;
        }
    }
}