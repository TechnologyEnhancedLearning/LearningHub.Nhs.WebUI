namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using LearningHub.Nhs.WebUI.Models.Resource;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement;

    /// <summary>
    /// Defines the <see cref="ResourceController" />.
    /// </summary>
    [ServiceFilter(typeof(LoginWizardFilter))]
    public class ResourceController : BaseController
    {
        private readonly IAzureMediaService azureMediaService;
        private readonly IResourceService resourceService;
        private readonly IRatingService ratingService;
        private readonly IUserService userService;
        private readonly IUserGroupService userGroupService;
        private readonly IActivityService activityService;
        private readonly ICatalogueService catalogueService;
        private readonly IHierarchyService hierarchyService;
        private readonly IMyLearningService myLearningService;
        private readonly IFileService fileService;
        private readonly ICacheService cacheService;
        private readonly IFeatureManager featureManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="azureMediaService">Azure media services.</param>
        /// <param name="resourceService">Resource services.</param>
        /// <param name="ratingService">rating services.</param>
        /// <param name="userService">user services.</param>
        /// <param name="userGroupService">The UserGroupService.</param>
        /// <param name="activityService">The ActivityService.</param>
        /// <param name="catalogueService">The catalogueService.</param>
        /// <param name="myLearningService">The myLearningService.</param>
        /// <param name="hierarchyService">The hierarchyService.</param>
        /// <param name="fileService">The fileService.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="featureManager"> The Feature flag manager.</param>
        public ResourceController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<ResourceController> logger,
            IOptions<Settings> settings,
            IHttpClientFactory httpClientFactory,
            IAzureMediaService azureMediaService,
            IResourceService resourceService,
            IRatingService ratingService,
            IUserService userService,
            IUserGroupService userGroupService,
            IActivityService activityService,
            ICatalogueService catalogueService,
            IMyLearningService myLearningService,
            IHierarchyService hierarchyService,
            IFileService fileService,
            ICacheService cacheService,
            IFeatureManager featureManager)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.azureMediaService = azureMediaService;
            this.resourceService = resourceService;
            this.ratingService = ratingService;
            this.userService = userService;
            this.userGroupService = userGroupService;
            this.activityService = activityService;
            this.catalogueService = catalogueService;
            this.hierarchyService = hierarchyService;
            this.myLearningService = myLearningService;
            this.fileService = fileService;
            this.cacheService = cacheService;
            this.featureManager = featureManager;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="acceptSensitiveContent">The acceptSensitiveContent.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [ServiceFilter(typeof(SsoLoginFilterAttribute))]
        [Route("Resource/{resourceReferenceId}")]
        [Route("Resource/{resourceReferenceId}/Item")]
        [Route("Resource/{*path}")]
        public async Task<IActionResult> Index(int resourceReferenceId, bool? acceptSensitiveContent)
        {
            this.ViewBag.UserAuthenticated = this.User.Identity.IsAuthenticated;
            this.ViewBag.UnableToViewELearningResources = this.Settings.SupportUrls.UnableToViewELearningResources;
            this.ViewBag.MediaActivityPlayingEventIntervalSeconds = this.Settings.MediaActivityPlayingEventIntervalSeconds;
            this.ViewBag.KeepUserSessionAliveIntervalSeconds = Convert.ToInt32(this.Settings.KeepUserSessionAliveIntervalMins) * 60000;
            this.ViewBag.SupportUrl = this.Settings.SupportUrls.SupportForm;
            var displayAVResourceFlag = Task.Run(() => this.featureManager.IsEnabledAsync(FeatureFlags.DisplayAudioVideoResource)).Result;
            this.ViewBag.DisplayAVResourceFlag = displayAVResourceFlag;

            if (resourceReferenceId == 0)
            {
                return this.Redirect("/Home/Error");
            }

            var resource = await this.resourceService.GetItemByIdAsync(resourceReferenceId);

            if ((resource == null && resource.Id == 0) || (resource.Catalogue != null && resource.Catalogue.Hidden))
            {
                this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
                return this.View("Unavailable");
            }

            if (resource.VersionStatusEnum == VersionStatusEnum.Unpublished && !resource.DisplayForContributor)
            {
                return this.RedirectToAction("unpublished");
            }

            var catalogueAccessRequest = await this.catalogueService.GetLatestCatalogueAccessRequestAsync(resource.Catalogue.NodeId);

            ExternalContentDetailsViewModel externalContentDetails = null;
            if (resource.ResourceTypeEnum == ResourceTypeEnum.Scorm
                || resource.ResourceTypeEnum == ResourceTypeEnum.GenericFile
                || resource.ResourceTypeEnum == ResourceTypeEnum.Html)
            {
                externalContentDetails = await this.resourceService.GetExternalContentDetailsAsync(resource.ResourceVersionId);
            }

            var hasCatalogueAccess = false;
            if (resource.Catalogue.RestrictedAccess && this.User.Identity.IsAuthenticated)
            {
                var userGroups = await this.userGroupService.GetRoleUserGroupDetailAsync();

                hasCatalogueAccess = userGroups.Any(x => x.CatalogueNodeId == resource.Catalogue.NodeId &&
                    (x.RoleEnum == RoleEnum.LocalAdmin || x.RoleEnum == RoleEnum.Editor || x.RoleEnum == RoleEnum.Reader)) || this.User.IsInRole("Administrator");
            }
            else if (!resource.Catalogue.RestrictedAccess)
            {
                hasCatalogueAccess = true;
            }

            var ratingSummary = await this.ratingService.GetRatingSummaryAsync(resource.ResourceVersionId);
            var resourceRating = new ResourceRatingViewModel
            {
                RatingSummary = ratingSummary,
                ResourceReferenceId = resourceReferenceId,
                ResourceVersionId = resource.ResourceVersionId,
                HasCatalogueAccess = hasCatalogueAccess,
            };

            // If user has accepted the sensitive content warning, record it in database.
            if (acceptSensitiveContent.HasValue && acceptSensitiveContent.Value)
            {
                await this.resourceService.AcceptSensitiveContentAsync(resource.ResourceVersionId);
                resource.SensitiveContent = false;
            }

            bool canAccessResource = this.User.Identity.IsAuthenticated && hasCatalogueAccess;
            if (canAccessResource && resource.ResourceAccessibilityEnum == ResourceAccessibilityEnum.FullAccess)
            {
                canAccessResource = !this.User.IsInRole("BasicUser");
            }

            // For article/image resources, immediately record the resource activity for this user.
            if ((resource.ResourceTypeEnum == ResourceTypeEnum.Article || resource.ResourceTypeEnum == ResourceTypeEnum.Image) && (!resource.SensitiveContent) && this.User.Identity.IsAuthenticated)
            {
                var activity = new CreateResourceActivityViewModel()
                {
                    ResourceVersionId = resource.ResourceVersionId,
                    NodePathId = resource.NodePathId,
                    ActivityStart = DateTime.UtcNow, // TODO: What about user's timezone offset when Javascript is disabled? Needs JavaScript.
                    ActivityStatus = ActivityStatusEnum.Completed,
                };
                await this.activityService.CreateResourceActivityAsync(activity);
            }

            // Get node path data for breadcrumbs.
            var nodePathNodes = await this.hierarchyService.GetNodePathNodes(resource.NodePathId);
            bool userHasCertificate = false;
            if (resource.CertificateEnabled.GetValueOrDefault(false) && this.User.Identity.IsAuthenticated)
            {
                var certDetails = await this.myLearningService.GetResourceCertificateDetails(resourceReferenceId);
                if (certDetails.Item2 != null && certDetails.Item2.IsCurrentResourceVersion)
                {
                    var activityDetailedItemViewModel = new ActivityDetailedItemViewModel(certDetails.Item2);
                    if (activityDetailedItemViewModel != null && ViewActivityHelper.CanDownloadCertificate(activityDetailedItemViewModel))
                    {
                        userHasCertificate = true;
                    }
                }
            }

            var viewModel = new ResourceIndexViewModel()
            {
                ResourceReferenceId = resourceReferenceId,
                ResourceItem = resource,
                ResourceRating = resourceRating,
                ExternalContentDetails = externalContentDetails,
                HasCatalogueAccess = hasCatalogueAccess,
                UserHasCertificate = userHasCertificate,
                CatalogueAccessRequest = catalogueAccessRequest,
                NodePathNodes = nodePathNodes,
            };

            return this.View("Resource", viewModel);
        }

        /// <summary>
        /// The Unpublished.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Resource/{resourceVersionId}/ValidationErrors")]
        public async Task<IActionResult> GetResourceVersionValidationResult(int resourceVersionId)
        {
            this.ViewBag.UserAuthenticated = this.User.Identity.IsAuthenticated;
            this.ViewBag.ValidationErrorsHelpUrl = this.Settings.SupportUrls.ValidationErrorsHelpUrl;

            if (resourceVersionId == 0)
            {
                return this.Redirect("/Home/Error");
            }

            var resourceVersionValidationResult = await this.resourceService.GetResourceVersionValidationResultAsync(resourceVersionId);

            if ((this.User.Identity.IsAuthenticated && this.CurrentUserId != resourceVersionValidationResult.ResourceVersionCreateUserId) || resourceVersionValidationResult.VersionStatus != VersionStatusEnum.FailedToPublish)
            {
                return this.Redirect("/AccessDenied");
            }

            var validationResultViewModel = new ResourceValidationResultViewModel()
            {
                HasAccess = this.CurrentUserId == resourceVersionValidationResult?.ResourceVersionCreateUserId,
                ResourceVersionValidationResult = resourceVersionValidationResult,
            };

            return this.View("ValidationErrors", validationResultViewModel);
        }

        /// <summary>
        /// The RevertToDraft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="hasPublishedVersion">The hasPublishedVersion<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("Resource/RevertToDraft")]
        public async Task<IActionResult> RevertToDraft(int resourceVersionId, bool hasPublishedVersion)
        {
            if (resourceVersionId == 0)
            {
                return this.Redirect("/Home/Error");
            }

            var vr = await this.resourceService.RevertToDraft(resourceVersionId);

            if (vr.IsValid)
            {
                return hasPublishedVersion ? this.Redirect("/my-contributions/published") : this.Redirect("/my-contributions/draft");
            }
            else
            {
                return this.Redirect("/Home/Error");
            }
        }

        /// <summary>
        /// The Unpublished.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Resource/unpublished")]
        public IActionResult Unpublished()
        {
            this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
            return this.View();
        }

        /// <summary>
        /// The Too Many Attempts.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Resource/TooManyAttempts")]
        public IActionResult TooManyAttempts()
        {
            this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
            return this.View();
        }

        /// <summary>
        /// Set the resource rating.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Authorize]
        [Route("Resource/RateResource")]
        public async Task<IActionResult> RateResource(ResourceRatingViewModel model)
        {
            RatingViewModel ratingViewModel = new RatingViewModel();

            if (model.RatingSummary == null || model.RatingSummary.UserRating <= 0)
            {
                return this.Redirect($"/Resource/{model.ResourceReferenceId}");
            }

            var userEmployment = await this.userService.GetPrimaryUserEmploymentForUser(this.CurrentUserId);
            if (userEmployment != null)
            {
                ratingViewModel.JobRoleId = userEmployment.JobRoleId.GetValueOrDefault();
                ratingViewModel.LocationId = userEmployment.LocationId;
            }

            ratingViewModel.EntityVersionId = model.ResourceVersionId;
            ratingViewModel.Rating = model.RatingSummary.UserRating;

            LearningHubValidationResult validationResult;
            if (model.RatingSummary.UserHasAlreadyRated)
            {
                validationResult = await this.ratingService.UpdateRatingAsync(ratingViewModel);
            }
            else
            {
                validationResult = await this.ratingService.CreateRatingAsync(ratingViewModel);
            }

            if (validationResult.IsValid)
            {
                return this.Redirect($"/Resource/{model.ResourceReferenceId}");
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "Failed to set rating");
                return this.View(model);
            }
        }

        /// <summary>
        /// Ask user to confirm that they wish to edit a published resource.
        /// </summary>
        /// <param name="viewModel">The ResourceIndexViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Authorize]
        [Route("Resource/EditConfirm")]
        public IActionResult EditConfirm(ResourceIndexViewModel viewModel)
        {
            return this.View("EditConfirm", new ResourceEditConfirmViewModel { ResourceId = viewModel.ResourceItem.ResourceId, ResourceReferenceId = viewModel.ResourceReferenceId, ResourceTitle = viewModel.ResourceItem.Title });
        }

        /// <summary>
        /// User confirms that they wish to edit a published resource.
        /// </summary>
        /// <param name="viewModel">The ResourceEditConfirmViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize]
        [Route("Resource/EditConfirmPost")]
        public IActionResult EditConfirm(ResourceEditConfirmViewModel viewModel)
        {
            return this.Redirect($"/Contribute/create-version/{viewModel.ResourceId}");
        }

        /// <summary>
        /// Ask user to confirm that they wish to unpublish a published resource.
        /// </summary>
        /// <param name="viewModel">The ResourceIndexViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Authorize]
        [Route("Resource/UnpublishConfirm")]
        public IActionResult UnpublishConfirm(ResourceIndexViewModel viewModel)
        {
            int scormEsrLinkType = viewModel.ResourceItem.ResourceTypeEnum == ResourceTypeEnum.Scorm || viewModel.ResourceItem.ResourceTypeEnum == ResourceTypeEnum.GenericFile ? (int)viewModel.ExternalContentDetails.EsrLinkType : 0;
            return this.View("UnpublishConfirm", new ResourceUnpublishConfirmViewModel
            {
                ResourceVersionId = viewModel.ResourceItem.ResourceVersionId,
                ResourceReferenceId = viewModel.ResourceReferenceId,
                ResourceType = (ResourceTypeEnum)(int)viewModel.ResourceItem.ResourceTypeEnum,
                CatalogueNodeVersionId = viewModel.ResourceItem.Catalogue.CatalogueNodeVersionId,
                ResourceTitle = viewModel.ResourceItem.Title,
                ScormEsrLinkType = (EsrLinkType)scormEsrLinkType,
            });
        }

        /// <summary>
        /// User confirms that they wish to edit a published resource.
        /// </summary>
        /// <param name="viewModel">The ResourceEditConfirmViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize]
        [Route("Resource/UnpublishConfirmPost")]
        public async Task<IActionResult> UnpublishConfirm(ResourceUnpublishConfirmViewModel viewModel)
        {
            var associatedFile = await this.resourceService.GetResourceVersionExtendedAsync(viewModel.ResourceVersionId);
            var validationResult = await this.resourceService.UnpublishResourceVersionAsync(viewModel.ResourceVersionId);
            var catalogue = await this.catalogueService.GetCatalogueAsync(viewModel.CatalogueNodeVersionId);

            if (validationResult.IsValid)
            {
                if (associatedFile.ScormDetails != null || associatedFile.HtmlDetails != null)
                {
                    _ = Task.Run(async () => { await this.fileService.PurgeResourceFile(associatedFile, null); });
                }

                if (viewModel.CatalogueNodeVersionId == 1)
                {
                    return this.Redirect("/my-contributions/unpublished");
                }
                else
                {
                    return this.Redirect($"/my-contributions/unpublished/{catalogue.Url}");
                }
            }
            else
            {
                return this.Redirect("/Home/Error");
            }
        }

        /// <summary>
        /// View HTML resource content.
        /// </summary>
        /// <param name="resourceReferenceId">Resource reference id.</param>
        /// <param name="currentResourceVersionId">Resource version id.</param>
        /// <param name="path">Html resource content relative path.</param>
        /// <returns>The file content.</returns>
        [HttpGet]
        [Authorize]
        [Route("resource/html/{resourceReferenceId}/{CurrentResourceVersionId}/{*path}")]
        public async Task<IActionResult> HtmlResourceContent(int resourceReferenceId, int currentResourceVersionId, string path)
        {
            if (resourceReferenceId == 0 || string.IsNullOrWhiteSpace(path))
            {
                return this.Redirect("/Home/Error");
            }

            var userId = this.User.Identity.GetCurrentUserId();
            var cacheKey = $"HtmlContent:{userId}:{resourceReferenceId}";
            var (cacheExists, cacheValue) = await this.cacheService.TryGetAsync<string>(cacheKey);
            var oldresourceVersionId = 0;
            if (cacheExists)
            {
                var cachesplits = cacheValue.Split(":");
                oldresourceVersionId = int.Parse(cachesplits[0]);
            }

            if (!cacheExists || (oldresourceVersionId != currentResourceVersionId))
            {
                var resource = await this.resourceService.GetItemByIdAsync(resourceReferenceId);

                if (resource == null || resource.Id == 0 || (resource.Catalogue != null && resource.Catalogue.Hidden))
                {
                    this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
                    return this.View("Unavailable");
                }

                if (resource.VersionStatusEnum == VersionStatusEnum.Unpublished && !resource.DisplayForContributor)
                {
                    return this.RedirectToAction("unpublished");
                }

                cacheValue = $"{resource.ResourceVersionId}:{resource.NodePathId}:{resource.HtmlDetails.ContentFilePath}";

                await this.cacheService.SetAsync(cacheKey, cacheValue);
            }

            var splits = cacheValue.Split(":");
            var resourceVersionId = int.Parse(splits[0]);
            var nodePathId = int.Parse(splits[1]);
            var contentFilePath = splits[2];

            if (path?.ToLower() == "index.html")
            {
                var activity = new CreateResourceActivityViewModel
                {
                    ResourceVersionId = resourceVersionId,
                    NodePathId = nodePathId,
                    ActivityStart = DateTime.UtcNow, // TODO: What about user's timezone offset when Javascript is disabled? Needs JavaScript.
                    ActivityStatus = ActivityStatusEnum.Completed,
                };
                await this.activityService.CreateResourceActivityAsync(activity);
            }

            if (!new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType))
            {
                contentType = "text/html";
            }

            if (contentType.Contains("video") || contentType.Contains("audio"))
            {
                var stream = await this.fileService.StreamFileAsync(contentFilePath, path);
                if (stream != null)
                {
                    return this.File(stream, contentType, enableRangeProcessing: true);
                }
            }
            else
            {
                var file = await this.fileService.DownloadFileAsync(contentFilePath, path);
                if (file != null)
                {
                    return this.File(file.Content, contentType);
                }
            }

            return this.Ok(this.Content("No file found"));
        }

        /// <summary>
        /// The GetAVUnavailableView.
        /// </summary>
        /// <returns> partial view.  </returns>
        [Route("Resource/GetAVUnavailableView")]
        [HttpGet("GetAVUnavailableView")]
        public IActionResult GetAVUnavailableView()
        {
            return this.PartialView("_AudioVideoUnavailable");
        }

        /// <summary>
        /// The GetContributeAVResourceFlag.
        /// </summary>
        /// <returns> Return Contribute Resource AV Flag.</returns>
        [Route("Resource/GetContributeAVResourceFlag")]
        [HttpGet("GetContributeAVResourceFlag")]
        public bool GetContributeResourceAVFlag() => this.featureManager.IsEnabledAsync(FeatureFlags.ContributeAudioVideoResource).Result;

        /// <summary>
        /// The GetDisplayAVResourceFlag.
        /// </summary>
        /// <returns> Return Display AV Resource Flag.</returns>
        [Route("Resource/GetDisplayAVResourceFlag")]
        [HttpGet("GetDisplayAVResourceFlag")]
        public bool GetDisplayAVResourceFlag() => this.featureManager.IsEnabledAsync(FeatureFlags.DisplayAudioVideoResource).Result;

        /// <summary>
        /// The GetMKPlayerKey.
        /// </summary>
        /// <returns>Mediakind MK Player Key.</returns>
        [Route("Resource/GetMKPlayerKey")]
        [HttpGet("GetMKPlayerKey")]
        public string GetMKPlayerKey() => this.Settings.MediaKindSettings.MKPlayerLicence;
    }
}