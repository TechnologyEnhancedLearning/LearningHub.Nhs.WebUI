namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Report.ReportCreate;
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
    using System.Text.Json;
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
            currentUserId = 2299585;
            var parameters = new Dictionary<string, string>
            {
                { "criteria[0][key]", "username" },
                { "criteria[0][value]", currentUserId.ToString() }
            };

            var response = await GetCallMoodleApiAsync<MoodleUserResponseModel>("core_user_get_users", parameters);

            var user = response?.Users?.FirstOrDefault(u => u.Username == currentUserId.ToString());
            return user?.Id ?? 0;
        }


        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleCourseResponseModel>> GetEnrolledCoursesAsync(int userId, int pageNumber)
        {
            var parameters = new Dictionary<string, string>
            {
                { "userid", userId.ToString() }
            };

            // Fetch enrolled courses
            var enrolledCourses = await GetCallMoodleApiAsync<List<MoodleCourseResponseModel>>(
                "core_enrol_get_users_courses",
                parameters
            );

            if (enrolledCourses == null || enrolledCourses.Count == 0)
                return new List<MoodleCourseResponseModel>();

            // Load course completion info in parallel
            var completionTasks = enrolledCourses
                .Where(c => c.Id.HasValue)
                .Select(async course =>
                {
                    try
                    {
                        course.CourseCompletionViewModel = await GetCourseCompletionAsync(userId, course.Id.Value, pageNumber);
                    }
                    catch (Exception ex)
                    {
                        course.CourseCompletionViewModel = new MoodleCourseCompletionModel
                        {
                            CompletionStatus = null,
                        };
                    }

                    return course;
                });

            var enrichedCourses = await Task.WhenAll(completionTasks);

            return enrichedCourses.ToList();

        }


        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="requestModel">MyLearningRequestModel.</param>
        /// <param name="months">The months.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleEnrolledCourseResponseModel>> GetRecentEnrolledCoursesAsync(int userId, MyLearningRequestModel requestModel, int? months = null)
        {
            try
            {
                int moodleUserId = await this.GetMoodleUserIdByUsernameAsync(userId);
                string statusFilter = string.Empty; ;

                if ((requestModel.Incomplete && requestModel.Complete)  || (!requestModel.Incomplete && !requestModel.Complete))
                {
                    statusFilter = string.Empty; ;
                }
                else if (requestModel.Incomplete)
                {
                    statusFilter = "inprogress";
                }
                else
                {
                    statusFilter = "completed";
                }

                var parameters = new Dictionary<string, string>
{
    { "userid", moodleUserId.ToString() },
    { "months", months.ToString() },
    { "statusfilter", statusFilter },
    { "search", requestModel.SearchText ?? string.Empty }
};

                // Fetch enrolled courses
                var recentEnrolledCourses = await GetCallMoodleApiAsync<List<MoodleEnrolledCourseResponseModel>>(
                    "mylearningservice_get_recent_courses",
                    parameters
                );

                if (recentEnrolledCourses == null || recentEnrolledCourses.Count == 0)
                    return new List<MoodleEnrolledCourseResponseModel>();

                return recentEnrolledCourses.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="requestModel">MyLearningRequestModel requestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleEnrolledCourseResponseModel>> GetEnrolledCoursesHistoryAsync(int userId, MyLearningRequestModel requestModel)
        {
            try
            {
                int moodleUserId = await this.GetMoodleUserIdByUsernameAsync(userId);
                string statusFilter = string.Empty;

                if ((requestModel.Incomplete && requestModel.Complete) || (!requestModel.Incomplete && !requestModel.Complete))
                {
                    statusFilter = string.Empty;
                }
                else if (requestModel.Incomplete)
                {
                    statusFilter = "inprogress";
                }
                else
                {
                    statusFilter = "completed";
                }

                var parameters = new Dictionary<string, string>
{
    { "userid", moodleUserId.ToString() },
    { "statusfilter", statusFilter },
    { "search", requestModel.SearchText ?? string.Empty }
};

                // Fetch enrolled courses
                var recentEnrolledCourses = await GetCallMoodleApiAsync<List<MoodleEnrolledCourseResponseModel>>(
                    "mylearningservice_get_recent_courses",
                    parameters
                );

                if (recentEnrolledCourses == null || recentEnrolledCourses.Count == 0)
                    return new List<MoodleEnrolledCourseResponseModel>();

                return recentEnrolledCourses.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetInProgressEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleEnrolledCourseResponseModel>> GetInProgressEnrolledCoursesAsync(int userId)
        {
            try
            {
                int moodleUserId = await this.GetMoodleUserIdByUsernameAsync(userId);
                string statusFilter = "inprogress";            

                var parameters = new Dictionary<string, string>
{
    { "userid", moodleUserId.ToString() },
    { "statusfilter", statusFilter },
    { "search", string.Empty }
};

                // Fetch enrolled courses
                var recentEnrolledCourses = await GetCallMoodleApiAsync<List<MoodleEnrolledCourseResponseModel>>(
                    "mylearningservice_get_recent_courses",
                    parameters
                );

                if (recentEnrolledCourses == null || recentEnrolledCourses.Count == 0)
                    return new List<MoodleEnrolledCourseResponseModel>();

                return recentEnrolledCourses.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetUserLearningHistory.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="filterText">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleUserCertificateResponseModel>> GetUserCertificateAsync(int userId, string filterText="")
        {
            try
            {
                int moodleUserId = await this.GetMoodleUserIdByUsernameAsync(userId);
                var parameters = new Dictionary<string, string>
            {
                { "userid", moodleUserId.ToString() },
                { "searchterm", filterText }
            };

                // Fetch enrolled courses
                var userCertificates = await GetCallMoodleApiAsync<List<MoodleUserCertificateResponseModel>>(
                    "mylearningservice_get_user_certificates",
                    parameters
                );

                if (userCertificates == null || userCertificates.Count == 0)
                    return new List<MoodleUserCertificateResponseModel>();

                return userCertificates.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
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
            var parameters = new Dictionary<string, string>
            {
                { "userid", userId.ToString() },
                { "courseid", courseId.ToString() }
            };

            // Call Moodle API and parse response
            var result = await GetCallMoodleApiAsync<MoodleCourseCompletionModel>("core_completion_get_course_completion_status", parameters);

            // If Moodle did not return an exception, return parsed completion data
            if (result.Warnings.Count == 0)
            {
                // Optionally map/convert if needed
                return JsonConvert.DeserializeObject<MoodleCourseCompletionModel>(JsonConvert.SerializeObject(result));
            }

            return new MoodleCourseCompletionModel(); // Return empty model or null as fallback
        }

        private async Task<T> GetCallMoodleApiAsync<T>(string wsFunction, Dictionary<string, string> parameters)
        {
            var client = await this.moodleHttpClient.GetClient();
            string defaultParameters = this.moodleHttpClient.GetDefaultParameters();

            // Build URL query string
            var queryBuilder = new StringBuilder($"&wsfunction={wsFunction}");
            foreach (var param in parameters)
            {
                queryBuilder.Append($"&{param.Key}={Uri.EscapeDataString(param.Value)}");
            }

            string fullUrl = "?" + defaultParameters + queryBuilder.ToString();

            HttpResponseMessage response = await client.GetAsync(fullUrl);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Moodle may still return an error with 200 OK
                try
                {
                    using var document = JsonDocument.Parse(result);
                    var root = document.RootElement;

                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("exception", out var exceptionProp))
                    {
                        string? message = root.TryGetProperty("message", out var messageProp)
                            ? messageProp.GetString()
                            : "Unknown error";

                        this.logger.LogError($"Moodle returned an exception: {exceptionProp.GetString()}, Message: {message}");
                        throw new Exception($"Moodle API Error: {exceptionProp.GetString()}, Message: {message}");
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    this.logger.LogError(ex, "Failed to parse Moodle API response as JSON.");
                    throw;
                }

                var deserialized = JsonConvert.DeserializeObject<T>(result);

                return deserialized == null
                    ? throw new Exception($"Failed to deserialize Moodle API response into type {typeof(T).Name}. Raw response: {result}")
                    : deserialized;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                this.logger.LogError($"Moodle API access denied. Status Code: {response.StatusCode}");
                throw new Exception("AccessDenied to MoodleApi");
            }
            else
            {
                this.logger.LogError($"Moodle API error. Status Code: {response.StatusCode}, Response: {result}");
                throw new Exception("Error with MoodleApi");
            }
        }

    }
}
