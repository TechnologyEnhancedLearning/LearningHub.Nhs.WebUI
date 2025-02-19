﻿namespace LearningHub.Nhs.WebUI.Models.Contribute
{
    /// <summary>
    /// Defines the <see cref="FileUploadSettingsModel" />.
    /// </summary>
    public class FileUploadSettingsModel
    {
        /// <summary>
        /// Gets or sets the AllowedThreads.
        /// </summary>
        public int AllowedThreads { get; set; }

        /// <summary>
        /// Gets or sets the ChunkSize.
        /// </summary>
        public long ChunkSize { get; set; }

        /// <summary>
        /// Gets or sets the FileUploadSizeLimit.
        /// </summary>
        public long FileUploadSizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the FileUploadSizeLimitText.
        /// </summary>
        public string FileUploadSizeLimitText { get; set; }

        /// <summary>
        /// Gets or sets the TimeoutSec.
        /// </summary>
        public int TimeoutSec { get; set; }
    }
}
