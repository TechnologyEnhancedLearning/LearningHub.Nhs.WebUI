namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Databricks;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="RegionService" />.
    /// </summary>
    public class ReportService : BaseService<ReportService>, IReportService
    {
        private readonly ICacheService cacheService;
        private readonly IHttpContextAccessor contextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportService"/> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="contextAccessor">The contextAccessor.</param>
        /// <param name="learningHubHttpClient">The Web Api Http Client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">logger.</param>
        public ReportService(ICacheService cacheService, IHttpContextAccessor contextAccessor, ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<ReportService> logger)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
            this.cacheService = cacheService;
            this.contextAccessor = contextAccessor;
        }

        /// <summary>
        /// The GetAllAsync.
        /// </summary>
        /// <returns>The <see cref="T:Task{bool}"/>.</returns>
        public async Task<bool> GetReporterPermission()
        {
            bool response = false;
            var cacheKey = $"{this.contextAccessor.HttpContext.User.Identity.GetCurrentUserId()}:DatabricksReporter";
            response = await this.cacheService.GetOrFetchAsync(cacheKey, this.FetchReporterPermission);
            return response;
        }

        /// <summary>
        /// The GetCourseCompletionReport.
        /// </summary>
        /// <param name="requestModel">The requestModel.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<DatabricksDetailedViewModel> GetCourseCompletionReport(DatabricksRequestModel requestModel)
        {
            DatabricksDetailedViewModel apiResponse = null;
            var json = JsonConvert.SerializeObject(requestModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Report/GetCourseCompletionReport";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<DatabricksDetailedViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse;
        }

        /// <summary>
        /// The GetReportHistory.
        /// </summary>
        /// <param name="requestModel">The requestModel.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<PagedResultSet<ReportHistoryModel>> GetReportHistory(PagingRequestModel requestModel)
        {
            PagedResultSet<ReportHistoryModel> apiResponse = null;
            var json = JsonConvert.SerializeObject(requestModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Report/GetReportHistory";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<PagedResultSet<ReportHistoryModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse;
        }

        /// <summary>
        /// The GetReportHistory.
        /// </summary>
        /// <param name="reportHistoryId">The reportHistoryId.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<ReportHistoryModel> GetReportHistoryById(int reportHistoryId)
        {
            ReportHistoryModel apiResponse = null;
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = $"Report/GetReportHistoryById/{reportHistoryId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ReportHistoryModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse;
        }

        /// <summary>
        /// The QueueReportDownload.
        /// </summary>
        /// <param name="reportHistoryId">The reportHistoryId.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<bool> QueueReportDownload(int reportHistoryId)
        {
            bool apiResponse = false;
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = $"Report/QueueReportDownload/{reportHistoryId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<bool>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse;
        }

        /// <summary>
        /// The DownloadReport.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<ReportHistoryModel> DownloadReport(int reportHistoryId)
        {
            ReportHistoryModel apiResponse = null;
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = $"Report/DownloadReport/{reportHistoryId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ReportHistoryModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse;
        }

        private async Task<bool> FetchReporterPermission()
        {
            bool viewmodel = false;
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Report/GetReporterPermission";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<bool>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
