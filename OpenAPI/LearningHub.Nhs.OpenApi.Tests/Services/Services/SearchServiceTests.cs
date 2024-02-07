// <copyright file="SearchServiceTests.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Tests.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
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

        public SearchServiceTests()
        {
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
            await this.searchService.Search(searchRequest);

            // Then
            this.findwiseClient.Verify(fc => fc.Search(searchRequest));
        }

        [Fact]
        public async Task SearchReturnsTotalHitsAndSearchResult()
        {
            // Given
            var searchRequest = new ResourceSearchRequest("search-text", 0, 100);
            var resources = Builder<Resource>.CreateListOfSize(34).Build();
            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(resources);
            this.GivenFindwiseReturnsSuccessfulResponse(74, Enumerable.Range(1, 34));

            // When
            var searchResult = await this.searchService.Search(searchRequest);

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

            var resources = Builder<Resource>.CreateListOfSize(2)
                .TheFirst(1)
                .With(r => r.Id = 1)
                .With(r => r.CurrentResourceVersion = currentResourceVersions[0])
                .With(
                    r => r.ResourceReference = new[]
                    {
                        ResourceTestHelper.CreateResourceReferenceWithDetails(catalogueName: "catalogue1", id: 1),
                    })
                .TheNext(1)
                .With(r => r.Id = 2)
                .With(r => r.CurrentResourceVersion = currentResourceVersions[1])
                .With(
                    r => r.ResourceReference = new[]
                    {
                        ResourceTestHelper.CreateResourceReferenceWithDetails(catalogueName: "catalogue2", id: 2),
                    })
                .With(r => r.ResourceTypeEnum = ResourceTypeEnum.Article)
                .Build();

            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(resources);
            this.GivenFindwiseReturnsSuccessfulResponse(2, new[] { 1, 2, 3 });

            // When
            var searchResult = await this.searchService.Search(searchRequest);

            // Then
            searchResult.Resources.Count.Should().Be(2);
            searchResult.TotalNumResources.Should().Be(2);
            searchResult.Resources[0].ResourceId.Should().Be(1);
            searchResult.Resources[0].Title.Should().Be("matching resource");
            searchResult.Resources[0].Description.Should().Be("resource description");
            searchResult.Resources[0].References[0].Catalogue?.Name.Should().Be("catalogue1");
            searchResult.Resources[1].ResourceId.Should().Be(2);
            searchResult.Resources[1].References[0].RefId.Should().Be(2);
            searchResult.Resources[1].References[0].Catalogue?.Name.Should().Be("catalogue2");
            searchResult.Resources[1].References[0].Link.Should().Be(link);
            searchResult.Resources[1].ResourceType.Should().Be("Article");
        }

        [Fact]
        public async Task SearchReturnsResourcesInOrderMatchingFindwise()
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
            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(new[] { 1, 3, 2 }))
                .ReturnsAsync(resources);

            // When
            var searchResultModel = await this.searchService.Search(new ResourceSearchRequest("text", 0, 10));

            // Then
            searchResultModel.Resources.Select(r => r.ResourceId).Should().ContainInOrder(new[] { 1, 3, 2 });
        }

        [Fact]
        public async Task SearchReplacesNullPropertiesOfResourceWithDefaultValues()
        {
            // Given
            var resource = new[]
            {
                Builder<Resource>.CreateNew()
                    .With(r => r.Id = 1)
                    .With(r => r.CurrentResourceVersion = null)
                    .With(
                        r => r.ResourceReference = new[]
                        {
                            Builder<ResourceReference>.CreateNew()
                                .With(rr => rr.Id = 1)
                                .With(rr => rr.NodePath = null)
                                .Build(),
                        })
                    .With(r => r.ResourceTypeEnum = ResourceTypeEnum.Article)
                    .Build(),
            };
            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(resource);
            this.learningHubService.Setup(lhs => lhs.GetResourceLaunchUrl(It.IsAny<int>())).Returns(string.Empty);
            this.GivenFindwiseReturnsSuccessfulResponse(1, new[] { 1 });

            // When
            var searchResult = await this.searchService.Search(new ResourceSearchRequest("text", 0, 10));

            // Then
            using var scope = new AssertionScope();

            searchResult.Resources.Count.Should().Be(1);
            var outputResource = searchResult.Resources.Single();
            var expectedResourceReferences = new[]
            {
                new ResourceReferenceViewModel(
                    1,
                    new CatalogueViewModel(0, ResourceHelpers.NoCatalogueText, false),
                    string.Empty),
            }.ToList();
            outputResource.Should().BeEquivalentTo(
                new ResourceMetadataViewModel(
                    1,
                    ResourceHelpers.NoResourceVersionText,
                    string.Empty,
                    expectedResourceReferences,
                    "Article",
                    0));
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