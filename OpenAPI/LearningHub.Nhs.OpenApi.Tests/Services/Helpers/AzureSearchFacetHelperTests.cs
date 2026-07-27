namespace LearningHub.Nhs.OpenApi.Tests.Services.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Search.Documents.Models;
    using FluentAssertions;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.AzureSearch;
    using LearningHub.Nhs.OpenApi.Services.Helpers.Search;
    using Xunit;

    public class AzureSearchFacetHelperTests
    {
        /// <summary>
        /// Creates a FacetResult dictionary via the cacheable conversion helpers so tests
        /// do not depend on internal Azure SDK constructors directly.
        /// </summary>
        private static IDictionary<string, IList<FacetResult>> BuildFacets(
            Dictionary<string, List<(string value, long count)>> data)
        {
            var cacheable = data.ToDictionary(
                kvp => kvp.Key,
                kvp => (IList<CacheableFacetResult>)kvp.Value
                    .Select(t => new CacheableFacetResult { Value = t.value, Count = t.count })
                    .ToList());

            return AzureSearchFacetHelper.ConvertFromCacheable(cacheable);
        }

        [Fact]
        public void MergeFacets_WhenFilteredFacetGroupIsMissingForUnrelatedFacet_ShouldReturnZeroCount()
        {
            // Given
            // unfiltered facets represent the initial cached state
            var unfilteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("tests", 5) },
                ["resources"] = new List<(string, long)> { ("test1", 2), ("test2", 2), ("test3", 1), ("test4", 2) },
            });

            // filtered facets represent what Azure returns after selecting 'test4' from 'resources'
            // resource_level has no matches, so Azure AI Search omits it entirely
            var filteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resources"] = new List<(string, long)> { ("test4", 2) },
            });

            // User has selected 'test4' from the 'resources' facet group
            var appliedFilters = new Dictionary<string, List<string>>
            {
                ["resources"] = new List<string> { "test4" },
            };

            // When
            var result = AzureSearchFacetHelper.MergeFacets(filteredFacets, unfilteredFacets, appliedFilters);

            // Then — unrelated facet group shows filtered (0) count
            var resourceLevelFacet = result.Single(f => f.Id == "resource_level");
            resourceLevelFacet.Filters.Single(f => f.DisplayName == "tests").Count
                .Should().Be(0, "because 'test4' is unrelated to 'tests' in resource_level, so the filtered count should be 0");

            // The 'resources' group itself retains its original cached counts (multi-select pattern)
            var resourcesFacet = result.Single(f => f.Id == "resources");
            resourcesFacet.Filters.Single(f => f.DisplayName == "test1").Count.Should().Be(2, "resources group keeps original counts for non-selected values");
            resourcesFacet.Filters.Single(f => f.DisplayName == "test2").Count.Should().Be(2, "resources group keeps original counts for non-selected values");
            resourcesFacet.Filters.Single(f => f.DisplayName == "test3").Count.Should().Be(1, "resources group keeps original counts for non-selected values");
            resourcesFacet.Filters.Single(f => f.DisplayName == "test4").Count.Should().Be(2, "resources group keeps original count for the selected value too");
        }

        [Fact]
        public void MergeFacets_WhenResourceLevelFilterIsApplied_ResourcesFacetShouldShowFilteredCounts()
        {
            // Given
            var unfilteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("tests", 5) },
                ["resources"] = new List<(string, long)> { ("test1", 2), ("test2", 2), ("test3", 1), ("test4", 2) },
            });

            // After selecting 'tests' from resource_level, only test1 and test2 belong to that level
            var filteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("tests", 5) },
                ["resources"] = new List<(string, long)> { ("test1", 2), ("test2", 2) },
            });

            // User has selected 'tests' from the 'resource_level' facet group
            var appliedFilters = new Dictionary<string, List<string>>
            {
                ["resource_level"] = new List<string> { "tests" },
            };

            // When
            var result = AzureSearchFacetHelper.MergeFacets(filteredFacets, unfilteredFacets, appliedFilters);

            // Then — the resources group shows updated filtered counts because resource_level filter is active
            var resourcesFacet = result.Single(f => f.Id == "resources");
            resourcesFacet.Filters.Single(f => f.DisplayName == "test1").Count.Should().Be(2);
            resourcesFacet.Filters.Single(f => f.DisplayName == "test2").Count.Should().Be(2);
            resourcesFacet.Filters.Single(f => f.DisplayName == "test3").Count.Should().Be(0, "test3 is absent from filtered results so its count is 0");
            resourcesFacet.Filters.Single(f => f.DisplayName == "test4").Count.Should().Be(0, "test4 is absent from filtered results so its count is 0");

            // resource_level itself keeps original counts (it has an active filter)
            var resourceLevelFacet = result.Single(f => f.Id == "resource_level");
            resourceLevelFacet.Filters.Single(f => f.DisplayName == "tests").Count
                .Should().Be(5, "because resource_level has an applied filter so its own counts stay at original");
        }

        [Fact]
        public void MergeFacets_WhenFilteredFacetGroupExistsButValueIsMissing_ShouldReturnZeroCount()
        {
            // Given
            var unfilteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("tests", 5), ("courses", 3) },
            });

            // Azure returns resource_level but only 'courses' matched the filter
            var filteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("courses", 2) },
            });

            var appliedFilters = new Dictionary<string, List<string>>();

            // When
            var result = AzureSearchFacetHelper.MergeFacets(filteredFacets, unfilteredFacets, appliedFilters);

            // Then
            var resourceLevelFacet = result.Single(f => f.Id == "resource_level");
            resourceLevelFacet.Filters.Single(f => f.DisplayName == "tests").Count
                .Should().Be(0, "because 'tests' is absent from the filtered results so its count is 0");
            resourceLevelFacet.Filters.Single(f => f.DisplayName == "courses").Count
                .Should().Be(2, "because 'courses' was present in the filtered results with count 2");
        }

        [Fact]
        public void MergeFacets_WhenFilterIsSelected_ShouldReturnUnfilteredCount()
        {
            // Given
            var unfilteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("tests", 5) },
            });

            // Filtered results omit 'tests' because it is the active filter
            var filteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)>(),
            });

            // The user has selected 'tests' in resource_level
            var appliedFilters = new Dictionary<string, List<string>>
            {
                ["resource_level"] = new List<string> { "tests" },
            };

            // When
            var result = AzureSearchFacetHelper.MergeFacets(filteredFacets, unfilteredFacets, appliedFilters);

            // Then
            var testsFilter = result.Single(f => f.Id == "resource_level").Filters.Single(f => f.DisplayName == "tests");
            testsFilter.Count.Should().Be(5, "because selected filters should show the unfiltered count to allow deselection");
            testsFilter.Selected.Should().BeTrue();
        }

        [Fact]
        public void MergeFacets_WhenFilteredFacetsIsNull_ShouldReturnUnfilteredCounts()
        {
            // Given
            var unfilteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("tests", 5) },
            });

            var appliedFilters = new Dictionary<string, List<string>>();

            // When
            var result = AzureSearchFacetHelper.MergeFacets(filteredFacets: null, unfilteredFacets, appliedFilters);

            // Then
            result.Single(f => f.Id == "resource_level").Filters.Single(f => f.DisplayName == "tests").Count
                .Should().Be(5, "because when no filtered search was performed, the unfiltered count should be used");
        }

        [Fact]
        public void MergeFacets_WhenUnfilteredFacetsIsEmpty_ShouldReturnEmptyArray()
        {
            // Given
            var unfilteredFacets = new Dictionary<string, IList<FacetResult>>();
            var filteredFacets = BuildFacets(new Dictionary<string, List<(string, long)>>
            {
                ["resource_level"] = new List<(string, long)> { ("tests", 5) },
            });

            // When
            var result = AzureSearchFacetHelper.MergeFacets(filteredFacets, unfilteredFacets, appliedFilters: null);

            // Then
            result.Should().BeEmpty();
        }
    }
}
