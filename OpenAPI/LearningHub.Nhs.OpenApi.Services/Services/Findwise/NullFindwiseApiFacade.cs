namespace LearningHub.Nhs.OpenApi.Services.Services.Findwise
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Null implementation of IFindwiseApiFacade for use when Azure Search is enabled.
    /// This implementation performs no operations and is used to avoid Findwise calls when using Azure Search.
    /// </summary>
    public class NullFindwiseApiFacade : IFindwiseApiFacade
    {
        private readonly ILogger<NullFindwiseApiFacade> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullFindwiseApiFacade"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public NullFindwiseApiFacade(ILogger<NullFindwiseApiFacade> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// No-op implementation. Does not add or replace catalogues in Findwise.
        /// </summary>
        /// <param name="catalogues">The catalogues to add/replace in the index.</param>
        /// <returns>The task.</returns>
        public Task AddOrReplaceAsync(List<SearchCatalogueRequestModel> catalogues)
        {
            this.logger.LogDebug("NullFindwiseApiFacade: Skipping AddOrReplaceAsync for {Count} catalogues (Azure Search is enabled)", catalogues?.Count ?? 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// No-op implementation. Does not add or replace resources in Findwise.
        /// </summary>
        /// <param name="resources">The resources to add/replace in the index.</param>
        /// <returns>The task.</returns>
        public Task AddOrReplaceAsync(List<SearchResourceRequestModel> resources)
        {
            this.logger.LogDebug("NullFindwiseApiFacade: Skipping AddOrReplaceAsync for {Count} resources (Azure Search is enabled)", resources?.Count ?? 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// No-op implementation. Does not remove resources from Findwise.
        /// </summary>
        /// <param name="resources">The resources to remove from Findwise.</param>
        /// <returns>The task.</returns>
        public Task RemoveAsync(List<SearchResourceRequestModel> resources)
        {
            this.logger.LogDebug("NullFindwiseApiFacade: Skipping RemoveAsync for {Count} resources (Azure Search is enabled)", resources?.Count ?? 0);
            return Task.CompletedTask;
        }
    }
}
