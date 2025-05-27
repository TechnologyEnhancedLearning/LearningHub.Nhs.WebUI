namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.OpenAthens;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using LearningHub.Nhs.WebUI.Models.UserProfile;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Management.Media.Models;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using NuGet.Common;
    using static Pipelines.Sockets.Unofficial.Threading.MutexSlim;
    using Login = elfhHub.Nhs.Models.Common.Login;

    /// <summary>
    /// The user service.
    /// </summary>
    public class UserService : BaseService<UserService>, IUserService
    {
        private readonly IUserApiHttpClient userApiHttpClient;
        private readonly Settings settings;
        private readonly ICountryService countryService;
        private readonly IRegionService regionService;
        private readonly IJobRoleService jobRoleService;
        private readonly IGradeService gradeService;
        private readonly ISpecialtyService specialtyService;
        private readonly ILocationService locationService;
        private readonly ILoginWizardService loginWizardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="userApiHttpClient">The userApiHttpClient<see cref="IUserApiHttpClient"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{UserService}"/>.</param>
        /// <param name="settings">The settings<see cref="IOptions{Settings}"/>.</param>
        /// <param name="countryService">The country service<see cref="countryService"/>.</param>
        /// <param name="regionService">The region service<see cref="regionService"/>.</param>
        /// <param name="jobRoleService">The jobRoleService<see cref="IJobRoleService"/>.</param>
        /// <param name="gradeService">The gradeService.</param>
        /// <param name="specialtyService">The specialtyService.</param>
        /// <param name="locationService">The locationService.</param>
        /// <param name="loginWizardService">The login wizard service service<see cref="loginWizardService"/>.</param>
        public UserService(
            ILearningHubHttpClient learningHubHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<UserService> logger,
            IOptions<Settings> settings,
            ICountryService countryService,
            IRegionService regionService,
            IJobRoleService jobRoleService,
            ISpecialtyService specialtyService,
            ILocationService locationService,
            IGradeService gradeService,
            ILoginWizardService loginWizardService)
            : base(learningHubHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
            this.settings = settings.Value;
            this.countryService = countryService;
            this.regionService = regionService;
            this.loginWizardService = loginWizardService;
            this.jobRoleService = jobRoleService;
            this.gradeService = gradeService;
            this.specialtyService = specialtyService;
            this.locationService = locationService;
        }

        /// <inheritdoc/>
        public async Task<List<ActiveContentViewModel>> GetActiveContentAsync()
        {
            List<ActiveContentViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = "User/GetActiveContent";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<List<ActiveContentViewModel>>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<LoginResultInternal> CheckUserCredentialsAsync(Login login)
        {
            var json = JsonConvert.SerializeObject(login);
            var stringJson = new StringContent(json, Encoding.UTF8, "application/json");
            var request = "authentication/checkcredentials";
            var client = await this.userApiHttpClient.GetClientAsync();
            var response = await client.PostAsync(request, stringJson).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden))
            {
                this.Logger.LogError($"Credential check for [{login.Username}] using LHAPI resulted in status code [{response.StatusCode}]");
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError($"Credential check for [{login.Username}] using LHAPI resulted in status code [{response.StatusCode}]");
                throw new Exception("LHAPICallFailure");
            }

            var result = await response.Content.ReadAsStringAsync();
            var rtnResult = JsonConvert.DeserializeObject<LoginResultInternal>(result);
            if (rtnResult.UserId > 0)
            {
                this.Logger.LogInformation(
                    "Successful credential check using LHAPI for [{username}] [{lhuserid}]",
                    login.Username,
                    rtnResult.UserId);
            }
            else
            {
                this.Logger.LogInformation(
                    $"Successful credential check using LHAPI for [{login.Username}]. UserId returned 0 so user details maybe incorrect");
            }

            return rtnResult;
        }

        /// <inheritdoc/>
        public async Task<int> CreateElfhAccountWithLinkedOpenAthensAsync(CreateOpenAthensLinkToLhUser newUserDetails)
        {
            if (string.IsNullOrWhiteSpace(newUserDetails.FirstName) || string.IsNullOrWhiteSpace(newUserDetails.LastName) || string.IsNullOrWhiteSpace(newUserDetails.EmailAddress) ||
                string.IsNullOrWhiteSpace(newUserDetails.OaUserId) || string.IsNullOrWhiteSpace(newUserDetails.OaOrganisationId))
            {
                throw new Exception("Some parameters are missing or empty.");
            }

            var json = JsonConvert.SerializeObject(newUserDetails);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = await this.userApiHttpClient.GetClientAsync();
            var request = "ElfhUser/CreateElfhAccountWithLinkedOpenAthens";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Failed to create linked Oen Athens user account: " + JsonConvert.SerializeObject(newUserDetails));
                }

                if (apiResponse.ValidationResult.CreatedId != null)
                {
                    return (int)apiResponse.ValidationResult.CreatedId;
                }

                throw new InvalidOperationException("apiResponse.ValidationResult.CreatedId was null.");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized
                ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return 0;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateOpenAthensLinkToUserAsync(int lhUserId, string openAthensUserId, string openAthensOrgId)
        {
            if (lhUserId == 0 || string.IsNullOrWhiteSpace(openAthensUserId) || string.IsNullOrWhiteSpace(openAthensOrgId))
            {
                throw new Exception("Some parameters are missing or empty.");
            }

            var json = JsonConvert.SerializeObject(new OpenAthensToElfhUserLinkDetails()
            {
                UserId = lhUserId,
                OaUserId = openAthensUserId,
                OaOrgId = openAthensOrgId,
            });
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = await this.userApiHttpClient.GetClientAsync();
            var request = "ElfhUser/CreateOpenAthensLinkToUser";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> DoesEmailAlreadyExist(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new Exception("No Email given to check.");
            }

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/DoesEmailExistForUser/{emailAddress}";
            var response = await client.GetAsync(request).ConfigureAwait(false);
            var doesEmailExist = false;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                doesEmailExist = bool.Parse(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return doesEmailExist;
        }

        /// <inheritdoc/>
        public async Task ForgotPasswordAsync(string email)
        {
            var json = JsonConvert.SerializeObject(
                new elfhHub.Nhs.Models.Common.ForgotPasswordViewModel { EmailAddress = email });
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/ForgotPassword";

            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to send forgot password email");
            }
        }

        /// <inheritdoc/>
        public async Task<UserViewModel> GetCurrentUserAsync()
        {
            UserViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = "ElfhUser/GetCurrentUser";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<UserViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<UserBasicViewModel> GetCurrentUserBasicDetailsAsync()
        {
            UserBasicViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = "ElfhUser/GetCurrentUserBasicDetails";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<UserBasicViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<UserProfile> GetCurrentUserProfileAsync()
        {
            UserProfile viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = "User/GetCurrentUserProfile";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<UserProfile>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<UserProfile> GetUserProfileAsync(int userId)
        {
            UserProfile viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/GetUserProfile/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<UserProfile>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> CreateUserProfileAsync(UserProfile userProfile)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(userProfile);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/CreateUserProfile";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                return apiResponse.ValidationResult;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception("save failed!");
            }
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> UpdateUserProfileAsync(UserProfile userProfile)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(userProfile);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/UpdateUserProfile";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                return apiResponse.ValidationResult;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception("save failed!");
            }
        }

        /// <inheritdoc/>
        public async Task<PersonalDetailsViewModel> GetCurrentUserPersonalDetailsAsync()
        {
            PersonalDetailsViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = "ElfhUser/GetPersonalDetails";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<PersonalDetailsViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<EmailRegistrationStatus> GetEmailAddressRegistrationStatusAsync(string emailAddress, string ipAddress)
        {
            EmailRegistrationStatus returnedStatus = EmailRegistrationStatus.Unknown;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetRegistrationStatus/{emailAddress}/{ipAddress}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                returnedStatus = JsonConvert.DeserializeObject<EmailRegistrationStatus>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return returnedStatus;
        }

        /// <inheritdoc/>
        public async Task<bool> ValidateUserRoleUpgradeAsync(string currentPrimaryEmail, string newPrimaryEmail)
        {
            bool returnedStatus = false;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/ValidateUserRoleUpgrade/{currentPrimaryEmail}?newPrimaryEmail={newPrimaryEmail}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                returnedStatus = JsonConvert.DeserializeObject<bool>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return returnedStatus;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckSamePrimaryemailIsPendingToValidate(string secondaryEmail)
        {
            bool returnedStatus = false;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/CheckSamePrimaryemailIsPendingToValidate/{secondaryEmail}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                returnedStatus = JsonConvert.DeserializeObject<bool>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return returnedStatus;
        }

        /// <inheritdoc/>
        public async Task<bool> GetEmailAddressStatusAsync(string primaryEmailAddress)
        {
            bool returnedStatus = false;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetEmailStatus/{primaryEmailAddress}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                returnedStatus = JsonConvert.DeserializeObject<bool>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return returnedStatus;
        }

        /// <inheritdoc/>
        public async Task<TenantDescription> GetTenantDescriptionByUserId(int userId)
        {
            TenantDescription model = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetTenantDescriptionByUserId/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                model = JsonConvert.DeserializeObject<TenantDescription>(result);
            }

            return model;
        }

        /// <inheritdoc/>
        public async Task<UserViewModel> GetUserByUserIdAsync(int id)
        {
            UserViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetByUserId/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<UserViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<UserLHBasicViewModel> GetLHUserByUserIdAsync(int id)
        {
            UserLHBasicViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/GetByUserId/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<UserLHBasicViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<UserEmployment> GetUserEmploymentByIdAsync(int id)
        {
            UserEmployment model = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"UserEmployment/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                model = JsonConvert.DeserializeObject<UserEmployment>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return model;
        }

        /// <inheritdoc/>
        public async Task<UserEmploymentViewModel> GetPrimaryUserEmploymentForUser(int userId)
        {
            UserEmploymentViewModel model = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"UserEmployment/GetPrimaryForUser/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                model = JsonConvert.DeserializeObject<UserEmploymentViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return model;
        }

        /// <inheritdoc/>
        public async Task<string> GetUserRoleName(int userId)
        {
            string userRoleName = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetUserRoleName/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                userRoleName = JsonConvert.DeserializeObject<string>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return userRoleName;
        }

        /// <inheritdoc/>
        public async Task<bool> HasMultipleUsersForEmailAsync(string emailAddress)
        {
            var request = $"ElfhUser/HasMultipleUsersForEmail/{emailAddress}";
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new Exception("No Email given to check.");
            }

            var client = await this.userApiHttpClient.GetClientAsync();
            var response = await client.GetAsync(request).ConfigureAwait(false);
            var hasMultipleUsers = false;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                hasMultipleUsers = bool.Parse(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return hasMultipleUsers;
        }

        /// <summary>
        /// The register new user.
        /// </summary>
        /// <param name="registrationRequest">The registrationRequest<see cref="RegistrationRequestViewModel"/>.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> RegisterNewUser(RegistrationRequestViewModel registrationRequest)
        {
            var json = JsonConvert.SerializeObject(registrationRequest);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/RegisterUser";

            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            LearningHubValidationResult result = null;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<LearningHubValidationResult>(content);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return result;
        }

        /// <summary>
        /// The set user initial password async.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="loctoken">The loctoken.</param>
        /// <param name="password">The password.</param>
        /// <returns>The <see cref="T:Task{bool}"/>.</returns>
        public async Task<bool> SetUserInitialPasswordAsync(string token, string loctoken, string password)
        {
            PasswordCreateModel passwordCreateModel = new PasswordCreateModel
            {
                Token = token,
                Loctoken = loctoken,
                PasswordHash = this.Base64MD5HashDigest(password),
            };

            var json = JsonConvert.SerializeObject(passwordCreateModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = "Authentication/SetInitialUserPassword";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                bool apiResponse = JsonConvert.DeserializeObject<bool>(result);
                return apiResponse;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized
                ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return false;
        }

        /// <summary>
        /// The store user history.
        /// </summary>
        /// <param name="userHistory">The userHistory<see cref="UserHistoryViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task StoreUserHistory(UserHistoryViewModel userHistory)
        {
            var json = JsonConvert.SerializeObject(userHistory);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = "UserHistory";

            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Failed to store UserHistory: " + JsonConvert.SerializeObject(userHistory));
                }
            }
        }

        /// <summary>
        /// The update login wizard flag.
        /// </summary>
        /// <param name="loginWizardInProgress">The loginWizardInProgress<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateLoginWizardFlag(bool loginWizardInProgress)
        {
            var json = JsonConvert.SerializeObject(loginWizardInProgress);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/UpdateLoginWizardFlag";

            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update user login wizard flag");
            }
        }

        /// <summary>
        /// The update password.
        /// </summary>
        /// <param name="newPasswordHash">The newPasswordHash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdatePassword(string newPasswordHash)
        {
            PasswordUpdateModel passwordUpdateModel = new PasswordUpdateModel() { PasswordHash = this.Base64MD5HashDigest(newPasswordHash) };
            var json = JsonConvert.SerializeObject(passwordUpdateModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/UpdateCurrentUserPassword";

            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update user password");
            }
        }

        /// <summary>
        /// The update personal details.
        /// </summary>
        /// <param name="personalDetailsViewModel">The personalDetailsViewModel<see cref="PersonalDetailsViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdatePersonalDetails(PersonalDetailsViewModel personalDetailsViewModel)
        {
            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/UpdatePersonalDetails";

            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update user employment");
            }
        }

        /// <summary>
        /// The update user employment.
        /// </summary>
        /// <param name="userEmployment">The userEmployment<see cref="UserEmployment"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateUserEmployment(UserEmployment userEmployment)
        {
            var json = JsonConvert.SerializeObject(userEmployment);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"UserEmployment/PutAsync";

            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update user employment");
            }
        }

        /// <summary>
        /// The update user security questions.
        /// </summary>
        /// <param name="userSecurityQuestions">The userSecurityQuestions<see cref="List{UserSecurityQuestionViewModel}"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateUserSecurityQuestions(List<UserSecurityQuestionViewModel> userSecurityQuestions)
        {
            foreach (var userSecurityQuestion in userSecurityQuestions)
            {
                if (userSecurityQuestion.SecurityQuestionAnswerHash != "********")
                {
                    userSecurityQuestion.SecurityQuestionAnswerHash = this.Base64MD5HashDigest(userSecurityQuestion.SecurityQuestionAnswerHash.ToLower());
                }
            }

            var json = JsonConvert.SerializeObject(userSecurityQuestions);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/UpdateUserSecurityQuestions";

            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update user security questions");
            }
        }

        /// <summary>
        /// The validate user password token async.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="loctoken">The loctoken.</param>
        /// <returns>The <see cref="Task{PasswordValidationTokenResult}"/>.</returns>
        public async Task<PasswordValidationTokenResult> ValidateUserPasswordTokenAsync(string token, string loctoken)
        {
            PasswordValidationTokenResult tokenResult = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Authentication/ValidateToken/{Uri.EscapeDataString(token.EncodeParameter())}/{Uri.EscapeDataString(loctoken.EncodeParameter())}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                tokenResult = JsonConvert.DeserializeObject<PasswordValidationTokenResult>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return tokenResult;
        }

        /// <inheritdoc/>
        public async Task SyncLHUserAsync(int userId)
        {
            var elfhUser = await this.GetUserByUserIdAsync(userId);
            var lhUser = await this.GetLHUserByUserIdAsync(userId);
            var lhUserProfile = await this.GetUserProfileAsync(elfhUser.Id);

            if (lhUser == null)
            {
                var newLhUser = new UserCreateViewModel()
                {
                    Id = elfhUser.Id,
                    UserName = elfhUser.UserName,
                };
                var result = await this.CreateUserAsync(newLhUser);
            }
            else if (elfhUser.UserName != lhUser.UserName)
            {
                var newLhUser = new UserUpdateViewModel()
                {
                    Id = elfhUser.Id,
                    UserName = elfhUser.UserName,
                };
                await this.UpdateUserAsync(newLhUser);
            }

            if (lhUserProfile == null)
            {
                // Create LH Profile if it does not exist yet
                var userProfile = new UserProfile
                {
                    Id = elfhUser.Id,
                    UserName = elfhUser.UserName,
                    EmailAddress = elfhUser.EmailAddress,
                    FirstName = elfhUser.FirstName,
                    LastName = elfhUser.LastName,
                    Active = elfhUser.Active,
                };

                await this.CreateUserProfileAsync(userProfile);
            }
            else if (lhUserProfile.UserName != elfhUser.UserName
                    || lhUserProfile.EmailAddress != elfhUser.EmailAddress
                    || lhUserProfile.FirstName != elfhUser.FirstName
                    || lhUserProfile.LastName != elfhUser.LastName
                    || lhUserProfile.Active != elfhUser.Active)
            {
                var userProfile = new UserProfile
                {
                    Id = elfhUser.Id,
                    UserName = elfhUser.UserName,
                    EmailAddress = elfhUser.EmailAddress,
                    FirstName = elfhUser.FirstName,
                    LastName = elfhUser.LastName,
                    Active = elfhUser.Active,
                };
                await this.UpdateUserProfileAsync(userProfile);
            }
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> UpdateUserAsync(UserUpdateViewModel userUpdateViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(userUpdateViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/UpdateUser";

            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                return apiResponse.ValidationResult;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception("save failed!");
            }
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> CreateUserAsync(UserCreateViewModel newLhUser)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(newLhUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/CreateUser";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                return apiResponse.ValidationResult;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception("save failed!");
            }
        }

        /// <inheritdoc/>
        public async Task<UserProfileSummaryViewModel> GetUserProfileSummaryAsync()
        {
            UserProfileSummaryViewModel viewModel = null;

            var personalDetailsModel = await this.GetCurrentUserPersonalDetailsAsync();
            var employmentViewModel = await this.GetPrimaryUserEmploymentForUser(personalDetailsModel.UserId);

            var securityQuestionsViewModel = await this.loginWizardService.GetSecurityQuestionsModel(personalDetailsModel.UserId);

            if (personalDetailsModel != null)
            {
                viewModel = new UserProfileSummaryViewModel
                {
                    Id = personalDetailsModel.UserId,
                    UserName = personalDetailsModel.UserName,
                    FirstName = personalDetailsModel.FirstName,
                    LastName = personalDetailsModel.LastName,
                    PreferredName = personalDetailsModel.PreferredName,
                    PrimaryEmailAddress = personalDetailsModel.PrimaryEmailAddress,
                    SecondaryEmailAddress = personalDetailsModel.SecondaryEmailAddress,
                    SecurityFirstQuestion = securityQuestionsViewModel.UserSecurityQuestions.Any() ? securityQuestionsViewModel.UserSecurityQuestions.First().QuestionText : null,
                    SecuritySecondQuestion = (securityQuestionsViewModel.UserSecurityQuestions.Any() && securityQuestionsViewModel.UserSecurityQuestions.Count > 1) ? securityQuestionsViewModel.UserSecurityQuestions[1].QuestionText : null,
                    LastUpdated = personalDetailsModel.LastUpdated,
                    PasswordHash = personalDetailsModel.PasswordHash,

                    EmploymentId = employmentViewModel.Id,
                    JobRoleId = employmentViewModel.JobRoleId,
                    MedicalCouncilId = employmentViewModel.MedicalCouncilId,
                    MedicalCouncilNo = employmentViewModel.MedicalCouncilNo,
                    GradeId = employmentViewModel.GradeId,
                    SpecialtyId = employmentViewModel.SpecialtyId,
                    JobStartDate = employmentViewModel.StartDate,
                    LocationId = employmentViewModel.LocationId,
                    NewPrimaryEmailAddress = personalDetailsModel.NewPrimaryEmailAddress,
                };

                if (personalDetailsModel.CountryId.HasValue)
                {
                    var country = await this.countryService.GetByIdAsync(personalDetailsModel.CountryId.Value);
                    viewModel.CountryName = country.Name;
                }

                if (personalDetailsModel.RegionId.HasValue)
                {
                    var region = await this.regionService.GetByIdAsync(personalDetailsModel.RegionId.Value);
                    viewModel.RegionName = region.Name;
                }

                if (employmentViewModel.JobRoleId.HasValue)
                {
                    var job = await this.jobRoleService.GetByIdAsync(employmentViewModel.JobRoleId.Value);
                    viewModel.JobRole = job.Name;

                    if (employmentViewModel.GradeId.HasValue)
                    {
                        var grades = await this.gradeService.GetGradesForJobRoleAsync(employmentViewModel.JobRoleId.Value);
                        var grade = grades.SingleOrDefault(g => g.Id == employmentViewModel.GradeId.Value);
                        viewModel.Grade = grade?.Name;
                    }
                }

                if (employmentViewModel.SpecialtyId.HasValue)
                {
                    var specialities = await this.specialtyService.GetSpecialtiesAsync();
                    var specialty = specialities.Single(s => s.Id == employmentViewModel.SpecialtyId.Value);
                    viewModel.PrimarySpecialty = specialty.Name;
                }

                var location = await this.locationService.GetByIdAsync(employmentViewModel.LocationId);
                viewModel.PlaceOfWork = $"{location.Name}<br />Address: {location.Address}<br />Org Code: {location.NhsCode}";
            }

            return viewModel;
        }

        /// <inheritdoc/>
        public async Task<UserPersonalDetailsViewModel> GetUserPersonalDetailsAsync()
        {
            UserPersonalDetailsViewModel viewModel = null;

            PersonalDetailsViewModel personalDetailsModel = await this.GetCurrentUserPersonalDetailsAsync();

            if (personalDetailsModel != null)
            {
                viewModel = new UserPersonalDetailsViewModel
                {
                    UserName = personalDetailsModel.UserName,
                    FirstName = personalDetailsModel.FirstName,
                    LastName = personalDetailsModel.LastName,
                    PreferredName = personalDetailsModel.PreferredName,
                };
            }

            return viewModel;
        }

        /// <inheritdoc/>
        public async Task UpdateUserPersonalDetailsAsync(int userId, UserPersonalDetailsViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PreferredName = model.PreferredName,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdatePersonalDetails";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update personal details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task<UserEmailDetailsViewModel> GetUserEmailDetailsAsync()
        {
            UserEmailDetailsViewModel viewModel = null;

            PersonalDetailsViewModel personalDetailsModel = await this.GetCurrentUserPersonalDetailsAsync();

            if (personalDetailsModel != null)
            {
                viewModel = new UserEmailDetailsViewModel
                {
                    PrimaryEmailAddress = personalDetailsModel.PrimaryEmailAddress,
                    SecondaryEmailAddress = personalDetailsModel.SecondaryEmailAddress,
                };
            }

            return viewModel;
        }

        /// <inheritdoc/>
        public async Task UpdateUserEmailDetailsAsync(int userId, UserEmailDetailsViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                PrimaryEmailAddress = model.PrimaryEmailAddress,
                SecondaryEmailAddress = model.SecondaryEmailAddress,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateEmailDetails";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update email details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task<UserLocationViewModel> GetUserLocationDetailsAsync()
        {
            PersonalDetailsViewModel personalDetailsModel = await this.GetCurrentUserPersonalDetailsAsync();

            UserLocationViewModel model = new UserLocationViewModel
            {
                SelectedCountryId = personalDetailsModel.CountryId,
                SelectedRegionId = personalDetailsModel.RegionId,
            };

            return model;
        }

        /// <inheritdoc/>
        public async Task UpdateUserLocationDetailsAsync(int userId, UserLocationViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                CountryId = model.SelectedCountryId,
                RegionId = model.SelectedRegionId,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateLocationDetails";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update user location details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserFirstNameAsync(int userId, UserPersonalDetailsViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                FirstName = model.FirstName,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateFirstName";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update first name failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserLastNameAsync(int userId, UserPersonalDetailsViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                LastName = model.LastName,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateLastName";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update last name failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserPreferredNameAsync(int userId, UserPersonalDetailsViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                PreferredName = model.PreferredName,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdatePreferredName";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update preferred name failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserPrimaryEmailAsync(int userId, UserEmailDetailsViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                PrimaryEmailAddress = model.PrimaryEmailAddress,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdatePrimaryEmail";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update email details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserSecondaryEmailAsync(int userId, UserEmailDetailsViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                SecondaryEmailAddress = model.SecondaryEmailAddress,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateSecondaryEmail";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update email details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserCountryDetailsAsync(int userId, UserLocationViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                CountryId = model.SelectedCountryId,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateCountryDetails";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update user country details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserRegionDetailsAsync(int userId, UserLocationViewModel model)
        {
            PersonalDetailsViewModel personalDetailsViewModel = new PersonalDetailsViewModel
            {
                UserId = userId,
                RegionId = model.SelectedRegionId,
            };

            var json = JsonConvert.SerializeObject(personalDetailsViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateRegionDetails";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update user region details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task<EmailChangeValidationTokenResult> ValidateEmailChangeTokenAsync(string token, string loctoken, bool isUserRoleUpgrade)
        {
            EmailChangeValidationTokenResult tokenResult = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/ValidateEmailChangeToken/{Uri.EscapeUriString(token.EncodeParameter())}/{Uri.EscapeUriString(loctoken.EncodeParameter())}/{isUserRoleUpgrade}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                tokenResult = JsonConvert.DeserializeObject<EmailChangeValidationTokenResult>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return tokenResult;
        }

        /// <summary>
        /// GetLastIssuedEmailChangeValidationToken.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<EmailChangeValidationTokenViewModel> GetLastIssuedEmailChangeValidationTokenAsync()
        {
            EmailChangeValidationTokenViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = "User/GetLastIssuedEmailChangeValidationToken";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<EmailChangeValidationTokenViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<EmailChangeValidationTokenViewModel> RegenerateEmailChangeValidationTokenAsync(string newPrimaryEmail, bool isUserRoleUpgrade)
        {
            EmailChangeValidationTokenViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/ReGenerateEmailChangeValidationToken/{newPrimaryEmail}/{isUserRoleUpgrade}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<EmailChangeValidationTokenViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<bool> CanRequestPasswordResetAsync(string emailAddress, int passwordRequestLimitingPeriod, int passwordRequestLimit)
        {
            bool status = false;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/CanRequestPasswordReset/{emailAddress}/{passwordRequestLimitingPeriod}/{passwordRequestLimit}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                status = JsonConvert.DeserializeObject<bool>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return status;
        }

        /// <inheritdoc/>
        public async Task<EmailChangeValidationTokenViewModel> GenerateEmailChangeValidationTokenAndSendEmailAsync(string emailAddress, bool isUserRoleUpgrade)
        {
            EmailChangeValidationTokenViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/GenerateEmailChangeValidationTokenAndSendEmail/{emailAddress}/{isUserRoleUpgrade}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<EmailChangeValidationTokenViewModel>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task CancelEmailChangeValidationTokenAsync()
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"User/CancelEmailChangeValidationToken";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpgradeAsFullAccessUserAsync(int userId, string email)
        {
            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpgradeAsFullAccessUser/{userId}/{email}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<UserRoleUpgrade> GetUserRoleUpgradeAsync(int userId)
        {
            UserRoleUpgrade model = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ElfhUser/GetUserRoleUpgrade/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                model = JsonConvert.DeserializeObject<UserRoleUpgrade>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return model;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateUserRoleUpgradeAsync(UserRoleUpgrade userRoleUpgradeModel)
        {
            var json = JsonConvert.SerializeObject(new UserRoleUpgrade()
            {
                UserId = userRoleUpgradeModel.UserId,
                EmailAddress = userRoleUpgradeModel.EmailAddress,
                CreateUserId = userRoleUpgradeModel.UserId,
                CreateDate = DateTimeOffset.Now,
                UserHistoryTypeId = userRoleUpgradeModel.UserHistoryTypeId,
            });
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/CreateUserRoleUpgrade";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task UpdateUserRoleUpgradeAsync()
        {
            UserRoleUpgrade userRoleUpgrade = new UserRoleUpgrade
            {
                Deleted = false,
            };

            var json = JsonConvert.SerializeObject(userRoleUpgrade);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateUserRoleUpgrade";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update user role upgrade details failed!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserPrimaryEmailAsync(string email)
        {
            UserRoleUpgrade userRoleUpgrade = new UserRoleUpgrade
            {
                Deleted = false,
                UpgradeDate = DateTimeOffset.Now,
            };

            var json = JsonConvert.SerializeObject(userRoleUpgrade);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"ElfhUser/UpdateUserPrimaryEmail/{email}";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update user role upgrade details failed!");
            }
        }

        /// <summary>
        /// Get providers by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>providers.</returns>
        public async Task<List<ProviderViewModel>> GetProvidersByUserIdAsync(int userId)
        {
            List<ProviderViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Provider/GetProvidersByUserId/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<ProviderViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <inheritdoc/>
        public async Task<PagedResultSet<UserHistoryViewModel>> CheckUserHasAnActiveSessionAsync(int userId)
        {
            PagedResultSet<UserHistoryViewModel> userHistoryViewModel = new PagedResultSet<UserHistoryViewModel>();

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"UserHistory/CheckUserHasActiveSession/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                userHistoryViewModel = JsonConvert.DeserializeObject<PagedResultSet<UserHistoryViewModel>>(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return userHistoryViewModel;
        }

        /// <summary>
        /// The base 64 m d 5 hash digest.
        /// </summary>
        /// <param name="szString">The szString.</param>
        /// <returns>Hashed string.</returns>
        public string Base64MD5HashDigest(string szString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Encoding eEncoding = new ASCIIEncoding();
            var abHashDigest = md5.ComputeHash(eEncoding.GetBytes(szString));

            return Convert.ToBase64String(abHashDigest);
        }
    }
}
