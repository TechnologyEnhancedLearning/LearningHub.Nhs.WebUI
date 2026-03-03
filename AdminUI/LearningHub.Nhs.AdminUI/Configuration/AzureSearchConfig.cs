namespace LearningHub.Nhs.AdminUI.Configuration
{
    /// <summary>
    /// Configuration settings for Azure AI Search.
    /// </summary>
    public class AzureSearchConfig
    {
        /// <summary>
        /// Gets or sets the Azure Search service endpoint URL.
        /// </summary>
        public string ServiceEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the admin API key for managing indexes and indexers.
        /// </summary>
        public string AdminApiKey { get; set; }

        /// <summary>
        /// Gets or sets the query API key for search operations.
        /// </summary>
        public string QueryApiKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the search index.
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Gets or sets the name of the indexer.
        /// </summary>
        public string IndexerName { get; set; }
    }
}