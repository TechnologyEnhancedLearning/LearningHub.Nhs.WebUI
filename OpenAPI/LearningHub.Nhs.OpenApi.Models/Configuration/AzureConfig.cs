namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The AzureConfig.
    /// </summary>
    public class AzureConfig
    {
        /// <summary>
        /// Gets or sets the azure blob settings.
        /// </summary>
        public AzureBlobSettings AzureBlobSettings { get; set; } = null!;
        /// <summary>
        /// Gets or sets the azure storage queue.
        /// </summary>
        public string AzureStorageQueueConnectionString { get; set; } = null!;
    }
}
