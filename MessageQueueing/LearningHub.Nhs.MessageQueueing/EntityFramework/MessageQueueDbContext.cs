namespace LearningHub.Nhs.MessageQueueing.EntityFramework
{
    using LearningHub.Nhs.Models.GovNotifyMessaging;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The Message Queue Db Context.
    /// </summary>
    public class MessageQueueDbContext : DbContext
    {
        /// <summary>
        /// The options.
        /// </summary>
        private readonly MessageQueueDbContextOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueueDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MessageQueueDbContext(MessageQueueDbContextOptions options)
            : base(options.Options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets the Options.
        /// </summary>
        public MessageQueueDbContextOptions Options
        {
            get { return this.options; }
        }

        ////public virtual DbSet<QueueRequests> QueueRequests { get; set; }

        /// <summary>
        /// Gets or sets the PendingMessageRequests.
        /// </summary>
        public virtual DbSet<PendingMessageRequests> PendingMessageRequests { get; set; }
    }
}
