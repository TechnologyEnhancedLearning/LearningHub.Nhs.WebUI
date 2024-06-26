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
    using LearningHub.Nhs.Models.Resource;
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
        private readonly List<int> emptyOriginalResourceIdLS = new List<int>() { }; // for readabiliy
        private readonly List<int> emptyResourceIdLS = new List<int>() { }; // for readabiliy

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

        private List<ResourceActivityDTO> ResourceActivityDTOList => new List<ResourceActivityDTO>()
{
            new ResourceActivityDTO
            {
                Id = 642,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 1,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 3,
                ActivityStart = DateTime.Parse("2020-10-07T08:32:42+00:00"),
                ActivityEnd = DateTime.Parse("2020-10-07T08:32:42+00:00"),
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8322,
                UserId = 57541,
                LaunchResourceActivityId = 8321,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 2,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 7,
                ActivityStart = DateTime.Parse("2023-08-01T13:52:01+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8324,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 3,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 3,
                ActivityStart = DateTime.Parse("2023-08-01T13:52:27+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8326,
                UserId = 57541,
                LaunchResourceActivityId = 8325,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 4,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 7,
                ActivityStart = DateTime.Parse("2023-08-01T13:53:41+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8329,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 5,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 5,
                ActivityStart = DateTime.Parse("2023-08-01T13:57:03+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
        };
        private List<CatalogueDTO> CatalogueDTOList => new List<CatalogueDTO>()
        {
            new CatalogueDTO(originalResourceReferenceId: 1, catalogueNodeId: 0, catalogueNodeName: "catalogue1", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 2, catalogueNodeId: 0, catalogueNodeName: "default catalogue name", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 3, catalogueNodeId: 0, catalogueNodeName: "default catalogue name", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 4, catalogueNodeId: 0, catalogueNodeName: "default catalogue name", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 5, catalogueNodeId: 0, catalogueNodeName: "default catalogue name", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 6, catalogueNodeId: 0, catalogueNodeName: "catalogue2", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 7, catalogueNodeId: 0, catalogueNodeName: "deleted", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 8, catalogueNodeId: 0, catalogueNodeName: "catalogue3", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 9, catalogueNodeId: 0, catalogueNodeName: "default catalogue name", isRestricted: true),
            new CatalogueDTO(originalResourceReferenceId: 10, catalogueNodeId: 0, catalogueNodeName: "default catalogue name", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 302, catalogueNodeId: 0, catalogueNodeName: "default catalogue name", isRestricted: false),
        };
        private List<ResourceReferenceAndCatalogueDTO> ResourceReferenceAndCatalogueDTOList => new List<ResourceReferenceAndCatalogueDTO>()
        {
            // Entry for resourceId 100
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 100,
                title: "title1",
                description: "description1",
                resourceTypeId: 1,  // ResourceTypeEnum.Article corresponds to 1
                majorVersion: 0,
                rating: 3m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[0] }  // Using the first item in CatalogueDTOList
            ),

            // Entry for resourceId 101
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 101,
                title: "title2",
                description: "description2",
                resourceTypeId: 11,  // ResourceTypeEnum.Assessment corresponds to 11
                majorVersion: 0,
                rating: 0m,  // Defaulting rating to 0 instead of null
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[1] }  // Using the second item in CatalogueDTOList
            ),

            // Entry for resourceId 102
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 102,
                title: "title3",
                description: "description3",
                resourceTypeId: 1,  // ResourceTypeEnum.Article corresponds to 1
                majorVersion: 1,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[2] }  // Using the third item in CatalogueDTOList
            ),

            // Entry for resourceId 103
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 103,
                title: "title4",
                description: "description4",
                resourceTypeId: 11,  // ResourceTypeEnum.Assessment corresponds to 11
                majorVersion: 0,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[3] }  // Using the fourth item in CatalogueDTOList
            ),

            // Entry for resourceId 104
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 104,
                title: "title5",
                description: "description5",
                resourceTypeId: 1,  // ResourceTypeEnum.Article corresponds to 1
                majorVersion: 1,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[4] }  // Using the fifth item in CatalogueDTOList
            ),

            // Entry for resourceId 105
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 105,
                title: "title6",
                description: "description6",
                resourceTypeId: 11,  // ResourceTypeEnum.Assessment corresponds to 11
                majorVersion: 0,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[5] }  // Using the sixth item in CatalogueDTOList
            ),

            // Entry for resourceId 106
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 106,
                title: "title7",
                description: "description7",
                resourceTypeId: 1,  // ResourceTypeEnum.Article corresponds to 1
                majorVersion: 2,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[6] }  // Using the seventh item in CatalogueDTOList
            ),

            // Entry for resourceId 107
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 107,
                title: "title8",
                description: "description8",
                resourceTypeId: 11,  // ResourceTypeEnum.Assessment corresponds to 11
                majorVersion: 0,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[7] }  // Using the eighth item in CatalogueDTOList
            ),

            // Entry for resourceId 302
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 302,
                title: "titleTwoOneWithOneWithoutSummary",
                description: "descriptiontitleTwoOneWithOneWithoutSummary",
                resourceTypeId: 1,  // ResourceTypeEnum.Article corresponds to 1
                majorVersion: 99,
                rating: 3m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[8] }  // Using the ninth item in CatalogueDTOList
            ),

            // Entry for resourceId 303
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 303,
                title: "titleTwoOneWithOneWithoutSummary",
                description: "descriptiontitleTwoOneWithOneWithoutSummary",
                resourceTypeId: 1,  // ResourceTypeEnum.Article corresponds to 1
                majorVersion: 99,
                rating: 3m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[9] }  // Using the tenth item in CatalogueDTOList
            ),

            // Entry for resourceId 304
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 304,
                title: "title9",
                description: "description9",
                resourceTypeId: 11,  // ResourceTypeEnum.Assessment corresponds to 11
                majorVersion: 0,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[10] }  // Using the eleventh item in CatalogueDTOList
            ),

            // Entry for resourceId 305
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 305,
                title: "title10",
                description: "description10",
                resourceTypeId: 1,  // ResourceTypeEnum.Article corresponds to 1
                majorVersion: 3,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[11] }  // Using the twelfth item in CatalogueDTOList
            ),

            // Entry for resourceId 306
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 306,
                title: "title11",
                description: "description11",
                resourceTypeId: 11,  // ResourceTypeEnum.Assessment corresponds to 11
                majorVersion: 0,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[12] } // Using the thirteenth item in CatalogueDTOList
            ),
        };

        [Fact]
        public async Task SingleResourceEndpointReturnsTheCorrectInformationIfThereIsAMatchingResource()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 1 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(1, currentUserId);

            // Then
            x.Rating.Should().Be(3);
            x.Catalogue.Name.Should().Be("catalogue1");
            x.Title.Should().Be("title1");
            x.ResourceType.Should().Be("Article");
            x.UserSummaryActivityStatuses.Should().BeEmpty();
        }

       [Fact]
        public async Task GetResourceByIdReturnsTheCorrectInformationIfThereIsAMatchingResourceNoUserId()
        {
            int resourceId = 100;
            List<int> singleResourceIdList = new List<int>() { resourceId };
            int? currentUserIdNull = null;

            // Given
            this.resourceRepository.Setup(expression: rr => rr.GetResourceReferenceAndCatalogues(singleResourceIdList, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1).ToList());


            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserIdNull);

            // Then
  
            result.Rating.Should().Be(3);
            result.Title.Should().Be("title1");
            result.ResourceType.Should().Be("Article");
            result.UserSummaryActivityStatuses.Should().BeEmpty();//Empty dictionary or null dictionary? qqqq
        }

        [Fact]
        public async Task GetResourceByIdReturnsTheCorrectInformationIfThereIsAMatchingResourceWithUserId()
        {
            int resourceId = 100;
            List<int> singleResourceIdList = new List<int>() { resourceId };

            // Given
            this.resourceRepository.Setup(expression: rr => rr.GetResourceReferenceAndCatalogues(singleResourceIdList, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(11, 1).ToList()); // qqqq give 11 a activitySummary

            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserId);

            // Then
            result.Rating.Should().Be(3);
            result.Title.Should().Be("titleTwoOneWithOneWithoutSummary");
            result.ResourceType.Should().Be("Article");
            result.UserSummaryActivityStatuses.First().Should().Be("Completed");//qqqq add a test for second as well check whole dictionary
        }

        [Fact]
        public async Task SingleResourceReturnsA404IfTheresNoResourceReferenceWithAMatchingId()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 999 }))
                .ReturnsAsync(new List<ResourceReferenceAndCatalogueDTO>() { });

            // qqqq mock activityStatus , currentUserId

            // When / Then
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await this.resourceService.GetResourceReferenceByOriginalId(999, currentUserId));
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ResponseBody.Should().Be("No matching resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAResourceMetadataViewModelObjectWithAMessageSayingNoResourceIfThereIsNoCurrentResourceVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 2 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(1, 1));

            // qqqq mock activity summary , currentUserId

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
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 3 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(2, 1));

            // qqqq mock activity summary , currentUserId

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(3, currentUserId);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoNodePath()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 4 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(3, 1));

            // , currentUserId qqqq

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(4, currentUserId);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoCatalogueNodeVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 6 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(5, 1));

            // qqqq , currentUserId

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(6, currentUserId);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAZeroForRatingIfTheresNoRatingSummary()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 8 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(7, 1));

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
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 9 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(8, 1));
            
            //, currentUserId qqqq

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(9, currentUserId);

            // Then
            x.ResourceType.Should().Be(string.Empty);
        }

        [Fact]
        public async Task SingleResourceEndpointThrowsAnErrorIfThereIsMoreThanOneResourceReferenceWithTheSameOriginalId()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, new List<int>() { 10 }))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(9, 2));

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

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 2));

            // , currentUserId qqqq

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
            x.ResourceReferences[6].MajorVersion.Should().Be(99);
            x.ResourceReferences[0].UserSummaryActivityStatuses.Should().BeEmpty();
        }

        [Fact]
        public async Task BulkEndpointReturnsA404IfThereAreNoMatchingResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 998, 999 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(new List<ResourceReferenceAndCatalogueDTO>());

            // qqqq handle currentUserId

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

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 4));

            // , currentUserId qqqq

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, currentUserId);

            // Then
            x.ResourceReferences.Count.Should().Be(4);
            x.ResourceReferences[0].Rating.Should().Be(3);
            x.ResourceReferences[1].Title.Should().Be("No current resource version");
            x.ResourceReferences[2].Catalogue.Name.Should().Be("No catalogue for resource reference");
            x.ResourceReferences[3].Title.Should().Be("title1");
            x.ResourceReferences[0].UserSummaryActivityStatuses.Should().BeEmpty();
        }

        [Fact]
        public async Task BulkEndpointReturnsUnmatchedResourcesWithMatchedResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 1, 999 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(0, 1));

            // , currentUserId qqqq

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
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, list))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(5, 2));

            //, currentUserId

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
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, list))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(5, 1));


            //, currentUserId qqqq

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
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, list))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(8, 1));

            //, currentUserId qqqq

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
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, list))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(7, 1));

            //, currentUserId qqqq

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
        public async Task GetResourceReferencesByOriginalIdsReturnsResourceWithCorrectUserSummaryActivityStatus(string originalResourceIdsStr, int currentUserId, string expectedFirstUserSummaryActivityStatus, int rangeStart, int rangeLength) 
        {
            // Given
            List<int> originalResourceIds = originalResourceIdsStr.Split(",").Select(x=>Int32.Parse(x)).ToList();

            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(emptyResourceIdLS, originalResourceIds))//zero is to allow for the null there is no user with id 0
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(rangeStart, rangeLength));

            //qqqq
            //this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(originalResourceIds))//zero is to allow for the null there is no user with id 0
            //    .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(rangeStart, rangeLength));


            // When
            var result = await this.resourceService.GetResourceReferencesByOriginalIds(originalResourceIds, currentUserId);

            // Then

            result.ResourceReferences[0].UserSummaryActivityStatuses.First().Should().Be(expectedFirstUserSummaryActivityStatus);
        }

        [Theory]
        [InlineData(303, 57541, "Completed", 11, 1)]
        [InlineData(303, 57541, "", 12, 1)]//qqqq try to handle with first or default?
        [InlineData(1, null, "", 0, 1)]
        public async Task GetResourceByIdReturnsResourceWithCorrectUserSummaryActivityStatus(int resourceId, int? currentUserId, string expectedFirstUserSummaryActivityStatus, int rangeStart, int rangeLength)
        {
            List<int> singleResourceIdList = new List<int>() { resourceId };
            currentUserId = currentUserId ?? 0; //zero is to allow for the null there is no user with id 0
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferenceAndCatalogues(singleResourceIdList, emptyOriginalResourceIdLS))
                .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(rangeStart, rangeLength).Select(x => x.Resource).ToList());
            //this.resourceRepository.Setup(expression: rr => rr.GetResourcesFromIds(singleResourceIdList))
            //    .ReturnsAsync(this.ResourceReferenceAndCatalogueDTOList.GetRange(rangeStart, rangeLength).Select(x => x.Resource).ToList());
            //qqqq

            //, currentUserId.Value

            // When
            var result = await this.resourceService.GetResourceById(resourceId, currentUserId);

            // Then
            result.UserSummaryActivityStatuses.FirstOrDefault().Should().Be(expectedFirstUserSummaryActivityStatus);
    }


}
}
