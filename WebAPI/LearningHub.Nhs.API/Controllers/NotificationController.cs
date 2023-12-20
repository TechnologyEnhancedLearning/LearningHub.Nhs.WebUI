// <copyright file="NotificationController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Notification operations.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ApiControllerBase
    {
        /// <summary>
        /// The notification service.
        /// </summary>
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="notificationService">The notification service.</param>
        /// <param name="logger">The logger.</param>
        public NotificationController(
                                        IUserService userService,
                                        INotificationService notificationService,
                                        ILogger<NotificationController> logger)
            : base(userService, logger)
        {
            this.notificationService = notificationService;
        }

        // GET api/Notification/GetById/id

        /// <summary>
        /// Get Notification record by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var notification = await this.notificationService.GetByIdAsync(id);

            return this.Ok(notification);
        }

        // GET api/Notification/GetFilteredPage/page/pageSize/sortColumn/sortDirection/filter

        /// <summary>
        /// The get filtered page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{filter}")]
        public async Task<IActionResult> GetFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string filter)
        {
            PagedResultSet<NotificationViewModel> pagedResultSet = await this.notificationService.GetPageAsync(page, pageSize, sortColumn, sortDirection, filter);
            return this.Ok(pagedResultSet);
        }

        /// <summary>
        /// Create a new Notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("{notification}")]
        public async Task<IActionResult> PostAsync([FromBody] NotificationViewModel notification)
        {
            var vr = await this.notificationService.CreateAsync(this.CurrentUserId, notification);
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Update an existing Notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPut("{notification}")]
        public async Task<IActionResult> PutAsync([FromBody] NotificationViewModel notification)
        {
            var vr = await this.notificationService.UpdateAsync(this.CurrentUserId, notification);
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Deletes a notification.
        /// </summary>
        /// <param name="id">The notification id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var notification = await this.notificationService.GetByIdAsync(id);
            notification.Deleted = true;

            await this.notificationService.UpdateAsync(this.CurrentUserId, notification);

            return this.Ok(new ApiResponse(true));
        }
    }
}