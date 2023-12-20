// <copyright file="PoliciesController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Policies;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Settings = LearningHub.Nhs.WebUI.Configuration.Settings;

    /// <summary>
    /// Defines the <see cref="PoliciesController" />.
    /// </summary>
    public class PoliciesController : BaseController
    {
        private readonly ITermsAndConditionsService termsAndConditionsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoliciesController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="termsAndConditionsService">Terms and conditions service.</param>
        public PoliciesController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            ILogger<HomeController> logger,
            IOptions<Settings> settings,
            ITermsAndConditionsService termsAndConditionsService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.termsAndConditionsService = termsAndConditionsService;
        }

        /// <summary>
        /// The ContentPolicy.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("/policies/content-policy")]
        public IActionResult ContentPolicy()
        {
            return this.View();
        }

        /// <summary>
        /// The CookiePolicy.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("/policies/cookie-policy")]
        public IActionResult CookiePolicy()
        {
            return this.View();
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult Index()
        {
            this.ViewBag.CookieBannerStyle = "display:block;";
            return this.View();
        }

        /// <summary>
        /// The PrivacyPolicy.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("/policies/privacy-policy")]
        public IActionResult PrivacyPolicy()
        {
            return this.View();
        }

        /// <summary>
        /// The TermsConditions.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("/policies/terms-and-conditions")]
        public async Task<IActionResult> TermsConditions()
        {
            var termsAndConditions = await this.termsAndConditionsService.LatestVersionAsync(this.Settings.LearningHubTenantId);
            return this.View("TermsConditions", new TermsAndConditionsViewModel
            {
                TermsAndConditions = termsAndConditions.Details,
                CreatedDate = termsAndConditions.CreatedDate,
            });
        }

        /// <summary>
        /// The Acceptable Use Policy.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("/policies/acceptable-use-policy")]
        public IActionResult AcceptableUsePolicy()
        {
            return this.View();
        }
    }
}
