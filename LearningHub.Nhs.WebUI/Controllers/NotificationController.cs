// <copyright file="NotificationController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Settings = LearningHub.Nhs.WebUI.Configuration.Settings;

    /// <summary>
    /// Defines the <see cref="NotificationController" />.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(LoginWizardFilter))]
    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Htp client factory.</param>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="notificationService">Notification service.</param>
        /// <param name="logger">Logger.</param>
        public NotificationController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            IOptions<Settings> settings,
            INotificationService notificationService,
            ILogger<NotificationController> logger)
        : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.notificationService = notificationService;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("/notification")]
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="notificationId">Notification Id.</param>
        /// <param name="userNotificationId">User notification id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("/api/notification/{notificationId}")]
        public async Task<IActionResult> Delete(int notificationId, [FromQuery] int userNotificationId)
        {
            await this.notificationService.Delete(userNotificationId, notificationId);
            return this.Ok();
        }

        /// <summary>
        /// The MarkAsRead.
        /// </summary>
        /// <param name="notificationId">Notification Id.</param>
        /// <param name="userNotificationId">User notification id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("/api/notification/{notificationId}/mark-as-read")]
        public async Task<ActionResult> MarkAsRead(int notificationId, [FromQuery] int userNotificationId)
        {
            await this.notificationService.MarkAsRead(userNotificationId, notificationId);
            return this.Ok();
        }

        /// <summary>
        /// The GetList.
        /// </summary>
        /// <param name="requestModel">Paging request model.</param>
        /// <param name="priorityType">Notification priority type.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("/api/notification")]
        public async Task<IActionResult> GetList([FromBody] PagingRequestModel requestModel, [FromQuery] NotificationPriorityEnum priorityType)
        {
            requestModel.PageSize = 10;

            var results = await this.notificationService.GetPagedAsync(requestModel, priorityType);

            int totalPages = (int)Math.Ceiling((double)results.TotalItemCount / requestModel.PageSize);

            if (totalPages < requestModel.Page)
            {
                requestModel.Page = totalPages;
            }

            var model = new TablePagingViewModel<UserNotificationViewModel>
            {
                Results = results,
                Paging = new PagingViewModel
                {
                    CurrentPage = requestModel.Page,
                    PageSize = requestModel.PageSize,
                    TotalItems = results.TotalItemCount,
                },
            };

            return this.Ok(model);
        }
    }
}