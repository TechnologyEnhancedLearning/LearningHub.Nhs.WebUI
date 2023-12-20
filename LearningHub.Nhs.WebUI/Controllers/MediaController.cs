// <copyright file="MediaController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Net.Http;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="MediaController" />.
    /// </summary>
    public class MediaController : BaseController
    {
        private readonly IAzureMediaService azureMediaService;
        private readonly ILogger<MediaController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="azureMediaService">Azure media services.</param>
        public MediaController(IWebHostEnvironment hostingEnvironment, ILogger<MediaController> logger, IOptions<Settings> settings, IHttpClientFactory httpClientFactory, IAzureMediaService azureMediaService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.azureMediaService = azureMediaService;
            this.logger = logger;
        }

        /// <summary>
        /// The MediaManifest.
        /// </summary>
        /// <param name="playBackUrl">The playBackUrl.</param>
        /// <param name="token">The token.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Media/MediaManifest")]
        public IActionResult MediaManifest(string playBackUrl, string token)
        {
            try
            {
                this.Logger.LogDebug($"playBackUrl={playBackUrl} token={token}");
                var hostPortion = this.Request.Host;

                var manifestProxyUrl = string.Format("https://{0}/api/MediaManifestProxy", hostPortion);
                this.Logger.LogDebug($"manifestProxyUrl={manifestProxyUrl}");

                var modifiedTopLeveLManifest = this.azureMediaService.GetTopLevelManifestForToken(manifestProxyUrl, playBackUrl, token);
                this.Logger.LogDebug($"modifiedTopLeveLManifest={modifiedTopLeveLManifest}");

                var response = new ContentResult
                {
                    Content = modifiedTopLeveLManifest,
                    ContentType = @"application/vnd.apple.mpegurl",
                };
                this.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                this.Response.Headers.Append("X-Content-Type-Options", "nosniff");

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Message: {ex.Message}");
                throw;
            }
        }
    }
}
