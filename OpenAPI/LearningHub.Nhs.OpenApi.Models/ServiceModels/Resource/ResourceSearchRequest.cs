namespace LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// ResourceSearchRequest.
    /// </summary>
    public class ResourceSearchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSearchRequest"/> class.
        /// </summary>
        /// <param name="searchText">1.</param>
        /// <param name="limit">3.</param>
        /// <param name="offset">2.</param>
        /// <param name="catalogueId">4.</param>
        /// <param name="resourceTypes">5.</param>
        public ResourceSearchRequest(
            string searchText,
            int offset,
            int limit,
            int? catalogueId = null,
            IEnumerable<string>? resourceTypes = null)
        {
            this.SearchText = searchText;
            this.Offset = offset;
            this.Limit = limit;
            this.CatalogueId = catalogueId;
            this.ResourceTypes = resourceTypes?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// Gets Text.
        /// </summary>
        public string SearchText { get; }

        /// <summary>
        /// Gets Offset.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets Limit.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets CatalogueId.
        /// </summary>
        public int? CatalogueId { get; }

        /// <summary>
        /// Gets ResourceTypeIds.
        /// </summary>
        public List<string> ResourceTypes { get; }
    }
}
