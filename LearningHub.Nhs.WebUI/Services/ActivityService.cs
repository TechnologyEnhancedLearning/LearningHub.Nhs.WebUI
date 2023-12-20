// <copyright file="ActivityService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ActivityService" />.
    /// </summary>
    public class ActivityService : BaseService<ActivityService>, IActivityService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="logger">Logger.</param>
        public ActivityService(ILearningHubHttpClient learningHubHttpClient, ILogger<ActivityService> logger)
        : base(learningHubHttpClient, logger)
        {
        }

        /// <summary>
        /// Validates and creates an assessment resource activity.
        /// </summary>
        /// <param name="createAssessmentResourceActivityViewModel">The createAssessmentResourceActivityViewModel<see cref="CreateAssessmentResourceActivityViewModel"/>.</param>
        /// <returns>.</returns>
        public async Task<LearningHubValidationResult> CreateAssessmentResourceActivityAsync(CreateAssessmentResourceActivityViewModel createAssessmentResourceActivityViewModel)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(createAssessmentResourceActivityViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/CreateAssessmentResourceActivity";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

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

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// Validates and creates an assessment resource activity interaction.
        /// </summary>
        /// <param name="createAssessmentResourceActivityInteractionViewModel">The createAssessmentResourceActivityInteractionViewModel<see cref="CreateAssessmentResourceActivityViewModel"/>.</param>
        /// <returns>The result of the API request.</returns>
        public async Task<AssessmentViewModel> CreateAssessmentResourceActivityInteractionAsync(CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionViewModel)
        {
            AssessmentViewModel viewModel = null;

            var json = JsonConvert.SerializeObject(createAssessmentResourceActivityInteractionViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/CreateAssessmentResourceActivityInteraction";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewModel = JsonConvert.DeserializeObject<AssessmentViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewModel;
        }

        /// <summary>
        /// The create media resource activity.
        /// </summary>
        /// <param name="createMediaResourceActivityViewModel">The createMediaResourceActivityViewModel<see cref="CreateMediaResourceActivityViewModel"/>.</param>
        /// <returns>.</returns>
        public async Task<LearningHubValidationResult> CreateMediaResourceActivityAsync(CreateMediaResourceActivityViewModel createMediaResourceActivityViewModel)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(createMediaResourceActivityViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/CreateMediaResourceActivity";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

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

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The create media resource activity interaction.
        /// </summary>
        /// <param name="createMediaResourceActivityInteractionViewModel">The createMediaResourceActivityInteractionViewModel<see cref="CreateMediaResourceActivityInteractionViewModel"/>.</param>
        /// <returns>.</returns>
        public async Task<LearningHubValidationResult> CreateMediaResourceActivityInteractionAsync(CreateMediaResourceActivityInteractionViewModel createMediaResourceActivityInteractionViewModel)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(createMediaResourceActivityInteractionViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/CreateMediaResourceActivityInteraction";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

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

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The create resource activity async.
        /// </summary>
        /// <param name="createResourceActivityViewModel">The createResourceActivityViewModel<see cref="CreateResourceActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateResourceActivityAsync(CreateResourceActivityViewModel createResourceActivityViewModel)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(createResourceActivityViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/CreateResourceActivity";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

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

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The LaunchScormActivityAsync.
        /// </summary>
        /// <param name="launchScormActivityViewModel">The launchScormActivityViewModel<see cref="LaunchScormActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<ScormActivityViewModel> LaunchScormActivityAsync(LaunchScormActivityViewModel launchScormActivityViewModel)
        {
            var json = JsonConvert.SerializeObject(launchScormActivityViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"activity/launchscormactivity";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ScormActivityViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return null;
        }

        /// <summary>
        /// The UpdateScormActivityAsync.
        /// </summary>
        /// <param name="updateScormActivityViewModel">The updateScormActivityViewModel<see cref="ScormActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<ScormUpdateResponseViewModel> UpdateScormActivityAsync(ScormActivityViewModel updateScormActivityViewModel)
        {
            ScormUpdateResponseViewModel updateResponse = null;

            var json = JsonConvert.SerializeObject(updateScormActivityViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/UpdateScormActivity";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                updateResponse = JsonConvert.DeserializeObject<ScormUpdateResponseViewModel>(result);

                if (!updateResponse.IsValid)
                {
                    throw new Exception("update failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return updateResponse;
        }

        /// <summary>
        /// The CompleteScormActivity.
        /// </summary>
        /// <param name="scormActivityViewModel">The updateScormActivityViewModel<see cref="ScormActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CompleteScormActivity(ScormActivityViewModel scormActivityViewModel)
        {
            var json = JsonConvert.SerializeObject(scormActivityViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/CompleteScormActivity";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            LearningHubValidationResult validationResult;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                validationResult = JsonConvert.DeserializeObject<LearningHubValidationResult>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception("Complete Scorm Activity failed!");
            }

            return validationResult;
        }

        /// <summary>
        /// The ResolveScormActivity.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ResolveScormActivity(int userId)
        {
            var request = $"Activity/ResolveScormActivity/{userId}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// Check user scorm activity data suspend data need to be cleared.
        /// </summary>
        /// <param name="lastScormActivityId">last scorm activity id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>boolean.</returns>
        public async Task<bool> CheckSuspendDataToBeCleared(int lastScormActivityId, int resourceVersionId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Activity/CheckUserScormActivitySuspendDataToBeCleared/{lastScormActivityId}/{resourceVersionId}";
            var content = new StringContent(JsonConvert.SerializeObject(new { }));
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<bool>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return false;
        }
    }
}
