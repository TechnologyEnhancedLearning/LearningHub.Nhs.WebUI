// <copyright file="CatalogueController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Catalogue controller.
    /// </summary>
    [Route("Catalogues")]
    [Authorize]
    public class CatalogueController : Controller
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
    }
}
