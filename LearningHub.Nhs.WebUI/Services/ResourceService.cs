namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Settings = LearningHub.Nhs.WebUI.Configuration.Settings;

    /// <summary>
    /// Defines the <see cref="ResourceService" />.
    /// </summary>
    public class ResourceService : BaseService<ResourceService>, IResourceService
    {
        private readonly Settings settings;
        private readonly IAzureMediaService azureMediaService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="azureMediaService">Azure media services.</param>
        /// <param name="contextAccessor">The http context accessor.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        public ResourceService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, IAzureMediaService azureMediaService, IHttpContextAccessor contextAccessor, ICacheService cacheService, ILogger<ResourceService> logger, IOptions<Settings> settings)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
            this.settings = settings.Value;
            this.azureMediaService = azureMediaService;
            this.contextAccessor = contextAccessor;
            this.cacheService = cacheService;
        }

        /// <summary>
        /// The AcceptSensitiveContentAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> AcceptSensitiveContentAsync(int resourceVersionId)
        {
            ApiResponse apiResponse = null;

            var stringContent = new StringContent(resourceVersionId.ToString(), Encoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/AcceptSensitiveContent";
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
        /// The GetArticleDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ArticleViewModel}"/>.</returns>
        public async Task<ArticleViewModel> GetArticleDetailsByIdAsync(int resourceVersionId)
        {
            ArticleViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetArticleDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ArticleViewModel>(result);
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
        /// The GetAudioDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{AudioViewModel}"/>.</returns>
        public async Task<AudioViewModel> GetAudioDetailsByIdAsync(int resourceVersionId)
        {
            AudioViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetAudioDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<AudioViewModel>(result);
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
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceHeaderViewModel}"/>.</returns>
        public async Task<ResourceHeaderViewModel> GetByIdAsync(int id)
        {
            ResourceHeaderViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceHeaderViewModelAsync/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ResourceHeaderViewModel>(result);
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
        /// The GetFileTypeAsync.
        /// </summary>
        /// <returns>The <see cref="T:Task{List{FileTypeViewModel}}"/>.</returns>
        public async Task<List<FileTypeViewModel>> GetFileTypeAsync()
        {
            List<FileTypeViewModel> fileTypeList = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetFileTypes";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                fileTypeList = JsonConvert.DeserializeObject<List<FileTypeViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return fileTypeList;
        }

        /// <summary>
        /// The GetGenericFileDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{GenericFileViewModel}"/>.</returns>
        public async Task<GenericFileViewModel> GetGenericFileDetailsByIdAsync(int resourceVersionId)
        {
            GenericFileViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetGenericFileDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<GenericFileViewModel>(result);
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
        /// The GetHtmlDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{HtmlViewModel}"/>.</returns>
        public async Task<HtmlResourceViewModel> GetHtmlDetailsByIdAsync(int resourceVersionId)
        {
            HtmlResourceViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetHtmlDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<HtmlResourceViewModel>(result);
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
        /// The GetScormDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{GenericFileViewModel}"/>.</returns>
        public async Task<ScormViewModel> GetScormDetailsByIdAsync(int resourceVersionId)
        {
            ScormViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetScormDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ScormViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetExternalContentDetailsAsync.
        /// </summary>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ExternalContentDetailsViewModel> GetExternalContentDetailsAsync(int resourceVersionId)
        {
            ExternalContentDetailsViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetExternalContentDetailsById/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ExternalContentDetailsViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The RecordExternalReferenceUserAgreementAsync.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> RecordExternalReferenceUserAgreementAsync(ExternalReferenceUserAgreementViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/RecordExternalReferenceUserAgreement";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Record External Reference User Agreement failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return true;
        }

        /// <summary>
        /// The GetImageDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ImageViewModel}"/>.</returns>
        public async Task<ImageViewModel> GetImageDetailsByIdAsync(int resourceVersionId)
        {
            ImageViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetImageDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ImageViewModel>(result);
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
        /// The GetInformationByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceInformationViewModel}"/>.</returns>
        public async Task<ResourceInformationViewModel> GetInformationByIdAsync(int id)
        {
            ResourceInformationViewModel resourceInfo = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceInformationViewModelAsync/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                resourceInfo = JsonConvert.DeserializeObject<ResourceInformationViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return resourceInfo;
        }

        /// <summary>
        /// The GetItemByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceItemViewModel}"/>.</returns>
        public async Task<ResourceItemViewModel> GetItemByIdAsync(int id)
        {
            ResourceItemViewModel resourceItem = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceItemViewModelAsync/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                resourceItem = JsonConvert.DeserializeObject<ResourceItemViewModel>(result);

                if (resourceItem != null)
                {
                    resourceItem.LicenseUrl = this.settings.ResourceLicenseUrl;

                    // if resource type is video, add azure media content protection authentication token
                    if (resourceItem.ResourceTypeEnum == ResourceTypeEnum.Video && resourceItem.VideoDetails != null && resourceItem.VideoDetails.ResourceAzureMediaAsset != null && !string.IsNullOrEmpty(resourceItem.VideoDetails.ResourceAzureMediaAsset.FilePath))
                    {
                        resourceItem.VideoDetails.ResourceAzureMediaAsset.AuthenticationToken = await this.azureMediaService.GetContentAuthenticationTokenAsync(resourceItem.VideoDetails.ResourceAzureMediaAsset.FilePath);
                    }

                    // if resource type is audio, add azure media content protection authentication token
                    if (resourceItem.ResourceTypeEnum == ResourceTypeEnum.Audio && resourceItem.AudioDetails != null && resourceItem.AudioDetails.ResourceAzureMediaAsset != null && !string.IsNullOrEmpty(resourceItem.AudioDetails.ResourceAzureMediaAsset.FilePath))
                    {
                        resourceItem.AudioDetails.ResourceAzureMediaAsset.AuthenticationToken = await this.azureMediaService.GetContentAuthenticationTokenAsync(resourceItem.AudioDetails.ResourceAzureMediaAsset.FilePath);
                    }
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return resourceItem;
        }

        /// <summary>
        /// The GetLicencesAsync.
        /// </summary>
        /// <returns>The <see cref="T:Task{List{ResourceLicenceViewModel}}"/>.</returns>
        public async Task<List<ResourceLicenceViewModel>> GetLicencesAsync()
        {
            List<ResourceLicenceViewModel> licences = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceLicences";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                licences = JsonConvert.DeserializeObject<List<ResourceLicenceViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return licences;
        }

        /// <summary>
        /// The GetLocationsByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{CatalogueLocationsViewModel}"/>.</returns>
        public async Task<CatalogueLocationsViewModel> GetLocationsByIdAsync(int id)
        {
            CatalogueLocationsViewModel resourceLocations = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetCatalogueLocations/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                resourceLocations = JsonConvert.DeserializeObject<CatalogueLocationsViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return resourceLocations;
        }

        /// <summary>
        /// The GetResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceDetailViewModel}"/>.</returns>
        public async Task<ResourceDetailViewModel> GetResourceVersionAsync(int resourceVersionId)
        {
            ResourceDetailViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceVersion/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ResourceDetailViewModel>(result);
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
        /// The GetResourceVersionViewModelAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceDetailViewModel}"/>.</returns>
        public async Task<ResourceVersionViewModel> GetResourceVersionViewModelAsync(int resourceVersionId)
        {
            ResourceVersionViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetResourceVersionViewModel/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ResourceVersionViewModel>(result);
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
        /// The GetResourceVersionExtendedAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceDetailViewModel}"/>.</returns>
        public async Task<ResourceVersionExtendedViewModel> GetResourceVersionExtendedAsync(int resourceVersionId)
        {
            ResourceVersionExtendedViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

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
        /// The GetVersionHistoryByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionHistoryViewModel}"/>.</returns>
        public async Task<ResourceVersionHistoryViewModel> GetVersionHistoryByIdAsync(int id)
        {
            ResourceVersionHistoryViewModel viewmodel = new ResourceVersionHistoryViewModel() { Id = 1 };
            return await Task.Run(() => viewmodel);
        }

        /// <summary>
        /// The GetVideoDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VideoViewModel}"/>.</returns>
        public async Task<VideoViewModel> GetVideoDetailsByIdAsync(int resourceVersionId)
        {
            VideoViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetVideoDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<VideoViewModel>(result);
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
        /// The GetVideoFileContentAuthenticationTokenAsync.
        /// </summary>
        /// <param name="assetId">The assetId.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<string> GetVideoFileContentAuthenticationTokenAsync(string assetId)
        {
            return await this.azureMediaService.GetContentAuthenticationTokenAsync(assetId);
        }

        /// <summary>
        /// The GetWeblinkDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{WebLinkViewModel}"/>.</returns>
        public async Task<WebLinkViewModel> GetWeblinkDetailsByIdAsync(int resourceVersionId)
        {
            WebLinkViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetWeblinkDetails/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<WebLinkViewModel>(result);
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
        /// The GetCaseDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{WebLinkViewModel}"/>.</returns>
        public async Task<CaseViewModel> GetCaseDetailsByIdAsync(int resourceVersionId)
        {
            CaseViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetCaseDetails/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<CaseViewModel>(result);
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
        /// The GetAssessmentDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{WebLinkViewModel}"/>.</returns>
        public async Task<AssessmentViewModel> GetAssessmentDetailsByIdAsync(int resourceVersionId)
        {
            AssessmentViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetAssessmentDetails/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<AssessmentViewModel>(result);
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
        /// The GetAssessmentContent.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The task.</returns>
        public async Task<AssessmentViewModel> GetAssessmentContent(int resourceVersionId)
        {
            AssessmentViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetAssessmentContent/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<AssessmentViewModel>(result);
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
        /// Retrieves the latest assessment progress by the resource version id.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AssessmentProgressViewModel> GetAssessmentProgressByResourceVersion(int resourceVersionId)
        {
            AssessmentProgressViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetAssessmentProgress/resource/{resourceVersionId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<AssessmentProgressViewModel>(result);
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
        /// Retrieves the latest assessment progress by a given assessment activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AssessmentProgressViewModel> GetAssessmentProgressByActivity(int assessmentResourceActivityId)
        {
            AssessmentProgressViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetAssessmentProgress/activity/{assessmentResourceActivityId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<AssessmentProgressViewModel>(result);
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
        /// The GetFileStatusDetailsAsync.
        /// </summary>
        /// <param name="fileIds">The File Ids.</param>
        /// <returns>The files.</returns>
        public async Task<List<FileViewModel>> GetFileStatusDetailsAsync(int[] fileIds)
        {
            List<FileViewModel> viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            string queryString = string.Join("&", fileIds.Select(fileId => $"fileIds={fileId}"));
            var request = $"Resource/GetFileStatusDetails?{queryString}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<FileViewModel>>(result);
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
        /// The UnpublishResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UnpublishResourceVersionAsync(int resourceVersionId)
        {
            ApiResponse apiResponse = null;
            var rv = new ResourceViewModel() { ResourceVersionId = resourceVersionId };
            var json = JsonConvert.SerializeObject(rv);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UnpublishResourceVersion";
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
        /// The UserHasPublishedResourcesAsync.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public async Task<bool> UserHasPublishedResourcesAsync()
        {
            var cacheKey = $"{this.contextAccessor.HttpContext.User.Identity.GetCurrentUserId()}:UserHasPublishedResources";
            return await this.cacheService.GetOrFetchAsync("UserHasPublishedResources", () => this.HasPublishedResources());
        }

        /// <summary>
        /// The DuplicateResourceAsync.
        /// </summary>
        /// <param name="model">The resourceVersionId<see cref="DuplicateResourceRequestModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DuplicateResourceAsync(DuplicateResourceRequestModel model)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DuplicateResource";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Duplicate failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                var err = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(err);
                string message = "ERROR:\r\n";
                foreach (string detail in apiResponse.ValidationResult.Details)
                {
                    message += detail + "\r\n";
                }

                throw new Exception(message);
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The DuplicateBlocksAsync.
        /// </summary>
        /// <param name="model">The duplicateBlocksRequestModel<see cref="DuplicateBlocksRequestModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DuplicateBlocksAsync(DuplicateBlocksRequestModel model)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DuplicateBlocks";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Duplicate failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                var err = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(err);
                string message = "ERROR:\r\n";
                foreach (string detail in apiResponse.ValidationResult.Details)
                {
                    message += detail + "\r\n";
                }

                throw new Exception(message);
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The GetMyContributionsAsync.
        /// </summary>
        /// <param name="model">The myContributionsRequestViewModel<see cref="MyContributionsRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{IEnumerable{MyContributionsBasicDetailsViewModel}}"/>.</returns>
        public async Task<IEnumerable<MyContributionsBasicDetailsViewModel>> GetMyContributionsAsync(MyContributionsRequestViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var response = await client.PostAsync("Resource/GetMyContributions", stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<MyContributionsBasicDetailsViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            throw new Exception("Unable to fetch the requested resources!");
        }

        /// <summary>
        /// Get All published resources id.
        /// </summary>
        /// <returns>The <see cref="T:Task{IEnumerable{int}}"/>.</returns>
        public async Task<IEnumerable<int>> GetAllPublishedResourceAsync()
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var response = await client.GetAsync("Resource/GetAllPublishedResource").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<int>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            throw new Exception("Unable to fetch all published resources!");
        }

        /// <summary>
        /// The GetResourceVersionValidationResultAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionValidationResultViewModel}"/>.</returns>
        public async Task<ResourceVersionValidationResultViewModel> GetResourceVersionValidationResultAsync(int resourceVersionId)
        {
            ResourceVersionValidationResultViewModel viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

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

            var client = await this.OpenApiHttpClient.GetClientAsync();

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
        /// The RevertToDraft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> RevertToDraft(int resourceVersionId)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(new { resourceVersionId });
            var stringContent = new StringContent(resourceVersionId.ToString(), Encoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

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
        /// The create resource version provider.
        /// </summary>
        /// <param name="model">The model<see cref="ResourceVersionProviderViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<LearningHubValidationResult> CreateResourceVersionProviderAsync(ResourceVersionProviderViewModel model)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/AddResourceProvider";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("Add resource provider failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                var err = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(err);
                string message = "ERROR:\r\n";
                foreach (string detail in apiResponse.ValidationResult.Details)
                {
                    message += detail + "\r\n";
                }

                throw new Exception(message);
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// Creates resource version validation results corresponding to the value in the corresponding input view model.
        /// </summary>
        /// <param name="validationResultViewModel">Details of the validation results.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateResourceVersionValidationResultAsync(ResourceVersionValidationResultViewModel validationResultViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(validationResultViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/CreateResourceVersionValidationResult";
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
        }

        /// <summary>
        /// The delete resource version provider.
        /// </summary>
        /// <param name="model">The model<see cref="ResourceVersionProviderViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteResourceVersionProviderAsync(ResourceVersionProviderViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DeleteResourceProvider";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete resource provider failed!");
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
        /// Delete all resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteAllResourceVersionProviderAsync(int resourceVersionId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { }));
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = $"Resource/DeleteAllResourceProvider/{resourceVersionId}";
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete all resource provider failed!");
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
        /// The GetObsoleteResourceFile.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="deletedResource">.</param>
        /// <returns>The <see cref="T:Task{List{FileTypeViewModel}}"/>.</returns>
        public async Task<List<string>> GetObsoleteResourceFile(int resourceVersionId, bool deletedResource = false)
        {
            List<string> filePaths = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/GetObsoleteResourceFile/{resourceVersionId}/{deletedResource}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                filePaths = JsonConvert.DeserializeObject<List<string>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return filePaths;
        }

        /// <summary>
        /// Check if the user has published resources.
        /// </summary>
        /// <returns>The bool.</returns>
        /// <exception cref="Exception"></exception>
        private async Task<bool> HasPublishedResources()
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/HasPublishedResources";
            var response = await client.GetAsync(request).ConfigureAwait(false);
            bool hasResources = false;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                hasResources = bool.Parse(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return hasResources;
        }
    }
}
