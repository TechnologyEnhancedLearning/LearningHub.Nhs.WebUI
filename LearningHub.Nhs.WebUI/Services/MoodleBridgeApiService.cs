namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// MoodleApiService.
    /// </summary>
    public class MoodleBridgeApiService : IMoodleBridgeApiService
    {
        private const string MoodleBridgeConfigurationRoute = "MoodleBridge/Configuration";

        private readonly IOpenApiHttpClient openApiHttpClient;
        private readonly IMoodleApiService moodleApiService;
        private readonly ILogger<MoodleBridgeApiService> logger;
        private readonly SemaphoreSlim configurationLock = new SemaphoreSlim(1, 1);
        private IDictionary<string, string> moodleInstanceBaseUrls;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleBridgeApiService"/> class.
        /// </summary>
        /// <param name="openApiHttpClient">The open Api Http Client.</param>
        /// <param name="moodleApiService">The Moodle API service.</param>
        /// <param name="logger">The logger.</param>
        public MoodleBridgeApiService(
            IOpenApiHttpClient openApiHttpClient,
            IMoodleApiService moodleApiService,
            ILogger<MoodleBridgeApiService> logger)
        {
            this.openApiHttpClient = openApiHttpClient;
            this.moodleApiService = moodleApiService;
            this.logger = logger;
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

        /// <summary>
        /// UpdateEmail.
        /// </summary>
        /// <param name="updateEmailaddressViewModel">The updateEmailaddressViewModel.</param>
        /// <returns>email update status.</returns>
        public async Task<MoodleUpdateEmailResponseModel> UpdateEmail(UpdateEmailaddressViewModel updateEmailaddressViewModel)
        {
            try
            {
                var client = await this.openApiHttpClient.GetClientAsync();

                var requestUrl = "MoodleBridge/UpdateEmail";

                var response = await client.PostAsJsonAsync(requestUrl, updateEmailaddressViewModel)
                                           .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var viewModel = await response.Content
                                                  .ReadFromJsonAsync<MoodleUpdateEmailResponseModel>();

                    return viewModel;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                         response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode}, Details: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update user email on moodle instances");
            }
        }

        /// <summary>
        /// Gets the configured Moodle instance base URLs keyed by instance short name.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IDictionary<string, string>> GetMoodleInstanceBaseUrlsAsync()
        {
            await this.EnsureMoodleInstanceBaseUrlsLoadedAsync().ConfigureAwait(false);
            return this.moodleInstanceBaseUrls;
        }

        /// <summary>
        /// Gets a Moodle course URL for the supplied Moodle instance source.
        /// </summary>
        /// <param name="source">The Moodle instance source identifier.</param>
        /// <param name="courseId">The Moodle course id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<string> GetCourseUrlAsync(string source, int courseId)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                this.logger.LogWarning("Unable to determine Moodle BaseUrl because the search result source was empty for course {CourseId}.", courseId);
                return string.Empty;
            }

            var moodleInstanceBaseUrls = await this.GetMoodleInstanceBaseUrlsAsync().ConfigureAwait(false);

            if (!moodleInstanceBaseUrls.TryGetValue(source, out var baseUrl) || string.IsNullOrWhiteSpace(baseUrl))
            {
                this.logger.LogWarning("Unable to determine Moodle BaseUrl for search result source {Source}.", source);
                return string.Empty;
            }

            return this.moodleApiService.GetCourseUrl(courseId, baseUrl);
        }

        private async Task EnsureMoodleInstanceBaseUrlsLoadedAsync()
        {
            if (this.moodleInstanceBaseUrls != null)
            {
                return;
            }

            await this.configurationLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.moodleInstanceBaseUrls != null)
                {
                    return;
                }

                var client = await this.openApiHttpClient.GetClientAsync().ConfigureAwait(false);
                var response = await client.GetAsync(MoodleBridgeConfigurationRoute).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    this.logger.LogError("Failed to retrieve Moodle configuration from OpenAPI. Status code: {StatusCode}", response.StatusCode);
                    this.moodleInstanceBaseUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    return;
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var configuration = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(
                    result,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                this.moodleInstanceBaseUrls = configuration?
                    .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase)
                    ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to retrieve Moodle configuration from OpenAPI.");
                this.moodleInstanceBaseUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            finally
            {
                this.configurationLock.Release();
            }
        }
    }
}
