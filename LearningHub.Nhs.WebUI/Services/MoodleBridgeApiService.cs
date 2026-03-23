namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// MoodleApiService.
    /// </summary>
    public class MoodleBridgeApiService : IMoodleBridgeApiService
    {
        private readonly IOpenApiHttpClient openApiHttpClient;
       //// private readonly MoodleBridgeApiConfig configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleBridgeApiService"/> class.
        /// </summary>
        /// <param name="openApiHttpClient">The open Api Http Client.</param>
        public MoodleBridgeApiService(IOpenApiHttpClient openApiHttpClient)
        {
            this.openApiHttpClient = openApiHttpClient;
            ////this.configuration = configuration.Value;
        }

        /// <summary>
        /// GetUserInstancesByEmailAsync.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>UserId from Moodle.</returns>
        public async Task<MoodleInstanceUserIdsViewModel> GetUserInstancesByEmail(string email)
        {
            MoodleInstanceUserIdsViewModel viewmodel = null;

            try
            {
                var client = await this.openApiHttpClient.GetClientAsync();

                var request = $"MoodleBridge/GetUserInstancesByEmail/{email}";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<MoodleInstanceUserIdsViewModel>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel;
            }
            catch (Exception ex)
            {
                return viewmodel;
            }
        }
    }
}
