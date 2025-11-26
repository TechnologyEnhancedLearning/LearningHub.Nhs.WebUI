namespace LearningHub.Nhs.AdminUI.Models
{
    /// <summary>
    /// View model for Azure Search index status.
    /// </summary>
    public class IndexStatusViewModel
    {
        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the document count in the index.
        /// </summary>
        public long? DocumentCount { get; set; }

        /// <summary>
        /// Gets or sets the storage size in bytes.
        /// </summary>
        public long? StorageSize { get; set; }
    }
}