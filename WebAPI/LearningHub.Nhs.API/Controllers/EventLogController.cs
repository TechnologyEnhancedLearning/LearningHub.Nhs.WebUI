// <copyright file="EventLogController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.EventLog;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Event operations.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/[controller]")]
    [ApiController]
    public class EventLogController : ApiControllerBase
    {
        /// <summary>
        /// The event log service.
        /// </summary>
        private readonly IEventLogService eventLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="eventLogService">The event service.</param>
        /// <param name="logger">The logger.</param>
        public EventLogController(
            IUserService userService,
            IEventLogService eventLogService,
            ILogger<EventLogController> logger)
            : base(userService, logger)
        {
            this.eventLogService = eventLogService;
        }

        /// <summary>
        /// Create event.
        /// </summary>
        /// <param name="eventViewModel">eventViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateEvent")]
        public IActionResult CreateEvent(EventViewModel eventViewModel)
        {
            if (eventViewModel.CreateUserId == 0)
            {
                eventViewModel.CreateUserId = this.CurrentUserId;
            }

            this.eventLogService.CreateEvent(eventViewModel);
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
        }
    }
}
