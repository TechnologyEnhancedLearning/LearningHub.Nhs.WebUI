namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Content;
    using LearningHub.Nhs.Models.Enums.Content;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement;
    using Settings = LearningHub.Nhs.WebUI.Configuration.Settings;

    /// <summary>
    /// Defines the <see cref="HomeController" />.
    /// </summary>
    [ServiceFilter(typeof(LoginWizardFilter))]
    public class HomeController : BaseController
    {
        private readonly LearningHubAuthServiceConfig authConfig;
        private readonly IResourceService resourceService;
        private readonly IUserService userService;
        private readonly IDashboardService dashboardService;
        private readonly IContentService contentService;
        private readonly IFeatureManager featureManager;
        private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="userService">User service.</param>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="authConfig">Auth config.</param>
        /// <param name="dashboardService">Dashboard service.</param>
        /// <param name="contentService">Content service.</param>
        /// <param name="featureManager"> featureManager.</param>
        /// <param name="configuration"> config.</param>
        public HomeController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            ILogger<HomeController> logger,
            IOptions<Settings> settings,
            IUserService userService,
            IResourceService resourceService,
            LearningHubAuthServiceConfig authConfig,
            IDashboardService dashboardService,
            IContentService contentService,
            IFeatureManager featureManager,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.authConfig = authConfig;
            this.userService = userService;
            this.resourceService = resourceService;
            this.dashboardService = dashboardService;
            this.contentService = contentService;
            this.featureManager = featureManager;
            this.configuration = configuration;
        }

        /// <summary>
        /// The Aboutus.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult Aboutus()
        {
            this.ViewBag.TwitterScreenName = this.Settings.TwitterCredentials.ScreenName;
            this.ViewBag.TwitterScreenName = "@NHSE_TEL";
            return this.View();
        }

        /// <summary>
        /// The AccessDenied.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult AccessDenied()
        {
            this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
            return this.View("AccessDenied");
        }

        /// <summary>
        /// The Accessibility.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult Accessibility()
        {
            return this.View();
        }

        /// <summary>
        /// The Contactus.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult Contactus()
        {
            return this.View();
        }

        /// <summary>
        /// The CreateAccount.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult CreateAccount()
        {
            return this.View();
        }

        /// <summary>
        /// The Error.
        /// </summary>
        /// <param name="httpStatusCode">httpStatusCode.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/Home/Error/{httpStatusCode:int?}")]
        public IActionResult Error(int? httpStatusCode)
        {
            string originalPathUrlMessage = null;
            string originalPath = null;
            if (httpStatusCode.HasValue && httpStatusCode.Value == 404)
            {
                var exceptionHandlerPathFeature = this.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                originalPath = exceptionHandlerPathFeature?.OriginalPath;
                originalPathUrlMessage = $"Page Not Found url: {originalPath}. ";
            }

            if (this.User.Identity.IsAuthenticated)
            {
                this.Logger.LogError(
                    "{originalPathUrlMessage}Uncaptured error page displayed. RequestId = {requestId}. User is authenticated: Username is {username} and userId is: {lhuserid}.",
                    originalPathUrlMessage ?? string.Empty,
                    Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
                    this.User.Identity.Name,
                    this.User.Identity.GetCurrentUserId());
            }
            else
            {
                this.Logger.LogError(
                    "{originalPathUrlMessage}Uncaptured error page displayed. RequestId = {requestId}. User is not authenticated.",
                    originalPathUrlMessage ?? string.Empty,
                    Activity.Current?.Id ?? this.HttpContext.TraceIdentifier);
            }

            this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;

            if (!httpStatusCode.HasValue)
            {
                return this.View("Error");
            }
            else
            {
                this.ViewBag.ErrorHeader = httpStatusCode.Value switch
                {
                    401 => "You do not have permission to access this page",
                    404 => "We cannot find the page you are looking for",
                    _ => "We cannot find the page you are looking for",
                };

                this.ViewBag.HttpStatusCode = httpStatusCode.Value;
                this.ViewBag.HomePageUrl = "/home";
                return this.View("CustomError");
            }
        }

        /// <summary>
        /// Dashboard, forces authentication.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [Authorize]
        [Route("HomepageWithAuthentication")]
        public IActionResult HomepageWithAuthentication()
        {
            return this.Redirect("/");
        }

        /// <summary>
        /// Index.
        /// </summary>
        /// <param name="myLearningDashboard">The my learning dashboard type.</param>
        /// <param name="resourceDashboard">The resource dashboard type.</param>
        /// <param name="catalogueDashboard">The catalogue dashboard type.</param>
        /// <returns>Home page.</returns>
        public async Task<IActionResult> Index(string myLearningDashboard = "my-in-progress", string resourceDashboard = "popular-resources", string catalogueDashboard = "popular-catalogues")
        {
            if (this.User?.Identity.IsAuthenticated == true)
            {
                this.Settings.ConcurrentId = this.CurrentUserId;
                this.Logger.LogInformation("User is authenticated: User is {fullname} and userId is: {lhuserid}", this.User.Identity.GetCurrentName(), this.User.Identity.GetCurrentUserId());
                if (this.User.IsInRole("Administrator") || this.User.IsInRole("BlueUser") || this.User.IsInRole("ReadOnly") || this.User.IsInRole("BasicUser"))
                {
                    var learningTask = this.dashboardService.GetMyAccessLearningsAsync(myLearningDashboard, 1);
                    var resourcesTask = this.dashboardService.GetResourcesAsync(resourceDashboard, 1);
                    var cataloguesTask = this.dashboardService.GetCataloguesAsync(catalogueDashboard, 1);

                    var enrolledCoursesTask = Task.FromResult(new List<MoodleCourseResponseViewModel>());
                    var enableMoodle = Task.Run(() => this.featureManager.IsEnabledAsync(FeatureFlags.EnableMoodle)).Result;
                    this.ViewBag.EnableMoodle = enableMoodle;
                    if (enableMoodle && myLearningDashboard == "my-enrolled-courses")
                    {
                       enrolledCoursesTask = this.dashboardService.GetEnrolledCoursesFromMoodleAsync(this.CurrentMoodleUserId, 1);
                    }

                    await Task.WhenAll(learningTask, resourcesTask, cataloguesTask);

                    var model = new DashboardViewModel()
                    {
                        MyLearnings = await learningTask,
                        Resources = await resourcesTask,
                        Catalogues = await cataloguesTask,
                        EnrolledCourses = await enrolledCoursesTask,
                    };

                    if (!string.IsNullOrEmpty(this.Request.Query["preview"]) && Convert.ToBoolean(this.Request.Query["preview"]))
                    {
                        return this.View("LandingPage", await this.GetLandingPageContent(Convert.ToBoolean(this.Request.Query["preview"])));
                    }

                    return this.View("Dashboard", model);
                }
                else
                {
                    return this.RedirectToAction("InvalidUserAccount", "Account");
                }
            }
            else
            {
                // new landing page
                bool previewCheck = this.HttpContext.Request.QueryString.ToString().ToLower().Contains("preview");
                return this.View("LandingPage", await this.GetLandingPageContent(previewCheck));
            }
        }

        /// <summary>
        /// Load the specified dashobard page.
        /// </summary>
        /// <param name="dashBoardTray">dashBoardTray.</param>
        /// <param name="myLearningDashBoard">myLearningDashBoard.</param>
        /// <param name="resourceDashBoard">resourceDashBoard.</param>
        /// <param name="catalogueDashBoard">catalogueDashBoard.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <returns>Dashboard page.</returns>
        [Authorize]
        [Route("/Home/loadpage/{dashBoardTray}/{myLearningDashBoard}/{resourceDashBoard}/{catalogueDashBoard}/{pageNumber:int}")]
        public async Task<IActionResult> LoadPage(string dashBoardTray = "my-learning", string myLearningDashBoard = "in-progress", string resourceDashBoard = "popular-resources", string catalogueDashBoard = "recent-catalogues", int pageNumber = 1)
        {
            if (this.User.IsInRole("Administrator") || this.User.IsInRole("BlueUser") || this.User.IsInRole("ReadOnly") || this.User.IsInRole("BasicUser"))
            {
                DashboardViewModel model = new DashboardViewModel
                {
                    MyLearnings = new Nhs.Models.Dashboard.DashboardMyLearningResponseViewModel { Type = myLearningDashBoard },
                    Resources = new Nhs.Models.Dashboard.DashboardResourceResponseViewModel { Type = resourceDashBoard },
                    Catalogues = new Nhs.Models.Dashboard.DashboardCatalogueResponseViewModel { Type = catalogueDashBoard },
                };

                bool isAjax = this.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (isAjax)
                {
                    switch (dashBoardTray)
                    {
                        case "my-learning":
                            model.MyLearnings = await this.dashboardService.GetMyAccessLearningsAsync(myLearningDashBoard, pageNumber);
                            return this.PartialView("_MyAccessedLearningTray", model);
                        case "resources":
                            model.Resources = await this.dashboardService.GetResourcesAsync(resourceDashBoard, pageNumber);
                            return this.PartialView("_ResourceTray", model);
                        case "catalogues":
                            model.Catalogues = await this.dashboardService.GetCataloguesAsync(catalogueDashBoard, pageNumber);
                            return this.PartialView("_CatalogueTray", model);
                    }
                }
                else
                {
                    var learningTask = this.dashboardService.GetMyAccessLearningsAsync(myLearningDashBoard, dashBoardTray == "my-learning" ? pageNumber : 1);
                    var resourcesTask = this.dashboardService.GetResourcesAsync(resourceDashBoard, dashBoardTray == "resources" ? pageNumber : 1);
                    var cataloguesTask = this.dashboardService.GetCataloguesAsync(catalogueDashBoard, dashBoardTray == "catalogues" ? pageNumber : 1);
                    await Task.WhenAll(learningTask, resourcesTask, cataloguesTask);
                    model.MyLearnings = await learningTask;
                    model.Resources = await resourcesTask;
                    model.Catalogues = await cataloguesTask;
                    return this.View("Dashboard", model);
                }
            }

            return this.RedirectToAction("InvalidUserAccount", "Account");
        }

        /// <summary>
        /// Gets Catalogues.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Catalogues()
        {
            return this.View("Catalogues");
        }

        /// <summary>
        /// The login page.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return new ChallengeResult(new AuthenticationProperties() { RedirectUri = "/" });
        }

        /// <summary>
        /// The NhsSites.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult NhsSites()
        {
            return this.View();
        }

        /// <summary>
        /// The ChangePasswordAcknowledgement.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult ChangePasswordAcknowledgement()
        {
            return this.View();
        }

        /// <summary>
        /// StatusUpdate.
        /// </summary>
        /// <returns>Actionresult.</returns>
        public IActionResult UserLogout()
        {
            if (!(this.User?.Identity.IsAuthenticated ?? false))
            {
                return this.RedirectToAction("Index");
            }

            return new SignOutResult(new[] { CookieAuthenticationDefaults.AuthenticationScheme, "oidc" });
        }

        /// <summary>
        /// The Logout.
        /// This is directly referenced in the LoginWizardFilter to allow
        /// logouts to bypass LoginWizard redirects.
        /// If the name is changed, the LoginWizardFilter MUST be updated.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [AllowAnonymous]
        public IActionResult Logout()
        {
            var redirectUri = $"{this.configuration["LearningHubAuthServiceConfig:Authority"]}/Home/SetIsPasswordUpdate?isLogout=true";
            return this.Redirect(redirectUri);
        }

        /// <summary>
        /// The SessionTimeout.
        /// </summary>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet("session-timeout")]
        public IActionResult SessionTimeout(string returnUrl = "/")
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.Redirect(returnUrl);
            }

            // Add successful logout to the UserHistory
            UserHistoryViewModel userHistory = new UserHistoryViewModel()
            {
                UserId = this.Settings.ConcurrentId,
                UserHistoryTypeId = (int)UserHistoryType.Logout,
                Detail = @"User session time out",
            };

            this.userService.StoreUserHistory(userHistory);

            this.ViewBag.AuthTimeout = this.authConfig.AuthTimeout;
            this.ViewBag.ReturnUrl = returnUrl;

            return this.View();
        }

        /// <summary>
        /// The SessionTimeout.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("browser-close")]
        public IActionResult BrowserClose()
        {
            // Add browser close to the UserHistory
            UserHistoryViewModel userHistory = new UserHistoryViewModel()
            {
                UserId = this.CurrentUserId,
                UserHistoryTypeId = (int)UserHistoryType.Logout,
                Detail = @"User browser closed",
            };

            this.userService.StoreUserHistory(userHistory);
            return this.Ok(true);
        }

        /// <summary>
        /// The SitemapXml.
        /// </summary>
        /// <returns>The sitemap xml.</returns>
        [Route("/sitemap.xml")]
        public async Task<IActionResult> SitemapXml()
        {
            this.Response.ContentType = "application/xml";

            var catalogues = await this.dashboardService.GetCataloguesAsync("all-catalogues", -1);

            var resources = await this.resourceService.GetAllPublishedResourceAsync();

            var model = new SitemapViewModel($"{this.Request.Scheme}://{this.Request.Host}", catalogues.Catalogues.Select(c => c.Url), resources);

            return this.View("Sitemap", model);
        }

        /// <summary>
        /// The GetLandingPageContent.
        /// </summary>
        /// <param name="preview">preview.</param>
        /// <returns>The LandingPageViewModel.</returns>
        private async Task<LandingPageViewModel> GetLandingPageContent(bool preview = false)
        {
            var model = new LandingPageViewModel { PageSectionDetailViewModels = new List<PageSectionDetailViewModel>() };
            var pageViewModel = await this.contentService.GetPageByIdAsync(1, preview);
            model.PageViewModel = pageViewModel;
            model.DisplayAudioVideo = Task.Run(() => this.featureManager.IsEnabledAsync(FeatureFlags.DisplayAudioVideoResource)).Result;
            model.MKPlayerLicence = this.Settings.MediaKindSettings.MKPlayerLicence;

            if (pageViewModel != null && pageViewModel.PageSections.Any())
            {
                foreach (var item in pageViewModel.PageSections)
                {
                    if (item.SectionTemplateType == SectionTemplateType.Video)
                    {
                        model.PageSectionDetailViewModels.Add(await this.contentService.GetPageSectionDetailVideoAssetByIdAsync(item.PageSectionDetail.Id));
                    }
                }

                return model;
            }
            else
            {
                return new LandingPageViewModel { PageSectionDetailViewModels = new List<PageSectionDetailViewModel>(), PageViewModel = new PageViewModel { PageSections = new List<PageSectionViewModel> { } } };
            }
        }
    }
}
