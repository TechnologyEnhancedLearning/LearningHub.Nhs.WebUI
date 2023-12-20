// <copyright file="ResourceSyncService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Resource;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ResourceSyncService" />.
    /// </summary>
    public class ResourceSyncService : BaseService, IResourceSyncService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSyncService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        public ResourceSyncService(ILearningHubHttpClient learningHubHttpClient)
        : base(learningHubHttpClient)
        {
        }

        /// <summary>
        /// The AddToSyncListAsync.
        /// </summary>
        /// <param name="resourceIds">The resourceIds<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task AddToSyncListAsync(List<int> resourceIds)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"ResourceSync/SyncList";
            var content = new StringContent(JsonConvert.SerializeObject(resourceIds), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The GetResourceSyncs.
        /// </summary>
        /// <returns>The <see cref="List{ResourceAdminSearchResultViewModel}"/>.</returns>
        public async Task<List<ResourceAdminSearchResultViewModel>> GetResourceSyncs()
        {
            List<ResourceAdminSearchResultViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"ResourceSync";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<ResourceAdminSearchResultViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The RemoveFromSyncListAsync.
        /// </summary>
        /// <param name="resourceIds">The resourceIds<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RemoveFromSyncListAsync(List<int> resourceIds)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"ResourceSync/SyncListRemove";
            var content = new StringContent(JsonConvert.SerializeObject(resourceIds), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The SyncSingle.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ApiResponse}"/>.</returns>
        public async Task<ApiResponse> SyncSingle(int resourceId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"ResourceSync/SyncSingle";
            var content = new StringContent(JsonConvert.SerializeObject(new { resourceId }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                return apiResponse;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Access Denied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }
        }

        /// <summary>
        /// The SyncWithFindwise.
        /// </summary>
        /// <returns>The <see cref="Task{ApiResponse}"/>.</returns>
        public async Task<ApiResponse> SyncWithFindwise()
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"ResourceSync/Sync";
            var content = new StringContent(string.Empty, Encoding.UTF8, "text/plain");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                return apiResponse;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Access Denied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }
        }
    }
}
