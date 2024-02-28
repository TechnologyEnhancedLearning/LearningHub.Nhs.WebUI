// <copyright file="ResourceServiceTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoMapper;
    using EntityFrameworkCore.Testing.Common;
    using FluentAssertions;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// The resource service tests.
    /// </summary>
    public class ResourceServiceTests
    {
        private readonly Mock<IResourceVersionRepository> mockResourceVersionRepo;
        private readonly Mock<INodePathRepository> mockNodePathRepo;
        private readonly Mock<INodeResourceRepository> mockNodeResourceRepo;
        private readonly Mock<IPublicationRepository> mockPublicationRepo;
        private readonly Mock<IResourceVersionEventRepository> mockResourceVersionEventRepo;
        private readonly Mock<IResourceSyncService> mockResourceSyncService;
        private readonly Mock<ISearchService> mockSearchService;
        private readonly Mock<INotificationService> mockNotificationService;
        private readonly Mock<IUserNotificationService> mockUserNotificationService;
        private readonly Mock<IResourceReferenceRepository> mockResourceReferenceRepo;
        private readonly Mock<IArticleResourceVersionRepository> mockArticleResourceVersionRepos;
        private readonly Mock<IFileTypeRepository> mockFileTypeRepository;
        private readonly Mock<IResourceVersionProviderRepository> mockResourceVersionProviderRepository;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<IOptions<Settings>> mockSettings;
        private readonly IResourceService resourceService;
        private readonly Mock<ICachingService> mockCachingService;
        private readonly Mock<IInternalSystemService> mockInternalSystemService;
        private readonly Mock<IProviderService> mockProviderService;
        private readonly Mock<DbContext> dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceTests"/> class.
        /// </summary>
        public ResourceServiceTests()
        {
            this.mockResourceVersionRepo = new Mock<IResourceVersionRepository>();
            this.mockNodePathRepo = new Mock<INodePathRepository>();
            this.mockNodeResourceRepo = new Mock<INodeResourceRepository>();
            this.mockPublicationRepo = new Mock<IPublicationRepository>();
            this.mockResourceVersionEventRepo = new Mock<IResourceVersionEventRepository>();
            this.mockResourceSyncService = new Mock<IResourceSyncService>();
            this.mockSearchService = new Mock<ISearchService>();
            this.mockNotificationService = new Mock<INotificationService>();
            this.mockUserNotificationService = new Mock<IUserNotificationService>();
            this.mockResourceReferenceRepo = new Mock<IResourceReferenceRepository>();
            this.mockArticleResourceVersionRepos = new Mock<IArticleResourceVersionRepository>();
            this.mockFileTypeRepository = new Mock<IFileTypeRepository>();
            this.mockResourceVersionProviderRepository = new Mock<IResourceVersionProviderRepository>();
            this.mockSettings = new Mock<IOptions<Settings>>();
            this.mockMapper = new Mock<IMapper>();
            this.mockCachingService = new Mock<ICachingService>();
            this.mockInternalSystemService = new Mock<IInternalSystemService>();
            this.mockProviderService = new Mock<IProviderService>();
            this.dbContext = new Mock<DbContext>();
            this.SetupSettings();

            this.resourceService = new ResourceService(
                null,
                null,
                null,
                this.mockResourceSyncService.Object,
                null,
                null,
                this.mockResourceReferenceRepo.Object,
                this.mockNodeResourceRepo.Object,
                this.mockNodePathRepo.Object,
                null,
                this.mockResourceVersionRepo.Object,
                null,
                null,
                this.mockResourceVersionEventRepo.Object,
                null,
                this.mockArticleResourceVersionRepos.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this.mockPublicationRepo.Object,
                null,
                null,
                this.mockSearchService.Object,
                null,
                this.mockMapper.Object,
                this.mockSettings.Object,
                this.mockNotificationService.Object,
                this.mockUserNotificationService.Object,
                null,
                null,
                null,
                null,
                this.mockCachingService.Object,
                this.mockFileTypeRepository.Object,
                null,
                null,
                null,
                null,
                this.dbContext.Object.As<LearningHubDbContext>(),
                null,
                null,
                null,
                this.mockResourceVersionProviderRepository.Object,
                this.mockInternalSystemService.Object,
                this.mockProviderService.Object);
        }

        /// <summary>
        /// The PublishResourceVersion method creates notification when resource is audio type.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceVersion_CreatesNotification_WhenResourceIsAudio()
        {
            // Arrange
            this.SetupMockForPublishResource(ResourceTypeEnum.Audio, timeToPublish: 120);
            var fixture = new Fixture();
            this.mockNotificationService.Setup(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(fixture.Create<int>()));

            // Act
            await this.resourceService.PublishResourceVersionAsync(Mock.Of<PublishViewModel>());

            // Assert
            this.mockNotificationService.Verify(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Once);
        }

        /// <summary>
        /// The PublishResourceVersion method creates notification when resource is video type.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceVersion_CreatesNotification_WhenResourceIsVideo()
        {
            // Arrange
            this.SetupMockForPublishResource(ResourceTypeEnum.Video, timeToPublish: 180);
            var fixture = new Fixture();
            this.mockNotificationService.Setup(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(fixture.Create<int>()));

            // Act
            await this.resourceService.PublishResourceVersionAsync(Mock.Of<PublishViewModel>());

            // Assert
            this.mockNotificationService.Verify(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Once);
        }

        /// <summary>
        /// The PublishResourceVersion method does not creates notification when publish time less than configured value.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceVersion_DoesNotCreatesNotification_WhenPublishIsQuick()
        {
            // Arrange
            this.SetupMockForPublishResource(ResourceTypeEnum.Video, timeToPublish: 5);

            // Act
            await this.resourceService.PublishResourceVersionAsync(Mock.Of<PublishViewModel>());

            // Assert
            this.mockNotificationService.Verify(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Never);
        }

        /// <summary>
        /// The PublishResourceVersion method does not create notification when resource is image.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceVersion_DoesNotCreateNotification_WhenResourceIsImage()
        {
            // Arrange
            this.SetupMockForPublishResource(ResourceTypeEnum.Image);

            // Act
            await this.resourceService.PublishResourceVersionAsync(Mock.Of<PublishViewModel>());

            // Assert
            this.mockNotificationService.Verify(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Never);
        }

        /// <summary>
        /// The PublishResourceVersion method does not create notification when resource is article.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceVersion_DoesNotCreateNotification_WhenResourceIsArticle()
        {
            // Arrange
            this.SetupMockForPublishResource(ResourceTypeEnum.Article);

            // Act
            await this.resourceService.PublishResourceVersionAsync(Mock.Of<PublishViewModel>());

            // Assert
            this.mockNotificationService.Verify(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Never);
        }

        /// <summary>
        /// The PublishResourceVersion method create notification when resource is article but has media file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceVersion_CreateNotification_WhenResourceIsArticleButHasMediaFile()
        {
            // Arrange
            this.SetupMockForPublishResource(ResourceTypeEnum.Article, articleHasMediaFile: true, timeToPublish: 60);
            var fixture = new Fixture();
            var model = new PublishViewModel { ResourceVersionId = fixture.Create<int>(), UserId = fixture.Create<int>() };
            this.mockNotificationService.Setup(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(fixture.Create<int>()));

            // Act
            await this.resourceService.PublishResourceVersionAsync(model);

            // Assert
            this.mockNotificationService.Verify(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Once);
        }

        /// <summary>
        /// The PublishResourceVersion method does not create notification when publish not successful.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceVersion_DoesNotCreateNotification_WhenPublishNotSuccessful()
        {
            // Arrange
            this.SetupMockForPublishResource(ResourceTypeEnum.Audio, false);

            // Act
            await this.resourceService.PublishResourceVersionAsync(Mock.Of<PublishViewModel>());

            // Assert
            this.mockNotificationService.Verify(m => m.CreateResourcePublishedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Never);
        }

        /// <summary>
        /// The SetResourceVersionFailedToPublish method creates notification when resource is audio type.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task SetResourceVersionFailedToPublish_CreatesNotification_WhenResourceIsAudio()
        {
            // Arrange
            var fixture = new Fixture();
            var model = new PublishViewModel { ResourceVersionId = fixture.Create<int>(), UserId = fixture.Create<int>() };
            this.SetupMockForPublishResource(ResourceTypeEnum.Audio);
            this.mockNotificationService.Setup(m => m.CreatePublishFailedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(fixture.Create<int>()));

            // Act
            await this.resourceService.SetResourceVersionFailedToPublish(model);

            // Assert
            this.mockNotificationService.Verify(m => m.CreatePublishFailedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Once);
        }

        /// <summary>
        /// The SetResourceVersionFailedToPublish method creates notification when resource is video type.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task SetResourceVersionFailedToPublish_CreatesNotification_WhenResourceIsVideo()
        {
            // Arrange
            var fixture = new Fixture();
            var model = new PublishViewModel { ResourceVersionId = fixture.Create<int>(), UserId = fixture.Create<int>() };
            this.SetupMockForPublishResource(ResourceTypeEnum.Video);
            this.mockNotificationService.Setup(m => m.CreatePublishFailedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(fixture.Create<int>()));

            // Act
            await this.resourceService.SetResourceVersionFailedToPublish(model);

            // Assert
            this.mockNotificationService.Verify(m => m.CreatePublishFailedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Once);
        }

        /// <summary>
        /// The SetResourceVersionFailedToPublish method does not creates notification when resource is image type.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task SetResourceVersionFailedToPublish_DoesCreatesNotification_WhenResourceIsImage()
        {
            // Arrange
            var fixture = new Fixture();
            var model = new PublishViewModel { ResourceVersionId = fixture.Create<int>(), UserId = fixture.Create<int>() };
            this.SetupMockForPublishResource(ResourceTypeEnum.Image);
            this.mockNotificationService.Setup(m => m.CreatePublishFailedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(fixture.Create<int>()));

            // Act
            await this.resourceService.SetResourceVersionFailedToPublish(model);

            // Assert
            this.mockNotificationService.Verify(m => m.CreatePublishFailedNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.mockUserNotificationService.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()), Times.Once);
        }

        /// <summary>
        /// The create resource provider is valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateResourceProvider_Valid()
        {
            // Arrange
            this.mockResourceVersionProviderRepository.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<ResourceVersionProvider>()))
                                        .ReturnsAsync(1001);

            // Act
            var result = await this.resourceService.CreateResourceVersionProviderAsync(1, 1, 27);

            // Assert
            this.mockResourceVersionProviderRepository.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<ResourceVersionProvider>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
            Assert.Equal(1001, result.CreatedId);
        }

        /// <summary>
        /// The delete resource provider is valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task DeleteResourceProvider_Valid()
        {
            // Arrange
            this.mockResourceVersionProviderRepository.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                        .Returns(Task.CompletedTask);

            // Act
            var result = await this.resourceService.DeleteResourceVersionProviderAsync(1, 1, 27);

            // Assert
            this.mockResourceVersionProviderRepository.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// The delete resource provider is invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task DeleteResourceProvider_InValid()
        {
            // Arrange
            this.mockResourceVersionProviderRepository.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception());

            // Act
            var result = await this.resourceService.DeleteResourceVersionProviderAsync(1, 1, 27);

            // Assert
            this.mockResourceVersionProviderRepository.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// The delete all resource provider is valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task DeleteAllResourceProvider_Valid()
        {
            // Arrange
            this.mockResourceVersionProviderRepository.Setup(r => r.DeleteAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                                        .Returns(Task.CompletedTask);

            // Act
            var result = await this.resourceService.DeleteAllResourceVersionProviderAsync(1, 1);

            // Assert
            this.mockResourceVersionProviderRepository.Verify(x => x.DeleteAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// The delete all resource provider is invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task DeleteAllResourceProvider_InValid()
        {
            // Arrange
            this.mockResourceVersionProviderRepository.Setup(r => r.DeleteAllAsync(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception());

            // Act
            var result = await this.resourceService.DeleteAllResourceVersionProviderAsync(1, 1);

            // Assert
            this.mockResourceVersionProviderRepository.Verify(x => x.DeleteAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
        }

        private void SetupMockForPublishResource(ResourceTypeEnum resourceType, bool published = true, bool articleHasMediaFile = false, int timeToPublish = 0)
        {
            var fixture = new Fixture();
            var resourceVersion = Mock.Of<ResourceVersion>();
            resourceVersion.Resource = new Resource { ResourceTypeEnum = resourceType };
            resourceVersion.ResourceVersionAuthor = new[] { new ResourceVersionAuthor { AuthorUserId = fixture.Create<int>() } };
            resourceVersion.CreateUser = new User { UserName = fixture.Create<string>() };
            resourceVersion.ResourceId = fixture.Create<int>();
            resourceVersion.Id = fixture.Create<int>();

            this.mockResourceVersionRepo
                .Setup(r => r.Publish(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
                .Returns(fixture.Create<int>());

            this.mockResourceVersionRepo
                .Setup(r => r.GetBasicByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(resourceVersion);

            this.mockResourceSyncService
                .Setup(r => r.BuildSearchResourceRequestModel(It.IsAny<int>()))
                .ReturnsAsync(Mock.Of<SearchResourceRequestModel>());

            this.mockSearchService
                .Setup(r => r.SendResourceForSearchAsync(It.IsAny<SearchResourceRequestModel>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(published);

            this.mockNotificationService
                .Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<NotificationViewModel>()))
                .ReturnsAsync(Mock.Of<LearningHubValidationResult>());

            this.mockUserNotificationService
                .Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<UserNotification>()))
                .ReturnsAsync(Mock.Of<LearningHubValidationResult>());

            var events = new[]
            {
                new ResourceVersionEvent
                {
                    ResourceVersionEventType = ResourceVersionEventTypeEnum.Publishing,
                    AmendDate = DateTimeOffset.Now.AddSeconds(-1 * timeToPublish),
                },
            }.AsEnumerable();

            this.mockResourceVersionEventRepo
                .Setup(r => r.GetByResourceVersionIdAsync(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceVersionEvent>(events));

            if (articleHasMediaFile)
            {
                this.mockResourceVersionRepo
                    .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
                    .ReturnsAsync(resourceVersion);

                this.mockResourceReferenceRepo
                    .Setup(r => r.GetDefaultByResourceIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(Mock.Of<ResourceReference>());

                this.mockArticleResourceVersionRepos
                    .Setup(r => r.GetByResourceVersionIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
                    .ReturnsAsync(Mock.Of<ArticleResourceVersion>());

                this.mockMapper
                    .Setup(r => r.Map<ArticleViewModel>(It.IsAny<ArticleResourceVersion>()))
                    .Returns(new ArticleViewModel { Files = new[] { new FileViewModel { FileName = "test.mp3" } }.ToList() });

                var fileTypes = new[] { new FileType { Extension = "mp3", DefaultResourceTypeId = 2 }, }.AsEnumerable();

                this.mockFileTypeRepository
                    .Setup(r => r.GetAll())
                    .Returns(new AsyncEnumerable<FileType>(fileTypes));
            }

            var cachingServiceRetVal = new CacheReadResponse<ResourceVersionExtendedViewModel>
            {
                ResponseEnum = CacheReadResponseEnum.NotFound,
            };
            this.mockCachingService
                .Setup(s => s.GetAsync<ResourceVersionExtendedViewModel>(It.IsAny<string>()))
                .ReturnsAsync(cachingServiceRetVal);

            this.mockNodeResourceRepo
                .Setup(r => r.GetByResourceIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<NodeResource>() { Mock.Of<NodeResource>() });

            var mockNodePath = Mock.Of<NodePath>();
            mockNodePath.IsActive = true;
            this.mockNodePathRepo
                .Setup(r => r.GetNodePathsForNodeId(It.IsAny<int>()))
                .ReturnsAsync(new List<NodePath>() { mockNodePath });

            this.mockPublicationRepo
                .Setup(r => r.GetCacheOperations(It.IsAny<int>()))
                .ReturnsAsync(new List<CacheOperationViewModel>() { Mock.Of<CacheOperationViewModel>() });

            var mockNodeResource = Mock.Of<NodeResource>();
            mockNodeResource.VersionStatusEnum = VersionStatusEnum.Published;
            this.mockNodeResourceRepo
                .Setup(r => r.GetByResourceIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<NodeResource>() { mockNodeResource });

            this.mockInternalSystemService
                .Setup(r => r.GetByIdAsync(2))
                .Returns(Task.FromResult(new Models.Maintenance.InternalSystemViewModel { Id = 2, Name = "ResourcePublishQueue" }));
        }

        private void SetupSettings()
        {
            var fixture = new Fixture();
            this.mockSettings.Setup(r => r.Value)
                .Returns(new Settings
                {
                    Notifications = new NotificationSettings
                    {
                        PublishResourceTimeToProcessInSec = 30,
                        ResourcePublished = fixture.Create<string>(),
                        ResourcePublishFailedTitle = fixture.Create<string>(),
                        ResourcePublishFailed = fixture.Create<string>(),
                    },
                });
        }
    }
}