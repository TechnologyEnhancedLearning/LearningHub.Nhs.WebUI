namespace LearningHub.Nhs.OpenApi.Services.Helpers.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Helper class for building Azure Search filter expressions.
    /// </summary>
    public static class SearchFilterBuilder
    {
        /// <summary>
        /// Builds a filter expression from a dictionary of filters.
        /// </summary>
        /// <param name="filters">The filters to apply.</param>
        /// <returns>The filter expression string.</returns>
        public static string BuildFilterExpression(Dictionary<string, List<string>>? filters)
        {
            if (filters == null || !filters.Any())
                return string.Empty;

            var filterExpressions = new List<string>();

            foreach (var filter in filters)
            {
                if (filter.Value?.Any() == true)
                {
                    var values = string.Join(",", filter.Value);
                    filterExpressions.Add($"search.in({filter.Key}, '{values}')");
                }
            }

            return filterExpressions.Any() ? string.Join(" and ", filterExpressions) : string.Empty;
        }

        /// <summary>
        /// Parses filter parameters from a query string.
        /// </summary>
        /// <param name="queryString">The query string to parse.</param>
        /// <returns>A dictionary of filter names and their values.</returns>
        public static Dictionary<string, List<string>> ParseFiltersFromQuery(string? queryString)
        {
            var filters = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(queryString))
                return filters;

            // Remove '?' if present
            if (queryString.StartsWith("?"))
                queryString = queryString.Substring(1);

            // Parse using HttpUtility
            var parsed = HttpUtility.ParseQueryString(queryString);

            foreach (string? key in parsed.AllKeys)
            {
                if (key == null) 
                    continue; // skip null keys

                var values = parsed.GetValues(key);
                if (values != null)
                {
                    // Add to dictionary (support multiple values per key)
                    if (!filters.ContainsKey(key))
                        filters[key] = new List<string>();

                    filters[key].AddRange(values);
                }
            }

            return filters;
        }

        /// <summary>
        /// Normalizes resource type filter values by capitalizing the first letter.
        /// </summary>
        /// <param name="filters">The filters dictionary to normalize.</param>
        public static void NormalizeResourceTypeFilters(Dictionary<string, List<string>>? filters)
        {
            if (filters == null || !filters.ContainsKey("resource_type"))
                return;

            filters["resource_type"] = filters["resource_type"]
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => char.ToUpper(v[0]) + v.Substring(1))
                .ToList();
        }
    }
}
