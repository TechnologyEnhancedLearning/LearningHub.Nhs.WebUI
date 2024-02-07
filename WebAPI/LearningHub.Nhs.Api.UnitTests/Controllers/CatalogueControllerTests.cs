// <copyright file="CatalogueControllerTests.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FluentAssertions;
    using LearningHub.Nhs.Api.Controllers;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// CatalogueControllerTests.
    /// </summary>
    public class CatalogueControllerTests
    {
        private MockRepository mockRepository;
        private Mock<IUserService> mockUserService;
        private Mock<ICatalogueService> mockCatalogueService;
        private Mock<ILogger<CatalogueController>> mockLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueControllerTests"/> class.
        /// </summary>
        public CatalogueControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            this.mockCatalogueService = this.mockRepository.Create<ICatalogueService>(MockBehavior.Strict);
            this.mockLogger = this.mockRepository.Create<ILogger<CatalogueController>>(MockBehavior.Loose);
        }

        /// <summary>
        /// GetCatalogueByReference_Returns_Success.
        /// </summary>
        /// <param name="urlReference">urlReference.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Theory]
        [InlineData("community-contibutions")]
        [InlineData("test-catalogue-1")]
        [InlineData("test-catalogue-2")]
        public async Task GetCatalogueByReference_Returns_Success(string urlReference)
        {
            // Arrange
            var catalogueController = this.GetCatalogueController();
            var ran = new Random();
            var expectedNodeId = ran.Next(100, 1000);
            this.mockCatalogueService.Setup(cs => cs.GetCatalogueAsync(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(new CatalogueViewModel
            {
                NodeId = expectedNodeId,
                Url = urlReference,
                BannerUrl = "http://someserver.co.uk/mybanner.png",
                BadgeUrl = "http://someserver.co.uk/mybadge.png",
                Name = $"Test Catalogue {expectedNodeId}",
                Description = "Some random description",
            }));

            // Act
            var result = await catalogueController.GetCatalogueByReference(urlReference);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var catalogueResult = okResult.Value as CatalogueViewModel;
            catalogueResult.NodeId.Should().Be(expectedNodeId);
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
            var catalogueController = this.GetCatalogueController();

            this.mockCatalogueService.Setup(cs => cs.GetResourcesAsync(It.IsAny<CatalogueResourceRequestViewModel>())).Returns(Task.FromResult(responseViewModel));

            // Act
            var result = await catalogueController.GetResources(requestViewModel);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var catalogueResourcesResult = okResult.Value as CatalogueResourceResponseViewModel;
            catalogueResourcesResult.NodeId.Should().Be(requestViewModel.NodeId);
            catalogueResourcesResult.TotalResources.Should().Be(responseViewModel.TotalResources);
        }

        private CatalogueController GetCatalogueController()
        {
            var catalogueController = new CatalogueController(
                this.mockUserService.Object,
                this.mockCatalogueService.Object,
                this.mockLogger.Object)
            {
                ControllerContext = this.SetContollerContext(),
            };
            return catalogueController;
        }

        private ControllerContext SetContollerContext()
        {
            IList<Claim> claimCollection = new List<Claim>
                    {
                        new Claim("given_name", "TestUser"),
                        new Claim("sub", "999"),
                    };

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claimCollection)) },
            };

            return context;
        }

        /// <summary>
        /// Test Data.
        /// </summary>
        public class CatalogueResourcesTestData : TheoryData<CatalogueResourceRequestViewModel, CatalogueResourceResponseViewModel>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CatalogueResourcesTestData"/> class.
            /// </summary>
            public CatalogueResourcesTestData()
            {
                this.Add(
                    new CatalogueResourceRequestViewModel
                    {
                        NodeId = 101,
                        CatalogueOrder = Models.Enums.CatalogueOrder.AlphabeticalAscending,
                        Offset = 0,
                    },
                    new CatalogueResourceResponseViewModel
                    {
                        NodeId = 101,
                        TotalResources = 7,
                        CatalogueResources = new List<CatalogueResourceViewModel> { },
                    });
                this.Add(
                new CatalogueResourceRequestViewModel
                {
                    NodeId = 301,
                    CatalogueOrder = Models.Enums.CatalogueOrder.DateDescending,
                    Offset = 0,
                },
                new CatalogueResourceResponseViewModel
                {
                    NodeId = 301,
                    TotalResources = 29,
                    CatalogueResources = new List<CatalogueResourceViewModel> { },
                });
                this.Add(
                new CatalogueResourceRequestViewModel
                {
                    NodeId = 301,
                    CatalogueOrder = Models.Enums.CatalogueOrder.DateDescending,
                    Offset = 10,
                },
                new CatalogueResourceResponseViewModel
                {
                    NodeId = 301,
                    TotalResources = 29,
                    CatalogueResources = new List<CatalogueResourceViewModel> { },
                });
            }
        }
    }
}
