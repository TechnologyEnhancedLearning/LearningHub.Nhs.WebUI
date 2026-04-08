namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Extensions;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="BaseController" />.
    /// </summary>
    [ServiceFilter(typeof(OfflineCheckFilter))]
    public abstract class BaseController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger logger;
        private readonly IMoodleBridgeApiService moodleBridgeApiService;
        private readonly Settings settings;
        private TenantViewModel currentTenant;

        /// <summary>
        /// The list of Moodle UserIds.
        /// </summary>
        private MoodleInstanceUserIdsViewModel moodleUserIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="hostingEnv">Hosting env.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="moodleBridgeApiService">moodleBridgeApiService.</param>
        /// <param name="settings">Settings.</param>
        protected BaseController(
            IWebHostEnvironment hostingEnv,
            IHttpClientFactory httpClientFactory,
            ILogger logger,
            IMoodleBridgeApiService moodleBridgeApiService,
            Settings settings)
        {
            this.hostingEnvironment = hostingEnv;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.moodleBridgeApiService = moodleBridgeApiService;
            this.settings = settings;
        }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        protected IWebHostEnvironment HostingEnvironment => this.hostingEnvironment;

        /// <summary>
        /// Gets the http client factory.
        /// </summary>
        protected IHttpClientFactory HttpClientFactory => this.httpClientFactory;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger => this.logger;

        /// <summary>
        /// Gets the settings.
        /// </summary>
        protected Settings Settings => this.settings;

        /// <summary>
        /// Gets the ContentRootPath.
        /// </summary>
        protected string ContentRootPath => this.HostingEnvironment.ContentRootPath;

        /// <summary>
        /// Gets the CurrentTenant.
        /// </summary>
        protected TenantViewModel CurrentTenant => this.currentTenant;

        /// <summary>
        /// Gets the CurrentUserId.
        /// </summary>
        protected int CurrentUserId => this.User.Identity.GetCurrentUserId();

        /// <summary>
        /// Gets the CurrentUserId.
        /// </summary>
        protected int CurrentMoodleUserId => this.User.Identity.GetMoodleUserId();

        /// <summary>
        /// Gets the CurrentUserEmail.
        /// </summary>
        protected string CurrentUserEmail => this.User.Identity.GetCurrentEmail();

        /// <summary>
        /// Gets the MoodleInstanceUserIds.
        /// </summary>
        protected MoodleInstanceUserIdsViewModel MoodleInstanceUserIds => this.GetUserIdsPerInstances().Result;

        /// <summary>
        /// The OnActionExecuting.
        /// </summary>
        /// <param name="context">The context<see cref="Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext"/>.</param>
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            this.Initialise();
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// The get user ids and instances async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<MoodleInstanceUserIdsViewModel> GetUserIdsPerInstances()
        {
            if (this.moodleUserIds == null)
            {
                var moodleUserInstances = await this.moodleBridgeApiService.GetUserInstancesByEmail(this.CurrentUserEmail);
                this.moodleUserIds = moodleUserInstances;
            }

            return this.moodleUserIds;
        }

        /// <summary>
        /// The Initialise.
        /// </summary>
        private void Initialise()
        {
            if (this.HttpContext != null)
            {
                this.currentTenant = new TenantViewModel
                {
                    Code = this.settings.TenantCode,
                    Id = this.settings.LearningHubTenantId,
                    Name = this.settings.TenantName,
                };

                var tenantFilePath = TenantHelper.GetLayoutPath(this.ContentRootPath, this.CurrentTenant);

                this.ViewData["Layout"] = tenantFilePath;
                this.ViewBag.CurrentTenant = this.CurrentTenant;
            }
        }
    }
}
