namespace LearningHub.Nhs.OpenApi.Tests.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentAssertions;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using LearningHub.Nhs.OpenApi.Tests.TestHelpers;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class CatalogueServiceTests
    {
        private readonly Mock<ICatalogueRepository> catalogueRepository;
        private readonly CatalogueService catalogueService;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<ICatalogueNodeVersionRepository> catalogueNodeVersionRepository;
        private readonly Mock<ICatalogueAccessRequestRepository> catalogueAccessRequestRepository;
        private readonly Mock<INodeResourceRepository> nodeResourceRepository;
        private readonly Mock<IResourceVersionRepository> resourceVersionRepository;
        private readonly Mock<IRoleUserGroupRepository> roleUserGroupRepository;
        private readonly Mock<IUserUserGroupRepository> userUserGroupRepository;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IProviderService> providerService;
        private readonly Mock<IUserProfileRepository> userProfileRepository;
        private readonly Mock<IEmailSenderService> emailSenderService;
        private readonly Mock<IBookmarkRepository> bookmarkRepository;
        private readonly Mock<INodeRepository> nodeRepository;
        private readonly Mock<INodeActivityRepository> nodeActivityRepository;
        private readonly Mock<IFindwiseApiFacade> findwiseApiFacade;
        private readonly Mock<IOptions<LearningHubConfig>> learningHubConfig;
        private readonly Mock<IOptions<FindwiseConfig>> findwiseConfig;
        private readonly Mock<INotificationSenderService> notificationSenderService;
        private readonly Mock<ITimezoneOffsetManager> timezoneOffsetManager;
        private readonly Mock<IGovMessageService> govMessageSevice;
        private readonly Mock<IEmailTemplateService> emailTemplateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueServiceTests"/> class.
        /// </summary>
        public CatalogueServiceTests()
        {
            this.catalogueRepository = new Mock<ICatalogueRepository>();
            this.mapper = new Mock<IMapper>();
            this.catalogueNodeVersionRepository = new Mock<ICatalogueNodeVersionRepository>();
            this.catalogueAccessRequestRepository = new Mock<ICatalogueAccessRequestRepository>();
            this.nodeResourceRepository = new Mock<INodeResourceRepository>();
            this.resourceVersionRepository = new Mock<IResourceVersionRepository>();
            this.roleUserGroupRepository = new Mock<IRoleUserGroupRepository>();
            this.userUserGroupRepository = new Mock<IUserUserGroupRepository>();
            this.userRepository = new Mock<IUserRepository>();
            this.providerService = new Mock<IProviderService>();
            this.userProfileRepository = new Mock<IUserProfileRepository>();
            this.emailSenderService= new Mock<IEmailSenderService>();
            this.bookmarkRepository = new Mock<IBookmarkRepository>();
            this.nodeRepository = new Mock<INodeRepository>();
            this.nodeActivityRepository = new Mock<INodeActivityRepository>();
            this.findwiseApiFacade = new Mock<IFindwiseApiFacade>();
            this.learningHubConfig = new Mock<IOptions<LearningHubConfig>>();
            this.findwiseConfig = new Mock<IOptions<FindwiseConfig>>();
            this.notificationSenderService = new Mock<INotificationSenderService>();
            this.timezoneOffsetManager = new Mock<ITimezoneOffsetManager>();
            this.govMessageSevice = new Mock<IGovMessageService>();
            this.emailTemplateService = new Mock<IEmailTemplateService>();
            this.catalogueService = new CatalogueService(this.catalogueRepository.Object, this.nodeRepository.Object, this.userUserGroupRepository.Object, this.mapper.Object, this.findwiseConfig.Object, this.learningHubConfig.Object, this.catalogueNodeVersionRepository.Object, this.nodeResourceRepository.Object, this.resourceVersionRepository.Object, this.roleUserGroupRepository.Object, this.providerService.Object, this.catalogueAccessRequestRepository.Object, this.userRepository.Object, this.userProfileRepository.Object, this.emailSenderService.Object, this.bookmarkRepository.Object, this.nodeActivityRepository.Object, this.findwiseApiFacade.Object, this.notificationSenderService.Object, this.timezoneOffsetManager.Object, this.govMessageSevice.Object, this.emailTemplateService.Object);
        }

        private static IEnumerable<CatalogueNodeVersion> CatalogueNodeVersionList => new List<CatalogueNodeVersion>()
        {
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 100, name: "Test 1", 100),
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 101, name: "Test 2", 101),
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 102, name: "Test 3", 102),
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 103, name: "Test 4", 103, true),
        };

        [Fact]
        public async Task CataloguesEndpointCallsRepositoryAndReturnsCataloguesCorrectly()
        {
            // Given
            this.catalogueRepository.Setup(cr => cr.GetAllCatalogues())
                .ReturnsAsync(CatalogueNodeVersionList);

            // When
            var x = await this.catalogueService.GetAllCatalogues();

            // Then
            var expectedCatalogueModels = CatalogueNodeVersionList.Select(c => new CatalogueViewModel(c)).ToList();
            x.Catalogues.Should().BeEquivalentTo(expectedCatalogueModels);
        }
    }
}