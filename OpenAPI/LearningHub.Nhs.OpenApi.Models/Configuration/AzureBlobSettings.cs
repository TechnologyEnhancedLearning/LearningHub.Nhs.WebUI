namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The AzureBlobSettings.
    /// </summary>
    public class AzureBlobSettings
    {
        /// <summary>
        /// Gets or sets the connectionString.
        /// </summary>
        public string ConnectionString { get; set; } = null!;

        /// <summary>
        /// Gets or sets the catalogue collection id.
        /// </summary>
        public string UploadContainer { get; set; } = null!;
    }
}
