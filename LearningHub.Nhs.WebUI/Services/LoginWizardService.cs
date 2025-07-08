namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="LoginWizardService" />.
    /// </summary>
    public class LoginWizardService : BaseService<LoginWizardService>, ILoginWizardService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWizardService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="userApiHttpClient">User api http client.</param>
        /// <param name="logger">Logger.</param>
        public LoginWizardService(
            ILearningHubHttpClient learningHubHttpClient,
            IOpenApiHttpClient openApiHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<LoginWizardService> logger)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The CompleteLoginWizard.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CompleteLoginWizard(int userId)
        {
            UserBasic user = new UserBasic { Id = userId };
            var json = JsonConvert.SerializeObject(user);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"LoginWizard/CompleteWizardForUser";

            var response = await client.PatchAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update user as logon wizard completed");
            }
        }

        /// <summary>
        /// The GetLoginWizard.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LoginWizardStagesViewModel}"/>.</returns>
        public async Task<LoginWizardStagesViewModel> GetLoginWizard(int userId)
        {
            LoginWizardStagesViewModel loginWizardStages = new LoginWizardStagesViewModel();

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"LoginWizard/GetLoginWizardStagesByUserId/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                loginWizardStages = JsonConvert.DeserializeObject<LoginWizardStagesViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return loginWizardStages;
        }

        /// <summary>
        /// The GetSecurityQuestions.
        /// </summary>
        /// <returns>The <see cref="T:Task{List{SecurityQuestion}}"/>.</returns>
        public async Task<List<SecurityQuestion>> GetSecurityQuestions()
        {
            List<SecurityQuestion> securityQuestions = new List<SecurityQuestion>();

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"LoginWizard/GetSecurityQuestions";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                securityQuestions = JsonConvert.DeserializeObject<List<SecurityQuestion>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return securityQuestions;
        }

        /// <summary>
        /// The GetSecurityQuestionsModel.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SecurityQuestionsViewModel}"/>.</returns>
        public async Task<SecurityQuestionsViewModel> GetSecurityQuestionsModel(int userId)
        {
            SecurityQuestionsViewModel model = new SecurityQuestionsViewModel();

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"LoginWizard/GetSecurityQuestionsByUserId/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                model = JsonConvert.DeserializeObject<SecurityQuestionsViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return model;
        }

        /// <summary>
        /// The SaveLoginWizardStageActivity.
        /// </summary>
        /// <param name="loginWizardStageEnum">The loginWizardStageEnum<see cref="LoginWizardStageEnum"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task SaveLoginWizardStageActivity(LoginWizardStageEnum loginWizardStageEnum, int userId)
        {
            var stageAndUser = new Tuple<int, int>((int)loginWizardStageEnum, userId);

            var json = JsonConvert.SerializeObject(stageAndUser);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = "LoginWizard/CreateStageActivity";

            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Failed to store LoginWizardStageActivity: " + JsonConvert.SerializeObject(stageAndUser));
                }
            }
        }

        /// <summary>
        /// The StartLoginWizard.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task StartLoginWizard(int userId)
        {
            UserBasic user = new UserBasic { Id = userId };
            var json = JsonConvert.SerializeObject(user);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"LoginWizard/StartWizardForUser";

            var response = await client.PatchAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update user as logon wizard started");
            }
        }
    }
}
