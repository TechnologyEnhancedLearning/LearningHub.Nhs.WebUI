namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// MoodleApiService.
    /// </summary>
    public class MoodleApiService : IMoodleApiService
    {
        private readonly IOpenApiHttpClient openApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleApiService"/> class.
        /// </summary>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        public MoodleApiService(IOpenApiHttpClient openApiHttpClient)
        {
            this.openApiHttpClient = openApiHttpClient;
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
                // this.Logger.LogError(string.Format("Error occurred in GetSearchResultAsync: {0}", ex.Message));
                return moodleUserId;
            }
        }

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="currentUserId">Moodle user id.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleCourseResponseModel>> GetEnrolledCoursesAsync(int currentUserId, int pageNumber)
        {
            List<MoodleCourseResponseModel> viewmodel = new List<MoodleCourseResponseModel>();

            try
            {
                var client = await this.openApiHttpClient.GetClientAsync();

                var request = $"Moodle/GetEnrolledCourses/{currentUserId}";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<List<MoodleCourseResponseModel>>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel;
            }
            catch (Exception ex)
            {
                // this.Logger.LogError(string.Format("Error occurred in GetSearchResultAsync: {0}", ex.Message));
                return viewmodel;
            }
        }
    }
}
