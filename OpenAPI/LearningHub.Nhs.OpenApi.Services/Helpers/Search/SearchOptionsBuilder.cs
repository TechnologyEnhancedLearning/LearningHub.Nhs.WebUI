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

            string sortByFinal = GetSortOption(sortBy);

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
                searchOptions.OrderBy.Add(sortByFinal);
            }
            else
            {
                searchOptions.QueryType = SearchQueryType.Full;
                searchOptions.OrderBy.Add(sortByFinal);
            }

            // Add facets
            if (includeFacets)
            {
                searchOptions.Facets.Add("resource_type");
                searchOptions.Facets.Add("resource_collection");
                searchOptions.Facets.Add("provider_ids");
            }

            Dictionary<string, List<string>> deletFilter = new Dictionary<string, List<string>> {{ "is_deleted", new List<string> {"false"} }};
            filters = filters == null ? deletFilter : filters.Concat(deletFilter).ToDictionary(k => k.Key, v => v.Value);

            // Apply filters
            if (filters?.Any() == true)
            {
                searchOptions.Filter = SearchFilterBuilder.BuildFilterExpression(filters);
            }

            return searchOptions;
        }

        private static string GetSortOption(Dictionary<string, string>? sortBy)
        {
            // If null or empty → Azure Search will default to relevance
            if (sortBy == null || sortBy.Count == 0)
                return string.Empty;

            // Extract key/value (only first pair used)
            string? uiSortKey = sortBy.Keys.FirstOrDefault();
            string? directionInput = sortBy.Values.FirstOrDefault();

            // Handle empty key → no sorting
            if (string.IsNullOrWhiteSpace(uiSortKey))
                return string.Empty;

            // Determine direction safely
            string sortDirection =
                directionInput != null &&
                directionInput.StartsWith("desc", StringComparison.OrdinalIgnoreCase)
                    ? "desc"
                    : "asc";

            // Map UI values to search fields
            string? sortColumn = uiSortKey.Trim().ToLowerInvariant() switch
            {
                "avgrating" => "rating",
                "rating" => "rating",

                "authored_date" => "date_authored",
                "authoreddate" => "date_authored",
                "authoredDate" => "date_authored",

                "title" => "normalised_title",
                "atoz" => "normalised_title",
                "alphabetical" => "normalised_title",
                "ztoa" => "normalised_title",

                _ => null // unknown sort → ignore
            };

            // No valid mapping → fall back to relevance
            if (string.IsNullOrWhiteSpace(sortColumn))
                return string.Empty;

            return $"{sortColumn} {sortDirection}";
        }

        /// <summary>
        /// Parses the search query type from configuration string.
        /// Parsing is case-insensitive. If the value is null, empty, or invalid, defaults to Semantic.
        /// </summary>
        /// <param name="searchQueryTypeString">The search query type string (semantic, full, or simple).</param>
        /// <returns>The parsed SearchQueryType enum value.</returns>
        public static SearchQueryType ParseSearchQueryType(string searchQueryTypeString)
        {
            if (string.IsNullOrWhiteSpace(searchQueryTypeString))
            {
                return SearchQueryType.Semantic;
            }

            if (Enum.TryParse<SearchQueryType>(searchQueryTypeString, ignoreCase: true, out var queryType) &&
                (queryType == SearchQueryType.Semantic || queryType == SearchQueryType.Full || queryType == SearchQueryType.Simple))
            {
                return queryType;
            }

            return SearchQueryType.Semantic;
        }
    }
}
