// <copyright file="IMessageRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Messaging;

    /// <summary>
    /// The IMessageRepository class.
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// Gets a list of all messages which have a message send which hasn't been sent.
        /// </summary>
        /// <returns>The messages.</returns>
        IQueryable<FullMessageDto> GetPendingMessages();

        /// <summary>
        /// Creates an email to be sent.
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
        Task CreateNotificationForUserAsync(
            int userId,
            string subject,
            string body,
            int recipientUserId,
            DateTimeOffset notificationStartDate,
            DateTimeOffset notificationEndDate,
            int notificationPriority,
            int notificationType);

        /// <summary>
        /// Marks a message send as having been successful.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageSends">The messageSends.</param>
        /// <returns>The task.</returns>
        Task MessageSendSuccess(int userId, List<int> messageSends);

        /// <summary>
        /// Either marks a message as failed, or queues it for a retry.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageSends">The messageSends.</param>
        /// <returns>The task.</returns>
        Task MessageSendFailure(int userId, List<int> messageSends);
    }
}
