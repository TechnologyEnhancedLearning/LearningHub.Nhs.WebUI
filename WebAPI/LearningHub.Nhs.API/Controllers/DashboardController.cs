// <copyright file="DashboardController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The DashboardController.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService dashboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="userService">userService.</param>
        /// <param name="dashboardService">dashboardService.</param>
        /// <param name="logger">The logger.</param>
        public DashboardController(IUserService userService, IDashboardService dashboardService, ILogger<DashboardController> logger)
            : base(userService, logger)
        {
            this.dashboardService = dashboardService;
        }

        /// <summary>
        ///  Gets resources.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("resources/{dashboardType}/{pageNumber}")]
        public async Task<ActionResult> GetResources(string dashboardType, int pageNumber = 1)
        {
           var response = await this.dashboardService.GetResources(dashboardType, pageNumber, this.CurrentUserId);
           return this.Ok(response);
        }

        /// <summary>
        /// Gets Catalogues.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("catalogues/{dashboardType}/{pageNumber}")]
        public async Task<IActionResult> GetCatalogues(string dashboardType, int pageNumber = 1)
        {
            var response = await this.dashboardService.GetCatalogues(dashboardType, pageNumber, this.CurrentUserId);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets Catalogues.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("myaccesslearning/{dashboardType}/{pageNumber}")]
        public async Task<ActionResult> GetMyAccessLearnings(string dashboardType, int pageNumber = 1)
        {
            var response = await this.dashboardService.GetMyAccessLearnings(dashboardType, pageNumber, this.CurrentUserId);
            return this.Ok(response);
        }
    }
}
