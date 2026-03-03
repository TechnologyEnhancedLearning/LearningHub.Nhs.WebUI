namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The DashboardController.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("Dashboard")]
    [ApiController]
    public class DashboardController : OpenApiControllerBase
    {
        private readonly IDashboardService dashboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="userService">userService.</param>
        /// <param name="dashboardService">dashboardService.</param>
        /// <param name="logger">The logger.</param>
        public DashboardController(IUserService userService, IDashboardService dashboardService, ILogger<DashboardController> logger)
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
           var response = await dashboardService.GetResources(dashboardType, pageNumber, this.CurrentUserId.GetValueOrDefault());
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
            var response = await dashboardService.GetCatalogues(dashboardType, pageNumber, this.CurrentUserId.GetValueOrDefault());
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
            var response = await dashboardService.GetMyAccessLearnings(dashboardType, pageNumber, this.CurrentUserId.GetValueOrDefault());
            return this.Ok(response);
        }

        /// <summary>
        /// Gets Catalogues.
        /// </summary>
        /// <param name="dashboardTrayLearningResourceType">The dashboardTrayLearningResource type.</param>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("GetMyCoursesAndElearning/{dashboardTrayLearningResourceType}/{dashboardType}/{pageNumber}")]
        public async Task<ActionResult> GetMyCoursesAndElearning(string dashboardTrayLearningResourceType, string dashboardType, int pageNumber = 1)
        {
            var response = await dashboardService.GetMyCoursesAndElearning(dashboardTrayLearningResourceType, dashboardType, pageNumber, this.CurrentUserId.GetValueOrDefault(), "All");
            return this.Ok(response);
        }
    }
}
