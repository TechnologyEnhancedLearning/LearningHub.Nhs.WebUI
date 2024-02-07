// <copyright file="NotificationsController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Paging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="NotificationsController" />.
    /// </summary>
    public class NotificationsController : BaseController
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
        /// Defines the _logger.
        /// </summary>
        private ILogger<HomeController> logger;

        /// <summary>
        /// Defines the _notificationService.
        /// </summary>
        private INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="config">The config<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{HomeController}"/>.</param>
        /// <param name="notificationService">The notificationService<see cref="INotificationService"/>.</param>
        /// /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public NotificationsController(
            IWebHostEnvironment hostingEnvironment,
            IOptions<WebSettings> config,
            ILogger<HomeController> logger,
            INotificationService notificationService,
            IOptions<WebSettings> websettings)
        : base(hostingEnvironment)
        {
            this.logger = logger;
            this.websettings = websettings;
            this.config = config.Value;
            this.notificationService = notificationService;
        }

        /// <summary>
        /// Get notification type text.
        /// </summary>
        /// <param name="notificationType">The NotificationTypeEnum.</param>
        /// <returns>Notification type text.</returns>
        public static string GetNotificationTypeText(NotificationTypeEnum notificationType)
        {
            return notificationType switch
            {
                NotificationTypeEnum.SystemUpdate => "System Update",
                NotificationTypeEnum.SystemRelease => "Service Release",
                NotificationTypeEnum.ActionRequired => "Action Required",
                NotificationTypeEnum.ResourcePublished => "Resource Published",
                NotificationTypeEnum.ResourceRated => "Resource Rated",
                NotificationTypeEnum.UserPermission => "User Permission",
                NotificationTypeEnum.PublishFailed => "Publish Failed",
                _ => notificationType.ToString(),
            };
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            this.ViewBag.source = "list";
            var notification = new NotificationViewModel();
            notification.StartDate = DateTime.Now.Date;
            notification.EndDate = DateTime.Now.Date.AddDays(7);
            return this.View("CreateEdit", notification);
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var notification = await this.notificationService.GetIdAsync(id);
            return this.View(notification);
        }

        /// <summary>
        /// Copy existing notification.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Copy(int id)
        {
            var notificationId = await this.notificationService.CopyAsync(id);
            return this.RedirectToAction("Edit", new { id = notificationId, copy = true, source = "list" });
        }

        /// <summary>
        /// Delete a notification.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await this.notificationService.DeleteAsync(id);
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="source">The source<see cref="string"/>.</param>
        /// <param name="copy">The copy<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string source = "", bool copy = false)
        {
            this.ViewBag.source = source;
            this.ViewBag.copy = copy;
            var notification = await this.notificationService.GetIdAsync(id);
            return this.View("CreateEdit", notification);
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="notification">The notification<see cref="NotificationViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(NotificationViewModel notification)
        {
            if (notification.Id == 0)
            {
                notification.Id = await this.notificationService.Create(notification);
            }
            else
            {
                await this.notificationService.Edit(notification);
            }

            return this.RedirectToAction("Details", new { id = notification.Id });
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> Index()
        {
            return await this.GetNotifications(
                new PagingRequestModel
                {
                    Page = 1,
                    PageSize = this.config.DefaultPageSize,
                    SortColumn = "StartDate",
                    SortDirection = "D",
                });
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(string pagingRequestModel)
        {
            var requestModel = JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);
            return await this.GetNotifications(requestModel);
        }

        private static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        private async Task<IActionResult> GetNotifications(PagingRequestModel requestModel)
        {
            List<PagingColumnFilter> filters = null;

            if (requestModel.Filter != null)
            {
                filters = Clone(requestModel.Filter);

                var priorityFilter = requestModel.Filter.FirstOrDefault(f => f.Column == "NotificationPriority");
                if (priorityFilter != null)
                {
                    if (priorityFilter.Value.Equals("low", StringComparison.InvariantCultureIgnoreCase))
                    {
                        priorityFilter.Value = ((int)NotificationPriorityEnum.General).ToString();
                    }
                    else if (priorityFilter.Value.Equals("high", StringComparison.InvariantCultureIgnoreCase))
                    {
                        priorityFilter.Value = ((int)NotificationPriorityEnum.Priority).ToString();
                    }
                    else
                    {
                        requestModel.Filter.Remove(priorityFilter);
                        filters.Remove(filters.FirstOrDefault(f => f.Column == "NotificationPriority"));
                    }
                }

                var notificationTypeFilter = requestModel.Filter.FirstOrDefault(f => f.Column == "NotificationType");
                if (notificationTypeFilter != null)
                {
                    var filterValue = Regex.Replace(notificationTypeFilter.Value, @"\s+", string.Empty).ToLower();

                    var matchedEnums = Enum.GetValues(typeof(NotificationTypeEnum))
                                            .Cast<NotificationTypeEnum>()
                                            .Where(e => e.ToString().ToLower().Contains(filterValue)).ToList();

                    if (matchedEnums.Count > 0)
                    {
                        notificationTypeFilter.Value = string.Join(",", matchedEnums.Select(m => ((int)m).ToString()));
                    }
                    else
                    {
                        requestModel.Filter.Remove(notificationTypeFilter);
                        filters.Remove(filters.FirstOrDefault(f => f.Column == "NotificationType"));
                    }
                }
            }

            requestModel.Sanitize();
            requestModel.PageSize = this.config.DefaultPageSize;

            var model = new TablePagingViewModel<NotificationViewModel>
            {
                Results = await this.notificationService.GetPagedAsync(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = filters,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.config.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };

            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = model.Filter != null ? model.Filter.Count() : 0,
                CreateRequired = true,
            };

            return this.View(model);
        }
    }
}