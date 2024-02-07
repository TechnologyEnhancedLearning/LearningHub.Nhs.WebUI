// <copyright file="TrackingController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The TrackingController.
    /// </summary>
    public class TrackingController : BaseController
    {
        private readonly IJsDetectionLogger jsDetectionLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The httpClientFactory.</param>
        /// <param name="hostingEnvironment">The hostingEnvironment.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="jsDetectionLogger">The JsDetectionLogger.</param>
        public TrackingController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            IOptions<Settings> settings,
            ILogger<TrackingController> logger,
            IJsDetectionLogger jsDetectionLogger)
        : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.jsDetectionLogger = jsDetectionLogger;
        }

        /// <summary>
        /// The TrackJavascript.
        /// </summary>
        /// <param name="js">javascript enabled.</param>
        /// <returns>The actionResult.</returns>
        [Route("track/javascript")]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public FileResult TrackJavascript(bool js)
        {
            if (js)
            {
                this.jsDetectionLogger.IncrementEnabled();
            }
            else
            {
                this.jsDetectionLogger.IncrementDisabled();
            }

            return this.File("/images/dot.gif", "image/gif");
        }
    }
}