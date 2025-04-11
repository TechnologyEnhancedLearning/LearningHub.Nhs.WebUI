namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Reporting;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// MoodleApiService.
    /// </summary>
    public class MoodleApiService : IMoodleApiService
    {
        private readonly IMoodleHttpClient moodleHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleApiService"/> class.
        /// </summary>
        /// <param name="moodleHttpClient">moodleHttpClient.</param>
        public MoodleApiService(IMoodleHttpClient moodleHttpClient)
        {
            this.moodleHttpClient = moodleHttpClient;
        }

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleCourseResponseViewModel>> GetEnrolledCoursesAsync(int userId, int pageNumber)
        {
            List<MoodleCourseResponseViewModel> viewmodel = new List<MoodleCourseResponseViewModel> { };
            MoodleApiService moodleApiService = new MoodleApiService(this.moodleHttpClient);

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
                    viewmodel = JsonConvert.DeserializeObject<List<MoodleCourseResponseViewModel>>(result);

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
        /// <returns> List of MoodleCourseResponseViewModel.</returns>
        public async Task<MoodleCourseCompletionViewModel> GetCourseCompletionAsync(int userId, int courseId, int pageNumber)
        {
            MoodleCourseCompletionViewModel viewmodel = new MoodleCourseCompletionViewModel { };
            MoodleApiService moodleApiService = new MoodleApiService(this.moodleHttpClient);

            var client = await this.moodleHttpClient.GetClient();
            string additionalParameters = $"userid={userId}&courseid={courseId}";
            string defaultParameters = this.moodleHttpClient.GetDefaultParameters();
            string url = $"&wsfunction=core_completion_get_course_completion_status&{additionalParameters}";

            HttpResponseMessage response = await client.GetAsync("?" + defaultParameters + url);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var canViewReport = JsonConvert.DeserializeObject<MoodleCompletionResponseViewModel>(result);

                if (string.IsNullOrEmpty(canViewReport.Exception))
                {
                    viewmodel = JsonConvert.DeserializeObject<MoodleCourseCompletionViewModel>(result);
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
