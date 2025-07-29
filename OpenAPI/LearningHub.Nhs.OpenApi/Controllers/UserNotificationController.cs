namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// UserNotification operations.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("UserNotification")]
    [ApiController]
    public class UserNotificationController : OpenApiControllerBase
    {
        /// <summary>
        /// The usernotification service.
        /// </summary>
        private readonly IUserNotificationService usernotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="usernotificationService">The usernotification service.</param>
        /// <param name="logger">The logger.</param>
        public UserNotificationController(IUserNotificationService usernotificationService)
        {
            this.usernotificationService = usernotificationService;
        }

        // GET api/UserNotification/GetById/id

        /// <summary>
        /// Get User Notification record by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usernotification = await usernotificationService.GetByIdAsync(id);

            return this.Ok(usernotification);
        }

        /// <summary>
        /// Get User Notification record by id and user Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetByIdAndUserId/{id}")]
        public async Task<IActionResult> GetByIdAndUserId(int id)
        {
            var usernotification = await usernotificationService.GetByIdAndUserIdAsync(id, this.CurrentUserId.GetValueOrDefault());

            return this.Ok(usernotification);
        }

        // GET api/UserNotification/GetPage/page/pageSize

        /// <summary>
        /// Get a page of user notifications.
        /// </summary>
        /// <param name="request">Paging request.</param>
        /// <param name="priorityType">Notification priority type.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("GetPage/{priorityType}")]
        public async Task<IActionResult> GetPage([FromBody] PagingRequestModel request, NotificationPriorityEnum priorityType)
        {
            PagedResultSet<UserNotificationViewModel> pagedResultSet = await usernotificationService.GetPageAsync(
                this.CurrentUserId.GetValueOrDefault(), priorityType, request.Page, request.PageSize, request.SortColumn, request.SortDirection);
            return this.Ok(pagedResultSet);
        }

        // GET api/UserNotification/GetUserUnreadCount/userid

        /// <summary>
        /// Get User Unread Notifications Count.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserUnreadNotificationCount/{userid}")]
        public async Task<IActionResult> GetUserUnreadNotificationCount(int userid)
        {
            var count = await usernotificationService.GetUserUnreadNotificationCountAsync(this.CurrentUserId.GetValueOrDefault());

            return this.Ok(count);
        }

        /// <summary>
        /// Update an existing user notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPut("{usernotification}")]
        public async Task<IActionResult> PutAsync([FromBody] UserNotification notification)
        {
            var vr = await usernotificationService.UpdateAsync(this.CurrentUserId.GetValueOrDefault(), notification);
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
        /// Create a new user notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("{usernotification}")]
        public async Task<IActionResult> PostAsync([FromBody] UserNotification notification)
        {
            if (notification.UserId == 0)
            {
                notification.UserId = this.CurrentUserId.GetValueOrDefault();
            }

            var vr = await usernotificationService.CreateAsync(this.CurrentUserId.GetValueOrDefault(), notification);
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }
    }
}