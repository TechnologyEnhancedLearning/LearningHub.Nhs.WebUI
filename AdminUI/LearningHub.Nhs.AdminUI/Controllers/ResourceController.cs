// <copyright file="ResourceController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.AdminUI.Helpers;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement;

    /// <summary>
    /// Defines the <see cref="ResourceController" />.
    /// </summary>
    public class ResourceController : BaseController
    {
        /// <summary>
        /// Defines the _config.
        /// </summary>
        private readonly WebSettings config;

        /// <summary>
        /// Defines the websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Defines the featureManager.
        /// </summary>
        private readonly IFeatureManager featureManager;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private ILogger<HomeController> logger;

        /// <summary>
        /// Defines the _pageSize.
        /// </summary>
        private int pageSize = 20;

        /// <summary>
        /// Defines the _resourceService.
        /// </summary>
        private IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="config">The config<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{HomeController}"/>.</param>
        /// <param name="resourceService">The resourceService<see cref="IResourceService"/>.</param>
        /// /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="featureManager">The featureManager<see cref="IFeatureManager"/>.</param>
        public ResourceController(
            IWebHostEnvironment hostingEnvironment,
            IOptions<WebSettings> config,
            ILogger<HomeController> logger,
            IResourceService resourceService,
            IOptions<WebSettings> websettings,
            IFeatureManager featureManager)
        : base(hostingEnvironment)
        {
            this.logger = logger;
            this.websettings = websettings;
            this.config = config.Value;
            this.resourceService = resourceService;
            this.featureManager = featureManager;
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var resource = await this.resourceService.GetResourceVersionExtendedViewModelAsync(id);
            return this.View(resource);
        }

        /// <summary>
        /// The GetEvents.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> GetEvents(int resourceVersionId)
        {
            var vm = await this.resourceService.GetResourceVersionEventsAsync(resourceVersionId);

            return this.PartialView("_Events", vm);
        }

        /// <summary>
        /// The GetVersionHistory.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> GetVersionHistory(int resourceId)
        {
            var vm = await this.resourceService.GetResourceVersionsAsync(resourceId);

            return this.PartialView("_History", vm);
        }

        /// <summary>
        /// The GetValidationResults.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> GetValidationResults(int resourceVersionId)
        {
            var vm = await this.resourceService.GetResourceVersionValidationResultAsync(resourceVersionId);

            return this.PartialView("_ValidationResults", vm);
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> Index()
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.pageSize;
            pagingRequestModel.SortColumn = "VersionId";
            pagingRequestModel.SortDirection = "D";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();

            var model = new TablePagingViewModel<ResourceAdminSearchResultViewModel>
            {
                Results = await this.resourceService.GetResourceAdminSearchPageAsync(pagingRequestModel),
                SortColumn = "VersionId",
                SortDirection = "D",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = this.pageSize,
                TotalItems = model.Results != null ? model.Results.TotalItemCount : 0,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results != null ? model.Results.TotalItemCount : 0,
                DisplayedCount = model.Results != null ? model.Results.Items.Count() : 0,
                DefaultPageSize = this.pageSize,
                FilterCount = 0,
                CreateRequired = false,
            };

            this.ViewBag.Options = FilterOptions();

            return this.View(model);
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            requestModel.Filter.ForEach(filter =>
            {
                switch (filter.Column.ToLower())
                {
                    case "type":
                        if (!Enum.TryParse<ResourceTypeEnum>(filter.Value, out var resourceType))
                        {
                            filter.Value = string.Empty;
                        }

                        break;
                    case "status":
                        if (!Enum.TryParse<VersionStatusEnum>(filter.Value, out var statusType))
                        {
                            filter.Value = string.Empty;
                        }

                        break;
                }
            });
            requestModel.Sanitize();
            requestModel.PageSize = this.pageSize;

            var model = new TablePagingViewModel<ResourceAdminSearchResultViewModel>
            {
                Results = await this.resourceService.GetResourceAdminSearchPageAsync(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.pageSize,
                TotalItems = model.Results.TotalItemCount,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.pageSize,
                FilterCount = model.Filter.Count(),
                CreateRequired = false,
            };

            this.ViewBag.Options = FilterOptions();

            return this.View(model);
        }

        /// <summary>
        /// The RevertToDraft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> RevertToDraft(int resourceVersionId)
        {
            var vr = await this.resourceService.RevertToDraft(resourceVersionId);

            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                    details = $"Successfully reverted resource version id={resourceVersionId} to Draft status",
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error reverting resource version id={resourceVersionId} to Draft status",
                });
            }
        }

        /// <summary>
        /// The TransferResourceOwnership.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <param name="newResourceOwner">The newResourceOwner<see cref="string"/>.</param>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> TransferResourceOwnership(int resourceId, string newResourceOwner, int resourceVersionId)
        {
            var vr = await this.resourceService.TransferResourceOwnership(resourceId, newResourceOwner, resourceVersionId);
            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                    details = $"Successfully transferred resource to '{newResourceOwner}'",
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error transferring resource to '{newResourceOwner}'",
                });
            }
        }

        /// <summary>
        /// The Unpublish.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="details">The details<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Unpublish(int resourceVersionId, string details)
        {
            var vr = await this.resourceService.UnpublishResourceVersionAsync(resourceVersionId, details);
            await this.resourceService.CreateResourceVersionEvent(resourceVersionId, Nhs.Models.Enums.ResourceVersionEventTypeEnum.UnpublishedByAdmin, "Unpublish using Admin UI", 0);

            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                    details = $"Successfully unpublished resource version id={resourceVersionId}",
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error unpublishing resource version id={resourceVersionId}",
                });
            }
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
        /// The GetAddAVFlag.
        /// </summary>
        /// <returns> Return AV Flag.</returns>
        [Route("Resource/GetAddAVFlag")]
        [HttpGet("GetAddAVFlag")]
        public bool GetAddAVFlag() => this.featureManager.IsEnabledAsync(FeatureFlags.AddAudioVideo).Result;

        /// <summary>
        /// The GetDisplayAVFlag.
        /// </summary>
        /// <returns> Return display AV flag.</returns>
        [Route("Resource/GetDisplayAVFlag")]
        [HttpGet("GetDisplayAVFlag")]
        public bool GetDisplayAVFlag() => this.featureManager.IsEnabledAsync(FeatureFlags.DisplayAudioVideo).Result;

        private static List<PagingOptionPair> FilterOptions()
        {
            List<PagingOptionPair> options = new List<PagingOptionPair>();
            List<PagingOptionPair> resourceTypes = new List<PagingOptionPair>();

            foreach (var resourceType in Enum.GetNames<ResourceTypeEnum>())
            {
                resourceTypes.Add(new PagingOptionPair() { Type = "Type", Name = resourceType });
            }

            options = resourceTypes.OrderBy(o => o.Name).ToList();

            List<PagingOptionPair> statuses = new List<PagingOptionPair>();
            foreach (var status in Enum.GetNames<VersionStatusEnum>())
            {
                statuses.Add(new PagingOptionPair() { Type = "Status", Name = status });
            }

            options.AddRange(statuses);
            return options;
        }
    }
}
