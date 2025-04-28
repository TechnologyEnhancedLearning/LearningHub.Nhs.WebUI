namespace LearningHub.Nhs.MessageQueueing.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessageQueueing.Entities;

    /// <summary>
    /// The IEmailQueueRepository class.
    /// </summary>
    public interface IEmailQueueRepository
    {
        /// <summary>
        /// The QueueEmailsAsync.
        /// </summary>
        /// <param name="emails">The emails list.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task QueueEmailsAsync(IEnumerable<EmailQueue> emails);

        /// <summary>
        /// The GetPendingEmailsAsync.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IEnumerable<EmailQueue>> GetPendingEmailsAsync();

        /// <summary>
        /// The UpdateEmailStatusAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="status">status.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateEmailStatusAsync(Guid id, string status, string errorMessage);

        /// <summary>
        /// The IncrementRetryAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="errorMessage">error message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task IncrementRetryAsync(Guid id, string errorMessage);
    }
}
