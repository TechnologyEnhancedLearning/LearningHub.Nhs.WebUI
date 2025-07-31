namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Newtonsoft.Json;
    using MoodleCourseCompletionModel = Nhs.Models.Moodle.API.MoodleCourseCompletionModel;

    /// <summary>
    /// MoodleApiService.
    /// </summary>
    public class MoodleApiService : IMoodleApiService
    {
        private readonly IMoodleHttpClient moodleHttpClient;
        private readonly IOpenApiHttpClient openApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleApiService"/> class.
        /// </summary>
        /// <param name="moodleHttpClient">moodleHttpClient.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        public MoodleApiService(IMoodleHttpClient moodleHttpClient, IOpenApiHttpClient openApiHttpClient)
        {
            this.moodleHttpClient = moodleHttpClient;
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
        /// <param name="userId">Moodle user id.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleCourseResponseModel>> GetEnrolledCoursesAsync(int userId, int pageNumber)
        {
            List<MoodleCourseResponseModel> viewmodel = new List<MoodleCourseResponseModel> { };
            MoodleApiService moodleApiService = new MoodleApiService(this.moodleHttpClient, this.openApiHttpClient);

            var client = await this.moodleHttpClient.GetClient();
            string additionalParameters = $"userid={userId}";
            string defaultParameters = this.moodleHttpClient.GetDefaultParameters();
            string url = $"&wsfunction=core_enrol_get_users_courses&{additionalParameters}";

            HttpResponseMessage response = await client.GetAsync("?" + defaultParameters + url);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                using var document = JsonDocument.Parse(result);
                var root = document.RootElement;

                // Check if it's a JSON object and contains "exception"
                if (!(root.ValueKind == JsonValueKind.Object && root.TryGetProperty("exception", out _)))
                {
                    viewmodel = JsonConvert.DeserializeObject<List<MoodleCourseResponseModel>>(result);

                    foreach (var course in viewmodel)
                    {
                        course.CourseCompletionViewModel = await moodleApiService.GetCourseCompletionAsync(userId, course.Id.Value, pageNumber);
                    }
                }
                else
                {
                    // Contains error, handle it as needed.
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="courseId">Moodle course id.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <returns> List of MoodleCourseResponseModel.</returns>
        public async Task<MoodleCourseCompletionModel> GetCourseCompletionAsync(int userId, int courseId, int pageNumber)
        {
            MoodleCourseCompletionModel viewmodel = new MoodleCourseCompletionModel { };
            MoodleApiService moodleApiService = new MoodleApiService(this.moodleHttpClient, this.openApiHttpClient);

            var client = await this.moodleHttpClient.GetClient();
            string additionalParameters = $"userid={userId}&courseid={courseId}";
            string defaultParameters = this.moodleHttpClient.GetDefaultParameters();
            string url = $"&wsfunction=core_completion_get_course_completion_status&{additionalParameters}";

            HttpResponseMessage response = await client.GetAsync("?" + defaultParameters + url);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var canViewReport = JsonConvert.DeserializeObject<MoodleCompletionResponseModel>(result);

                if (string.IsNullOrEmpty(canViewReport.Exception))
                {
                    viewmodel = JsonConvert.DeserializeObject<MoodleCourseCompletionModel>(result);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
