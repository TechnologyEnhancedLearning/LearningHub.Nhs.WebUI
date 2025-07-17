namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Services;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The catalogue service.
    /// </summary>
    public class CatalogueService : BaseService<CatalogueService>, ICatalogueService
    {
        private readonly ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learning hub http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cacheService.</param>
        public CatalogueService(ILearningHubHttpClient learningHubHttpClient, ILogger<CatalogueService> logger, ICacheService cacheService)
        : base(learningHubHttpClient, logger)
        {
            this.cacheService = cacheService;
        }

        /// <summary>
        /// Gets catalogues for user.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<CatalogueBasicViewModel>> GetCataloguesForUserAsync()
        {
            List<CatalogueBasicViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Catalogue/GetForCurrentUser";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<CatalogueBasicViewModel>>(result);
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
        /// GetCatalogue.
        /// </summary>
        /// <param name="reference">reference.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(string reference)
        {
            CatalogueViewModel viewmodel = new CatalogueViewModel { };

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"catalogue/catalogue/{reference}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<CatalogueViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// GetCatalogue.
        /// </summary>
        /// <param name="catalogueNodeVersionId">The catalogueNodeVersionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(int catalogueNodeVersionId)
        {
            CatalogueViewModel viewmodel = new CatalogueViewModel { };

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"catalogue/catalogues/{catalogueNodeVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<CatalogueViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// GetCatalogueRecorded.
        /// </summary>
        /// <param name="reference">reference.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<CatalogueViewModel> GetCatalogueRecordedAsync(string reference)
        {
            CatalogueViewModel viewmodel = new CatalogueViewModel { };

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"catalogue/catalogue-recorded/{reference}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<CatalogueViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<CatalogueResourceResponseViewModel> GetResourcesAsync(CatalogueResourceRequestViewModel requestViewModel)
        {
            CatalogueResourceResponseViewModel viewmodel = new CatalogueResourceResponseViewModel { };

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var json = JsonConvert.SerializeObject(requestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var request = $"catalogue/resources";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<CatalogueResourceResponseViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// Can the current user edit the catalogue.
        /// </summary>
        /// <param name="catalogueId">The catalogue id.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public async Task<bool> CanCurrentUserEditCatalogue(int catalogueId)
        {
            var request = $"Catalogue/CanCurrentUserEditCatalogue/{catalogueId}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var response = await client.GetAsync(request).ConfigureAwait(false);
            var catalogueIsEditable = false;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                catalogueIsEditable = bool.Parse(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return catalogueIsEditable;
        }

        /// <summary>
        /// Gets the access details for the catalogue.
        /// </summary>
        /// <param name="reference">The catalogue reference.</param>
        /// <returns>The access details.</returns>
        public async Task<CatalogueAccessDetailsViewModel> AccessDetailsAsync(string reference)
        {
            var request = $"Catalogue/AccessDetails/{reference}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var response = await client.GetAsync(request).ConfigureAwait(false);
            var accessDetails = new CatalogueAccessDetailsViewModel();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                accessDetails = JsonConvert.DeserializeObject<CatalogueAccessDetailsViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return accessDetails;
        }

        /// <summary>
        /// Gets the latest catalogue access request for the catalogue, for the logged in user.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogue node id.</param>
        /// <returns>The latest catalogue access request details.</returns>
        public async Task<CatalogueAccessRequestViewModel> GetLatestCatalogueAccessRequestAsync(int catalogueNodeId)
        {
            var request = $"Catalogue/GetLatestCatalogueAccessRequest/{catalogueNodeId}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var response = await client.GetAsync(request).ConfigureAwait(false);
            var car = new CatalogueAccessRequestViewModel();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                car = JsonConvert.DeserializeObject<CatalogueAccessRequestViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }

            return car;
        }

        /// <summary>
        /// The RequestAccessAsync.
        /// </summary>
        /// <param name="reference">The catalogue reference.</param>
        /// <param name="vm">The view model.</param>
        /// <param name="accessType">The accessType.</param>
        /// <returns>The task.</returns>
        public async Task<LearningHubValidationResult> RequestAccessAsync(string reference, CatalogueAccessRequestViewModel vm, string accessType)
        {
            var request = $"Catalogue/RequestAccess/{reference}/{accessType}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(vm), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);
            var catalogueAccessRequested = false;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                catalogueAccessRequested = bool.Parse(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }

            return new LearningHubValidationResult(catalogueAccessRequested);
        }

        /// <summary>
        /// The InviteUserAsync.
        /// </summary>
        /// <param name="vm">The view model.</param>
        /// <returns>The task.</returns>
        public async Task<LearningHubValidationResult> InviteUserAsync(RestrictedCatalogueInviteUserViewModel vm)
        {
            var request = $"Catalogue/InviteUser";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(vm), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);
            var catalogueAccessRequested = false;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                catalogueAccessRequested = bool.Parse(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }

            return new LearningHubValidationResult(catalogueAccessRequested);
        }

        /// <summary>
        /// Gets the access requests for the supplied request model.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<RestrictedCatalogueAccessRequestViewModel>> GetRestrictedCatalogueAccessRequestsAsync(RestrictedCatalogueAccessRequestsRequestViewModel requestModel)
        {
            var viewModel = new List<RestrictedCatalogueAccessRequestViewModel>();

            var json = JsonConvert.SerializeObject(requestModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Catalogue/GetRestrictedCatalogueAccessRequests";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewModel = JsonConvert.DeserializeObject<List<RestrictedCatalogueAccessRequestViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }

            return viewModel;
        }

        /// <summary>
        /// GetRestrictedCatalogueSummary.
        /// </summary>
        /// <param name="catalogueNodeId">catalogueNodeId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<RestrictedCatalogueSummaryViewModel> GetRestrictedCatalogueSummary(int catalogueNodeId)
        {
            var viewmodel = new RestrictedCatalogueSummaryViewModel();

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"catalogue/GetRestrictedCatalogueSummary/{catalogueNodeId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<RestrictedCatalogueSummaryViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }

            return viewmodel;
        }

        /// <summary>
        /// Gets the restricted catalogue users for the supplied request model.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<RestrictedCatalogueUsersViewModel> GetRestrictedCatalogueUsersAsync(RestrictedCatalogueUsersRequestViewModel requestModel)
        {
            RestrictedCatalogueUsersViewModel viewModel;

            var json = JsonConvert.SerializeObject(requestModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Catalogue/GetRestrictedCatalogueUsers";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewModel = JsonConvert.DeserializeObject<RestrictedCatalogueUsersViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }

            return viewModel;
        }

        /// <summary>
        /// The AcceptAccessRequestAsync.
        /// </summary>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <param name="accessRequestUserId">The accessRequestUserId.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> AcceptAccessRequestAsync(int accessRequestId, int accessRequestUserId)
        {
            var request = $"Catalogue/AcceptAccessRequest/{accessRequestId}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(new { }));
            var response = await client.PostAsync(request, content).ConfigureAwait(false);
            var catalogueAccessRequested = new LearningHubValidationResult();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                catalogueAccessRequested = JsonConvert.DeserializeObject<LearningHubValidationResult>(result);
                var cacheKey = $"{accessRequestUserId}:AllRolesWithPermissions";
                await this.cacheService.RemoveAsync(cacheKey);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return catalogueAccessRequested;
        }

        /// <summary>
        /// The RejectAccessRequestAsync.
        /// </summary>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <param name="rejectionReason">The rejectionReason.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> RejectAccessRequestAsync(int accessRequestId, string rejectionReason)
        {
            var request = $"Catalogue/RejectAccessRequest/{accessRequestId}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(new CatalogueAccessRejectionViewModel { RejectionReason = rejectionReason }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);
            var vr = new LearningHubValidationResult();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                vr = JsonConvert.DeserializeObject<LearningHubValidationResult>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return vr;
        }

        /// <summary>
        /// The RejectAccessRequestAsync.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> DismissAccessRequestAsync(int catalogueNodeId)
        {
            var request = $"Catalogue/DismissAccessRequest/{catalogueNodeId}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(new { }));
            var response = await client.PostAsync(request, content).ConfigureAwait(false);
            var vr = new LearningHubValidationResult(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                vr = JsonConvert.DeserializeObject<LearningHubValidationResult>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return vr;
        }

        /// <summary>
        /// The GetCatalogueAccessRequestAsync.
        /// </summary>
        /// <param name="catalogueAccessRequestId">The catalogueAccessRequestId.</param>
        /// <returns>The catalogue access request.</returns>
        public async Task<CatalogueAccessRequestViewModel> GetCatalogueAccessRequestAsync(int catalogueAccessRequestId)
        {
            var request = $"Catalogue/AccessRequest/{catalogueAccessRequestId}";

            var client = await this.LearningHubHttpClient.GetClientAsync();
            var response = await client.GetAsync(request).ConfigureAwait(false);
            var catalogueAccessRequest = new CatalogueAccessRequestViewModel();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                catalogueAccessRequest = JsonConvert.DeserializeObject<CatalogueAccessRequestViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }

            return catalogueAccessRequest;
        }

        /// <summary>
        /// The RemoveUserFromRestrictedAccess.
        /// </summary>
        /// <param name="userUserGroupId">The user - user group id.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> RemoveUserFromRestrictedAccessUserGroup(int userUserGroupId)
        {
            var userGroup = new UserUserGroupViewModel() { Id = userUserGroupId };
            var json = JsonConvert.SerializeObject(userGroup);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/DeleteUserUserGroup";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Delete failed!");
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
        /// GetAllCatalogueAsync.
        /// </summary>
        /// <param name="filterChar">The filterChar.</param>
        /// /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<AllCatalogueResponseViewModel> GetAllCatalogueAsync(string filterChar)
        {
            AllCatalogueResponseViewModel viewmodel = new AllCatalogueResponseViewModel { };
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"catalogue/allcatalogues/{filterChar}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<AllCatalogueResponseViewModel>(result);
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
