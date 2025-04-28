namespace LearningHub.Nhs.MessageQueueing.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessageQueueing.Entities;
    using LearningHub.Nhs.MessageQueueing.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The EmailQueueRepository.
    /// </summary>
    public class EmailQueueRepository : IEmailQueueRepository
    {
        private readonly MessageQueueDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailQueueRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The context.</param>
        public EmailQueueRepository(MessageQueueDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// The QueueEmailsAsync.
        /// </summary>
        /// <param name="emails">The emails.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task QueueEmailsAsync(IEnumerable<EmailQueue> emails)
        {
            await this.dbContext.EmailQueues.AddRangeAsync(emails);
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// The GetPendingEmailsAsync.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IEnumerable<EmailQueue>> GetPendingEmailsAsync()
        {
            return await this.dbContext.EmailQueues
                .Where(e => e.Status == "Pending" && e.RetryCount < 3)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// The UpdateEmailStatusAsync.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="status">The status.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateEmailStatusAsync(Guid id, string status, string errorMessage = null)
        {
            var email = await this.dbContext.EmailQueues.FindAsync(id);
            if (email != null)
            {
                email.Status = status;
                email.ErrorMessage = errorMessage;
                email.SentAt = status == "Sent" ? DateTime.UtcNow : null;

                await this.dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// The IncrementRetryAsync.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="errorMessage">The errorMessage.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task IncrementRetryAsync(Guid id, string errorMessage)
        {
            var email = await this.dbContext.EmailQueues.FindAsync(id);
            if (email != null)
            {
                email.RetryCount++;
                email.LastAttemptAt = DateTime.UtcNow;
                email.ErrorMessage = errorMessage;

                if (email.RetryCount >= 3)
                {
                    email.Status = "Failed";
                }

                await this.dbContext.SaveChangesAsync();
            }
        }
    }
}
