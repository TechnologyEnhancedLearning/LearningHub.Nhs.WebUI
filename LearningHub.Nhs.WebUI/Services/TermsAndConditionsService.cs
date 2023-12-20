// <copyright file="TermsAndConditionsService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="TermsAndConditionsService" />.
    /// </summary>
    public class TermsAndConditionsService : BaseService<TermsAndConditionsService>, ITermsAndConditionsService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsAndConditionsService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="userApiHttpClient">User api http client.</param>
        /// <param name="logger">Logger.</param>
        public TermsAndConditionsService(
            ILearningHubHttpClient learningHubHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<TermsAndConditionsService> logger)
        : base(learningHubHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The AcceptByUser.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> AcceptByUser(int id, int currentUserId)
        {
            UserTermsAndConditionsViewModel userTermsAndConditions = new UserTermsAndConditionsViewModel()
            {
                TermsAndConditionsId = id,
                UserId = currentUserId,
            };

            int createId = 0;
            var json = JsonConvert.SerializeObject(userTermsAndConditions);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"TermsAndConditions/AcceptByUser";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                createId = apiResponse.ValidationResult.CreatedId ?? 0;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return createId;
        }

        /// <summary>
        /// The LatestVersionAsync.
        /// </summary>
        /// <param name="tenantId">The tenantId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{TermsAndConditions}"/>.</returns>
        public async Task<TermsAndConditions> LatestVersionAsync(int tenantId)
        {
            TermsAndConditions viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"TermsAndConditions/LatestVersion/{tenantId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<TermsAndConditions>(result);
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
