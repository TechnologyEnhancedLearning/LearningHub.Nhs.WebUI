// <copyright file="ReleaseController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.Maintenance;
    using LearningHub.Nhs.Models.Paging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ReleaseController" />.
    /// </summary>
    public class ReleaseController : BaseController
    {
        private readonly ICacheService cacheService;
        private readonly IUserGroupService userGroupService;
        private readonly IEventService eventService;
        private readonly IInternalSystemService internalSystemService;
        private readonly IOptions<WebSettings> config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="userGroupService">The userGroupService.</param>
        /// <param name="eventService">The eventService.</param>
        /// <param name="internalSystemService">The internalSystemService.</param>
        /// <param name="config">The config.</param>
        public ReleaseController(
            IWebHostEnvironment hostingEnvironment,
            ICacheService cacheService,
            IUserGroupService userGroupService,
            IEventService eventService,
            IInternalSystemService internalSystemService,
            IOptions<WebSettings> config)
            : base(hostingEnvironment)
        {
            this.cacheService = cacheService;
            this.userGroupService = userGroupService;
            this.eventService = eventService;
            this.internalSystemService = internalSystemService;
            this.config = config;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> Index()
        {
            var internalSystems = await this.internalSystemService.GetAllAsync();

            this.ViewBag.lhOffline = internalSystems.Single(x => x.Id == (int)InternalSystemType.LearningHub).IsOffline;
            this.ViewBag.canTakeOffline = await this.userGroupService.UserHasPermissionAsync("Take_LH_Offline");

            var internalSystemQueues = internalSystems.Skip(1).ToList();

            var model = new TablePagingViewModel<InternalSystemViewModel>
            {
                Results = new PagedResultSet<InternalSystemViewModel> { Items = internalSystemQueues, TotalItemCount = internalSystemQueues.Count },
                SortColumn = "Id",
                SortDirection = "A",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results.Items.Any(),
                PageSize = this.config.Value.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.config.Value.DefaultPageSize,
                FilterCount = 0,
                CreateRequired = false,
            };

            return this.View(model);
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="id">
        /// The status.
        /// </param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> ToggleOfflineStatus(int id)
        {
            if (await this.userGroupService.UserHasPermissionAsync("Take_LH_Offline"))
            {
                var internalSystem = await this.internalSystemService.ToggleOfflineStatus(id);
                var currentUserId = this.User.Identity.GetCurrentUserId();
                var currentUser = this.User.Identity.GetCurrentName();
                var eventEntity = new Event();
                eventEntity.EventTypeEnum = EventTypeEnum.ChangeOfflineStatus;
                eventEntity.JsonData = JsonConvert.SerializeObject(new { UserId = currentUserId, UserName = currentUser, Name = internalSystem.Name, OfflineAction = internalSystem.IsOffline });
                eventEntity.UserId = currentUserId;
                await this.eventService.Create(eventEntity);
            }

            return this.RedirectToAction("Index");
        }
    }
}
