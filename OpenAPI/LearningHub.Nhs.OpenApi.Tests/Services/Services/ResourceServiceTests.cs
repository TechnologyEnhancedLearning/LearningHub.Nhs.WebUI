namespace LearningHub.Nhs.OpenApi.Tests.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Migrations;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using LearningHub.Nhs.OpenApi.Tests.TestHelpers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class ResourceServiceTests
    {
        private readonly Mock<ILearningHubService> learningHubService;
        private readonly ResourceService resourceService;
        private readonly Mock<IResourceSyncService> resourceSyncService;
        private readonly Mock<IResourceRepository> resourceRepository;
        private readonly Mock<ILogger<ResourceService>> logger;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IFileTypeService> fileTypeService;
        private readonly Mock<IUserProfileService> userProfileService;
        private readonly Mock<IBlockCollectionRepository> blockCollectionRepository;
        private readonly Mock<IWebLinkResourceVersionRepository> webLinkResourceVersionRepository;
        private readonly Mock<ICaseResourceVersionRepository> caseResourceVersionRepository;
        private readonly Mock<IScormResourceVersionRepository> scormResourceVersionRepository;
        private readonly Mock<IGenericFileResourceVersionRepository> genericFileResourceVersionRepository;
        private readonly Mock<IResourceVersionRepository> resourceVersionRepository;
        private readonly Mock<IAssessmentResourceActivityMatchQuestionRepository> assessmentResourceActivityMatchQuestionRepository;
        private readonly Mock<IHtmlResourceVersionRepository> htmlResourceVersionRepository;
        private readonly Mock<IArticleResourceVersionRepository> articleResourceVersionRepository;
        private readonly Mock<IArticleResourceVersionFileRepository> articleResourceVersionFileRepository;
        private readonly Mock<IAudioResourceVersionRepository> audioResourceVersionRepository;
        private readonly Mock<IBookmarkRepository> bookmarkRepository;
        private readonly Mock<IImageResourceVersionRepository> imageResourceVersionRepository;
        private readonly Mock<IVideoResourceVersionRepository> videoResourceVersionRepository;
        private readonly Mock<IAssessmentResourceVersionRepository> assessmentResourceVersionRepository;
        private readonly Mock<IResourceLicenceRepository> resourceLicenceRepository;
        private readonly Mock<IResourceReferenceRepository> resourceReferenceRepository;
        private readonly Mock<IResourceVersionUserAcceptanceRepository> resourceVersionUserAcceptanceRepository;
        private readonly Mock<IResourceVersionFlagRepository> resourceVersionFlagRepository;
        private readonly Mock<IResourceVersionValidationResultRepository> resourceVersionValidationResultRepository;
        private readonly Mock<IResourceVersionKeywordRepository> resourceVersionKeywordRepository;
        private readonly Mock<ICatalogueNodeVersionRepository> catalogueNodeVersionRepository;
        private readonly Mock<IEmbeddedResourceVersionRepository> embeddedResourceVersionRepository;
        private readonly Mock<IVideoRepository> videoRepository;
        private readonly Mock<IWholeSlideImageRepository> wholeSlideImageRepository;
        private readonly Mock<INodePathRepository> nodePathRepository;
        private readonly Mock<INodeResourceRepository> nodeResourceRepository;
        private readonly Mock<IEquipmentResourceVersionRepository> equipmentResourceVersionRepository;
        private readonly Mock<IPublicationRepository> publicationRepository;
        private readonly Mock<IQuestionBlockRepository> questionBlockRepository;
        private readonly Mock<IMigrationSourceRepository> migrationSourceRepository;
        private readonly Mock<DbContext> dbContext;
        private readonly Mock<INodeRepository> nodeRepository;
        private readonly Mock<IFileRepository> fileRepository;
        private readonly Mock<IOptions<AzureConfig>> azureConfig;
        private readonly Mock<IOptions<LearningHubConfig>> learningHubConfig;
        private readonly Mock<ICachingService> cachingService;
        private readonly Mock<ISearchService> searchService;
        private readonly Mock<ICatalogueService> catalogueService;
        private readonly Mock<IUserService> userService;
        private readonly Mock<IProviderService> providerService;
        private readonly Mock<IInternalSystemService> internalSystemService;
        private readonly Mock<IQueueCommunicatorService> queueCommunicatorService;
        private readonly Mock<IResourceVersionProviderRepository> resourceVersionProviderRepository;
        private readonly Mock<IResourceVersionAuthorRepository> resourceVersionAuthorRepository;
        private readonly Mock<IFileChunkDetailRepository> fileChunkDetailRepository;
        private readonly Mock<IResourceSyncRepository> resourceSyncRepository;
        private readonly Mock<IResourceVersionEventRepository> resourceVersionEventRepository;

        private readonly int currentUserId;

        public ResourceServiceTests()
        {
            // This Id is the development accountId
            this.currentUserId = 57541;

            this.learningHubService = new Mock<ILearningHubService>();
            this.resourceSyncService = new Mock<IResourceSyncService>();
            this.resourceRepository = new Mock<IResourceRepository>();
            this.fileRepository = new Mock<IFileRepository>();
            this.articleResourceVersionFileRepository = new Mock<IArticleResourceVersionFileRepository>();
            this.articleResourceVersionRepository = new Mock<IArticleResourceVersionRepository>();
            this.mapper = new Mock<IMapper>();
            this.fileTypeService = new Mock<IFileTypeService>();
            this.userProfileService = new Mock<IUserProfileService>();
            this.blockCollectionRepository = new Mock<IBlockCollectionRepository>();
            this.webLinkResourceVersionRepository = new Mock<IWebLinkResourceVersionRepository>();
            this.caseResourceVersionRepository = new Mock<ICaseResourceVersionRepository>();
            this.scormResourceVersionRepository = new Mock<IScormResourceVersionRepository>();
            this.genericFileResourceVersionRepository = new Mock<IGenericFileResourceVersionRepository>();
            this.resourceVersionRepository = new Mock<IResourceVersionRepository>();
            this.assessmentResourceActivityMatchQuestionRepository = new Mock<IAssessmentResourceActivityMatchQuestionRepository>();
            this.htmlResourceVersionRepository = new Mock<IHtmlResourceVersionRepository>();
            this.audioResourceVersionRepository = new Mock<IAudioResourceVersionRepository>();
            this.bookmarkRepository = new Mock<IBookmarkRepository>();
            this.imageResourceVersionRepository = new Mock<IImageResourceVersionRepository>();
            this.videoResourceVersionRepository = new Mock<IVideoResourceVersionRepository>();
            this.assessmentResourceVersionRepository = new Mock<IAssessmentResourceVersionRepository>();
            this.resourceLicenceRepository = new Mock<IResourceLicenceRepository>();
            this.resourceReferenceRepository = new Mock<IResourceReferenceRepository>();
            this.resourceVersionUserAcceptanceRepository = new Mock<IResourceVersionUserAcceptanceRepository>();
            this.resourceVersionFlagRepository = new Mock<IResourceVersionFlagRepository>();
            this.resourceVersionValidationResultRepository = new Mock<IResourceVersionValidationResultRepository>();
            this.resourceVersionKeywordRepository = new Mock<IResourceVersionKeywordRepository>();
            this.catalogueNodeVersionRepository = new Mock<ICatalogueNodeVersionRepository>();
            this.embeddedResourceVersionRepository = new Mock<IEmbeddedResourceVersionRepository>();
            this.videoRepository = new Mock<IVideoRepository>();
            this.wholeSlideImageRepository = new Mock<IWholeSlideImageRepository>();
            this.nodePathRepository = new Mock<INodePathRepository>();
            this.nodeResourceRepository = new Mock<INodeResourceRepository>();
            this.equipmentResourceVersionRepository = new Mock<IEquipmentResourceVersionRepository>();
            this.publicationRepository = new Mock<IPublicationRepository>();
            this.questionBlockRepository = new Mock<IQuestionBlockRepository>();
            this.migrationSourceRepository = new Mock<IMigrationSourceRepository>();
            this.dbContext = new Mock<DbContext>();
            this.nodeRepository = new Mock<INodeRepository>();
            this.azureConfig = new Mock<IOptions<AzureConfig>>();
            this.learningHubConfig = new Mock<IOptions<LearningHubConfig>>();
            this.cachingService = new Mock<ICachingService>();
            this.searchService = new Mock<ISearchService>();
            this.catalogueService = new Mock<ICatalogueService>();
            this.userService = new Mock<IUserService>();
            this.providerService = new Mock<IProviderService>();
            this.internalSystemService = new Mock<IInternalSystemService>();
            this.queueCommunicatorService = new Mock<IQueueCommunicatorService>();
            this.resourceVersionProviderRepository = new Mock<IResourceVersionProviderRepository>();
            this.resourceVersionAuthorRepository = new Mock<IResourceVersionAuthorRepository>();
            this.fileChunkDetailRepository = new Mock<IFileChunkDetailRepository>();
            this.resourceSyncRepository = new Mock<IResourceSyncRepository>();
            this.resourceVersionEventRepository = new Mock<IResourceVersionEventRepository>();
            this.logger = new Mock<ILogger<ResourceService>>();
            this.resourceService = new ResourceService(this.learningHubService.Object, this.fileTypeService.Object, this.blockCollectionRepository.Object, this.internalSystemService.Object, this.resourceVersionAuthorRepository.Object, this.fileChunkDetailRepository.Object, this.queueCommunicatorService.Object, this.resourceRepository.Object, this.resourceVersionProviderRepository.Object, this.providerService.Object, this.articleResourceVersionFileRepository.Object, this.publicationRepository.Object, this.migrationSourceRepository.Object, this.questionBlockRepository.Object, this.videoRepository.Object, this.wholeSlideImageRepository.Object, this.embeddedResourceVersionRepository.Object, this.equipmentResourceVersionRepository.Object, this.imageResourceVersionRepository.Object, this.bookmarkRepository.Object, this.assessmentResourceActivityMatchQuestionRepository.Object, this.resourceVersionKeywordRepository.Object, this.resourceVersionValidationResultRepository.Object, this.logger.Object, this.webLinkResourceVersionRepository.Object, this.caseResourceVersionRepository.Object, this.scormResourceVersionRepository.Object, this.genericFileResourceVersionRepository.Object, this.resourceVersionRepository.Object, this.htmlResourceVersionRepository.Object, this.mapper.Object, this.fileRepository.Object, this.azureConfig.Object, this.learningHubConfig.Object, this.userProfileService.Object, this.resourceVersionFlagRepository.Object, this.articleResourceVersionRepository.Object, this.audioResourceVersionRepository.Object, this.videoResourceVersionRepository.Object, this.assessmentResourceVersionRepository.Object, this.resourceLicenceRepository.Object, this.resourceReferenceRepository.Object, this.resourceVersionUserAcceptanceRepository.Object, this.catalogueNodeVersionRepository.Object, this.cachingService.Object, this.searchService.Object, this.catalogueService.Object, this.nodeResourceRepository.Object, this.nodePathRepository.Object, this.userService.Object, this.nodeRepository.Object,this.resourceSyncService.Object, this.resourceSyncRepository.Object, this.resourceVersionEventRepository.Object, this.dbContext.Object.As<LearningHubDbContext>());
        }

        private List<ResourceActivityDTO> ResourceActivityDTOList => new List<ResourceActivityDTO>()
        {
            new ResourceActivityDTO{ ResourceId = 1, ActivityStatusId = 5, MajorVersion = 5 },
            new ResourceActivityDTO{ ResourceId = 1, ActivityStatusId = 7, MajorVersion = 4 },
            new ResourceActivityDTO{ ResourceId = 1, ActivityStatusId = 3, MajorVersion = 3 },
            new ResourceActivityDTO{ ResourceId = 1, ActivityStatusId = 7, MajorVersion = 2 },
            new ResourceActivityDTO{ ResourceId = 1, ActivityStatusId = 3, MajorVersion = 1 },

            new ResourceActivityDTO{ ResourceId = 2, ActivityStatusId = 5, MajorVersion = 5 }, // Passed
            new ResourceActivityDTO{ ResourceId = 2, ActivityStatusId = 4, MajorVersion = 4 }, // Failed
            new ResourceActivityDTO{ ResourceId = 2, ActivityStatusId = 3, MajorVersion = 3 }, // complete

            new ResourceActivityDTO{ ResourceId = 3, ActivityStatusId = 4, MajorVersion = 2 }, // Failed
            new ResourceActivityDTO{ ResourceId = 3, ActivityStatusId = 4, MajorVersion = 1 }, // Failed
            new ResourceActivityDTO{ ResourceId = 3, ActivityStatusId = 7, MajorVersion = 4 }, // In complete
        };

        private List<Resource> ResourceList => new List<Resource>()
        {
            ResourceTestHelper.CreateResourceWithDetails(id: 1, title: "title1", description: "description1", rating: 3m, resourceType: ResourceTypeEnum.Article),
            ResourceTestHelper.CreateResourceWithDetails(id: 2, hasCurrentResourceVersion: false, hasNodePath: false, resourceType: ResourceTypeEnum.Assessment),
            ResourceTestHelper.CreateResourceWithDetails(id: 3, title: "title2", description: "description2"),
            ResourceTestHelper.CreateResourceWithDetails(id: 4),
            ResourceTestHelper.CreateResourceWithDetails(id: 5, hasRatingSummary: false),
            ResourceTestHelper.CreateResourceWithDetails(id: 6, resourceType: (ResourceTypeEnum)999), // resource with resourceType that does not exist on ResourceTypeEnum to check error handling
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
        };

        [Fact]
        public async Task SingleResourceEndpointReturnsTheCorrectInformationIfThereIsAMatchingResource()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 1 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(1, null);

            // Then
            x.Rating.Should().Be(3);
            x.Catalogue.Name.Should().Be("catalogue1");
            x.Title.Should().Be("title1");
            x.ResourceType.Should().Be("Article");
        }

        [Fact]
        public async Task SingleResourceReturnsA404IfTheresNoResourceReferenceWithAMatchingId()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 999 }))
                .ReturnsAsync(new List<ResourceReference>());

            // When / Then
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await this.resourceService.GetResourceReferenceByOriginalId(999, null));
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ResponseBody.Should().Be("No matching resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAResourceMetadataViewModelObjectWithAMessageSayingNoResourceIfThereIsNoCurrentResourceVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 2 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(1, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(2, null);

            // Then
            x.Title.Should().Be("No current resource version");
            x.Rating.Should().Be(0);
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoNodeVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 3 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(2, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(3, null);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoNodePath()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 4 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(3, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(4, null);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAMessageSayingNoCatalogueIfThereIsNoCatalogueNodeVersion()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 6 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(5, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(6, null);

            // Then
            x.Catalogue.Name.Should().Be("No catalogue for resource reference");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsAZeroForRatingIfTheresNoRatingSummary()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 8 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(7, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(8, null);

            // Then
            x.Catalogue.Name.Should().Be("catalogue3");
            x.Rating.Should().Be(0);
        }

        [Fact]
        public async Task SingleResourceEndpointThrowsAnErrorAndReturnsABlankStringIfTheresANullResourceType()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 9 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(8, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(9, null);

            // Then
            x.ResourceType.Should().Be(string.Empty);
        }

        [Fact]
        public async Task SingleResourceEndpointThrowsAnErrorIfThereIsMoreThanOneResourceReferenceWithTheSameOriginalId()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 10 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(9, 2));

            // When / Then
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.resourceService.GetResourceReferenceByOriginalId(10, null));
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

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 2));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, null);

            // Then
            x.ResourceReferences.Count.Should().Be(2);
            x.UnmatchedResourceReferenceIds.Count.Should().Be(0);
            x.ResourceReferences[0].Rating.Should().Be(3);
            x.ResourceReferences[0].Catalogue.Name.Should().Be("catalogue1");
            x.ResourceReferences[0].ResourceType.Should().Be("Article");
            x.ResourceReferences[1].ResourceId.Should().Be(1);
            x.ResourceReferences[1].ResourceType.Should().Be("Assessment");
        }

        [Fact]
        public async Task BulkEndpointReturnsA404IfThereAreNoMatchingResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 998, 999 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp))
                .ReturnsAsync(new List<ResourceReference>());

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, null);

            // Then
            x.UnmatchedResourceReferenceIds.Count.Should().Be(2);
            x.ResourceReferences.Count.Should().Be(0);
        }

        [Fact]
        public async Task BulkEndpointReturnsResourcesWithIncompleteInformation()
        {
            // Given
            var idsToLookUp = new List<int>() { 1, 2, 3, 4 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 4));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, null);

            // Then
            x.ResourceReferences.Count.Should().Be(4);
            x.ResourceReferences[0].Rating.Should().Be(3);
            x.ResourceReferences[1].Title.Should().Be("No current resource version");
            x.ResourceReferences[2].Catalogue.Name.Should().Be("No catalogue for resource reference");
            x.ResourceReferences[3].Title.Should().Be("title1");
        }

        [Fact]
        public async Task BulkEndpointReturnsUnmatchedResourcesWithMatchedResources()
        {
            // Given
            var idsToLookUp = new List<int>() { 1, 999 };

            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(idsToLookUp))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 1));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(idsToLookUp, null);

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
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(5, 2));

            // When
            var x = await this.resourceService.GetResourceReferencesByOriginalIds(list, null);

            // Then
            x.ResourceReferences[0].RefId.Should().Be(6);
            x.ResourceReferences[1].RefId.Should().Be(7);
        }

        [Fact]
        public async Task ResourceServiceReturnsTheOriginalResourceReferenceIdAsTheRefIdForSingleResourceEndpoint()
        {
            // Given
            List<int> list = new List<int>() { 6 };
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(5, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(6, null);

            // Then
            x.RefId.Should().Be(6);
        }

        [Fact]
        public async Task ResourceServiceReturnsThatARestrictedCatalogueIsRestricted()
        {
            // Given
            List<int> list = new List<int>() { 9 };
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(8, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(9, null);

            // Then
            x.Catalogue.IsRestricted.Should().BeTrue();
        }

        [Fact]
        public async Task ResourceServiceReturnsThatAnUnrestrictedCatalogueIsUnrestricted()
        {
            // Given
            List<int> list = new List<int>() { 8 };
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(list))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(7, 1));

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(8, null);

            // Then
            x.Catalogue.IsRestricted.Should().BeFalse();
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsActivitySummaryWhenCurrentUserIdProvided()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 1 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 1));

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(new List<int>() { 1 }, new List<int>() { currentUserId }))
                .ReturnsAsync(this.ResourceActivityDTOList.ToList());

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(1, currentUserId);

            // Then
            x.UserSummaryActivityStatuses.Should().NotBeNull();
            x.UserSummaryActivityStatuses[0].MajorVersionId.Should().Be(5);
            x.UserSummaryActivityStatuses[1].MajorVersionId.Should().Be(4);
            x.UserSummaryActivityStatuses[2].MajorVersionId.Should().Be(3);
            x.UserSummaryActivityStatuses[3].MajorVersionId.Should().Be(2);
            x.UserSummaryActivityStatuses[4].MajorVersionId.Should().Be(1);

            x.UserSummaryActivityStatuses[0].ActivityStatusDescription.Should().Be("Passed");
            x.UserSummaryActivityStatuses[1].ActivityStatusDescription.Should().Be("In progress");
            x.UserSummaryActivityStatuses[2].ActivityStatusDescription.Should().Be("Viewed");
            x.UserSummaryActivityStatuses[3].ActivityStatusDescription.Should().Be("In progress");
            x.UserSummaryActivityStatuses[4].ActivityStatusDescription.Should().Be("Viewed");
        }

        [Fact]
        public async Task SingleResourceEndpointReturnsEmptyActivitySummaryWhenNoCurrentUserIdProvided()
        {
            // Given
            this.resourceRepository.Setup(rr => rr.GetResourceReferencesByOriginalResourceReferenceIds(new List<int>() { 1 }))
                .ReturnsAsync(this.ResourceReferenceList.GetRange(0, 1));

            // This should not be hit
            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(new List<int>() { 1 }, new List<int>() { currentUserId }))
                .ReturnsAsync(this.ResourceActivityDTOList.ToList());

            // When
            var x = await this.resourceService.GetResourceReferenceByOriginalId(1, null);

            // Then
            x.UserSummaryActivityStatuses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetResourceReferencesByCompleteReturnsCorrectInformation()
        {
            // Given
            List<int> resourceIds = new List<int>() { 1, 2 };
            List<Resource> resources = this.ResourceList.GetRange(0, 2);
            resources[0].ResourceReference.ToList()[0].Resource = ResourceTestHelper.CreateResourceWithDetails(id: 1, title: "title1", description: "description1", rating: 3m, resourceType: ResourceTypeEnum.Article);
            resources[1].ResourceReference.ToList()[0].Resource = ResourceTestHelper.CreateResourceWithDetails(id: 2, hasCurrentResourceVersion: false, hasNodePath: false, resourceType: ResourceTypeEnum.Assessment);

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .ReturnsAsync(this.ResourceActivityDTOList);

            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(resourceIds))
                .ReturnsAsync(resources);

            // When
            var x = await this.resourceService.GetResourceReferenceByActivityStatus(new List<int>() { (int)ActivityStatusEnum.Completed }, currentUserId);

            // Then

            // Two groups resourceId 1 and 2 have completed for a major version. ResourceId 3 had resourceActivity data but not completed
            x.Count().Should().Be(2);

            // We are including all the major versions not just the matching ones if there exists one matching one
            x[0].ResourceId.Should().Be(1);
            x[0].UserSummaryActivityStatuses.Count().Should().Be(5);

            // Return all the activitySummaries if one match
            x[1].ResourceId.Should().Be(2);
            x[1].UserSummaryActivityStatuses.Count().Should().Be(3);

            // we are not excluding major version that are not completed. We return the resource and all its activitySummaries if one matches
            x[0].UserSummaryActivityStatuses[1].ActivityStatusDescription.Should().Be("In progress");
            x[0].UserSummaryActivityStatuses[2].ActivityStatusDescription.Should().Be("Viewed"); // Rename completed and still return it

        }

        [Fact]
        public async Task GetResourceReferencesByInProgressReturnsCorrectInformation()
        {
            // Given
            List<int> resourceIds = new List<int>() { 1, 3 };
            List<Resource> resources = new List<Resource>() { this.ResourceList[0], this.ResourceList[2] };
            resources[0].ResourceReference.ToList()[0].Resource = ResourceTestHelper.CreateResourceWithDetails(id: 1, title: "title1", description: "description1", rating: 3m, resourceType: ResourceTypeEnum.Article);
            resources[1].ResourceReference.ToList()[0].Resource = ResourceTestHelper.CreateResourceWithDetails(id: 3, title: "title2", description: "description2");

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .ReturnsAsync(this.ResourceActivityDTOList);

            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(resourceIds))
                .ReturnsAsync(resources);

            // When
            var x = await this.resourceService.GetResourceReferenceByActivityStatus(new List<int>() { (int)ActivityStatusEnum.Incomplete }, currentUserId); // In complete in the database is in progress im database

            // Then

            // Two groups resourceId 1 and 3 have completed for a major version. ResourceId 2 had resourceActivity data but not "in progress"
            x.Count().Should().Be(2);

            // We are including all the major versions not just the matching ones if there exists one matching one
            x[0].ResourceId.Should().Be(1);
            x[0].UserSummaryActivityStatuses.Count().Should().Be(5);

            // Return all the activitySummaries if one match
            x[1].ResourceId.Should().Be(3);
            x[1].UserSummaryActivityStatuses.Count().Should().Be(3);

            // we are not excluding major version that are not completed. We return the resource and all its activitySummaries if one matches
            x[0].UserSummaryActivityStatuses[1].ActivityStatusDescription.Should().Be("In progress");
            x[0].UserSummaryActivityStatuses[2].ActivityStatusDescription.Should().Be("Viewed"); // Rename completed and still return it

        }

        [Fact]
        public async Task GetResourceReferencesByCertificatesReturnsCorrectInformation()
        {

            // Given
            List<int> resourceIds = new List<int>() { 1, 3 }; // Ids returned from activity

            List<Resource> resources = new List<Resource>() { this.ResourceList[0], this.ResourceList[2] };
            resources[0].ResourceReference.ToList()[0].Resource = ResourceTestHelper.CreateResourceWithDetails(id: 1, title: "title1", description: "description1", rating: 3m, resourceType: ResourceTypeEnum.Article);
            resources[1].ResourceReference.ToList()[0].Resource = ResourceTestHelper.CreateResourceWithDetails(id: 3, title: "title2", description: "description2");


            // Will be passed resourceIds and currentUserId
            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .ReturnsAsync(this.ResourceActivityDTOList);

            this.resourceRepository.Setup(rr => rr.GetAchievedCertificatedResourceIds(currentUserId))
                .ReturnsAsync(resourceIds);

            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(resourceIds))
                .ReturnsAsync(resources);

            // When
            var x = await this.resourceService.GetResourceReferencesForCertificates(currentUserId);

            // Then

            x.Count().Should().Be(2);

            // We are including all the major versions not just the matching ones if there exists one matching one
            x[0].ResourceId.Should().Be(1);
            x[0].UserSummaryActivityStatuses.Count().Should().Be(5);

            // Return all the activitySummaries if one match
            x[1].ResourceId.Should().Be(3);
            x[1].UserSummaryActivityStatuses.Count().Should().Be(3);

            // we are not excluding major version that are not completed (assuming here that its completed and has certificated flag). We return the resource and all its activitySummaries if one matches
            x[0].UserSummaryActivityStatuses[1].ActivityStatusDescription.Should().Be("In progress");
            x[0].UserSummaryActivityStatuses[2].ActivityStatusDescription.Should().Be("Viewed"); // Rename completed and still return it
        }

        [Fact]
        public async Task GetResourceReferencesByCompleteNoActivitySummaryFound()
        {
            // Given
            List<int> resourceIds = new List<int>() { };
            List<Resource> resources = this.ResourceList.GetRange(0, 0);

            this.resourceRepository.Setup(rr => rr.GetResourceActivityPerResourceMajorVersion(It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .ReturnsAsync(this.ResourceActivityDTOList.GetRange(8, 3));

            this.resourceRepository.Setup(rr => rr.GetResourcesFromIds(resourceIds))
                .ReturnsAsync(resources);

            // When
            var x = await this.resourceService.GetResourceReferenceByActivityStatus(new List<int>() { (int)ActivityStatusEnum.Completed }, currentUserId);

            // Then
            x.Count().Should().Be(0);
        }
    }
}
