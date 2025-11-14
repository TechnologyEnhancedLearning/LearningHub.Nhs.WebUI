namespace LearningHub.Nhs.OpenApi.Services.Helpers
{
    using System;
    using Azure;
    using Azure.Search.Documents;
    using LearningHub.Nhs.OpenApi.Models.Configuration;

    /// <summary>
    /// Factory for creating Azure Search clients.
    /// </summary>
    public static class AzureSearchClientFactory
    {
        /// <summary>
        /// Creates a SearchClient for querying the Azure Search index.
        /// </summary>
        /// <param name="config">The Azure Search configuration.</param>
        /// <returns>A configured SearchClient instance.</returns>
        public static SearchClient CreateQueryClient(AzureSearchConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var credential = new AzureKeyCredential(config.QueryApiKey);
            return new SearchClient(
                new Uri(config.ServiceEndpoint),
                config.IndexName,
                credential);
        }

        /// <summary>
        /// Creates a SearchClient with admin credentials for indexing operations.
        /// </summary>
        /// <param name="config">The Azure Search configuration.</param>
        /// <returns>A configured SearchClient instance with admin credentials.</returns>
        public static SearchClient CreateAdminClient(AzureSearchConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var adminCredential = new AzureKeyCredential(config.AdminApiKey);
            return new SearchClient(
                new Uri(config.ServiceEndpoint),
                config.IndexName,
                adminCredential);
        }
    }
}
