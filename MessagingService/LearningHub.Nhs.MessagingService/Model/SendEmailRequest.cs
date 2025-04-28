namespace LearningHub.Nhs.MessagingService.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="SendEmailRequest" />.
    /// </summary>
    public class SendEmailRequest
    {
        /// <summary>
        /// Gets or Sets the Email.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or Sets the TemplateId.
        /// </summary>
        [Required]
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or Sets the Personalisation.
        /// </summary>
        public Dictionary<string, dynamic> Personalisation { get; set; }
    }
}
