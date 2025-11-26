namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// MoodleApiService.
    /// </summary>
    public class MoodleApiService : IMoodleApiService
    {
        private readonly IOpenApiHttpClient openApiHttpClient;
        private readonly MoodleApiConfig configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleApiService"/> class.
        /// </summary>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="configuration">configuration.</param>
        public MoodleApiService(IOpenApiHttpClient openApiHttpClient, IOptions<MoodleApiConfig> configuration)
        {
            this.openApiHttpClient = openApiHttpClient;
            this.configuration = configuration.Value;
        }

        /// <summary>
        /// GetMoodleUserIdByUsernameAsync.
        /// </summary>
        /// <param name="currentUserId">current User Id.</param>
        /// <returns>UserId from Moodle.</returns>
        public async Task<int> GetMoodleUserIdByUsernameAsync(int currentUserId)
        {
            int moodleUserId = 0;

            try
            {
                var client = await this.openApiHttpClient.GetClientAsync();

                var request = $"Moodle/GetMoodleUserId/{currentUserId}";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    moodleUserId = JsonConvert.DeserializeObject<int>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return moodleUserId;
            }
            catch (Exception ex)
            {
                return moodleUserId;
            }
        }

        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleCategory>> GetAllMoodleCategoriesAsync()
        {
            List<MoodleCategory> viewmodel = new List<MoodleCategory>();

            try
            {
                var client = await this.openApiHttpClient.GetClientAsync();

                var request = $"Moodle/GetAllMoodleCategories";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<List<MoodleCategory>>(result);
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
