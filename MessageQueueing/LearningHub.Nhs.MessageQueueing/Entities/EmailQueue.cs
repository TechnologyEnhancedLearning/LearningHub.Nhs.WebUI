namespace LearningHub.Nhs.MessageQueueing.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The Email Queue.
    /// </summary>
    public class EmailQueue
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Recipient.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the Body.
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Gets or sets the TemplateId.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the Personalisation.
        /// </summary>
        public string? Personalisation { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the NotificationId.
        /// </summary>
        public string? NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the RetryCount.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the CreatedAt.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the DeliverAfter.
        /// </summary>
        public DateTimeOffset? DeliverAfter { get; set; }

        /// <summary>
        /// Gets or sets the SentAt.
        /// </summary>
        public DateTimeOffset? SentAt { get; set; }

        /// <summary>
        /// Gets or sets the LastAttemptAt.
        /// </summary>
        public DateTimeOffset? LastAttemptAt { get; set; }

        /// <summary>
        /// Gets or sets the ErrorMessage.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
