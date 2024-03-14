namespace LearningHub.Nhs.ReportApi.Shared.Configuration
{
    /// <summary>
    /// The settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the client identity key.
        /// </summary>
        public string ClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the report api identity key.
        /// </summary>
        public string ReportApiClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubApiUrl.
        /// </summary>
        public string LearningHubApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the Azure blob storage settings.
        /// </summary>
        public AzureBlobStorageSettings AzureBlobStorageSettings { get; set; }

        /// <summary>
        /// Gets or sets the Azure service bus settings.
        /// </summary>
        public AzureServiceBusSettings AzureServiceBusSettings { get; set; }
    }
}
