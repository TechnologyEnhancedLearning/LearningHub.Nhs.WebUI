namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using LearningHub.Nhs.Models.Moodle;
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
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using System.Net.Http.Json;
    using static System.Net.WebRequestMethods;
    using IdentityModel.Client;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
    using LearningHub.Nhs.Models.User;

    /// <summary>
    /// MoodleBridgeApiService.
    /// </summary>
    public class MoodleBridgeApiService : IMoodleBridgeApiService
    {
        private readonly IMoodleBridgeHttpClient moodleBridgeHttpClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleApiService"/> class.
        /// </summary>
        /// <param name="moodleBridgeHttpClient">moodleHttpClient.</param>
        /// <param name="logger">logger.</param>
        public MoodleBridgeApiService(IMoodleBridgeHttpClient moodleBridgeHttpClient, ILogger<MoodleApiService> logger)
        {
            this.moodleBridgeHttpClient = moodleBridgeHttpClient;
            this.logger = logger;
        }

        /// <summary>
        /// GetUserInstancesByEmailAsync.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>UserId from Moodle.</returns>
        public async Task<MoodleInstanceUserIdsViewModel> GetUserInstancesByEmail(string email)
        {
            MoodleUserIdsResponseModel viewmodel = null;

            try
            {
                var client = await this.moodleBridgeHttpClient.GetClient();

                var request = $"/api/v1/Users/{Uri.EscapeDataString(email)}/instance-ids";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    viewmodel = System.Text.Json.JsonSerializer.Deserialize<MoodleUserIdsResponseModel>(result, options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                if (viewmodel?.MoodleUserIds != null)
                {
                    return MoodleInstanceUsersHelper.BuildUserIdsByInstance(viewmodel.MoodleUserIds);
                }
                else
                {
                    throw new Exception("Failed to retrieve Moodle user IDs.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching user instances by email.");
                throw;
            }
        }

        /// <summary>
        /// UpdateEmail.
        /// </summary>
        /// <param name="updateEmailaddressViewModel">The UpdateEmailaddressViewModel.</param>
        /// <returns></returns>
        public async Task<MoodleUpdateEmailResponseModel> UpdateEmail(UpdateEmailaddressViewModel updateEmailaddressViewModel)
        {
            try
            {
                var client = await this.moodleBridgeHttpClient.GetClient();

                var requestUrl = "/api/v1/Users/update-email";

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var response = await client.PostAsJsonAsync(requestUrl, updateEmailaddressViewModel)
                                           .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    var viewModel = System.Text.Json.JsonSerializer.Deserialize<MoodleUpdateEmailResponseModel>(result, options);

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
                this.logger.LogError(ex, "An error occurred while updating email.");
                throw;
            }
        }

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="moodleUserInstanceUserIds">Moodle instances user id.</param>
        /// <param name="requestModel">MyLearningRequestModel.</param>
        /// <param name="months">The months.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<MoodleCompletionsApiResponseModel> GetRecentEnrolledCoursesAsync(MoodleInstanceUserIdsViewModel moodleUserInstanceUserIds, MyLearningRequestModel requestModel, int? month = null)
        {
            try
            {
                if (moodleUserInstanceUserIds?.MoodleInstanceUserIds == null ||
               !moodleUserInstanceUserIds.MoodleInstanceUserIds.Any())
                {
                    throw new ArgumentException("UserIds are required.");
                }

                string statusFilter = string.Empty; ;

                if ((requestModel.Incomplete && requestModel.Complete) || (!requestModel.Incomplete && !requestModel.Complete))
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

                var client = await this.moodleBridgeHttpClient.GetClient();

                var queryParams = new List<string>();

                if (!string.IsNullOrWhiteSpace(statusFilter))
                    queryParams.Add($"statusfilter={statusFilter}");

                if (month.HasValue)
                    queryParams.Add($"months={month.Value}");

                if (!string.IsNullOrWhiteSpace(requestModel?.SearchText))
                {
                    queryParams.Add($"search={Uri.EscapeDataString(requestModel.SearchText)}");
                }

                var queryString = queryParams.Any()
                    ? "?" + string.Join("&", queryParams)
                    : string.Empty;


                var requestUri = $"api/v1/users/recent-courses{queryString}";

                var response = await client.PostAsJsonAsync(
                    requestUri,
                    moodleUserInstanceUserIds).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    var result = System.Text.Json.JsonSerializer.Deserialize<MoodleCompletionsApiResponseModel>(json, options);

                    return result ?? new MoodleCompletionsApiResponseModel();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching user's recent learning activities ");
                throw;
            }
        }

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="moodleUserInstanceUserIds">Moodle Instances user id.</param>
        /// <param name="requestModel">MyLearningRequestModel requestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<MoodleCompletionsApiResponseModel> GetEnrolledCoursesHistoryAsync(MoodleInstanceUserIdsViewModel moodleUserInstanceUserIds, MyLearningRequestModel requestModel)
        {
            try
            {
                if (moodleUserInstanceUserIds?.MoodleInstanceUserIds == null ||
                  !moodleUserInstanceUserIds.MoodleInstanceUserIds.Any())
                {
                    throw new ArgumentException("UserIds are required.");
                }

                string statusFilter = string.Empty; ;

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

                var client = await this.moodleBridgeHttpClient.GetClient();
                // Build query string (optional params)
                var queryParams = new List<string>();
                queryParams.Add($"months=0");
                if (!string.IsNullOrWhiteSpace(statusFilter))
                    queryParams.Add($"statusfilter={statusFilter}");

                if (!string.IsNullOrWhiteSpace(requestModel?.SearchText))
                {
                    queryParams.Add($"search={Uri.EscapeDataString(requestModel.SearchText)}");
                }

                var queryString = queryParams.Any()
                    ? "?" + string.Join("&", queryParams)
                    : string.Empty;

                var requestUri = $"api/v1/users/recent-courses{queryString}";

                var response = await client.PostAsJsonAsync(
                   requestUri,
                   moodleUserInstanceUserIds).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    var result = System.Text.Json.JsonSerializer.Deserialize<MoodleCompletionsApiResponseModel>(json, options);

                    return result ?? new MoodleCompletionsApiResponseModel();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetInProgressEnrolledCoursesAsync.
        /// </summary>
        /// <param name="email">user email.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<MoodleCompletionsApiResponseModel> GetInProgressEnrolledCoursesAsync(string email)
        {
            try
            {
                var moodleUserInstanceUserIds = await this.GetUserInstancesByEmail(email);
                if (moodleUserInstanceUserIds?.MoodleInstanceUserIds == null ||
                  !moodleUserInstanceUserIds.MoodleInstanceUserIds.Any())
                {
                    throw new ArgumentException("UserIds are required.");
                }

                string statusFilter = "inprogress";

                var client = await this.moodleBridgeHttpClient.GetClient();
                // Build query string (optional params)
                var queryParams = new List<string>();
                queryParams.Add($"months=0");
                if (statusFilter != null)
                    queryParams.Add($"statusfilter={statusFilter}");

                var queryString = queryParams.Any()
    ? "?" + string.Join("&", queryParams)
    : string.Empty;

                var requestUri = $"api/v1/users/recent-courses{queryString}";

                var response = await client.PostAsJsonAsync(
                    requestUri,
                    moodleUserInstanceUserIds).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    var result = System.Text.Json.JsonSerializer.Deserialize<MoodleCompletionsApiResponseModel>(json, options);

                    return result ?? new MoodleCompletionsApiResponseModel();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetUserLearningHistory.
        /// </summary>
        /// <param name="email">user email.</param>
        /// <param name="filterText">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<MoodleCertificateResponseModel> GetUserCertificateAsync(string email, string filterText = "")
        {
            try
            {
                var moodleUserInstanceUserIds = await this.GetUserInstancesByEmail(email);
                if (moodleUserInstanceUserIds?.MoodleInstanceUserIds == null ||
                  !moodleUserInstanceUserIds.MoodleInstanceUserIds.Any())
                {
                    throw new ArgumentException("UserIds are required.");
                }
                var client = await this.moodleBridgeHttpClient.GetClient();
                // Build query string (optional params)
                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    queryParams.Add($"searchterm={Uri.EscapeDataString(filterText)}");
                }

                var queryString = queryParams.Any()
                    ? "?" + string.Join("&", queryParams)
                    : string.Empty;


                var requestUri = $"api/v1/Users/certificates{queryString}";

                var response = await client.PostAsJsonAsync(
                    requestUri,
                    moodleUserInstanceUserIds).ConfigureAwait(false);


                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    var result = await response.Content
                        .ReadFromJsonAsync<MoodleCertificateResponseModel>(options)
                        .ConfigureAwait(false);

                    return result ?? new MoodleCertificateResponseModel();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetUserCertificatefromMoodleInstancesAsync.
        /// </summary>
        /// <param name="moodleUserInstanceUserIds">moodleUserInstanceUserIds.</param>
        /// <param name="filterText">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<MoodleCertificateResponseModel> GetUserCertificateFromMoodleInstancesAsync(MoodleInstanceUserIdsViewModel moodleUserInstanceUserIds, string filterText = "")
        {
            try
            {
                if (moodleUserInstanceUserIds?.MoodleInstanceUserIds == null ||
                  !moodleUserInstanceUserIds.MoodleInstanceUserIds.Any())
                {
                    throw new ArgumentException("UserIds are required.");
                }
                var client = await this.moodleBridgeHttpClient.GetClient();

                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    queryParams.Add($"searchterm={Uri.EscapeDataString(filterText)}");
                }

                var queryString = queryParams.Any()
                    ? "?" + string.Join("&", queryParams)
                    : string.Empty;


                var requestUri = $"api/v1/Users/certificates{queryString}";

                var response = await client.PostAsJsonAsync(
                    requestUri,
                    moodleUserInstanceUserIds).ConfigureAwait(false);


                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    var result = await response.Content
                        .ReadFromJsonAsync<MoodleCertificateResponseModel>(options)
                        .ConfigureAwait(false);

                    return result ?? new MoodleCertificateResponseModel();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<CategoryResult>> GetAllMoodleCategoriesAsync()
        {
            MoodleInstancesCategoriesResponseModel viewmodel = null;

            try
            {
                var client = await this.moodleBridgeHttpClient.GetClient();

                var request = $"/api/v1/Courses/categories";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    viewmodel = System.Text.Json.JsonSerializer.Deserialize<MoodleInstancesCategoriesResponseModel>(result, options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel.Results;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching user instances by email.");
                throw;
            }
        }

        /// <summary>
        /// GetCoursesByCategoryIdAsync.
        /// </summary>
        /// <param name="selectedcategoryId">The categoryId.</param>
        /// <returns></returns>
        public async Task<MoodleCourseResultsResponseModel> GetCoursesByCategoryIdAsync(string selectedcategoryId)
        {
            MoodleCourseResultsResponseModel viewmodel = null;
            var (instanceName, categoryId) = selectedcategoryId.Split(':') is var p && p.Length == 2 ? (p[0], p[1]) : (null, null);

            try
            {
                var client = await this.moodleBridgeHttpClient.GetClient();

                var request = $"api/v1/Courses/search?value={Uri.EscapeDataString(categoryId.ToString())}" + $"&instance={Uri.EscapeDataString(instanceName)}";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    viewmodel = System.Text.Json.JsonSerializer
                        .Deserialize<MoodleCourseResultsResponseModel>(result, options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                         response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel ?? new MoodleCourseResultsResponseModel();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching Moodle categories.");
                throw;
            }
        }

        /// <summary>
        /// GetSubCategoryByCategoryIdAsync.
        /// </summary>
        /// <param name="selectedcategoryId">The categoryId.</param>
        /// <returns></returns>
        public async Task<List<SubCategoryResult>> GetSubCategoryByCategoryIdAsync(string selectedcategoryId)
        {
            MoodleInstanceSubCategoryResponseModel subcategories = null;
            var (instanceName, categoryId) = selectedcategoryId.Split(':') is var p && p.Length == 2 ? (p[0], p[1]) : (null, null);

            try
            {
                var client = await this.moodleBridgeHttpClient.GetClient();

                var request = $"api/v1/Courses/{categoryId}/subcategories?{instanceName}";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    subcategories = System.Text.Json.JsonSerializer.Deserialize<MoodleInstanceSubCategoryResponseModel>(result, options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return subcategories.Results;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching sub categories by category id.");
                throw;
            }
        }
    }
}
