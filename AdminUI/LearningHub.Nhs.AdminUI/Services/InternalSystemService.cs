// <copyright file="InternalSystemService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Maintenance;
    using Newtonsoft.Json;

    /// <summary>
    /// The InternalSystemService.
    /// </summary>
    public class InternalSystemService : BaseService, IInternalSystemService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSystemService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        public InternalSystemService(ILearningHubHttpClient learningHubHttpClient)
        : base(learningHubHttpClient)
        {
        }

        /// <inheritdoc/>
        public async Task<List<InternalSystemViewModel>> GetAllAsync()
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            List<InternalSystemViewModel> viewmodel = null;
            var request = $"internalsystem/getall";
            var response = await client.GetAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<InternalSystemViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<InternalSystemViewModel> ToggleOfflineStatus(int id)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            InternalSystemViewModel viewmodel = null;
            var request = $"InternalSystem/ToggleOfflineStatus/{id}";
            var response = await client.PutAsync(request, null).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<InternalSystemViewModel>(result);
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
