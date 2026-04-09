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
    /// MoodleBridgeApiService.
    /// </summary>
    public class MoodleBridgeApiService : IMoodleBridgeApiService
    {
        private readonly IOpenApiHttpClient openApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleApiService"/> class.
        /// </summary>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        public MoodleBridgeApiService(IOpenApiHttpClient openApiHttpClient)
        {
            this.openApiHttpClient = openApiHttpClient;
        }

        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<CategoryResult>> GetAllMoodleCategoriesAsync()
        {
            List<CategoryResult> viewmodel = new List<CategoryResult>();

            try
            {
                var client = await this.openApiHttpClient.GetClientAsync();

                var request = $"MoodleBridge/GetAllMoodleCategories";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<List<CategoryResult>>(result);
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
