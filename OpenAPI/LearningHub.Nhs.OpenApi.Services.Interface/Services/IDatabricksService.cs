using LearningHub.Nhs.Models.Common;
using LearningHub.Nhs.Models.Databricks;
using LearningHub.Nhs.OpenApi.Models.ViewModels;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// IDatabricks service
    /// </summary>
    public interface IDatabricksService
    {
        /// <summary>
        /// IsUserReporter.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> IsUserReporter(int userId);

        /// <summary>
        /// CourseCompletionReport.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TReDatabricksDetailedViewModelsult}"/> representing the result of the asynchronous operation.</returns>
        Task<DatabricksDetailedViewModel> CourseCompletionReport(int userId, DatabricksRequestModel model);

        /// <summary>
        /// CourseCompletionReport.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PagedResultSet<ReportHistoryModel>> GetPagedReportHistory(int userId, int page, int pageSize);

        /// <summary>
        /// GetPagedReportHistoryById.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="reportHistoryId">The reportHistoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ReportHistoryModel> GetPagedReportHistoryById(int userId, int reportHistoryId);

        /// <summary>
        /// QueueReportDownload
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reportHistoryId"></param>
        /// <returns></returns>
        Task<bool> QueueReportDownload(int userId, int reportHistoryId);

        /// <summary>
        /// DownloadReport
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reportHistoryId"></param>
        /// <returns></returns>
        Task<ReportHistoryModel> DownloadReport(int userId, int reportHistoryId);

        /// <summary>
        /// DatabricksJobUpdate.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databricksNotification"></param>
        /// <returns></returns>
        Task DatabricksJobUpdate(int userId, DatabricksNotification databricksNotification);

        /// <summary>
        /// DatabricksJobUpdate.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="databricksUpdateRequest">databricksUpdateRequest.</param>
        /// <returns></returns>
        Task UpdateDatabricksReport(int userId, DatabricksUpdateRequest databricksUpdateRequest);
    }
}
