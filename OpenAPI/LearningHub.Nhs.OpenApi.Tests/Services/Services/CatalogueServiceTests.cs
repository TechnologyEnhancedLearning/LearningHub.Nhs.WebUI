namespace LearningHub.Nhs.OpenApi.Tests.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using LearningHub.Nhs.OpenApi.Tests.TestHelpers;
    using Moq;
    using Xunit;

    public class CatalogueServiceTests
    {
        private readonly Mock<ICatalogueRepository> catalogueRepository;
        private readonly CatalogueService catalogueService;

        public CatalogueServiceTests()
        {
            this.catalogueRepository = new Mock<ICatalogueRepository>();
            this.catalogueService = new CatalogueService(this.catalogueRepository.Object);
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