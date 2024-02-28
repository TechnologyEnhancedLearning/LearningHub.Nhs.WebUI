namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="ContributeResourceController" />.
    /// </summary>
    [Authorize]
    [Route("contribute-resource")]
    public class ContributeResourceController : BaseController
    {
        private static readonly HashSet<ResourceTypeEnum> PreviewableResourceTypes = new HashSet<ResourceTypeEnum>()
        {
            ResourceTypeEnum.Case,
            ResourceTypeEnum.Assessment,
        };

        private readonly IResourceService resourceService;
        private readonly ICatalogueService catalogueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContributeResourceController"/> class.
        /// </summary>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="catalogueService">Catalogue service.</param>
        /// <param name="hostingEnv">Hosting environment.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        public ContributeResourceController(
            IResourceService resourceService,
            ICatalogueService catalogueService,
            IWebHostEnvironment hostingEnv,
            IHttpClientFactory httpClientFactory,
            ILogger<ContributeResourceController> logger,
            IOptions<Settings> settings)
            : base(hostingEnv, httpClientFactory, logger, settings.Value)
        {
            this.resourceService = resourceService;
            this.catalogueService = catalogueService;
        }

        /// <summary>
        /// The Select Resource action.
        /// Creates a new resource of the specified type (in the specified catalogue (if applicable) and redirects the user to edit that resource.
        /// </summary>
        /// <returns>A <see cref="IActionResult"/>.</returns>
        [HttpGet("select-resource")]
        public IActionResult SelectResource()
        {
            var errorResult = this.RedirectIfUserUnauthorized();
            if (errorResult != null)
            {
                return errorResult;
            }

            return this.View("ContributeResource");
        }

        /// <summary>
        /// The Edit action.
        /// Edits an existing resource.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id to edit.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("edit/{resourceVersionId}")]
        public async Task<IActionResult> Edit(int resourceVersionId)
        {
            var errorResult = this.RedirectIfUserUnauthorized();
            if (errorResult != null)
            {
                return errorResult;
            }

            /*
             * Validate resource with this ResourceVersionID exists
             * Validate this resource is editable by the current user
             * Validate the version specified is editable (e.g. can we only edit the current version)
             */
            ResourceDetailViewModel resourceVersion = await this.resourceService.GetResourceVersionAsync(resourceVersionId);
            if (resourceVersion != null &&
                (resourceVersion.CreateUserId == this.CurrentUserId || await this.UserCanEditCatalogue((int)resourceVersion.ResourceCatalogueId)) &&
                resourceVersion.VersionStatusEnum == VersionStatusEnum.Draft)
            {
                // Perhaps surprisingly, we don't pass the resourceVersionId to the view
                // This is because the Vue.js Router infers the resourceVersionId from the URL
                // See contributeResourceRouter.js
                return this.View("ContributeResource");
            }
            else
            {
                return this.Forbid();
            }
        }

        /// <summary>
        /// The Preview action.
        /// Preview an existing resource.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id to preview.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("preview/{resourceVersionId}")]
        public async Task<IActionResult> Preview(int resourceVersionId)
        {
            var errorResult = this.RedirectIfUserUnauthorized();
            if (errorResult != null)
            {
                return errorResult;
            }

            /*
             * Validate resource with this ResourceVersionID exists
             * Validate this resource is editable by the current user
             * Validate the version specified is editable (e.g. can we only edit the current version)
             * Validate this resource is previewable
             */
            ResourceDetailViewModel resourceVersion = await this.resourceService.GetResourceVersionAsync(resourceVersionId);
            if (resourceVersion != null &&
                (resourceVersion.CreateUserId == this.CurrentUserId || await this.UserCanEditCatalogue((int)resourceVersion.ResourceCatalogueId)) &&
                resourceVersion.VersionStatusEnum == VersionStatusEnum.Draft &&
                PreviewableResourceTypes.Contains(resourceVersion.ResourceType))
            {
                this.ViewBag.KeepUserSessionAliveIntervalSeconds = Convert.ToInt32(this.Settings.KeepUserSessionAliveIntervalMins) * 60000;

                // Perhaps surprisingly, we don't pass the resourceVersionId to the view
                // This is because the Vue.js Router infers the resourceVersionId from the URL
                // See contributeResourceRouter.js
                return this.View("ContributeResource");
            }
            else
            {
                return this.Forbid();
            }
        }

        private async Task<bool> UserCanEditCatalogue(int catalogueId)
        {
            return await this.catalogueService.CanCurrentUserEditCatalogue(catalogueId);
        }

        private RedirectToActionResult RedirectIfUserUnauthorized()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            if (this.User.IsInRole("ReadOnly") || this.User.IsInRole("BasicUser"))
            {
                return this.RedirectToAction("AccessDenied", "Home");
            }

            return null;
        }
    }
}