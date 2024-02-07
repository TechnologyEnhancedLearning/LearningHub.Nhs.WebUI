// <copyright file="RoadMapController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="RoadMapController" />.
    /// </summary>
    public class RoadMapController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoadMapController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        public RoadMapController(
            IWebHostEnvironment hostingEnvironment,
            IHttpClientFactory httpClientFactory,
            ILogger<RoadMapController> logger,
            IOptions<Configuration.Settings> settings)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
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
