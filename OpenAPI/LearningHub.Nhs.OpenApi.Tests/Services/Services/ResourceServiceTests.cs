namespace LearningHub.Nhs.OpenApi.Tests.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.WebSockets;
    using System.Threading.Tasks;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using LearningHub.Nhs.OpenApi.Tests.TestHelpers;
    using LearningHub.Nhs.OpenApi.Tests.TestMockData;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    public class ResourceServiceTests
    {
        private readonly Mock<ILearningHubService> learningHubService;
        private readonly ResourceService resourceService;
        private readonly Mock<IResourceRepository> resourceRepository;
        private readonly int currentUserId;
        private readonly List<int> currentUserIdLS;
        private readonly List<int> emptyOriginalResourceIdLS = new List<int>() { }; // for readabiliy
        private readonly List<int> emptyResourceIdLS = new List<int>() { }; // for readabiliy

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceTests"/> class.
        /// </summary>
        public ResourceServiceTests()
        {
            //This Id is the development accountId
            this.currentUserId = 57541;
            this.currentUserIdLS = new List<int>() { currentUserId };

            this.learningHubService = new Mock<ILearningHubService>();
            this.resourceRepository = new Mock<IResourceRepository>();
            this.resourceService = new ResourceService(this.learningHubService.Object, this.resourceRepository.Object, new NullLogger<ResourceService>());
        }

        // Note : Completed when changed due to resourcetype -->    Article(1) Image(5) Case(10) = Viewed ... WebLink(8) -> launched ... GenericFile(9) -> downloaded
        private List<ResourceActivityDTO> ResourceActivityDTOList => MockDataForOpenApiResourceTests.GetResourceActivityDTOList;

        private List<ResourceReferenceAndCatalogueDTO> ResourceReferenceAndCatalogueDTOList => MockDataForOpenApiResourceTests.GetResourceReferenceAndCatalogueDTOList;

        [Fact]
        public async Task SingleResourceEndpointReturnsTheCorrectInformationIfThereIsAMatchingResource()
        {
            // Given
            int originalResourceId = 100;
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { originalResourceId }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, currentUserId);

            // Then
            x.Rating.Should().Be(3);
            x.Catalogue.Name.Should().Be("catalogue1");
            x.Title.Should().Be("title1AudioNoActivitySummaryData");
            x.ResourceType.Should().Be("Assessment");
            x.UserSummaryActivityStatuses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetResourceByIdReturnsTheCorrectInformationIfThereIsAMatchingResourceNoUserId()
        {
            int resourceId = 1;
            List<int> singleResourceIdList = new List<int>() { resourceId };
            int? currentUserIdNull = null;

            // Given
            this.resourceRepository.Setup(expression: rr => rr.GetResourceReferenceAndCatalogues(singleResourceIdList, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1).ToList());

            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserIdNull);

            // Then
            result.Rating.Should().Be(3);
            result.Title.Should().Be("title1AudioNoActivitySummaryData");
            result.ResourceType.Should().Be("Assessment");
            result.UserSummaryActivityStatuses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetResourceByIdReturnsTheCorrectInformationIfThereIsAMatchingResourceWithUserId()
        {
            int resourceId = 2;
            List<int> singleResourceIdList = new List<int>() { resourceId };

            // Given
            this.resourceRepository.Setup(expression: rr => rr.GetResourceReferenceAndCatalogues(singleResourceIdList, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(1, 1).ToList());

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(singleResourceIdList, currentUserIdLS))
                .ReturnsAsync(this.ResourceActivityDTOList.GetRange(0, 1));

            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserId);

            // Then
            result.Rating.Should().Be(0);
            result.Title.Should().Be("title2Article");
            result.ResourceType.Should().Be("Article");
            result.UserSummaryActivityStatuses[0].MajorVersionId.Should().Be(1);
            result.UserSummaryActivityStatuses[0].ActivityStatusDescription.Should().Be("Viewed");
        }

        [Fact]
        public async Task SingleResourceReturnsA404IfTheresNoResourceReferenceWithAMatchingId()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 999 }))
                .ReturnsAsync(new List<ResourceReferenceAndCatalogueDTO>() { });

            // When / Then
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await this.resourceService.GetResourceReferenceByOriginalId(999, currentUserId));
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ResponseBody.Should().Be("No matching resource reference");
        }

        [Fact] // qqqqc not currently possible because no longer left joining resourceVersion, but it is nullable in the db, and title isnt
        public async Task SingleResourceEndpointReturnsAResourceMetadataViewModelObjectWithAMessageSayingNoResourceIfThereIsNoCurrentResourceVersion()
        {
            // Given
            int originalResourceId = 101; // qqqqc we dont have example data mocked
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { originalResourceId }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(1, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, null);

            // Then
            x.Title.Should().Be("No current resource version");
            x.Rating.Should().Be(0);
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoNodeVersion()
        {
            // Given
            int originalResourceId = 104;
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { originalResourceId }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(3, 1));
            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, null);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }
        /*
                qqqqc based on the data dont think this should occur
                nodepathid in rref is nullable but never is in the data
                currentNodeVersion is nullable in node has only 40 null entries that arnt deleted
                catalogueNodeVersion nodeversionId is nullable has no null entries
         */
        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoNodePath()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 4 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(3, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(4, null);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoCatalogueNodeVersion()
        {
            // The stored procedure joins so the only way this happens is if it nullifies the catalogue information because its an external catalogue qqqqc
            // Removed the catalogue check
            // Given
            int originalResourceId = 104;
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { originalResourceId }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(3, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, null);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }
        [Fact]
        public async Task SingleResourceEndpointNoCatalogueDefaultTest()
        {
            // The stored procedure joins so the only way this happens is if it nullifies the catalogue information because its an external catalogue
            // Given
            int originalResourceId = 104;
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { originalResourceId }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(3, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, null);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        //[Fact] // qqqqdelete happens in stored procedure so unneeded
        //public async Task SingleResourceEndpointReturnsAZeroForRatingIfTheresNoRatingSummary()
        //{
        //    // Given
        //    int originalResourceId = ;
        //    this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 8 }))
        //        .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(7, 1));

        //    // When
        //    var x = await this.resourceService.GetResourceReferenceByOriginalId(8, currentUserId);

        //    // Then
        //    x.Catalogue.Name.Should().Be("catalogue3");
        //    x.Rating.Should().Be(0);
        //}

        //[Fact] qqqqdelete
        //public async Task SingleResourceEndpointThrowsAnErrorAndReturnsABlankStringIfTheresANullResourceType() 
        //{
        //    // Given
        //    int originalResourceId = 9;
        //    this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { originalResourceId }))
        //        .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(8, 1));

        //    // When
        //    var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, currentUserId);

        //    // Then
        //    x.ResourceType.Should().Be(string.Empty);
        //}

        [Fact]
        public async Task SingleResourceEndpointThrowsAnErrorIfThereIsMoreThanOneResourceReference()
        {
            // Given
            int originalResourceId = 102; // However the test deliberately returns data for if 102 and 103 had been requested
            // Note if searching by a single originalResourceId this resourceDTO would be the same except with a single catalogue for just that originalResourceId
            List<ResourceReferenceAndCatalogueDTO> resourceWithTwoOriginalResourceIdsS = this.ResourceReferenceAndCatalogueDTOList.GetRange(2, 1);
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { originalResourceId }))
                .ReturnsAsync(resourceWithTwoOriginalResourceIdsS);

            // When / Then

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, null));

            Assert.Equal($"For one originalResourceId only one CatalogueDTOs should be found. Count was {resourceWithTwoOriginalResourceIdsS[0].CatalogueDTOs.Count}.", exception.Message);
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
            var idsToLookUp = new List<int>() { 100, 101 };
            List<int> foundResourceIds = new List<int>() { 1,2}; 

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 2));

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(foundResourceIds, currentUserIdLS))
                .ReturnsAsync(this.ResourceActivityDTOList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.ResourceReferences.Count.Should().Be(2);
            x.UnmatchedResourceReferenceIds.Count.Should().Be(0);
            x.ResourceReferences[0].Rating.Should().Be(3);
            x.ResourceReferences[0].Catalogue.Name.Should().Be("catalogue1");
            x.ResourceReferences[0].ResourceType.Should().Be("Assessment");
            x.ResourceReferences[1].ResourceId.Should().Be(2);
            x.ResourceReferences[1].ResourceType.Should().Be("Article");
            x.ResourceReferences[0].MajorVersion.Should().Be(1);
            x.ResourceReferences[0].UserSummaryActivityStatuses.Should().BeEmpty();
            x.ResourceReferences[1].UserSummaryActivityStatuses.Should().HaveCount(1);
            x.ResourceReferences[1].UserSummaryActivityStatuses[0].ActivityStatusDescription.Should().Be("Viewed"); // Completed for type Article becomes Viewed
        }

        [Fact]
        public async Task BulkEndpointReturnsA404IfThereAreNoMatchingResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 998, 999 };
            List<int> foundResourceIds = new List<int>() {  };

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(new List<ResourceReferenceAndCatalogueDTO>());

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(foundResourceIds, currentUserIdLS))
            .ReturnsAsync(this.ResourceActivityDTOList.GetRange(0, 0));// None

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
            var idsToLookUp = new List<int>() { 100, 101, 102, 103, 104 }; // 102, 103 are associated with resourceId 3 and 104 with 4 and having no catalogue data returned due to external catalogue wiping
            List<int> foundResourceIds = new List<int>() { 1, 2, 3, 4 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 4));

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(foundResourceIds, currentUserIdLS))
            .ReturnsAsync(this.ResourceActivityDTOList.GetRange(0, 4));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.ResourceReferences.Count.Should().Be(5); //One for each originalResourceId because it returns flattened data and they each have 
            x.UnmatchedResourceReferenceIds.Count().Should().Be(0); // I think this should be 0 not 1 for originalResourceId 104 because we do match it, it just has no catalogue so its resource is found qqqqa
            x.ResourceReferences[0].Rating.Should().Be(3);
            x.ResourceReferences[4].Catalogue.Name.Should().Be("title4NullifiedExternalCatalogue"); 
            x.ResourceReferences[1].Title.Should().Be("title2Article");
            x.ResourceReferences[0].UserSummaryActivityStatuses.Should().BeEmpty();
        }

        [Fact]
        public async Task BulkEndpointReturnsUnmatchedResourcesWithMatchedResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 1, 999 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.ResourceReferences.Count.Should().Be(1);
            x.ResourceReferences[0].Title.Should().Be("title1AudioNoActivitySummaryData");
            x.UnmatchedResourceReferenceIds.Count.Should().Be(1); // qqqqa again fails because originalResourceId nullified
        }

        [Fact]
        public async Task ResourceServiceReturnsTheOriginalResourceReferenceIdAsTheRefIdForBulkEndpoint()
        {
            // Given
            List<int> list = new List<int>()
            {
                100, 101,
            };
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, list))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 2));

            //, currentUserId

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(list, currentUserId);

            // Then
            x.ResourceReferences[0].RefId.Should().Be(100);
            x.ResourceReferences[1].RefId.Should().Be(101);
        }

        [Fact]
        public async Task ResourceServiceReturnsTheOriginalResourceReferenceIdAsTheRefIdForSingleResourceEndpoint()
        {
            // Given
            int originalResourceId = 100;
            List<int> list = new List<int>() { originalResourceId };
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, list))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, currentUserId);

            // Then
            x.RefId.Should().Be(originalResourceId);
        }

        [Fact]
        public async Task ResourceServiceReturnsThatARestrictedCatalogueIsRestricted()
        {
            // Given
            int originalResourceId = 101;
            List<int> list = new List<int>() { originalResourceId };
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, list))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(1, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, null);

            // Then
            x.Catalogue.IsRestricted.Should().BeTrue();
        }

        [Fact]
        public async Task ResourceServiceReturnsThatAnUnrestrictedCatalogueIsUnrestricted()
        {
            // Given
            int originalResourceId = 100;
            List<int> originalResourceList = new List<int>() { originalResourceId };
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, originalResourceList))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(originalResourceId, null);

            // Then
            x.Catalogue.IsRestricted.Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 57541, null, 1, 0, 1, 0, 0)] // No activity status
        [InlineData(2, null, null, 1, 1, 1, 0, 0)] // If not user provided return no activity
        [InlineData(2, 57541, "Viewed", 1, 1, 1, 0, 1)]
        [InlineData(3, 57541, "Viewed", 1, 2, 1, 1, 1)]
        [InlineData(4, 57541, "Launched", 1, 3, 1, 2, 1)]
        [InlineData(303, 57541, "Failed", 4, 4, 1, 3, 4)]

        public async Task GetResourceByIdReturnsResourceWithCorrectUserSummaryActivityStatus(int resourceId, int? currentUserId, string? expectedFirstUserSummaryActivityStatusDescription, int? majorVersionId, int rangeStartGetResourceReferenceAndCataloguesLS, int rangeLengthGetResourceReferenceAndCataloguesLS, int rangeStartResourceActivityPerResourceMajorVersionLS, int rangeLengthResourceActivityPerResourceMajorVersionLS )
        {
            List<int> singleResourceIdList = new List<int>() { resourceId };
            List<int> currentUserIdList = currentUserId == null? null : new List<int>() { currentUserId.Value };
 

            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(singleResourceIdList, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(rangeStartGetResourceReferenceAndCataloguesLS, rangeLengthGetResourceReferenceAndCataloguesLS).ToList());

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(singleResourceIdList, currentUserIdList))
            .ReturnsAsync(this.ResourceActivityDTOList.GetRange(rangeStartResourceActivityPerResourceMajorVersionLS, rangeLengthResourceActivityPerResourceMajorVersionLS));

            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserId);

            // Then

            // Handling for if there is no activityStatus when testing
            if (expectedFirstUserSummaryActivityStatusDescription == null)
            {
                result.UserSummaryActivityStatuses.Should().BeEmpty();
            }
            else
            {
                result.UserSummaryActivityStatuses[0].ActivityStatusDescription.Should().Be(expectedFirstUserSummaryActivityStatusDescription);
                result.UserSummaryActivityStatuses[0].MajorVersionId.Should().Be(majorVersionId.Value);
            }

        }

        [Fact]
        public async Task GetResourceByIdReturnsResourceWithCorrectUserSummaryActivityStatusMulipleMajorVersions()
        {
            int rangeStartGetResourceReferenceAndCataloguesLS = 4;
            int rangeLengthGetResourceReferenceAndCataloguesLS = 1;
            int rangeStartResourceActivityPerResourceMajorVersionLS = 3;
            int rangeLengthResourceActivityPerResourceMajorVersionLS = 4;
            int resourceId = 303;
            List<int> singleResourceIdList = new List<int>() { resourceId };

            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(singleResourceIdList, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(rangeStartGetResourceReferenceAndCataloguesLS, rangeLengthGetResourceReferenceAndCataloguesLS).ToList());

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(singleResourceIdList, currentUserIdLS))
            .ReturnsAsync(this.ResourceActivityDTOList.GetRange(rangeStartResourceActivityPerResourceMajorVersionLS, rangeLengthResourceActivityPerResourceMajorVersionLS));

            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserId);

            // Then
            result.UserSummaryActivityStatuses[0].ActivityStatusDescription.Should().Be("Failed");
            result.UserSummaryActivityStatuses[0].MajorVersionId.Should().Be(4);

            result.UserSummaryActivityStatuses[1].ActivityStatusDescription.Should().Be("Downloaded");
            result.UserSummaryActivityStatuses[1].MajorVersionId.Should().Be(3);

            result.UserSummaryActivityStatuses[2].ActivityStatusDescription.Should().Be("In progress");
            result.UserSummaryActivityStatuses[2].MajorVersionId.Should().Be(2);

            result.UserSummaryActivityStatuses[3].ActivityStatusDescription.Should().Be("Downloaded");
            result.UserSummaryActivityStatuses[3].MajorVersionId.Should().Be(1);

        }



    }
}
