namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="RoadMapController" />.
    /// </summary>
    public class RoadMapController : BaseController
    {
        private readonly IMoodleBridgeApiService moodleBridgeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoadMapController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="moodleBridgeApiService">moodleBridgeApiService.</param>
        public RoadMapController(
            IWebHostEnvironment hostingEnvironment,
            IMoodleBridgeApiService moodleBridgeApiService,
            IHttpClientFactory httpClientFactory,
            ILogger<RoadMapController> logger,
            IOptions<Configuration.Settings> settings)
            : base(hostingEnvironment, httpClientFactory, logger, moodleBridgeApiService, settings.Value)
        {
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>A <see cref="IActionResult"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("Updates")]
        public IActionResult Updates()
        {
            return this.View("Index");
        }
    }
}
