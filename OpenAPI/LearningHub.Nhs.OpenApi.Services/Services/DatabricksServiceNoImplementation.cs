using LearningHub.Nhs.Models.Common;
using LearningHub.Nhs.Models.Databricks;
using LearningHub.Nhs.OpenApi.Models.ViewModels;
using LearningHub.Nhs.OpenApi.Services.Interface.Services;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Services
{
    /// <summary>
    /// DatabricksServiceNoImplementation
    /// </summary>
    public class DatabricksServiceNoImplementation : IDatabricksService
    {
        /// <inheritdoc/>
        public Task<bool> IsUserReporter(int userId)
        {
            // Feature disabled → always return false
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<DatabricksDetailedViewModel> CourseCompletionReport(
            int userId,
            DatabricksRequestModel model)
        {
            // Return an empty model to avoid null reference issues
            return Task.FromResult(new DatabricksDetailedViewModel());
        }

        /// <inheritdoc/>
        public Task<PagedResultSet<ReportHistoryModel>> GetPagedReportHistory(
            int userId,
            int page,
            int pageSize)
        {
            // Return an empty paged result
            return Task.FromResult(new PagedResultSet<ReportHistoryModel>
            {
                Items = new List<ReportHistoryModel>(),
                TotalItemCount = 0
            });
        }

        /// <inheritdoc/>
        public Task<ReportHistoryModel> GetPagedReportHistoryById(
            int userId,
            int reportHistoryId)
        {
            // Return an empty model
            return Task.FromResult(new ReportHistoryModel());
        }

        /// <inheritdoc/>
        public Task<bool> QueueReportDownload(int userId, int reportHistoryId)
        {
            // Pretend the queue operation succeeded
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<ReportHistoryModel> DownloadReport(int userId, int reportHistoryId)
        {
            // Return an empty model
            return Task.FromResult(new ReportHistoryModel());
        }

        /// <inheritdoc/>
        public Task DatabricksJobUpdate(int userId, DatabricksNotification databricksNotification)
        {
            // No-op
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task UpdateDatabricksReport(int userId, DatabricksUpdateRequest databricksUpdateRequest)
        {
            // No-op
            return Task.CompletedTask;
        }
    }

}
