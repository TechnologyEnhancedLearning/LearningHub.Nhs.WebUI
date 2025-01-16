namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Catalogue controller.
    /// </summary>
    [Route("Catalogues")]
    [Authorize]
    public class CatalogueController : OpenApiControllerBase
    {
        private readonly ICatalogueService catalogueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueController"/> class.
        /// </summary>
        /// <param name="catalogueService">The catalogue service.</param>
        public CatalogueController(ICatalogueService catalogueService)
        {
            this.catalogueService = catalogueService;
        }

        /// <summary>
        /// Get all catalogues.
        /// </summary>
        /// <returns>BulkCatalogueViewModel.</returns>
        [HttpGet]
        public async Task<BulkCatalogueViewModel> GetAllCatalogues()
        {
            return await this.catalogueService.GetAllCatalogues();
        }

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="id">The catalogue node version id.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("Catalogues/{id}")]
        public async Task<IActionResult> GetCatalogue(int id)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(id);
            return this.Ok(catalogue);
        }

        /// <summary>
        /// The GetCatalogueResources.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <param name="sortColumn">The sortColumn.</param>
        /// <param name="sortDirection">The sortDirection.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The catalogue resources.</returns>
        [HttpGet]
        [Route("Resources/{id}/{page}/{pageSize}/{sortColumn}/{sortDirection}/{filter}")]
        public async Task<IActionResult> GetCatalogueResources(int id, int page, int pageSize, string sortColumn, string sortDirection, string filter)
        {
            var catalogueResourceViewModel = await this.catalogueService.GetResourcesAsync(this.CurrentUserId.GetValueOrDefault(), id, page, pageSize, sortColumn, sortDirection, filter);
            return this.Ok(catalogueResourceViewModel);
        }

        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost]
        [Route("resources")]
        public async Task<IActionResult> GetResources(CatalogueResourceRequestViewModel requestViewModel)
        {
            var response = await this.catalogueService.GetResourcesAsync(requestViewModel);
            return this.Ok(response);
        }
    }
}
