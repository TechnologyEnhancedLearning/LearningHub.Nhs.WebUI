namespace LearningHub.Nhs.WebUI.Models
{
    /// <summary>
    /// MoodleCompletionResponseViewModel.
    /// </summary>
    public class MoodleCompletionResponseViewModel
    {
        /// <summary>
        /// Gets or sets the completion status.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets error code.
        /// </summary>
        public string Errorcode { get; set; }

        /// <summary>
        ///  Gets or sets Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///  Gets or sets Debug info.
        /// </summary>
        public string Debuginfo { get; set; }
    }
}
