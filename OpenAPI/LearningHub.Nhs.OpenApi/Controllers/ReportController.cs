namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Databricks;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Report Controller.
    /// </summary>
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("Report")]
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

        /// <summary>
        /// Get CourseCompletionReport from Databricks.
        /// </summary>
        /// <param name="requestModel">requestModel.</param>
        /// <returns>Task.</returns>
        [HttpPost]
        [Route("GetCourseCompletionReport")]
        public async Task<DatabricksDetailedViewModel> CourseCompletionReport(DatabricksRequestModel requestModel)
        {
            return await this.databricksService.CourseCompletionReport(this.CurrentUserId.GetValueOrDefault(),requestModel);
        }

        /// <summary>
        /// Get CourseCompletionReport from Databricks.
        /// </summary>
        /// <param name="request">request.</param>
        /// <returns>Task.</returns>
        [HttpPost]
        [Route("GetReportHistory")]
        public async Task<PagedResultSet<ReportHistoryModel>> GetReportHistory(PagingRequestModel request)
        {
            return await this.databricksService.GetPagedReportHistory(this.CurrentUserId.GetValueOrDefault(), request.Page, request.PageSize);
        }

        /// <summary>
        /// Get GetReportHistoryById.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>Task.</returns>
        [HttpGet]
        [Route("GetReportHistoryById/{reportHistoryId}")]
        public async Task<ReportHistoryModel> GetReportHistoryById(int reportHistoryId)
        {
            return await this.databricksService.GetPagedReportHistoryById(this.CurrentUserId.GetValueOrDefault(), reportHistoryId);
        }

        /// <summary>
        /// Get QueueReportDownload.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>Task.</returns>
        [HttpGet]
        [Route("QueueReportDownload/{reportHistoryId}")]
        public async Task<bool> QueueReportDownload(int reportHistoryId)
        {
            return await this.databricksService.QueueReportDownload(this.CurrentUserId.GetValueOrDefault(), reportHistoryId);
        }

        /// <summary>
        /// Get DownloadReport.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>Task.</returns>
        [HttpGet]
        [Route("DownloadReport/{reportHistoryId}")]
        public async Task<ReportHistoryModel> DownloadReport(int reportHistoryId)
        {
            return await this.databricksService.DownloadReport(this.CurrentUserId.GetValueOrDefault(), reportHistoryId);
        }

        /// <summary>
        /// DatabricksJobNotify.
        /// </summary>
        /// <param name="databricksNotification">databricksNotification.</param>
        /// <returns>Task.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("DatabricksJobNotify")]
        public async Task<IActionResult> DatabricksJobNotify(DatabricksNotification databricksNotification)
        {
            await this.databricksService.DatabricksJobUpdate(this.CurrentUserId.GetValueOrDefault(), databricksNotification);
            return this.Ok(new ApiResponse(true));
        }

        /// <summary>
        /// UpdateDatabricksReport.
        /// </summary>
        /// <param name="databricksUpdateRequest">databricksUpdateRequest.</param>
        /// <returns>Task.</returns>
        [HttpPost]
        [Route("UpdateDatabricksReport")]
        public async Task<IActionResult> UpdateDatabricksReport(DatabricksUpdateRequest databricksUpdateRequest)
        {
            await this.databricksService.UpdateDatabricksReport(this.CurrentUserId.GetValueOrDefault(), databricksUpdateRequest);
            return this.Ok(new ApiResponse(true));
        }
    }
}
