namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.WebUtilitiesInterfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="CacheController" />.
    /// </summary>
    public class CacheController : BaseController
    {
        private readonly ICacheService cacheService;
        private readonly IEventService eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="eventService">The eventService.</param>
        public CacheController(
            IWebHostEnvironment hostingEnvironment,
            ICacheService cacheService,
            IEventService eventService)
            : base(hostingEnvironment)
        {
            this.cacheService = cacheService;
            this.eventService = eventService;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="cacheCleared">The copy<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public IActionResult Index(bool cacheCleared = false)
        {
            this.ViewBag.cacheCleared = cacheCleared;
            return this.View();
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> FlushAllCache()
        {
            var currentUserId = this.User.Identity.GetCurrentUserId();
            var currentUser = this.User.Identity.GetCurrentName();
            var eventEntity = new Event();
            eventEntity.EventTypeEnum = EventTypeEnum.FlushAllCache;
            eventEntity.JsonData = JsonConvert.SerializeObject(new { UserId = currentUserId, UserName = currentUser });
            eventEntity.UserId = currentUserId;

            await this.eventService.Create(eventEntity);

            await this.cacheService.FlushAll();
            return this.RedirectToAction("Index", new { cacheCleared = true });
        }
    }
}
