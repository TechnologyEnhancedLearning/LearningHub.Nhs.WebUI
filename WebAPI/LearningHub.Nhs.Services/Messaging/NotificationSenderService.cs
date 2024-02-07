// <copyright file="NotificationSenderService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Messaging
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Messaging;

    /// <summary>
    /// The NotificationSenderService class.
    /// </summary>
    public class NotificationSenderService : INotificationSenderService
    {
        private readonly INotificationTemplateService notificationTemplateService;
        private readonly IMessageService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSenderService"/> class.
        /// </summary>
        /// <param name="notificationTemplateService">The notificationTemplateService.</param>
        /// <param name="messageService">The messageService.</param>
        public NotificationSenderService(
            INotificationTemplateService notificationTemplateService,
            IMessageService messageService)
        {
            this.notificationTemplateService = notificationTemplateService;
            this.messageService = messageService;
        }

        /// <summary>
        /// The SendCatalogueAccessRequestAcceptedNotification.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <param name="recipientUserId">The recipientUserId.</param>
        /// <returns>The task.</returns>
        public async Task SendCatalogueAccessRequestAcceptedNotification(int userId, string catalogueName, string catalogueUrl, int recipientUserId)
        {
            var notification = this.notificationTemplateService.GetCatalogueAccessRequestAccepted(catalogueName, catalogueUrl);
            await this.messageService.CreateNotificationForUserAsync(userId, notification.Title, notification.Message, recipientUserId, DateTimeOffset.Now, DateTimeOffset.MaxValue, (int)NotificationPriorityEnum.General, (int)NotificationTypeEnum.AccessRequest);
        }

        /// <summary>
        /// The SendCatalogueAccessRequestAcceptedNotification.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <param name="rejectionReason">The rejectionReason.</param>
        /// <param name="recipientUserId">The recipientUserId.</param>
        /// <returns>The task.</returns>
        public async Task SendCatalogueAccessRequestRejectedNotification(int userId, string catalogueName, string catalogueUrl, string rejectionReason, int recipientUserId)
        {
            var notification = this.notificationTemplateService.GetCatalogueAccessRequestFailure(catalogueName, catalogueUrl, rejectionReason);
            await this.messageService.CreateNotificationForUserAsync(
                userId,
                notification.Title,
                notification.Message,
                recipientUserId,
                DateTimeOffset.Now,
                DateTimeOffset.MaxValue,
                (int)NotificationPriorityEnum.General,
                (int)NotificationTypeEnum.AccessRequest);
        }
    }
}
