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
        /// Determines if a sort direction is descending.
        /// </summary>
        /// <param name="sortDirection">The sort direction string.</param>
        /// <returns>True if descending, false otherwise.</returns>
        public static bool IsDescendingSort(string? sortDirection)
        {
            return sortDirection != null &&
                   (sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ||
                    sortDirection.Equals("descending", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Maps a UI sort column name to the corresponding Azure Search field name.
        /// </summary>
        /// <param name="uiSortColumn">The UI sort column name.</param>
        /// <returns>The Azure Search field name, or null if no mapping exists.</returns>
        public static string? MapSortColumnToSearchField(string? uiSortColumn)
        {
            if (string.IsNullOrWhiteSpace(uiSortColumn))
                return null;

            return uiSortColumn.Trim().ToLowerInvariant() switch
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
                _ => null
            };
        }

        /// <summary>
        /// Maps a UI sort column name to the corresponding Document property name for in-memory sorting.
        /// </summary>
        /// <param name="uiSortColumn">The UI sort column name.</param>
        /// <returns>The Document property name.</returns>
        public static string? MapSortColumnToDocumentProperty(string? uiSortColumn)
        {
            if (string.IsNullOrWhiteSpace(uiSortColumn))
                return null;

            return uiSortColumn.Trim().ToLowerInvariant() switch
            {
                "avgrating" => "rating",
                "rating" => "rating",
                "authored_date" => "authored_date",
                "authoreddate" => "authored_date",
                "authoredDate" => "authored_date",
                "title" => "title",
                "atoz" => "title",
                "alphabetical" => "title",
                "ztoa" => "title",
                _ => "title" // Default to title
            };
        }

        /// <summary>
        /// Applies post-processing sort to a list of documents.
        /// Used when semantic search is active with non-relevance sorting.
        /// </summary>
        /// <param name="documents">The documents to sort.</param>
        /// <param name="sortColumn">The column to sort by (title, rating, authored_date).</param>
        /// <param name="sortDirection">The sort direction (ascending/descending).</param>
        /// <returns>The sorted list of documents.</returns>
        public static List<LearningHub.Nhs.Models.Search.Document> ApplyPostProcessingSort(
            List<LearningHub.Nhs.Models.Search.Document> documents,
            string? sortColumn,
            string? sortDirection)
        {
            if (documents == null || documents.Count == 0)
            {
                return documents;
            }

            bool isDescending = IsDescendingSort(sortDirection);
            string? mappedColumn = MapSortColumnToDocumentProperty(sortColumn);

            IOrderedEnumerable<LearningHub.Nhs.Models.Search.Document> sortedDocuments = mappedColumn?.ToLowerInvariant() switch
            {
                "title" => isDescending
                    ? documents.OrderByDescending(d => d.Title, StringComparer.OrdinalIgnoreCase)
                    : documents.OrderBy(d => d.Title, StringComparer.OrdinalIgnoreCase),
                "rating" => isDescending
                    ? documents.OrderByDescending(d => d.Rating)
                    : documents.OrderBy(d => d.Rating),
                "authored_date" => isDescending
                    ? documents.OrderByDescending(d =>
                        DateTime.TryParse(d.AuthoredDate, out var dt) ? dt : DateTime.MinValue)
                    : documents.OrderBy(d =>
                        DateTime.TryParse(d.AuthoredDate, out var dt) ? dt : DateTime.MinValue),
                _ => isDescending
                    ? documents.OrderByDescending(d => d.Title, StringComparer.OrdinalIgnoreCase)
                    : documents.OrderBy(d => d.Title, StringComparer.OrdinalIgnoreCase)
            };

            return sortedDocuments.ToList();
        }

        /// <summary>
        /// Builds search options for Azure Search queries.
        /// </summary>
        /// <param name="searchQueryType">The type of search query.</param>
        /// <param name="offset">The number of results to skip.</param>
        /// <param name="pageSize">The number of results to return.</param>
        /// <param name="filters">The filters to apply.</param>
        /// <param name="sortBy">The sort to apply.</param>
        /// <param name="includeFacets">Whether to include facets.</param>
        /// <param name="config">The Azure Search configuration.</param>
        /// <returns>The configured search options.</returns>
        public static SearchOptions BuildSearchOptions(
            SearchQueryType searchQueryType,
            int offset,
            int pageSize,
            Dictionary<string, List<string>>? filters,
            Dictionary<string, string>? sortBy,
            bool includeFacets,
            Models.Configuration.AzureSearchConfig config)
        {
            var searchOptions = new SearchOptions
            {
                Skip = offset,
                Size = pageSize,
                IncludeTotalCount = true,
                ScoringProfile = config.ScoringProfile
            };

            string sortByFinal = GetSortOption(sortBy);

            // Configure query type
            if (searchQueryType == SearchQueryType.Semantic)
            {
                searchOptions.QueryType = SearchQueryType.Semantic;
                searchOptions.SemanticSearch = new SemanticSearchOptions
                {
                    SemanticConfigurationName = config.SemanticConfigurationName
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
            if (includeFacets && config.FacetFields != null)
            {
                foreach (var facet in config.FacetFields)
                {
                    var facetValue = facet.Contains("count:")
                        ? facet: $"{facet},count:20";

                    searchOptions.Facets.Add(facetValue);
                }
            }

            // Add deleted filter
            Dictionary<string, List<string>> deleteFilter = new Dictionary<string, List<string>>
            {
                { config.DeletedFilterField, new List<string> { config.DeletedFilterValue } }
            };
            filters = filters == null ? deleteFilter : filters.Concat(deleteFilter).ToDictionary(k => k.Key, v => v.Value);

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

            // Determine direction using shared helper
            string sortDirection = IsDescendingSort(directionInput) ? "desc" : "asc";

            // Map UI values to search fields using shared helper
            string? sortColumn = MapSortColumnToSearchField(uiSortKey);

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
