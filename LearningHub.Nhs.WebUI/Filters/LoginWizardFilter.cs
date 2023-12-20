// <copyright file="LoginWizardFilter.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Filters
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="LoginWizardFilter" />.
    /// </summary>
    public class LoginWizardFilter : ActionFilterAttribute // IActionFilter
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<LoginWizardFilter> logger;
        private readonly ILoginWizardService loginWizardService;
        private readonly IUserService userService;
        private readonly ICacheService cacheService;
        private readonly IUserSessionHelper userSessionHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWizardFilter"/> class.
        /// </summary>
        /// <param name="loginWizardService">Login wizard service.</param>
        /// <param name="userService">User service.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="cacheService">The cacheService<see cref="ICacheService"/>.</param>
        /// <param name="userSessionHelper">The userSessionHelper<see cref="IUserSessionHelper"/>.</param>
        public LoginWizardFilter(ILoginWizardService loginWizardService, IUserService userService, ILogger<LoginWizardFilter> logger, IConfiguration configuration, ICacheService cacheService, IUserSessionHelper userSessionHelper)
        {
            this.loginWizardService = loginWizardService;
            this.userService = userService;
            this.logger = logger;
            this.configuration = configuration;
            this.cacheService = cacheService;
            this.userSessionHelper = userSessionHelper;
        }

        /// <summary>
        /// The OnActionExecutionAsync.
        /// </summary>
        /// <param name="context">The context<see cref="ActionExecutingContext"/>.</param>
        /// <param name="next">The next<see cref="ActionExecutionDelegate"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.ActionDescriptor.RouteValues["Controller"];
            var action = context.ActionDescriptor.RouteValues["Action"];
            var user = context.HttpContext.User;
            var executeNextAction = true;
            var loginWizardCacheKey = string.Empty;
            var returnUrl = context.HttpContext.Request.Path;

            if (user.Identity.IsAuthenticated)
            {
                loginWizardCacheKey = $"{user.Identity.GetCurrentUserId()}:LoginWizard";
            }

            if (!(controller == "Home" && action == "Error"))
            {
                if (controller == "Home" && action == "Logout" && !string.IsNullOrWhiteSpace(loginWizardCacheKey))
                {
                    await this.cacheService.RemoveAsync(loginWizardCacheKey);
                }
                else if (user.Identity.IsAuthenticated)
                {
                    if (this.configuration.GetValue<string>("Settings:EnableTempDebugging") != null
                        && this.configuration.GetValue<string>("Settings:EnableTempDebugging") == "true")
                    {
                        this.logger.LogError("Temp Debugging: LoginWizardFilter > OnActionExecutionAsync User is authenticated. UserId=" + user.Identity.GetCurrentUserId().ToString());
                    }

                    var currentUser = await this.userService.GetUserByUserIdAsync(user.Identity.GetCurrentUserId());
                    bool wizardInProgress = currentUser.LoginWizardInProgress;

                    var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<string>(loginWizardCacheKey);

                    if (cacheExists == false && wizardInProgress)
                    {
                        await this.cacheService.SetAsync(loginWizardCacheKey, "start");
                    }

                    (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<string>(loginWizardCacheKey);

                    if (cacheExists && (user.IsInRole("Administrator") || user.IsInRole("BlueUser") || user.IsInRole("ReadOnly") || user.IsInRole("BasicUser")))
                    {
                        if (loginWizard == "start")
                        {
                            var userId = user.Identity.GetCurrentUserId();
                            await this.userSessionHelper.StartSession(userId);

                            var wizard = await this.loginWizardService.GetLoginWizard(userId);
                            if (wizard.LoginWizardStages.Count > 0 || wizardInProgress)
                            {
                                await this.loginWizardService.StartLoginWizard(userId);
                                await this.cacheService.SetAsync(loginWizardCacheKey, wizard);
                                context.Result = new RedirectToRouteResult(
                                                        new RouteValueDictionary
                                                        {
                                                            { "Controller", "LoginWizard" },
                                                            { "Action", "Index" },
                                                            { "returnUrl", returnUrl },
                                                        });
                                executeNextAction = false;
                            }
                            else
                            {
                                await this.cacheService.RemoveAsync(loginWizardCacheKey);
                            }
                        }
                        else
                        {
                            context.Result = new RedirectToRouteResult(
                                                new RouteValueDictionary
                                                {
                                                    { "Controller", "LoginWizard" },
                                                    { "Action", "Index" },
                                                    { "returnUrl", returnUrl },
                                                });
                            executeNextAction = false;
                        }
                    }
                }
            }

            if (executeNextAction)
            {
                await next();
            }
        }
    }
}
