namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The Azure AI Search configuration settings.
    /// </summary>
    public class AzureSearchConfig
    {
        /// <summary>
        /// Gets or sets the Azure Search service endpoint URL.
        /// </summary>
        public string ServiceEndpoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Azure Search admin API key.
        /// </summary>
        public string AdminApiKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Azure Search query API key.
        /// </summary>
        public string QueryApiKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the index name.
        /// </summary>
        public string IndexName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the default item limit for search results.
        /// </summary>
        public int DefaultItemLimitForSearch { get; set; } = 10;

        /// <summary>
        /// Gets or sets the description length limit.
        /// </summary>
        public int DescriptionLengthLimit { get; set; } = 3000;

        /// <summary>
        /// Gets or sets the maximum description length.
        /// </summary>
        public int MaximumDescriptionLength { get; set; } = 150;

        /// <summary>
        /// Gets or sets the suggester name for auto-complete and suggestions.
        /// </summary>
        public string SuggesterName { get; set; } = "test-search-suggester";
    }
}
