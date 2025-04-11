namespace LearningHub.Nhs.WebUI.Models
{
    /// <summary>
    /// MoodleOverviewFileViewModel.
    /// </summary>
    public class MoodleOverviewFileViewModel
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string? FilePath { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// Gets or sets the file URL.
        /// </summary>
        public string? FileUrl { get; set; }

        /// <summary>
        /// Gets or sets the time the file was modified (Unix timestamp).
        /// </summary>
        public long TimeModified { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the file.
        /// </summary>
        public string? MimeType { get; set; }
    }
}
