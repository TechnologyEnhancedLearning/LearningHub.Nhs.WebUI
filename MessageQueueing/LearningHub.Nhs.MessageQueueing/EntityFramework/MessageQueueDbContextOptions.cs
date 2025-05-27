namespace LearningHub.Nhs.MessageQueueing.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The MessageQueueDbContextOptions.
    /// </summary>
    public class MessageQueueDbContextOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueueDbContextOptions"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MessageQueueDbContextOptions(DbContextOptions<MessageQueueDbContext> options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public DbContextOptions<MessageQueueDbContext> Options { get; }
    }
}
