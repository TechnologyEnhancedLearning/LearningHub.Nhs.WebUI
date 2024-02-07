// <copyright file="IMessageService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Messaging;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The IMessageService interface.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// The GetPendingMessages.
        /// </summary>
        /// <returns>The pending messages.</returns>
        List<MessageViewModel> GetPendingMessages();

        /// <summary>
        /// The CreateEmailAsync.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientUserId">The recipient user id.</param>
        /// <returns>The task.</returns>
        Task CreateEmailAsync(int userId, string subject, string body, int recipientUserId);

        /// <summary>
        /// The CreateEmailAsync.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientEmailAddress">The recipientEmailAddress.</param>
        /// <returns>The task.</returns>
        Task CreateEmailAsync(int userId, string subject, string body, string recipientEmailAddress);

        /// <summary>
        /// The MarkAsFailedAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageFailedIds">The messageFailedIds.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> MarkAsFailedAsync(int userId, List<int> messageFailedIds);

        /// <summary>
        /// The MarkAsSentAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageSendIds">The messageSendIds.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> MarkAsSentAsync(int userId, List<int> messageSendIds);

        /// <summary>
        /// The CreateNotificationForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientUserId">The recipientUserId.</param>
        /// <param name="notificationStartDate">The notificationStartDate.</param>
        /// <param name="notificationEndDate">The notificationEndDate.</param>
        /// <param name="notificationPriority">The notificationPriority.</param>
        /// <param name="notificationType">The notificationType.</param>
        /// <returns>The task.</returns>
        Task CreateNotificationForUserAsync(int userId, string subject, string body, int recipientUserId, DateTimeOffset notificationStartDate, DateTimeOffset notificationEndDate, int notificationPriority, int notificationType);
    }
}
