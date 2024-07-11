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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public sealed class ResourceControllerTests
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
            int? currentUserId = null; //E.g if hitting endpoint with ApiKey auth
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
                service => service.Search(It.Is<ResourceSearchRequest>(request => request.Limit == 12), currentUserId));
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

        [Fact]
        public void CurrentUserIdSetByAuth()
        {
            // Arrange
            ResourceController resourceController = new ResourceController(
                this.searchService.Object,
                this.resourceService.Object,
                this.findwiseConfigOptions.Object
            );


            // This Id is the development accountId
            int currentUserId = 57541;

            // Create claims identity with the specified user id
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation"); // Set the authentication type to "Federation"

            // Create claims principal with the claims identity
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Create a mock HttpContext and set it to the ControllerContext
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            var controllerContext = new ControllerContext { HttpContext = httpContext };
            resourceController.ControllerContext = controllerContext;

            // Act

            // Assert that the CurrentUserId property of the resourceController matches the currentUserId
            Assert.Equal(currentUserId, resourceController.CurrentUserId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(20)]
        [InlineData(46)]
        public async Task SearchEndpointUsesPassedInLimitIfGiven(int limit)
        {
            // Given
            int? currentUserId = null; //E.g if hitting endpoint with ApiKey auth
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
                service => service.Search(It.Is<ResourceSearchRequest>(request => request.Limit == limit), currentUserId));
        }

        [Fact]
        public async Task GetResourceReferencesByCompleteThrowsErrorWhenNoUserId()
        {
            // When
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await this.resourceController.GetResourceReferencesByComplete();
            });

            // Then
            Assert.Equal("User Id required.", exception.Message);
        }

        [Fact]
        public async Task GetResourceReferencesByInProgressThrowsErrorWhenNoUserId()
        {
            // When
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await this.resourceController.GetResourceReferencesByInProgress();
            });

            // Then
            Assert.Equal("User Id required.", exception.Message);
        }

        [Fact]
        public async Task GetResourceReferencesBycertificatesThrowsErrorWhenNoUserId()
        {
            // When
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await this.resourceController.GetResourceReferencesByCertificates();
            });

            // Then
            Assert.Equal("User Id required.", exception.Message);
        }

        private void GivenDefaultLimitForFindwiseSearchIs(int limit)
        {
            this.findwiseConfigOptions.Setup(options => options.Value)
                .Returns(new FindwiseConfig { DefaultItemLimitForSearch = limit });
        }

        private void GivenSearchServiceFailsWithStatus(FindwiseRequestStatus status)
        {
            int? currentUserId = null; //E.g if hitting endpoint with ApiKey auth
            this.searchService.Setup(ss => ss.Search(It.IsAny<ResourceSearchRequest>(), currentUserId)).ReturnsAsync(
                new ResourceSearchResultModel(new List<ResourceMetadataViewModel>(), status, 0));
        }

        private void GivenSearchServiceSucceedsButFindsNoItems()
        {
            int? currentUserId = null; //E.g if hitting endpoint with ApiKey auth
            this.searchService.Setup(ss => ss.Search(It.IsAny<ResourceSearchRequest>(), currentUserId)).ReturnsAsync(
                new ResourceSearchResultModel(new List<ResourceMetadataViewModel>(), FindwiseRequestStatus.Success, 0));
        }

    }
}
