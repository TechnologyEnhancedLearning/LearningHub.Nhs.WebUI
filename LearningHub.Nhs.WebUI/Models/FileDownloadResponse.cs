namespace LearningHub.Nhs.WebUI.Models
{
    using System.IO;

    /// <summary>
    /// Defines the <see cref="FileDownloadResponse" />.
    /// </summary>
    public class FileDownloadResponse
    {
        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        /// Gets or sets the ContentType.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the ContentType.
        /// </summary>
        public long ContentLength { get; set; }
    }
}