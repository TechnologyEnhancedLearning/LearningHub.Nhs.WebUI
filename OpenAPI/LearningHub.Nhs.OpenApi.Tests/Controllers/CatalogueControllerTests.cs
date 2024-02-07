// <copyright file="CatalogueControllerTests.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.NHS.OpenAPI.Controllers;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Tests.TestHelpers;
    using Moq;
    using Xunit;

    public sealed class CatalogueControllerTests : IDisposable
    {
        private readonly Mock<ICatalogueService> catalogueService;
        private CatalogueController? catalogueController;

        public CatalogueControllerTests()
        {
            this.catalogueService = new Mock<ICatalogueService>();
            this.catalogueController = new CatalogueController(this.catalogueService.Object);
        }

        private static List<CatalogueNodeVersion> CatalogueNodeVersionList => new List<CatalogueNodeVersion>()
        {
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 100, name: "Test 1"),
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 101, name: "Test 2"),
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 102, name: "Test 3"),
            CatalogueTestHelper.CreateCatalogueNodeVersion(id: 103, name: "Test 4", isRestricted: true),
        };

        private static List<CatalogueViewModel> CatalogueViewModelsList => new List<CatalogueViewModel>()
        {
            new CatalogueViewModel(CatalogueNodeVersionList[0]),
            new CatalogueViewModel(CatalogueNodeVersionList[1]),
            new CatalogueViewModel(CatalogueNodeVersionList[2]),
        };

        [Fact]
        public async Task CataloguesEndpointCallsServiceAndReturnsAllCatalogues()
        {
            // Given
            var bulkCatalogueViewModel = new BulkCatalogueViewModel(CatalogueViewModelsList);

            this.catalogueService.Setup(cs => cs.GetAllCatalogues())
                .ReturnsAsync(bulkCatalogueViewModel);
            this.catalogueController = new CatalogueController(
                this.catalogueService.Object);

            // When
            var x = await this.catalogueController.GetAllCatalogues();

            // Then
            x.Should().BeEquivalentTo(bulkCatalogueViewModel);
            this.catalogueService.Verify(service => service.GetAllCatalogues());
        }

        public void Dispose()
        {
            this.catalogueController?.Dispose();
        }
    }
}