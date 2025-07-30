namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Event operations.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("Event")]
    [ApiController]
    public class EventController : OpenApiControllerBase
    {
        /// <summary>
        /// The event service.
        /// </summary>
        private readonly IEventService eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="eventService">The event service.</param>
        public EventController(
            IEventService eventService)
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
            return this.Ok(await eventService.GetByIdAsync(id));
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
            var vr = await eventService.CreateAsync(this.CurrentUserId.GetValueOrDefault(), eventEntity);

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
        public async Task<IActionResult> PutAsync([FromBody] Event eventEntity)
        {
            var vr = await eventService.UpdateAsync(this.CurrentUserId.GetValueOrDefault(), eventEntity);
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
