namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// MoodleApiService.
    /// </summary>
    public class MoodleApiService : IMoodleApiService
    {
        private readonly IMoodleHttpClient moodleHttpClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleApiService"/> class.
        /// </summary>
        /// <param name="moodleHttpClient">moodleHttpClient.</param>
        /// <param name="logger">logger.</param>
        public MoodleApiService(IMoodleHttpClient moodleHttpClient, ILogger<MoodleApiService> logger)
        {
            this.moodleHttpClient = moodleHttpClient;
            this.logger = logger;
        }
        /// <summary>
        /// GetMoodleUserIdByUsernameAsync.
        /// </summary>
        /// <param name="currentUserId">current User Id.</param>
        /// <returns>UserId from Moodle.</returns>
        public async Task<int> GetMoodleUserIdByUsernameAsync(int currentUserId)
        {
            var parameters = new Dictionary<string, string>
            {
                { "criteria[0][key]", "username" },
                { "criteria[0][value]", currentUserId.ToString() }
            };

            var response = await GetCallMoodleApiAsync<MoodleUserResponseModel>("core_user_get_users", parameters);

            var user = response?.Users?.FirstOrDefault(u => u.Username == currentUserId.ToString());
            return user?.Id ?? 0;
        }


        private async Task<T> GetCallMoodleApiAsync<T>(string wsFunction, Dictionary<string, string> parameters)
        {
            var client = await this.moodleHttpClient.GetClient();
            string defaultParameters = this.moodleHttpClient.GetDefaultParameters();

            // Build URL query
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"&wsfunction={wsFunction}");

            foreach (var param in parameters)
            {
                queryBuilder.Append($"&{param.Key}={Uri.EscapeDataString(param.Value)}");
            }

            string fullUrl = "?" + defaultParameters + queryBuilder.ToString();

            HttpResponseMessage response = await client.GetAsync(fullUrl);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                this.logger.LogError($"Moodle API access denied. Status Code: {response.StatusCode}");
                throw new Exception("AccessDenied to MoodleApi");
            }
            else
            {
                this.logger.LogError($"Moodle API error. Status Code: {response.StatusCode}, Message: {result}");
                throw new Exception("Error with MoodleApi");
            }
        }
    }
}
