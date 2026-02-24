namespace LearningHub.Nhs.MessageQueueing.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.GovNotifyMessaging;
    using LearningHub.Nhs.Models.GovNotifyMessaging;

    /// <summary>
    /// The IEmailQueueRepository class.
    /// </summary>
    public interface IMessageQueueRepository
    {
        /// <summary>
        /// The QueueMessagesAsync.
        /// </summary>
        /// <param name="emails">The emails list.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task QueueMessagesAsync(IEnumerable<QueueRequests> emails);

        /// <summary>
        /// The GetPendingEmailsAsync.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IEnumerable<PendingMessageRequests>> GetPendingEmailsAsync();

        /// <summary>
        /// Marks a message as failed, or queues it for a retry.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task MessageDeliveryFailed(GovNotifyResponse response);

        /// <summary>
        /// Marks a message as send.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task MessageDeliverySuccess(GovNotifyResponse response);

        /// <summary>
        /// Save one-off emails.
        /// </summary>
        /// <param name="request">The email request.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task SaveSingleEmailTransactions(SingleEmailRequest request);
    }
}
