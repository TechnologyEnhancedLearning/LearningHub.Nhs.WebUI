﻿namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The dashboard service.
    /// </summary>
    public class DashboardService : BaseService<DashboardService>, IDashboardService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">learningHubHttpClient.</param>
        /// <param name="logger">logger.</param>
        public DashboardService(ILearningHubHttpClient learningHubHttpClient, ILogger<DashboardService> logger)
         : base(learningHubHttpClient, logger)
        {
        }

        /// <summary>
        /// GetCataloguesAsync.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardMyLearningResponseViewModel> GetMyAccessLearningsAsync(string dashboardType, int pageNumber)
        {
            DashboardMyLearningResponseViewModel viewmodel = new DashboardMyLearningResponseViewModel { };

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"dashboard/myaccesslearning/{dashboardType}/{pageNumber}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<DashboardMyLearningResponseViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// GetCataloguesAsync.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardCatalogueResponseViewModel> GetCataloguesAsync(string dashboardType, int pageNumber)
        {
            DashboardCatalogueResponseViewModel viewmodel = new DashboardCatalogueResponseViewModel { };

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"dashboard/catalogues/{dashboardType}/{pageNumber}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<DashboardCatalogueResponseViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardResourceResponseViewModel> GetResourcesAsync(string dashboardType, int pageNumber)
        {
            DashboardResourceResponseViewModel viewmodel = new DashboardResourceResponseViewModel { };

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"dashboard/resources/{dashboardType}/{pageNumber}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<DashboardResourceResponseViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// Logs Dashboared viewed event.
        /// </summary>
        /// <param name="dashboardEventViewModel">dashboardEventViewModel.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RecordDashBoardEventAsync(DashboardEventViewModel dashboardEventViewModel)
        {
            var eventEntity = new Event
            {
                EventTypeEnum = dashboardEventViewModel.EventType,
                JsonData = JsonConvert.SerializeObject(dashboardEventViewModel),
            };

            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(eventEntity), Encoding.UTF8, "application/json");
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"event/Create";
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error! {response.StatusCode} error code returned from request to record dashboard event.");
            }
        }
    }
}
