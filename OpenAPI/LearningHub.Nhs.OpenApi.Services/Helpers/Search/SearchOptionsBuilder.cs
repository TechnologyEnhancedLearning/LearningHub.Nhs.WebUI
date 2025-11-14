namespace LearningHub.Nhs.OpenApi.Services.Helpers.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Search.Documents;
    using Azure.Search.Documents.Models;

    /// <summary>
    /// Helper class for building Azure Search options.
    /// </summary>
    public static class SearchOptionsBuilder
    {
        /// <summary>
        /// Builds search options for Azure Search queries.
        /// </summary>
        /// <param name="searchQueryType">The type of search query.</param>
        /// <param name="offset">The number of results to skip.</param>
        /// <param name="pageSize">The number of results to return.</param>
        /// <param name="filters">The filters to apply.</param>
        /// <param name="sortBy">The sort to apply.</param>
        /// <param name="includeFacets">Whether to include facets.</param>
        /// <returns>The configured search options.</returns>
        public static SearchOptions BuildSearchOptions(
            SearchQueryType searchQueryType,
            int offset,
            int pageSize,
            Dictionary<string, List<string>>? filters,
            Dictionary<string, string>? sortBy,
            bool includeFacets)
        {
            var searchOptions = new SearchOptions
            {
                Skip = offset,
                Size = pageSize,
                IncludeTotalCount = true,
                ScoringProfile = "boostExactTitle"
            };

            string? columns = sortBy?.Keys.FirstOrDefault();
            string? directions = sortBy?.Values.FirstOrDefault();
            string sortDirection = "asc";

            string sortColumn = columns ?? "relevance";
            if (directions?.StartsWith("desc", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                sortDirection = "desc";
            }

            // Configure query type
            if (searchQueryType == SearchQueryType.Semantic)
            {
                searchOptions.QueryType = SearchQueryType.Semantic;
                searchOptions.SemanticSearch = new SemanticSearchOptions
                {
                    SemanticConfigurationName = "default"
                };
            }
            else if (searchQueryType == SearchQueryType.Simple)
            {
                searchOptions.QueryType = SearchQueryType.Simple;
                searchOptions.SearchMode = SearchMode.Any;
                searchOptions.OrderBy.Add($"{sortColumn} {sortDirection}");
            }
            else
            {
                searchOptions.QueryType = SearchQueryType.Full;
                searchOptions.OrderBy.Add($"{sortColumn} {sortDirection}");
            }

            // Add facets
            if (includeFacets)
            {
                searchOptions.Facets.Add("resource_type");
                searchOptions.Facets.Add("resource_collection");
                searchOptions.Facets.Add("provider_ids");
            }

            // Apply filters
            if (filters?.Any() == true)
            {
                searchOptions.Filter = SearchFilterBuilder.BuildFilterExpression(filters);
            }

            return searchOptions;
        }
    }
}
