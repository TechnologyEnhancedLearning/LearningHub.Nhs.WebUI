using System.Collections.Generic;

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
        public string SuggesterName { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the suggester size for auto-complete and suggestions.
        /// </summary>
        public int ConceptsSuggesterSize { get; set; } = 5;

        /// <summary>
        /// Gets or sets the resource collection size (catalogue, course and resources) for auto-complete and suggestions.
        /// </summary>
        public int ResourceCollectionSuggesterSize { get; set; } = 5;

        /// <summary>
        /// Gets or sets the search query type (semantic, full, or simple).
        /// </summary>
        public string SearchQueryType { get; set; } = "Semantic";

        /// <summary>
        /// Gets or sets the semantic result buffer size for post-processing sorts.
        /// When sorting is applied to semantic search results, this many results are retrieved
        /// before applying the sort and pagination. Default is 50.
        /// </summary>
        public int SemanticResultBufferSize { get; set; } = 55;

        /// <summary>
        /// Gets or sets the scoring profile name used for boosting search results.
        /// Default is "boostExactTitle".
        /// </summary>
        public string ScoringProfile { get; set; } = "boostExactTitle";

        /// <summary>
        /// Gets or sets the semantic configuration name for semantic search.
        /// Default is "default".
        /// </summary>
        public string SemanticConfigurationName { get; set; } = "default";

        /// <summary>
        /// Gets or sets the facet fields to include in search results.
        /// Default is ["resource_type", "resource_collection", "provider_ids"].
        /// </summary>
        public List<string> FacetFields { get; set; } = new List<string> { "resource_type", "resource_collection", "provider_ids", "resource_access_level" };

        /// <summary>
        /// Gets or sets the field name for the deleted filter.
        /// Default is "is_deleted".
        /// </summary>
        public string DeletedFilterField { get; set; } = "is_deleted";

        /// <summary>
        /// Gets or sets the value for the deleted filter.
        /// Default is "false".
        /// </summary>
        public string DeletedFilterValue { get; set; } = "false";
    }
}
