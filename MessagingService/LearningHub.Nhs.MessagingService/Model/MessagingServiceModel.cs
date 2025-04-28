namespace LearningHub.Nhs.MessagingService.Model
{
    /// <summary>
    /// MessagingServiceModel.
    /// </summary>
    public class MessagingServiceModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingServiceModel"/> class.
        /// </summary>
        public MessagingServiceModel()
        {
            // Current = this;
        }

        /// <summary>
        /// Gets or Sets ApiKey.
        /// </summary>
        public string GovNotifyApiKey { get; set; } = string.Empty;
    }
}
