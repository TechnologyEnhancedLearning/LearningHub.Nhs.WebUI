namespace LearningHub.Nhs.OpenApi.Tests.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using LearningHub.Nhs.OpenApi.Tests.TestHelpers;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    public class ResourceServiceTests
    {
        private readonly Mock<ILearningHubService> learningHubService;
        private readonly ResourceService resourceService;
        private readonly Mock<IResourceRepository> resourceRepository;
        private readonly int currentUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceTests"/> class.
        /// </summary>
        public ResourceServiceTests()
        {
            //This Id is the development accountId
            this.currentUserId = 57541;

            this.learningHubService = new Mock<ILearningHubService>();
            this.resourceRepository = new Mock<IResourceRepository>();
            this.resourceService = new ResourceService(this.learningHubService.Object, this.resourceRepository.Object, new NullLogger<ResourceService>());
        }

        private List<Resource> ResourceList => new List<Resource>()
        {
            ResourceTestHelper.CreateResourceWithDetails(id: 1, title: "title1", description: "description1", rating: 3m, resourceType: ResourceTypeEnum.Article),
            ResourceTestHelper.CreateResourceWithDetails(id: 2, hasCurrentResourceVersion: false, hasNodePath: false, resourceType: ResourceTypeEnum.Assessment),
            ResourceTestHelper.CreateResourceWithDetails(id: 3, title: "title2", description: "description2"),
            ResourceTestHelper.CreateResourceWithDetails(id: 4),
            ResourceTestHelper.CreateResourceWithDetails(id: 5, hasRatingSummary: false),
            ResourceTestHelper.CreateResourceWithDetails(id: 6, resourceReferenceId:6,deleted:false, resourceType: (ResourceTypeEnum)999), // resource with resourceType that does not exist on ResourceTypeEnum to check error handling
            ResourceTestHelper.CreateResourceWithDetails(
                id: 302,
                resourceReferenceId:303,
                deleted:false,
                catalogueName : "default catalogue name",
                amendDate : null, 
                amendUserId : 57541,
                createDate : null, 
                createUserId : 57541,
                title: "titleTwoOneWithOneWithoutSummary",
                description: "descriptiontitleTwoOneWithOneWithoutSummary", 
                hasNodePath :true,
                hasCurrentResourceVersion:true,
                hasRatingSummary:true,
                rating: 3m,
                resourceType: ResourceTypeEnum.Article,
                userId : this.currentUserId,
                activityStatusId : 3 //activityStatusIdCompleted
                ),
            ResourceTestHelper.CreateResourceWithDetails(
                id: 302,
                resourceReferenceId:303,
                deleted:false,
                catalogueName : "default catalogue name",
                amendDate : null,
                amendUserId : 57541,
                createDate : null,
                createUserId : 57541,
                title: "titleTwoOneWithOneWithoutSummary",
                description: "descriptiontitleTwoOneWithOneWithoutSummary",
                hasNodePath :true,
                hasCurrentResourceVersion:true,
                hasRatingSummary:true,
                rating: 3m,
                resourceType: ResourceTypeEnum.Article,
                userId : this.currentUserId,
                activityStatusId : null //No activity status present
                ),
        };

        private List<ResourceReference> ResourceReferenceList => new List<ResourceReference>()
        {
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 100, originalResourceReferenceId: 1, resource: this.ResourceList[0], catalogueName: "catalogue1"),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 101, originalResourceReferenceId: 2, resource: this.ResourceList[1]),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 102, originalResourceReferenceId: 3, resource: this.ResourceList[1], hasNodeVersion: false),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 103, originalResourceReferenceId: 4, resource: this.ResourceList[0], hasCatalogueNodeVersion: false),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 104, originalResourceReferenceId: 5, resource: this.ResourceList[2]),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 105, originalResourceReferenceId: 6, resource: this.ResourceList[1], hasNodePath: false, catalogueName: "catalogue2"),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 106, originalResourceReferenceId: 7, resource: this.ResourceList[3], deleted: true, catalogueName: "deleted"),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 107, originalResourceReferenceId: 8, resource: this.ResourceList[4], catalogueName: "catalogue3"),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 108, originalResourceReferenceId: 9, resource: this.ResourceList[5], catalogueNodeVersionIsRestricted: true),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 109, originalResourceReferenceId: 10, resource: this.ResourceList[0]),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 110, originalResourceReferenceId: 10, resource: this.ResourceList[0]),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 303, originalResourceReferenceId: 302, resource: this.ResourceList[6]),
            ResourceTestHelper.CreateResourceReferenceWithDetails(id: 303, originalResourceReferenceId: 302, resource: this.ResourceList[7]),
        };

        [Fact]
        public async Task SingleResourceEndpointReturnsTheCorrectInformationIfThereIsAMatchingResource()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 1 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(1, currentUserId);

            // Then
            x.Rating.Should().Be(3);
            x.Catalogue.Name.Should().Be("catalogue1");
            x.Title.Should().Be("title1");
            x.ResourceType.Should().Be("Article");
            x.UserSummaryActvityStatus.Should().Be(String.Empty);
        }

       [Fact]
        public async Task GetResourceByIdReturnsTheCorrectInformationIfThereIsAMatchingResourceNoUserId()
        {
            int resourceId = 100;
            List<int> singleResourceIdList = new List<int>() { resourceId };
            int? currentUserIdNull = null;

            // Given
            this.resourceRepository.Setup(expression: rr => rr.GetResourcesFromIds(singleResourceIdList))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 1).Select(x => x.Resource).ToList());


            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserIdNull);

            // Then
  
            result.Rating.Should().Be(3);
            result.Title.Should().Be("title1");
            result.ResourceType.Should().Be("Article");
            result.UserSummaryActvityStatus.Should().Be(String.Empty);
        }

        [Fact]
        public async Task GetResourceByIdReturnsTheCorrectInformationIfThereIsAMatchingResourceWithUserId()
        {
            int resourceId = 100;
            List<int> singleResourceIdList = new List<int>() { resourceId };

            // Given
            this.resourceRepository.Setup(expression: rr => rr.GetResourcesFromIds(singleResourceIdList, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(11, 1).Select(x => x.Resource).ToList());

            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserId);

            // Then
            result.Rating.Should().Be(3);
            result.Title.Should().Be("titleTwoOneWithOneWithoutSummary");
            result.ResourceType.Should().Be("Article");
            result.UserSummaryActvityStatus.Should().Be("Completed");
        }

        [Fact]
        public async Task SingleResourceReturnsA404IfTheresNoResourceReferenceWithAMatchingId()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 999 }, currentUserId))
                .ReturnsAsync(new List<ResourceReference>());

            // When / Then
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await this.resourceService.GetResourceReferenceByOriginalId(999, currentUserId));
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ResponseBody.Should().Be("No matching resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAResourceMetadataViewModelObjectWithAMessageSayingNoResourceIfThereIsNoCurrentResourceVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 2 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(1, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(2, currentUserId);

            // Then
            x.Title.Should().Be("No current resource version");
            x.Rating.Should().Be(0);
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoNodeVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 3 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(2, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(3, currentUserId);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoNodePath()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 4 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(3, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(4, currentUserId);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoCatalogueNodeVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 6 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(5, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(6, currentUserId);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAZeroForRatingIfTheresNoRatingSummary()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 8 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(7, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(8, currentUserId);

            // Then
            x.Catalogue.Name.Should().Be("catalogue3");
            x.Rating.Should().Be(0);
        }

        [Fact]
        public async Task SingleResourceEndpointThrowsAnErrorAndReturnsABlankStringIfTheresANullResourceType()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 9 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(8, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(9, currentUserId);

            // Then
            x.ResourceType.Should().Be(string.Empty);
        }

        [Fact]
        public async Task SingleResourceEndpointThrowsAnErrorIfThereIsMoreThanOneResourceReferenceWithTheSameOriginalId()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 10 }, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(9, 2));

            // When / Then
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.resourceService.GetResourceReferenceByOriginalId(10, currentUserId));
        }

        /*[Fact]
        public async Task SingleResourceEndpointReturnsCorrectValueForRestrictedAccess()
        {
            // todo 12894 write this test when we have restricted access models
        }*/

        [Fact]
        public async Task BulkEndpointReturnsAllMatchingResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 1, 2 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 2));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.ResourceReferences.Count.Should().Be(2);
            x.UnmatchedResourceReferenceIds.Count.Should().Be(0);
            x.ResourceReferences[0].Rating.Should().Be(3);
            x.ResourceReferences[0].Catalogue.Name.Should().Be("catalogue1");
            x.ResourceReferences[0].ResourceType.Should().Be("Article");
            x.ResourceReferences[1].ResourceId.Should().Be(1);
            x.ResourceReferences[1].ResourceType.Should().Be("Assessment");
            x.ResourceReferences[0].UserSummaryActvityStatus.Should().Be(String.Empty);
        }

        [Fact]
        public async Task BulkEndpointReturnsA404IfThereAreNoMatchingResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 998, 999 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp, currentUserId))
                .ReturnsAsync(new List<ResourceReference>());

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.UnmatchedResourceReferenceIds.Count.Should().Be(2);
            x.ResourceReferences.Count.Should().Be(0);
        }

        [Fact]
        public async Task BulkEndpointReturnsResourcesWithIncompleteInformation()
        {
            // Given
            var idsToLookUp = new List<int>() { 1, 2, 3, 4 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 4));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.ResourceReferences.Count.Should().Be(4);
            x.ResourceReferences[0].Rating.Should().Be(3);
            x.ResourceReferences[1].Title.Should().Be("No current resource version");
            x.ResourceReferences[2].Catalogue.Name.Should().Be("No catalogue for resource reference");
            x.ResourceReferences[3].Title.Should().Be("title1");
            x.ResourceReferences[0].UserSummaryActvityStatus.Should().Be(String.Empty);
        }

        [Fact]
        public async Task BulkEndpointReturnsUnmatchedResourcesWithMatchedResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 1, 999 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.ResourceReferences.Count.Should().Be(1);
            x.ResourceReferences[0].Title.Should().Be("title1");
            x.UnmatchedResourceReferenceIds.Count.Should().Be(1);
        }

        [Fact]
        public async Task ResourceServiceReturnsTheOriginalResourceReferenceIdAsTheRefIdForBulkEndpoint()
        {
            // Given
            List<int> list = new List<int>()
            {
                6, 7,
            };
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(5, 2));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(list, currentUserId);

            // Then
            x.ResourceReferences[0].RefId.Should().Be(6);
            x.ResourceReferences[1].RefId.Should().Be(7);
        }

        [Fact]
        public async Task ResourceServiceReturnsTheOriginalResourceReferenceIdAsTheRefIdForSingleResourceEndpoint()
        {
            // Given
            List<int> list = new List<int>() { 6 };
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(5, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(6, currentUserId);

            // Then
            x.RefId.Should().Be(6);
        }

        [Fact]
        public async Task ResourceServiceReturnsThatARestrictedCatalogueIsRestricted()
        {
            // Given
            List<int> list = new List<int>() { 9 };
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(8, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(9, currentUserId);

            // Then
            x.Catalogue.IsRestricted.Should().BeTrue();
        }

        [Fact]
        public async Task ResourceServiceReturnsThatAnUnrestrictedCatalogueIsUnrestricted()
        {
            // Given
            List<int> list = new List<int>() { 8 };
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list, currentUserId))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(7, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(8, currentUserId);

            // Then
            x.Catalogue.IsRestricted.Should().BeFalse();
        }


        [Theory]
        [InlineData("302,301", 57541, "Completed", 11, 1)]
        [InlineData("302", 57541, "Completed", 11, 1)]
        [InlineData("302,301", 57541, "", 12, 1)]
        [InlineData("302", 57541, "", 12, 1)]
        [InlineData("302,301", 57541, "Completed", 11, 2)]
        public async Task GetResourceReferencesByOriginalIdsReturnsResourceWithCorrectUserSummaryActvityStatus(string originalResourceIdsStr, int currentUserId, string expectedUserSummaryActvityStatus, int rangeStart, int rangeLength) 
        {
            // Given
            List<int> originalResourceIds = originalResourceIdsStr.Split(",").Select(x=>Int32.Parse(x)).ToList();

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(originalResourceIds, currentUserId))//zero is to allow for the null there is no user with id 0
                .ReturnsAsync(this.ResourceReferenceList.GetRange(rangeStart, rangeLength));
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(originalResourceIds))//zero is to allow for the null there is no user with id 0
                .ReturnsAsync(this.ResourceReferenceList.GetRange(rangeStart, rangeLength));


            // When
            var result = await this.resourceService.GetResourceReferencesByOriginalIds(originalResourceIds, currentUserId);

            // Then
            result.ResourceReferences[0].UserSummaryActvityStatus.Should().Be(expectedUserSummaryActvityStatus);
        }

        [Theory]
        [InlineData(303, 57541, "Completed", 11, 1)]
        [InlineData(303, 57541, "", 12, 1)]
        [InlineData(1, null, "", 0, 1)]
        public async Task GetResourceByIdReturnsResourceWithCorrectUserSummaryActivityStatus(int resourceId, int? currentUserId, string expectedUserSummaryActivityStatus, int rangeStart, int rangeLength)
        {
            List<int> singleResourceIdList = new List<int>() { resourceId };
            currentUserId = currentUserId ?? 0; //zero is to allow for the null there is no user with id 0
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(singleResourceIdList, currentUserId.Value))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(rangeStart, rangeLength).Select(x => x.Resource).ToList());
            this.resourceRepository.Setup(expression: rr => rr.GetResourcesFromIds(singleResourceIdList))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(rangeStart, rangeLength).Select(x => x.Resource).ToList());


            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserId);

            // Then
            result.UserSummaryActvityStatus.Should().Be(expectedUserSummaryActivityStatus);
    }


}
}
