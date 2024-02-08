namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// DashboardController.
    /// </summary>
    [Authorize]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;
        private readonly IFileService fileService;
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="dashboardService">dashboardService.</param>
        /// <param name="fileService">fileService.</param>
        public DashboardController(IDashboardService dashboardService, IFileService fileService)
        {
            this.fileService = fileService;
            this.dashboardService = dashboardService;
            this.filePath = "CatalogueImageDirectory";
        }

        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("resources/{dashboardType}/{pageNumber}")]
        public async Task<IActionResult> GetResources(string dashboardType, int pageNumber)
        {
            var responseViewModel = await this.dashboardService.GetResourcesAsync(dashboardType, pageNumber);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// GetCatalogues.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("catalogues/{dashboardType}/{pageNumber}")]
        public async Task<IActionResult> GetCatalogues(string dashboardType, int pageNumber)
        {
            var responseViewModel = await this.dashboardService.GetCataloguesAsync(dashboardType, pageNumber);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// GetCatalogues.
        /// </summary>
        /// <param name="dashboardEventViewModel">dashboardEventViewModel.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [Route("RecordEvent")]
        public async Task<IActionResult> RecordEvent([FromBody] DashboardEventViewModel dashboardEventViewModel)
        {
            await this.dashboardService.RecordDashBoardEventAsync(dashboardEventViewModel);
            return this.Ok(true);
        }

        /// <summary>
        /// Downloads the catalogue media.
        /// </summary>
        /// <param name="fileName">fileName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("download-image/{fileName}")]
        public async Task<IActionResult> DownloadImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return this.Content("Filename doesn't exist in request");
            }

            var file = await this.fileService.DownloadFileAsync(this.filePath, fileName);
            if (file != null)
            {
                return this.File(file.Content, file.ContentType, fileName);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }

        /// <summary>
        /// Records an analytics event before navigating to a particular URL. Progressive enhancement method to record analytics.
        /// </summary>
        /// <param name="eventType">The analytics event type.</param>
        /// <param name="url">The url to navigate to.</param>
        /// <param name="resourceReference">The resource reference, if the event concerns a particular resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IActionResult> RecordDashboardNavigation(EventTypeEnum eventType, string url, int? resourceReference)
        {
            var dashboardEventViewModel = new DashboardEventViewModel { EventType = eventType, ResourceReference = resourceReference };
            await this.dashboardService.RecordDashBoardEventAsync(dashboardEventViewModel);

            return this.Redirect(url);
        }
    }
}
