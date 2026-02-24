namespace LearningHub.Nhs.MessagingService.Model
{
    /// <summary>
    /// MessagingServiceModel.
    /// </summary>
    public class MessagingServiceOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingServiceOptions"/> class.
        /// </summary>
        public MessagingServiceOptions()
        {
            // Current = this;
        }

        /// <summary>
        /// Gets or Sets ApiKey.
        /// </summary>
        public string GovNotifyApiKey { get; set; } = string.Empty;
    }
}
