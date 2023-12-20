// <copyright file="CatalogueServiceTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentAssertions;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Messaging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// CatalogueServiceTests.
    /// </summary>
    public class CatalogueServiceTests : TestBase
    {
        private readonly Mock<IUserRepository> mockUserRepository;
        private readonly Mock<IResourceVersionRepository> mockResourceVersionRepository;
        private readonly Mock<IFindwiseApiFacade> mockFindwiseApiFacade;
        private Mock<IRoleUserGroupRepository> mockRoleUserGroupRepository;
        private Mock<IUserUserGroupRepository> mockUserUserGroupRepository;
        private MockRepository mockRepository;

        private Mock<IMapper> mockMapper;
        private Mock<INodeRepository> mockNodeRepository;
        private Mock<ICatalogueNodeVersionRepository> mockCatalogueNodeVersionRepository;
        private Mock<INodeResourceRepository> mockNodeResourceRepository;
        private Mock<ICatalogueAccessRequestRepository> mockCatalogueAccessRequestRepository;
        private Mock<IUserProfileRepository> mockUserDetailsRepository;
        private Mock<IOptions<Settings>> mockOptions;
        private Mock<IEmailSenderService> mockEmailSenderService;
        private Mock<INotificationSenderService> mockNotificationSenderService;
        private Mock<IProviderService> mockProviderService;
        ////private Mock<elfhHub.Nhs.Services.Interface.IUserService> mockUserService;
        private Mock<INodeActivityRepository> mockNodeActivityRepository;
        private Mock<IBookmarkRepository> mockBookmarkRepository;
        private Mock<ITimezoneOffsetManager> mockTimezoneOffsetManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueServiceTests"/> class.
        /// </summary>
        public CatalogueServiceTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockMapper = this.mockRepository.Create<IMapper>();
            this.mockNodeRepository = this.mockRepository.Create<INodeRepository>();
            this.mockCatalogueNodeVersionRepository = this.mockRepository.Create<ICatalogueNodeVersionRepository>();
            this.mockNodeResourceRepository = this.mockRepository.Create<INodeResourceRepository>();
            this.mockRoleUserGroupRepository = this.mockRepository.Create<IRoleUserGroupRepository>();
            this.mockUserRepository = this.mockRepository.Create<IUserRepository>();
            this.mockResourceVersionRepository = this.mockRepository.Create<IResourceVersionRepository>();
            this.mockFindwiseApiFacade = this.mockRepository.Create<IFindwiseApiFacade>();
            this.mockRoleUserGroupRepository = this.mockRepository.Create<IRoleUserGroupRepository>();
            this.mockUserUserGroupRepository = this.mockRepository.Create<IUserUserGroupRepository>();
            this.mockEmailSenderService = this.mockRepository.Create<IEmailSenderService>();
            this.mockNodeActivityRepository = this.mockRepository.Create<INodeActivityRepository>();
            ////this.mockUserService = new Mock<elfhHub.Nhs.Services.Interface.IUserService>();
            this.mockCatalogueAccessRequestRepository = this.mockRepository.Create<ICatalogueAccessRequestRepository>();
            this.mockNotificationSenderService = this.mockRepository.Create<INotificationSenderService>();
            this.mockProviderService = this.mockRepository.Create<IProviderService>();
            this.mockUserDetailsRepository = this.mockRepository.Create<IUserProfileRepository>();
            this.mockBookmarkRepository = this.mockRepository.Create<IBookmarkRepository>();
            this.mockTimezoneOffsetManager = this.mockRepository.Create<ITimezoneOffsetManager>();
            this.mockOptions = this.mockRepository.Create<IOptions<Settings>>();
            this.mockOptions.SetupGet(x => x.Value)
                .Returns(new Settings());
        }

        /// <summary>
        /// GetCatalogueByReference_Returns_Success.
        /// </summary>
        /// <param name="urlReference">urlReference.</param>
        /// <param name="catalogueViewModel">catalogueViewModel.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Theory]
        [ClassData(typeof(CatalogueTestData))]
        public async Task GetCatalogueByReference_Returns_Success(string urlReference, CatalogueViewModel catalogueViewModel)
        {
            // Arrange
            var catalogueService = this.CreateService();

            this.mockCatalogueNodeVersionRepository.Setup(cs => cs.GetCatalogueAsync(urlReference)).Returns(Task.FromResult(catalogueViewModel));
            this.mockNodeActivityRepository.Setup(nar => nar.CreateAsync(It.IsAny<int>(), It.IsAny<NodeActivity>())).Returns(Task.FromResult(1));

            // Act
            var result = await catalogueService.GetCatalogueAsync(urlReference);

            // Assert
            Assert.IsType<CatalogueViewModel>(result);

            var catalogueResult = result as CatalogueViewModel;
            catalogueResult.Url.Should().Be(urlReference);
        }

        /// <summary>
        /// GetResources_Returns_RelatedResources.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <param name="responseViewModel">responseViewModel.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Theory]
        [ClassData(typeof(CatalogueResourcesTestData))]
        public async Task GetResources_Returns_RelatedResources(CatalogueResourceRequestViewModel requestViewModel, CatalogueResourceResponseViewModel responseViewModel)
        {
            // Arrange
            var catalogueService = this.CreateService();

            this.mockNodeResourceRepository.Setup(nr => nr.GetResourcesAsync(requestViewModel.NodeId, requestViewModel.CatalogueOrder, requestViewModel.Offset)).Returns(Task.FromResult(responseViewModel));

            // Act
            var result = await catalogueService.GetResourcesAsync(requestViewModel);

            // Assert
            Assert.IsType<CatalogueResourceResponseViewModel>(result);

            var catalogueResourceResult = result as CatalogueResourceResponseViewModel;
            catalogueResourceResult.TotalResources.Should().Be(responseViewModel.TotalResources);
            catalogueResourceResult.TotalResources.Should().Be(responseViewModel.CatalogueResources.Count());
            catalogueResourceResult.NodeId.Should().Be(responseViewModel.NodeId);
        }

        private CatalogueService CreateService()
        {
            return new CatalogueService(
                this.MapperProfile(),
                this.mockNodeRepository.Object,
                this.mockCatalogueNodeVersionRepository.Object,
                this.mockNodeResourceRepository.Object,
                this.mockUserRepository.Object,
                this.mockRoleUserGroupRepository.Object,
                this.mockResourceVersionRepository.Object,
                this.mockFindwiseApiFacade.Object,
                this.mockUserUserGroupRepository.Object,
                this.mockEmailSenderService.Object,
                ////this.mockUserService.Object,
                this.mockCatalogueAccessRequestRepository.Object,
                this.mockUserDetailsRepository.Object,
                this.mockNotificationSenderService.Object,
                this.mockProviderService.Object,
                this.mockOptions.Object,
                this.mockNodeActivityRepository.Object,
                this.mockBookmarkRepository.Object,
                this.mockTimezoneOffsetManager.Object);
        }

        /// <summary>
        /// CatalogueResourcesTestData.
        /// </summary>
        public class CatalogueResourcesTestData : TheoryData<CatalogueResourceRequestViewModel, CatalogueResourceResponseViewModel>
        {
            private readonly Random randomNumberGenerator = new Random();

            /// <summary>
            /// Initializes a new instance of the <see cref="CatalogueResourcesTestData"/> class.
            /// </summary>
            public CatalogueResourcesTestData()
            {
                var expectedResources = 7;
                var catalogueResourcesSinglePage = new List<CatalogueResourceViewModel>();
                for (int i = 0; i < expectedResources; i++)
                {
                    catalogueResourcesSinglePage.Add(new CatalogueResourceViewModel
                    {
                        ResourceId = this.randomNumberGenerator.Next(101, 9999).ToString(),
                        ResourceReferenceId = this.randomNumberGenerator.Next(101, 9999).ToString(),
                        ResourceVersionId = this.randomNumberGenerator.Next(101, 9999).ToString(),
                        AuthoredBy = $"Test Author {this.randomNumberGenerator.Next(101, 9999)}",
                        RatingCount = 5,
                        Rating = 4,
                        Organisation = $"Test Org {this.randomNumberGenerator.Next(101, 9999)}",
                        Title = "Ttile ",
                        Description = "Description",
                        Type = ((ResourceTypeEnum)this.randomNumberGenerator.Next(1, 9)).ToString(),
                    });
                }

                this.Add(
                    new CatalogueResourceRequestViewModel { NodeId = 1, Offset = 0, CatalogueOrder = Models.Enums.CatalogueOrder.DateDescending },
                    new CatalogueResourceResponseViewModel { NodeId = 1, TotalResources = expectedResources, CatalogueResources = catalogueResourcesSinglePage });

                expectedResources = 16;
                var catalogueResourcesMultiplePages = new List<CatalogueResourceViewModel>();
                for (int i = 0; i < expectedResources; i++)
                {
                    catalogueResourcesMultiplePages.Add(new CatalogueResourceViewModel
                    {
                        ResourceId = this.randomNumberGenerator.Next(101, 9999).ToString(),
                        ResourceReferenceId = this.randomNumberGenerator.Next(101, 9999).ToString(),
                        ResourceVersionId = this.randomNumberGenerator.Next(101, 9999).ToString(),
                        AuthoredBy = $"Test Author {this.randomNumberGenerator.Next(101, 9999)}",
                        RatingCount = 5,
                        Rating = 4,
                        Organisation = $"Test Org {this.randomNumberGenerator.Next(101, 9999)}",
                        Title = "Ttile ",
                        Description = "Description",
                        Type = ((ResourceTypeEnum)this.randomNumberGenerator.Next(1, 9)).ToString(),
                    });
                }

                this.Add(
                 new CatalogueResourceRequestViewModel { NodeId = 2, Offset = 10, CatalogueOrder = Models.Enums.CatalogueOrder.DateDescending },
                 new CatalogueResourceResponseViewModel { NodeId = 2, TotalResources = expectedResources, CatalogueResources = catalogueResourcesMultiplePages });
            }
        }

        /// <summary>
        /// CatalogueTestData.
        /// </summary>
        public class CatalogueTestData : TheoryData<string, CatalogueViewModel>
        {
            private readonly Random randomNumberGenerator;

            /// <summary>
            /// Initializes a new instance of the <see cref="CatalogueTestData"/> class.
            /// </summary>
            public CatalogueTestData()
            {
                this.randomNumberGenerator = new Random();

                this.CatalogueViewModels = new List<CatalogueViewModel> { };

                for (int i = 0; i < 5; i++)
                {
                    var id = this.randomNumberGenerator.Next(1, 999);
                    this.CatalogueViewModels.Add(new CatalogueViewModel
                    {
                        NodeVersionId = id,
                        Url = $"some-url-{i}",
                        BannerUrl = $"http://someserver.co.uk/mybanner-{id}.png",
                        BadgeUrl = $"http://someserver.co.uk/mybadge-{id}.png",
                        Name = $"Test Catalogue {id}",
                        Description = "Some random description {id}",
                        ResourceOrder = 0,
                    });
                }

                this.Add("some-url-1", this.CatalogueViewModels.SingleOrDefault(cnv => cnv.Url == "some-url-1"));
                this.Add("some-url-2", this.CatalogueViewModels.SingleOrDefault(cnv => cnv.Url == "some-url-2"));
                this.Add("some-url-3", this.CatalogueViewModels.SingleOrDefault(cnv => cnv.Url == "some-url-3"));
                this.Add("some-url-4", this.CatalogueViewModels.SingleOrDefault(cnv => cnv.Url == "some-url-4"));
            }

            /// <summary>
            /// Gets or sets CatalogueViewModels.
            /// </summary>
            public List<CatalogueViewModel> CatalogueViewModels { get; set; }
        }
    }
}