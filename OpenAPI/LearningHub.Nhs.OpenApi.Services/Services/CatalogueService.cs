// <copyright file="CatalogueService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;

    /// <summary>
    /// The resource service.
    /// </summary>
    public class CatalogueService : ICatalogueService
    {
        /// <summary>
        /// The learning hub service.
        /// </summary>
        private readonly ICatalogueRepository catalogueRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueService"/> class.
        /// The catalogue service.
        /// </summary>
        /// <param name="catalogueRepository">
        /// The <see cref="ICatalogueRepository"/>.
        /// </param>
        public CatalogueService(ICatalogueRepository catalogueRepository)
        {
            this.catalogueRepository = catalogueRepository;
        }

        /// <summary>
        /// Get all catalogues async.
        /// </summary>
        /// <returns>BulkCatalogueViewModel.</returns>
        public async Task<BulkCatalogueViewModel> GetAllCatalogues()
        {
            var catalogueNodeVersions = await this.catalogueRepository.GetAllCatalogues();
            var catalogueViewModels = catalogueNodeVersions.Select(c => new CatalogueViewModel(c)).ToList();
            return new BulkCatalogueViewModel(catalogueViewModels);
        }
    }
}