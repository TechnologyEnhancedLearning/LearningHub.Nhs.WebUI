// <copyright file="HomeController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="HomeController" />.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Defines the _learningHubHttpClient.
        /// </summary>
        private readonly ILearningHubHttpClient learningHubHttpClient;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<HomeController> logger;

        /// <summary>
        /// Defines the _userService.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="config">The config<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{HomeController}"/>.</param>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        public HomeController(
            IWebHostEnvironment hostingEnvironment,
            IOptions<WebSettings> config,
            ILogger<HomeController> logger,
            IUserService userService,
            ILearningHubHttpClient learningHubHttpClient)
        : base(hostingEnvironment)
        {
            this.logger = logger;
            this.userService = userService;
            this.learningHubHttpClient = learningHubHttpClient;
        }

        /// <summary>
        /// The Error.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            UserViewModel user = null;

            if (this.User.Identity.IsAuthenticated)
            {
                this.logger.LogInformation("User is authenticated with id: {lhuserid}", this.User.Identity.GetCurrentUserId());
                foreach (var claim in this.User.Claims)
                {
                    if (claim.Type == "given_name")
                    {
                        this.ViewBag.FirstName = claim.Value;
                    }
                }
            }
            else
            {
                this.ViewBag.FirstName = string.Empty;
            }

            return this.View(user);
        }

        /// <summary>
        /// The Login.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult Login()
        {
            // Authorization is required
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// The Logout.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [AllowAnonymous]
        public IActionResult Logout()
        {
            if (!(this.User?.Identity.IsAuthenticated ?? false))
            {
                return this.RedirectToAction("Index");
            }

            return new SignOutResult(new[] { "Cookies", "oidc" });
        }

        /// <summary>
        /// The TestPage.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> TestPage()
        {
            UserViewModel user = null;

            if (this.User.Identity.IsAuthenticated)
            {
                foreach (var claim in this.User.Claims)
                {
                    if (claim.Type == "given_name")
                    {
                        this.ViewBag.FirstName = claim.Value;
                    }

                    if (claim.Type == "family_name")
                    {
                        this.ViewBag.LastName = claim.Value;
                    }
                }

                try
                {
                    user = await this.userService.GetCurrentUser();
                }
                catch (Exception ex)
                {
                    if (ex.Message == "AccessDenied")
                    {
                        return this.RedirectToAction("AccessDenied", "Authorisation");
                    }
                }
            }
            else
            {
                this.ViewBag.FirstName = string.Empty;
                this.ViewBag.LastName = string.Empty;
            }

            return this.View(user);
        }
    }
}
