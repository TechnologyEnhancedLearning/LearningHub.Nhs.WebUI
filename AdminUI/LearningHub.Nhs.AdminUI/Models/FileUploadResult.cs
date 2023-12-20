// <copyright file="FileUploadResult.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Models
{
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// Defines the <see cref="FileUploadResult" />.
    /// </summary>
    public class FileUploadResult
    {
        /// <summary>
        /// Gets or sets the AttachedFileTypeId.
        /// </summary>
        public int AttachedFileTypeId { get; set; }

        /// <summary>
        /// Gets or sets the FileId.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets the FileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the FileSizeKb.
        /// </summary>
        public int FileSizeKb { get; set; }

        /// <summary>
        /// Gets or sets the FileTypeId.
        /// </summary>
        public int FileTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Invalid.
        /// </summary>
        public bool Invalid { get; set; }

        /// <summary>
        /// Gets or sets the ResourceType.
        /// </summary>
        public ResourceTypeEnum ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the PageSectionDetailId.
        /// </summary>
        public int PageSectionDetailId { get; set; }
    }
}
