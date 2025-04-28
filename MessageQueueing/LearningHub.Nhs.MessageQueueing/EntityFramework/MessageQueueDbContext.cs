namespace LearningHub.Nhs.MessageQueueing.EntityFramework
{
    using LearningHub.Nhs.MessageQueueing.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The Message Queue Db Context.
    /// </summary>
    public class MessageQueueDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueueDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MessageQueueDbContext(DbContextOptions<MessageQueueDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the EmailQueues.
        /// </summary>
        public virtual DbSet<EmailQueue> EmailQueues { get; set; }
    }
}
