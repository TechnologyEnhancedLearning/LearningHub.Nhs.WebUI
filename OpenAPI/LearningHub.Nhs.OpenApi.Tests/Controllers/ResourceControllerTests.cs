// <copyright file="ResourceControllerTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using LearningHub.NHS.OpenAPI.Controllers;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Options;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    public sealed class ResourceControllerTests : IDisposable
    {
        private readonly Mock<ISearchService> searchService;
        private readonly Mock<IResourceService> resourceService;
        private readonly Mock<IOptions<FindwiseConfig>> findwiseConfigOptions;
        private ResourceController? resourceController;

        public ResourceControllerTests()
        {
            this.searchService = new Mock<ISearchService>();
            this.findwiseConfigOptions = new Mock<IOptions<FindwiseConfig>>();
            this.resourceService = new Mock<IResourceService>();
            this.resourceController = new ResourceController(this.searchService.Object, this.resourceService.Object, this.findwiseConfigOptions.Object);
        }

        [Fact]
        public async Task SearchEndpointReturnsUnsuccessfulStatusIfFindwiseResultUnsuccessful()
        {
            // Given
            this.GivenSearchServiceFailsWithStatus(FindwiseRequestStatus.Timeout);
            this.GivenDefaultLimitForFindwiseSearchIs(12);
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);

            // When / Then
            var exception =
                await Assert.ThrowsAsync<HttpResponseException>(() => this.resourceController.Search("stuff"));
            exception.StatusCode.Should().Be(HttpStatusCode.GatewayTimeout);
        }

        [Fact]
        public async Task SearchEndpointReturnsBadRequestIfGivenNegativeOffset()
        {
            // Given
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);

            // When / Then
            var exception =
                await Assert.ThrowsAsync<HttpResponseException>(
                    () => this.resourceController.Search("stuff", offset: -1));
            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SearchEndpointReturnsBadRequestIfGivenNegativeLimit()
        {
            // Given
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);

            // When / Then
            var exception =
                await Assert.ThrowsAsync<HttpResponseException>(
                    () => this.resourceController.Search("stuff", limit: -1));
            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SearchEndpointUsesDefaultLimitGivenInConfig()
        {
            // Given
            this.GivenSearchServiceSucceedsButFindsNoItems();
            this.GivenDefaultLimitForFindwiseSearchIs(12);
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);

            // When
            await this.resourceController.Search("Bob Mortimer");

            // Then
            this.searchService.Verify(
                service => service.Search(It.Is<ResourceSearchRequest>(request => request.Limit == 12)));
        }

        [Fact]
        public async Task BulkGetEndpointReturns400IfGivenAListWithMoreThan1000Elements()
        {
            // Given
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);
            var bigList = Builder<int>.CreateListOfSize(1001).Build().ToList();

            // When
            var exception =
                await Assert.ThrowsAsync<HttpResponseException>(
                    () => this.resourceController.GetResourceReferencesByOriginalIds(bigList));

            // Then
            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task BulkJsonGetEndpointReturns400IfGivenAListWithMoreThan1000Elements()
        {
            // Given
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);
            var bigList = Builder<int>.CreateListOfSize(1001).Build().ToList();
            var resourceIdsObject = new { referenceIds = bigList };
            var jsonWithTooManyResourceIds = JsonConvert.SerializeObject(resourceIdsObject);

            // When
            var exception =
                await Assert.ThrowsAsync<HttpResponseException>(
                    () => this.resourceController.GetResourceReferencesByOriginalIdsFromJson(jsonWithTooManyResourceIds));

            // Then
            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task BulkJsonGetEndpointThrowsExceptionIfGivenANonJsonString()
        {
            // Given
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);
            const string? notAJsonString = "randomString";

            // When
            await Assert.ThrowsAsync<JsonReaderException>(
                    () => this.resourceController.GetResourceReferencesByOriginalIdsFromJson(notAJsonString));
        }

        [Fact]
        public async Task BulkJsonGetEndpointReturns400IfGivenAJsonWithIncorrectKey()
        {
            // Given
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);
            var randomObject = new { notTheRightKey = new List<int> { 1, 2, 3 } };
            var jsonWithWrongKey = JsonConvert.SerializeObject(randomObject);

            // When
            var exception =
                await Assert.ThrowsAsync<HttpResponseException>(
                    () => this.resourceController.GetResourceReferencesByOriginalIdsFromJson(jsonWithWrongKey));

            // Then
            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(20)]
        [InlineData(46)]
        public async Task SearchEndpointUsesPassedInLimitIfGiven(int limit)
        {
            // Given
            this.GivenSearchServiceSucceedsButFindsNoItems();
            this.GivenDefaultLimitForFindwiseSearchIs(20);
            this.resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object);

            // When
            await this.resourceController.Search("Bob Mortimer", limit: limit);

            // Then
            this.searchService.Verify(
                service => service.Search(It.Is<ResourceSearchRequest>(request => request.Limit == limit)));
        }

        public void Dispose()
        {
            this.resourceController?.Dispose();
        }

        private void GivenDefaultLimitForFindwiseSearchIs(int limit)
        {
            this.findwiseConfigOptions.Setup(options => options.Value)
                .Returns(new FindwiseConfig { DefaultItemLimitForSearch = limit });
        }

        private void GivenSearchServiceFailsWithStatus(FindwiseRequestStatus status)
        {
            this.searchService.Setup(ss => ss.Search(It.IsAny<ResourceSearchRequest>())).ReturnsAsync(
                new ResourceSearchResultModel(new List<ResourceMetadataViewModel>(), status, 0));
        }

        private void GivenSearchServiceSucceedsButFindsNoItems()
        {
            this.searchService.Setup(ss => ss.Search(It.IsAny<ResourceSearchRequest>())).ReturnsAsync(
                new ResourceSearchResultModel(new List<ResourceMetadataViewModel>(), FindwiseRequestStatus.Success, 0));
        }
    }
}
