namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Databricks;
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="IRegionService" />.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// The GetReporterPermission.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> GetReporterPermission();

        /// <summary>
        /// The GetCourseCompletionReport.
        /// </summary>
        /// <param name="requestModel">The requestModel.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<DatabricksDetailedViewModel> GetCourseCompletionReport(DatabricksRequestModel requestModel);

        /// <summary>
        /// The GetReportHistory.
        /// </summary>
        /// <param name="requestModel">The requestModel.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<PagedResultSet<ReportHistoryModel>> GetReportHistory(PagingRequestModel requestModel);

        /// <summary>
        /// The GetReportHistory.
        /// </summary>
        /// <param name="reportHistoryId">The reportHistoryId.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<ReportHistoryModel> GetReportHistoryById(int reportHistoryId);

        /// <summary>
        /// The QueueReportDownload.
        /// </summary>
        /// <param name="reportHistoryId">The reportHistoryId.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<bool> QueueReportDownload(int reportHistoryId);

        /// <summary>
        /// The DownloadReport.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<ReportHistoryModel> DownloadReport(int reportHistoryId);
    }
}
