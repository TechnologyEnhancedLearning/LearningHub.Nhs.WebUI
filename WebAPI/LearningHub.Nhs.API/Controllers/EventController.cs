// <copyright file="EventController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Analytics;
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
    public class EventController : ApiControllerBase
    {
        /// <summary>
        /// The event service.
        /// </summary>
        private readonly IEventService eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="eventService">The event service.</param>
        /// <param name="logger">The logger.</param>
        public EventController(
            IUserService userService,
            IEventService eventService,
            ILogger<EventController> logger)
            : base(userService, logger)
        {
            this.eventService = eventService;
        }

        /// <summary>
        /// Get specific event by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            return this.Ok(await this.eventService.GetByIdAsync(id));
        }

        /// <summary>
        /// The create event async.
        /// </summary>
        /// <param name="eventEntity">The event.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateAsync([FromBody] Event eventEntity)
        {
            eventEntity.UserId = !eventEntity.UserId.HasValue ? this.CurrentUserId : eventEntity.UserId;
            var vr = await this.eventService.CreateAsync(this.CurrentUserId, eventEntity);

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
        /// Update an existing Event.
        /// </summary>
        /// <param name="eventEntity">The event.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] LearningHub.Nhs.Models.Entities.Analytics.Event eventEntity)
        {
            var vr = await this.eventService.UpdateAsync(this.CurrentUserId, eventEntity);
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
