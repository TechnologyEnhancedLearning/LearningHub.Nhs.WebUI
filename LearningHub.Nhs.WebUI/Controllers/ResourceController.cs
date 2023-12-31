﻿// <copyright file="ResourceController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
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
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

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
            IHierarchyService hierarchyService)
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

            if (resourceReferenceId == 0)
            {
                return this.Redirect("/Home/Error");
            }

            var resource = await this.resourceService.GetItemByIdAsync(resourceReferenceId);

            if (resource.Id == 0 || (resource.Catalogue != null && resource.Catalogue.Hidden))
            {
                this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
                return this.View("Unavailable");
            }

            if (resource.VersionStatusEnum == VersionStatusEnum.Unpublished && !resource.DisplayForContributor)
            {
                return this.RedirectToAction("unpublished");
            }

            var catalogueAccessRequest = await this.catalogueService.GetLatestCatalogueAccessRequestAsync(resource.Catalogue.NodeId);

            ScormContentDetailsViewModel scormContentDetails = null;
            if (resource.ResourceTypeEnum == ResourceTypeEnum.Scorm)
            {
                scormContentDetails = await this.resourceService.GetScormContentDetailsAsync(resource.ResourceVersionId);
            }

            var hasCatalogueAccess = false;
            if (resource.Catalogue.RestrictedAccess && this.User.Identity.IsAuthenticated)
            {
                var userGroups = await this.userGroupService.GetRoleUserGroupDetailForUserAsync(this.CurrentUserId);

                hasCatalogueAccess = userGroups.Any(x => x.CatalogueNodeId == resource.Catalogue.NodeId &&
                    (x.RoleEnum == RoleEnum.LocalAdmin || x.RoleEnum == RoleEnum.Editor || x.RoleEnum == RoleEnum.Reader));
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
            if ((resource.ResourceTypeEnum == ResourceTypeEnum.Article || resource.ResourceTypeEnum == ResourceTypeEnum.Image) && ((resource.SensitiveContent && acceptSensitiveContent.HasValue && acceptSensitiveContent.Value) || !resource.SensitiveContent) && canAccessResource)
            {
                var activity = new CreateResourceActivityViewModel()
                {
                    ResourceVersionId = resource.ResourceVersionId,
                    NodePathId = resource.NodePathId,
                    ActivityStart = DateTime.UtcNow, // TODO: What about user's timezone offset when Javascript is disabled? Needs JavaScript.
                    ActivityStatus = ActivityStatusEnum.Launched,
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
                ScormContentDetails = scormContentDetails,
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
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="resourceTitle">The resourceTitle.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Authorize]
        [Route("Resource/EditConfirm/{resourceId}/{resourceReferenceId}/{resourceTitle}")]
        public IActionResult EditConfirm(int resourceId, int resourceReferenceId, string resourceTitle)
        {
            return this.View("EditConfirm", new ResourceEditConfirmViewModel { ResourceId = resourceId, ResourceReferenceId = resourceReferenceId, ResourceTitle = resourceTitle });
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
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="resourceType">The resourceType.</param>
        /// <param name="catalogueNodeVersionId"> The catalogueNodeVersionId.</param>
        /// <param name="resourceTitle">The resourceTitle.</param>
        /// <param name="scormEsrLinkType">The SCORM ESR link type.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Authorize]
        [Route("Resource/UnpublishConfirm/{resourceVersionId}/{resourceReferenceId}/{resourceType}/{catalogueNodeVersionId}/{resourceTitle}/{scormEsrLinkType?}")]
        public IActionResult UnpublishConfirm(int resourceVersionId, int resourceReferenceId, int resourceType, int catalogueNodeVersionId, string resourceTitle, int scormEsrLinkType)
        {
            return this.View("UnpublishConfirm", new ResourceUnpublishConfirmViewModel
            {
                ResourceVersionId = resourceVersionId,
                ResourceReferenceId = resourceReferenceId,
                ResourceType = (ResourceTypeEnum)resourceType,
                CatalogueNodeVersionId = catalogueNodeVersionId,
                ResourceTitle = resourceTitle,
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
            var validationResult = await this.resourceService.UnpublishResourceVersionAsync(viewModel.ResourceVersionId);
            var catalogue = await this.catalogueService.GetCatalogueAsync(viewModel.CatalogueNodeVersionId);

            if (validationResult.IsValid)
            {
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
    }
}
