namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Admin;
    using LearningHub.Nhs.Models.Validation;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ResourceService" />.
    /// </summary>
    public class ResourceService : BaseService, IResourceService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        public ResourceService(ILearningHubHttpClient learningHubHttpClient)
        : base(learningHubHttpClient)
        {
        }

        /// <summary>
        /// The GetResourceAdminSearchPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{ResourceAdminSearchResultViewModel}"/>.</returns>
        public async Task<PagedResultSet<ResourceAdminSearchResultViewModel>> GetResourceAdminSearchPageAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<ResourceAdminSearchResultViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var json = JsonConvert.SerializeObject(pagingRequestModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = $"Resource/GetResourceAdminSearchFilteredPage";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

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
        /// The GetResourceVersionEventsAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionEventViewModel}"/>.</returns>
        public async Task<List<ResourceVersionEventViewModel>> GetResourceVersionEventsAsync(int resourceVersionId)
        {
            List<ResourceVersionEventViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceVersionEvents/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<ResourceVersionEventViewModel>>(result);
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
        /// The GetResourceVersionValidationResultAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{List{ResourceVersionValidationResultViewModel}}"/>.</returns>
        public async Task<ResourceVersionValidationResultViewModel> GetResourceVersionValidationResultAsync(int resourceVersionId)
        {
            ResourceVersionValidationResultViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceVersionValidationResult/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ResourceVersionValidationResultViewModel>(result);
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
        /// The GetResourceVersionExtendedViewModelAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionExtendedViewModel}"/>.</returns>
        public async Task<ResourceVersionExtendedViewModel> GetResourceVersionExtendedViewModelAsync(int resourceVersionId)
        {
            ResourceVersionExtendedViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceVersionExtendedViewModel/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ResourceVersionExtendedViewModel>(result);
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
        /// The GetResourceVersionsAsync.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionViewModel}"/>.</returns>
        public async Task<List<ResourceVersionViewModel>> GetResourceVersionsAsync(int resourceId)
        {
            List<ResourceVersionViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceVersions/{resourceId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<ResourceVersionViewModel>>(result);
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
        /// The RevertToDraft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> RevertToDraft(int resourceVersionId)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(new { resourceVersionId });
            var stringContent = new StringContent(resourceVersionId.ToString(), Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/RevertToDraft";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("RevertPublishingToDraft failed!");
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
        /// The TransferResourceOwnership.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <param name="newResourceOwner">The newResourceOwner<see cref="string"/>.</param>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> TransferResourceOwnership(int resourceId, string newResourceOwner, int resourceVersionId)
        {
            ApiResponse apiResponse = null;
            var vm = new TransferResourceOwnershipViewModel()
            {
                ResourceId = resourceId,
                NewOwnerUsername = newResourceOwner,
                ResourceVersionId = resourceVersionId,
            };

            var json = JsonConvert.SerializeObject(vm);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/TransferResourceOwnership";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("TransferResourceOwnership failed!");
                }
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
        /// The UnpublishResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="details">The details<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UnpublishResourceVersionAsync(int resourceVersionId, string details)
        {
            ApiResponse apiResponse = null;
            var vm = new UnpublishViewModel()
            {
                ResourceVersionId = resourceVersionId,
                SetFlag = true,
                Details = details,
            };

            var json = JsonConvert.SerializeObject(vm);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/UnpublishResourceVersion";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Unpublish failed!");
                }
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
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="resourceVersionEventType">resource version event.</param>
        /// <param name="details">details.</param>
        /// <param name="userId">user id.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<LearningHubValidationResult> CreateResourceVersionEvent(int resourceVersionId, ResourceVersionEventTypeEnum resourceVersionEventType, string details, int userId)
        {
            ApiResponse apiResponse = null;

            ResourceVersionEventViewModel vm = new ResourceVersionEventViewModel();
            vm.ResourceVersionId = resourceVersionId;
            vm.ResourceVersionEventType = resourceVersionEventType;
            vm.Details = details;
            vm.CreateUserId = userId;

            var json = JsonConvert.SerializeObject(vm);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/CreateResourceVersionEvent";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("CreateResourceVersionEvent failed!");
                }
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
    }
}
