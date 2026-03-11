namespace LearningHub.Nhs.OpenApi.Services.Helpers.Search
{
    using Azure.Search.Documents.Models;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.AzureSearch;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Helper class for Azure Search facet operations including conversion and caching.
    /// </summary>
    public static class AzureSearchFacetHelper
    {
        private static readonly ConstructorInfo FacetResultCtor =
            typeof(FacetResult)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 2 &&
                           parameters[0].ParameterType == typeof(long?) &&
                           typeof(IReadOnlyDictionary<string, object>).IsAssignableFrom(parameters[1].ParameterType);
                });

        /// <summary>
        /// Converts Azure Search FacetResult dictionary to cacheable DTO.
        /// </summary>
        /// <param name="facets">The facets from Azure Search.</param>
        /// <returns>A cacheable dictionary of facets.</returns>
        public static IDictionary<string, IList<CacheableFacetResult>> ConvertToCacheable(
            IDictionary<string, IList<FacetResult>>? facets)
        {
            if (facets == null)
                return new Dictionary<string, IList<CacheableFacetResult>>();

            return facets.ToDictionary(
                kvp => kvp.Key,
                kvp => (IList<CacheableFacetResult>)kvp.Value
                    .Select(f => new CacheableFacetResult
                    {
                        Value = f.Value,
                        Count = f.Count ?? 0
                    })
                    .ToList());
        }

        /// <summary>
        /// Converts cacheable DTO back to Azure Search FacetResult dictionary.
        /// </summary>
        /// <param name="cacheableFacets">The cached facets.</param>
        /// <returns>A dictionary of FacetResult.</returns>
        public static IDictionary<string, IList<FacetResult>> ConvertFromCacheable(
            IDictionary<string, IList<CacheableFacetResult>>? cacheableFacets)
        {
            var result = new Dictionary<string, IList<FacetResult>>(StringComparer.OrdinalIgnoreCase);

            if (cacheableFacets == null || FacetResultCtor == null)
                return result;

            foreach (var kvp in cacheableFacets)
            {
                var facetResults = new List<FacetResult>();

                foreach (var cf in kvp.Value)
                {
                    // Build the additionalProperties dictionary just like Azure returns in JSON
                    var additionalProps = new Dictionary<string, object>
                    {
                        ["value"] = cf.Value ?? string.Empty
                    };

                    // Use the internal constructor via reflection
                    var facet = (FacetResult)FacetResultCtor.Invoke(new object?[] { cf.Count, additionalProps });

                    facetResults.Add(facet);
                }

                result[kvp.Key] = facetResults;
            }

            return result;
        }


        /// <summary>
        /// Merges filtered and unfiltered facets to maintain visibility of all filter options.
        /// </summary>
        /// <param name="filteredFacets">Facets from the filtered search.</param>
        /// <param name="unfilteredFacets">Facets from the unfiltered search.</param>
        /// <param name="appliedFilters">The currently applied filters.</param>
        /// <returns>An array of merged facets.</returns>
        public static Facet[] MergeFacets(
            IDictionary<string, IList<FacetResult>> filteredFacets,
            IDictionary<string, IList<FacetResult>> unfilteredFacets,
            Dictionary<string, List<string>> appliedFilters)
        {
            if (unfilteredFacets == null || !unfilteredFacets.Any())
            {
                return Array.Empty<Facet>();
            }

            var facets = new Facet[unfilteredFacets.Count];
            var index = 0;

            foreach (var facetGroup in unfilteredFacets)
            {
                var facetKey = facetGroup.Key;
                var hasAppliedFilter = appliedFilters?.ContainsKey(facetKey) == true;
                var appliedValues = hasAppliedFilter ? appliedFilters[facetKey] : new List<string>();

                // Get filtered facet values if available
                var filteredFacetValues = filteredFacets?.ContainsKey(facetKey) == true
                    ? filteredFacets[facetKey].ToDictionary(f => f.Value?.ToString()?.ToLower() ?? "", f => (int)f.Count)
                    : new Dictionary<string, int>();

                var filters = facetGroup.Value.Select(f =>
                {
                    var displayName = f.Value?.ToString()?.ToLower() ?? "";
                    var isSelected = appliedValues.Any(av => av.Equals(f.Value?.ToString(), StringComparison.OrdinalIgnoreCase));

                    // Default to the unfiltered count. This is used when:
                    //  - no filtered search has been performed, OR
                    //  - the filter value is selected (keep count so the option remains visible for deselection), OR
                    //  - this facet group itself has an applied filter (multi-select pattern: a group's own
                    //    counts are not narrowed down by its own selections so the user can still pick other values).
                    // Only update counts using the filtered results when a filter from a DIFFERENT group
                    // is driving the narrowing of this group.
                    var count = (int)f.Count;
                    if (!isSelected && filteredFacets != null && !hasAppliedFilter)
                    {
                        count = filteredFacetValues.TryGetValue(displayName, out var filteredCount) ? filteredCount : 0;
                    }

                    return new Filter
                    {
                        DisplayName = displayName,
                        Count = count,
                        Selected = isSelected
                    };
                }).ToList();

                // Ensure all selected filter values remain visible even if absent from the unfiltered
                // baseline (e.g. a resource selected under a resource-level filter that yields no results).
                foreach (var selectedValue in appliedValues)
                {
                    var alreadyPresent = filters.Any(f =>
                        f.DisplayName.Equals(selectedValue, StringComparison.OrdinalIgnoreCase));

                    if (!alreadyPresent)
                    {
                        filters.Add(new Filter
                        {
                            DisplayName = selectedValue.ToLower(),
                            Count = 0,
                            Selected = true,
                        });
                    }
                }

                facets[index++] = new Facet
                {
                    Id = facetKey,
                    Filters = filters.ToArray()
                };
            }

            return facets;
        }
    }
}
