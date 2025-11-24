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

        public static Dictionary<string, List<string>> CombineAndNormaliseFilters(string requestTypeFilterText, string? providerFilterText)
        {
            var filters = new Dictionary<string, List<string>>
            {
                //  { "resource_collection", new List<string> { "Resource" } }
            };

            // Parse and merge additional filters from query string
            var requestTypeFilters = ParseQueryStringFilters(requestTypeFilterText);
            var providerFilters = ParseQueryStringFilters(providerFilterText);

            // Merge filters from both sources
            MergeFilterDictionary(filters, requestTypeFilters);
            //  MergeFilterDictionary(filters, providerFilters);

            //NormaliseFilters(filters);

            return filters;
        }

        private static void MergeFilterDictionary(Dictionary<string, List<string>> target, Dictionary<string, List<string>> source)
        {
            foreach (var kvp in source)
            {
                if (!target.ContainsKey(kvp.Key))
                    target[kvp.Key] = new List<string>();

                target[kvp.Key].AddRange(kvp.Value);
            }
        }

        /// <summary>
        /// Builds a filter expression from a dictionary of filters.
        /// </summary>
        /// <param name="filters">The filters to apply.</param>
        /// <returns>The filter expression string.</returns>
        /// <summary>
        /// Build an OData filter that supports multi-select values.
        /// Pass a dictionary where key = field name, value = list of selected values.
        /// If `collectionFields` contains a field name, that field will be treated as a collection and use any(...).
        /// </summary>
        public static string BuildFilterExpression(
            Dictionary<string, List<string>>? filters,
            ISet<string>? collectionFields = null)
        {
            if (filters == null || !filters.Any())
                return string.Empty;

            collectionFields ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Handle spacing, NBSP, escaping quotes
            string Normalize(string v)
            {
                if (v == null) return string.Empty;

                // Replace NBSP
                v = v.Replace('\u00A0', ' ').Trim();

                // Collapse multiple spaces
                v = System.Text.RegularExpressions.Regex.Replace(v, @"\s+", " ");

                // Escape single quotes for OData
                v = v.Replace("'", "''");

                return v;
            }

            var expressions = new List<string>();

            foreach (var kvp in filters)
            {
                var field = kvp.Key;
                var values = kvp.Value?.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

                if (values == null || values.Count == 0)
                    continue;

                // Normalize all values
                var normalizedValues = values.Select(Normalize).Distinct().ToList();

                // Single value → use eq
                if (normalizedValues.Count == 1)
                {
                    var v = normalizedValues[0];

                    if (collectionFields.Contains(field))
                    {
                        expressions.Add($"{field}/any(t: t eq '{v}')");
                    }
                    else
                    {
                        expressions.Add($"{field} eq '{v}'");
                    }

                    continue;
                }

                // Multiple values → use OR conditions (ALWAYS works)
                if (collectionFields.Contains(field))
                {
                    // collection field (array) → OR any(...) conditions
                    var ors = normalizedValues
                        .Select(v => $"{field}/any(t: t eq '{v}')");

                    expressions.Add("(" + string.Join(" or ", ors) + ")");
                }
                else
                {
                    // single string field → OR eq conditions
                    var ors = normalizedValues
                        .Select(v => $"{field} eq '{v}'");

                    expressions.Add("(" + string.Join(" or ", ors) + ")");
                }
            }

            return expressions.Count > 0
                ? string.Join(" and ", expressions)
                : string.Empty;
        }



        /// <summary>
        /// Parses filter parameters from a query string.
        /// </summary>
        /// <param name="queryString">The query string to parse.</param>
        /// <returns>A dictionary of filter names and their values.</returns>
        public static Dictionary<string, List<string>> ParseQueryStringFilters(string? queryString)
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
        /// Normalizes resource type and resource collection filter values by capitalizing the first letter.
        /// </summary>
        /// <param name="filters">The filters dictionary to normalize.</param>
        public static void NormaliseFilters(Dictionary<string, List<string>>? filters)
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
