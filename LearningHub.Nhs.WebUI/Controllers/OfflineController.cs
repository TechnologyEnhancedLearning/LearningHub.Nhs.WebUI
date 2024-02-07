// <copyright file="OfflineController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="OfflineController" />.
    /// </summary>
    public class OfflineController : BaseController
    {
        private readonly IInternalSystemService internalSystemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfflineController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The httpClientFactory<see cref="IHttpClientFactory"/>.</param>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IHostingEnvironment"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{AccountController}"/>.</param>
        /// <param name="settings">The settings<see cref="IOptions{Settings}"/>.</param>
        /// <param name="internalSystemService">The internalSystemService<see cref="IInternalSystemService"/>.</param>
        public OfflineController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            ILogger<OfflineController> logger,
            IOptions<Settings> settings,
            IInternalSystemService internalSystemService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.internalSystemService = internalSystemService;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> Index()
        {
            var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.LearningHub);

            if (!internalSystem.IsOffline)
            {
                return this.Redirect("/");
            }

            this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
            return this.View();
        }

        /// <summary>
        /// Method that forces authentication to allow admin access.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Authorize]
        public IActionResult Access()
        {
            return this.Redirect("/");
        }
    }
}
