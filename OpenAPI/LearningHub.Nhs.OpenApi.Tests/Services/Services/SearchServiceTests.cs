namespace LearningHub.Nhs.OpenApi.Tests.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using LearningHub.Nhs.OpenApi.Tests.TestHelpers;
    using LearningHub.Nhs.OpenApi.Tests.TestMockData;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    public class SearchServiceTests
    {
        private readonly Mock<IFindwiseClient> findwiseClient;
        private readonly Mock<ILearningHubService> learningHubService;
        private readonly Mock<IResourceRepository> resourceRepository;
        private readonly Mock<IResourceService> resourceService;
        private readonly SearchService searchService;
        private readonly int currentUserId;
        private readonly List<int> currentUserIdLS;
        private readonly List<int> emptyOriginalResourceIdLS = new List<int>() { }; // for readabiliy
        private readonly List<int> emptyResourceIdLS = new List<int>() { }; // for readabiliy

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchServiceTests"/> class.
        /// </summary>
        public SearchServiceTests()
        {
            //This Id is the development accountId
            this.currentUserId = 57541;
            this.currentUserIdLS = new List<int>() { currentUserId };

            this.findwiseClient = new Mock<IFindwiseClient>();
            this.learningHubService = new Mock<ILearningHubService>();
            this.resourceRepository = new Mock<IResourceRepository>();
            this.resourceService = new Mock<IResourceService>();
            this.searchService = new SearchService(
                this.learningHubService.Object,
                this.findwiseClient.Object,
                this.resourceRepository.Object,
                this.resourceService.Object,
                new NullLogger<SearchService>());


        }

        private List<ResourceActivityDTO> ResourceActivityDTOList => MockDataForOpenApiResourceTests.GetResourceActivityDTOList;

        private List<ResourceReferenceAndCatalogueDTO> ResourceReferenceAndCatalogueDTOList => MockDataForOpenApiResourceTests.GetResourceReferenceAndCatalogueDTOList;
        public static IEnumerable<object[]> TestFindwiseResultModel => new[]
        {
            new object[]
            {
                FindwiseResultModel.Success(
                    new SearchResultModel
                    {
                        DocumentList = new Documentlist
                        {
                            Documents = Builder<Document>.CreateListOfSize(4)
                                .TheFirst(1).With(d => d.CatalogueIds = new[] { 1, 3 }.ToList())
                                .TheNext(1).With(d => d.CatalogueIds = new[] { 8 }.ToList())
                                .TheNext(2).With(d => d.CatalogueIds = new[] { 17, 3 }.ToList())
                                .Build().ToArray(),
                        },
                        Stats = new Stats { TotalHits = 74 },
                    }),
            },
        };

        [Fact]
        public async Task SearchPassesQueryOnToFindwise()
        {
            // Given
            var searchRequest = new ResourceSearchRequest("search-text", 13, 46, 1, new[] { "dog", "pig" });
            this.findwiseClient.Setup(fc => fc.Search(It.IsAny<ResourceSearchRequest>()))
                .ReturnsAsync(FindwiseResultModel.Failure(FindwiseRequestStatus.Timeout));

            // When
            await this.searchService.Search(searchRequest, this.currentUserId);

            // Then
            this.findwiseClient.Verify(fc => fc.Search(searchRequest));
        }

        [Fact]
        public async Task SearchReturnsTotalHitsAndSearchResult()
        {
            // Given
            var searchRequest = new ResourceSearchRequest("search-text", 0, 100);
            var resources = Builder<ResourceReferenceAndCatalogueDTO>.CreateListOfSize(34)
                     .All()
                    .With(x => x.CatalogueDTOs = new List<CatalogueDTO>())
                    .Build();
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(It.IsAny<IEnumerable<int>>(), emptyOriginalResourceIdLS))
                .ReturnsAsync(resources);
            this.GivenFindwiseReturnsSuccessfulResponse(74, Enumerable.Range(1, 34));

            // When
            var searchResult = await this.searchService.Search(searchRequest, null);

            // Then
            searchResult.Resources.Count.Should().Be(34);
            searchResult.TotalNumResources.Should().Be(74);
        }

        [Fact]
        public async Task SearchResultsReturnExpectedValues()
        {
            // Given
            var searchRequest = new ResourceSearchRequest("search-text", 0, 100);
            const string link = "resources/1";
            this.learningHubService.Setup(lhs => lhs.GetResourceLaunchUrl(It.IsAny<int>()))
                .Returns(link);

            var currentResourceVersions = Builder<ResourceVersion>.CreateListOfSize(2)
                .TheFirst(1)
                .With(r => r.Title = "matching resource")
                .With(r => r.Description = "resource description")
                .Build().ToList();

            //var resources = Builder<ResourceReferenceAndCatalogueDTO>.CreateListOfSize(2) // qqqqdelete instead of builder maybe we to use TestMockData
            //    .TheFirst(1)
            //    .With(r => r.ResourceId = 1)
            //    .With(r => r.CurrentResourceVersion = currentResourceVersions[0]) // qqqqe doesnt exist are we missing something
            //    .With(
            //        r => r.ResourceReference = new[]
            //        {
            //            ResourceTestHelper.CreateResourceReferenceWithDetails(catalogueName: "catalogue1", id: 1),
            //        })
            //    .TheNext(1)
            //    .With(r => r.Id = 2)
            //    .With(r => r.CurrentResourceVersion = currentResourceVersions[1])
            //    .With(
            //        r => r.ResourceReference = new[]
            //        {
            //            ResourceTestHelper.CreateResourceReferenceWithDetails(catalogueName: "catalogue2", id: 2),
            //        })
            //    .With(r => r.ResourceTypeEnum = ResourceTypeEnum.Article)
            //    .Build();

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(It.IsAny<IEnumerable<int>>(), emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 2).ToList());
            this.GivenFindwiseReturnsSuccessfulResponse(2, new[] { 1, 2, 3 });

            // When
            var searchResult = await this.searchService.Search(searchRequest, this.currentUserId);

            // Then
            searchResult.Resources.Count.Should().Be(2);
            searchResult.TotalNumResources.Should().Be(2);
            searchResult.Resources[0].ResourceId.Should().Be(1);
            searchResult.Resources[0].Title.Should().Be("title1AudioNoActivitySummaryData");
            searchResult.Resources[0].Description.Should().Be("description1AudioActivitySummaryData");
            searchResult.Resources[0].References[0].Catalogue?.Name.Should().Be("catalogue1");
            searchResult.Resources[1].ResourceId.Should().Be(2);
            searchResult.Resources[1].References[0].RefId.Should().Be(101); // comes from originalResourceId
            searchResult.Resources[1].References[0].Catalogue?.Name.Should().Be("catalogue2");
            searchResult.Resources[1].References[0].Link.Should().Be(link);
            searchResult.Resources[1].ResourceType.Should().Be("Article");
        }

        [Fact]
        public async Task SearchReturnsResourcesInOrderMatchingFindwise() // qqqqf 
        {
            // Given
            this.findwiseClient.Setup(fc => fc.Search(It.IsAny<ResourceSearchRequest>())).ReturnsAsync(
                FindwiseResultModel.Success(
                    new SearchResultModel
                    {
                        DocumentList = new Documentlist
                        {
                            Documents = Builder<Document>.CreateListOfSize(3)
                                .TheFirst(1)
                                .With(d => d.Id = "1")
                                .TheNext(1)
                                .With(d => d.Id = "3")
                                .TheNext(1)
                                .With(d => d.Id = "2")
                                .Build().ToArray(),
                        },
                        Stats = new Stats { TotalHits = 2 },
                    }));

            var resources = Builder<Resource>.CreateListOfSize(3).Build();
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(new List<int>(){ 1, 3, 2 }, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 3).ToList());

            // When
            var searchResultModel = await this.searchService.Search(new ResourceSearchRequest("text", 0, 10), null);

            // Then
            searchResultModel.Resources.Select(r => r.ResourceId).Should().ContainInOrder(new[] { 1, 3, 2 });
        }

        [Fact]
        public async Task SearchReplacesNullPropertiesOfResourceWithDefaultValues()
        {
            // Given

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(It.IsAny<IEnumerable<int>>(), emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(3, 1).ToList());
            this.learningHubService.Setup(lhs => lhs.GetResourceLaunchUrl(It.IsAny<int>())).Returns(string.Empty);
            this.GivenFindwiseReturnsSuccessfulResponse(1, new[] { 1 });

            // When
            var searchResult = await this.searchService.Search(new ResourceSearchRequest("text", 0, 10), null);

            // Then
            using var scope = new AssertionScope();

            searchResult.Resources.Count.Should().Be(1);
            var outputResource = searchResult.Resources.Single();
            var expectedResourceReferences = new[]
            {
                new ResourceReferenceViewModel(
                    0, // qqqqc originalResourceId
                    new CatalogueViewModel(0, ResourceHelpers.NoCatalogueText, false),
                    string.Empty),
            }.ToList();
            outputResource.Should().BeEquivalentTo(
                new ResourceMetadataViewModel(
                    4, // qqqqc maybe should be null
                    "title4NullifiedExternalCatalogue",   // qqqqc no because its never null -> ResourceHelpers.NoResourceVersionText,
                    "description4NullifiedExternalCatalogue",                                      // qqqqc no because its never null -> string.Empty,
                    expectedResourceReferences,
                    "WebLink",
                    1, //Major version
                    0,
                    new List<MajorVersionIdActivityStatusDescription>() { }));

        }

        private void GivenFindwiseReturnsSuccessfulResponse(int totalHits, IEnumerable<int> resourceIds)
        {
            this.findwiseClient.Setup(fc => fc.Search(It.IsAny<ResourceSearchRequest>())).ReturnsAsync(
                FindwiseResultModel.Success(
                    new SearchResultModel
                    {
                        DocumentList = new Documentlist
                        {
                            Documents = resourceIds
                                .Select(id => Builder<Document>.CreateNew().With(d => d.Id = id.ToString()).Build())
                                .ToArray(),
                        },
                        Stats = new Stats { TotalHits = totalHits },
                    }));
        }
    }
}