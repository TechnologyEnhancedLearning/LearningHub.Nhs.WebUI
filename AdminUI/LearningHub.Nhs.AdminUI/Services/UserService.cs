namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="UserService" />.
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        private readonly IUserApiHttpClient userApiHttpClient;
        private ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="cacheService">The CacheService <see cref="ICacheService"/>.</param>
        /// <param name="userApiHttpClient">The userApiHttpClient <see cref="IUserApiHttpClient"/>.</param>
        public UserService(ILearningHubHttpClient learningHubHttpClient, ICacheService cacheService, IUserApiHttpClient userApiHttpClient)
        : base(learningHubHttpClient)
        {
            this.cacheService = cacheService;
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetCurrentUser.
        /// </summary>
        /// <returns>The <see cref="Task{UserViewModel}"/>.</returns>
        public async Task<UserViewModel> GetCurrentUser()
        {
            UserViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetCurrentUser";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<UserViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetEmailAddressRegistrationStatusAsync.
        /// </summary>
        /// <param name="emailAddress">The emailAddress<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{EmailRegistrationStatus}"/>.</returns>
        public async Task<EmailRegistrationStatus> GetEmailAddressRegistrationStatusAsync(string emailAddress)
        {
            EmailRegistrationStatus returnedStatus = EmailRegistrationStatus.Unknown;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetRegistrationStatus/{emailAddress}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                returnedStatus = JsonConvert.DeserializeObject<EmailRegistrationStatus>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return returnedStatus;
        }

        /// <summary>
        /// The GetUserAdminBasicPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserAdminBasicViewModel}"/>.</returns>
        public async Task<PagedResultSet<elfhHub.Nhs.Models.Common.UserAdminBasicViewModel>> GetUserAdminBasicPageAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<elfhHub.Nhs.Models.Common.UserAdminBasicViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetUserAdminBasicFilteredPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{HttpUtility.UrlEncode(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<elfhHub.Nhs.Models.Common.UserAdminBasicViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetLHUserAdminBasicPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserAdminBasicViewModel}"/>.</returns>
        public async Task<PagedResultSet<LearningHub.Nhs.Models.User.UserAdminBasicViewModel>> GetLHUserAdminBasicPageAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<LearningHub.Nhs.Models.User.UserAdminBasicViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);
            var presetFilter = JsonConvert.SerializeObject(pagingRequestModel.PresetFilter);

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/GetLHUserAdminBasicFilteredPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{HttpUtility.UrlEncode(presetFilter)}"
                + $"/{HttpUtility.UrlEncode(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<LearningHub.Nhs.Models.User.UserAdminBasicViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserAdminDetailbyIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserAdminDetailViewModel}"/>.</returns>
        public async Task<UserAdminDetailViewModel> GetUserAdminDetailbyIdAsync(int id)
        {
            UserAdminDetailViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetUserAdminDetailById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<UserAdminDetailViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserByUserId.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserViewModel}"/>.</returns>
        public async Task<UserViewModel> GetUserByUserId(int id)
        {
            UserViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetByUserId/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<UserViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserContributionsAsync.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="PagedResultSet{ResourceAdminSearchResultViewModel}"/>.</returns>
        public async Task<PagedResultSet<ResourceAdminSearchResultViewModel>> GetUserContributionsAsync(int userId)
        {
            PagedResultSet<ResourceAdminSearchResultViewModel> viewmodel = null;

            PagingRequestModel pagingRequestModel = new PagingRequestModel()
            {
                Page = 1,
                PageSize = 9999,
                SortColumn = "Id",
                SortDirection = "D",
                Filter = new List<PagingColumnFilter>
                {
                    new PagingColumnFilter() { Column = "userid", Value = userId.ToString() },
                },
            };
            var modelString = JsonConvert.SerializeObject(pagingRequestModel);
            var content = new StringContent(modelString, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceAdminSearchFilteredPage";

            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<ResourceAdminSearchResultViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The SendAdminPasswordResetEmail.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> SendAdminPasswordResetEmail(int userId)
        {
            var stringContent = new StringContent(userId.ToString(), UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/SendAdminPasswordResetEmail";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("SendAdminPasswordResetEmail failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The ClearUserCachedPermissions.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> ClearUserCachedPermissions(int userId)
        {
            await this.cacheService.RemoveAsync($"{userId}:AllRolesWithPermissions");
            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The SendEmailToUserAsync.
        /// </summary>
        /// <param name="emailAddress">The emailAddress<see cref="string"/>.</param>
        /// <param name="subject">The subject<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="string"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task SendEmailToUserAsync(string emailAddress, string subject, string body, int userId)
        {
            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/SendEmailToUser";
            var postBody = new UserContactViewModel
            {
                AdminId = 0,
                Body = body,
                EmailAddress = emailAddress,
                Subject = subject,
                UserId = userId,
            };

            var response = await client.PostAsync(request, new StringContent(JsonConvert.SerializeObject(postBody), Encoding.UTF8, "application/json")).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The StoreUserHistory.
        /// </summary>
        /// <param name="userHistory">The userHistory<see cref="UserHistoryViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task StoreUserHistory(UserHistoryViewModel userHistory)
        {
            var json = JsonConvert.SerializeObject(userHistory);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = "UserHistory";

            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Failed to store UserHistory: " + JsonConvert.SerializeObject(userHistory));
                }
            }
        }

        /// <summary>
        /// The UpdateUser.
        /// </summary>
        /// <param name="user">The user<see cref="UserAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateUser(UserAdminDetailViewModel user)
        {
            var json = JsonConvert.SerializeObject(user);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/UpdateUser";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The AddUserGroupsToUser.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="userGroupIdList">The userGroupIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<LearningHubValidationResult> AddUserGroupsToUser(int userId, string userGroupIdList)
        {
            var userUserGroups = new List<UserUserGroupViewModel>();
            foreach (var userGroupId in userGroupIdList.Split(","))
            {
                userUserGroups.Add(new UserUserGroupViewModel() { UserId = userId, UserGroupId = int.Parse(userGroupId) });
            }

            var json = JsonConvert.SerializeObject(userUserGroups);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/AddUserUserGroups";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The GetUserHistoryPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserHistoryViewModel}"/>.</returns>
        public async Task<PagedResultSet<UserHistoryViewModel>> GetUserHistoryPageAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<UserHistoryViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);
            var presetFilter = JsonConvert.SerializeObject(pagingRequestModel.PresetFilter);

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"UserHistory/GetUserHistoryPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{Uri.EscapeUriString(presetFilter)}"
                + $"/{Uri.EscapeUriString(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<UserHistoryViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserLearningRecordsAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="Task{PagedResultSet{UserLearningRecordViewModel}}"/>.</returns>
        public async Task<PagedResultSet<MyLearningDetailedItemViewModel>> GetUserLearningRecordsAsync(PagingRequestModel pagingRequestModel)
        {
            try
            {
                PagedResultSet<MyLearningDetailedItemViewModel> viewmodel = null;

                if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
                {
                    pagingRequestModel.SortColumn = " ";
                }

                if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
                {
                    pagingRequestModel.SortDirection = " ";
                }

                var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);
                var presetFilter = JsonConvert.SerializeObject(pagingRequestModel.PresetFilter);

                var client = await this.LearningHubHttpClient.GetClientAsync();

                var request = $"UserLearningRecord/GetUserLearningRecords"
                    + $"/{pagingRequestModel.Page}"
                    + $"/{pagingRequestModel.PageSize}"
                    + $"/{pagingRequestModel.SortColumn}"
                    + $"/{pagingRequestModel.SortDirection}"
                    + $"/{Uri.EscapeUriString(presetFilter)}"
                    + $"/{Uri.EscapeUriString(filter)}";

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<PagedResultSet<MyLearningDetailedItemViewModel>>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                            ||
                         response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
