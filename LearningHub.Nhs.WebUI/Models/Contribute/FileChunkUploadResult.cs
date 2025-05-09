﻿namespace LearningHub.Nhs.WebUI.Models.Contribute
{
    /// <summary>
    /// Defines the <see cref="FileChunkUploadResult" />.
    /// </summary>
    public class FileChunkUploadResult
    {
        /// <summary>
        /// Gets or sets the FileChunkDetailId.
        /// </summary>
        public int FileChunkDetailId { get; set; }

        /// <summary>
        /// Gets or sets the FileChunkId.
        /// </summary>
        public int FileChunkId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Success.
        /// </summary>
        public bool Success { get; set; }
    }
}
