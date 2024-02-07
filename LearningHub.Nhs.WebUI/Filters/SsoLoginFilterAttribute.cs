// <copyright file="SsoLoginFilterAttribute.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The SsoLoginFilterAttribute.
    /// </summary>
    public class SsoLoginFilterAttribute : ActionFilterAttribute
    {
        private readonly Settings settings;
        private readonly LearningHubAuthServiceConfig authConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SsoLoginFilterAttribute"/> class.
        /// </summary>
        /// <param name="options">The IOptions.</param>
        /// <param name="authConfig">The authConfig<see cref="LearningHubAuthServiceConfig"/>.</param>
        public SsoLoginFilterAttribute(
            IOptions<Settings> options,
            LearningHubAuthServiceConfig authConfig)
        {
            this.settings = options.Value;
            this.authConfig = authConfig;
        }

        /// <summary>
        /// The OnActionExecutionAsync.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="next">The next.</param>
        /// <returns>The task.</returns>
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            var parameters = QueryHelpers.ParseQuery(request.QueryString.ToString());
            var queryParameters = parameters.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var fromAuth = queryParameters.RemoveAll(x => x.Key == "FromAuthService" && x.Value == "true") > 0;

            if (!request.Headers.ContainsKey("Referer") && !fromAuth)
            {
                await next();
            }
            else
            {
                var referer = request.Headers["Referer"].ToString();
                var ssoWhitelist = new[]
                {
                    this.settings.ELfhHubUrl,
                    this.settings.LearningHubAdminUrl,
                };

                if (
                    (fromAuth && !httpContext.User.Identity.IsAuthenticated)
                    ||
                    (ssoWhitelist.Any(x => referer.StartsWith(x)) && !httpContext.User.Identity.IsAuthenticated))
                {
                    var destinationUrl = request.Path.ToUriComponent();
                    destinationUrl = QueryHelpers.AddQueryString(destinationUrl, queryParameters);

                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                        { "Controller", "Account" },
                        { "Action", "AuthorisationRequired" },
                        { "originalUrl", destinationUrl },
                        });
                }
                else
                {
                    await next();
                }
            }
        }
    }
}
