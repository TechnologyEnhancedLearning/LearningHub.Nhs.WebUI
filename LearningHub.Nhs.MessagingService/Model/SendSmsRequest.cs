namespace LearningHub.Nhs.MessagingService.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="SendSmsRequest" />.
    /// </summary>
    public class SendSmsRequest
    {
        /// <summary>
        /// Gets or Sets the PhoneNumber.
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }

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
