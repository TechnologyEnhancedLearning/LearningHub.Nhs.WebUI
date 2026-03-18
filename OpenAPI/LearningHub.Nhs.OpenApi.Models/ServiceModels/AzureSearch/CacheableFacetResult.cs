namespace LearningHub.Nhs.OpenApi.Models.ServiceModels.AzureSearch
{
    /// <summary>
    /// A cacheable representation of Azure Search FacetResult.
    /// This DTO is used to cache facet data without serialization issues with Azure SDK types.
    /// </summary>
    public class CacheableFacetResult
    {
        /// <summary>
        /// Gets or sets the facet value.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the count of documents matching this facet value.
        /// </summary>
        public long Count { get; set; }
    }
}
