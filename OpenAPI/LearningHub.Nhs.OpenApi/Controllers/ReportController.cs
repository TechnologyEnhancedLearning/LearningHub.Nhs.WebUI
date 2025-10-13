namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Report Controller.
    /// </summary>
    [ApiController]
    [Authorize]
    public class ReportController : OpenApiControllerBase
    {
        private readonly IDatabricksService databricksService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController"/> class.
        /// </summary>
        /// <param name="databricksService">The catalogue service.</param>
        public ReportController(IDatabricksService databricksService)
        {
            this.databricksService = databricksService;
        }

        /// <summary>
        /// Get all catalogues.
        /// </summary>
        /// <returns>Task.</returns>
        [HttpGet]
        [Route("GetReporterPermission")]
        public async Task<bool> GetReporterPermission()
        {
            return await this.databricksService.IsUserReporter(this.CurrentUserId.GetValueOrDefault());
        }
    }
}
