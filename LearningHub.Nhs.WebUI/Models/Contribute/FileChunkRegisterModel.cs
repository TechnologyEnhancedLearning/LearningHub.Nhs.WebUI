namespace LearningHub.Nhs.WebUI.Models.Contribute
{
    /// <summary>
    /// Defines the <see cref="FileChunkRegisterModel" />.
    /// </summary>
    public class FileChunkRegisterModel
    {
        /// <summary>
        /// Gets or sets the AttachedFileType.
        /// </summary>
        public int AttachedFileType { get; set; }

        /// <summary>
        /// Gets or sets the ChangeingFileId.
        /// </summary>
        public int ChangeingFileId { get; set; }

        /// <summary>
        /// Gets or sets the FileChunkDetailId.
        /// </summary>
        public int FileChunkDetailId { get; set; }

        /// <summary>
        /// Gets or sets the FileUploadType.
        /// </summary>
        public FileUploadTypeEnum FileUploadType { get; set; }

        /// <summary>
        /// Gets or sets the ResourceType.
        /// </summary>
        public int ResourceType { get; set; }
    }
}
