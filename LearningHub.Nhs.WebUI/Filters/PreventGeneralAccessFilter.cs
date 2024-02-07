// <copyright file="PreventGeneralAccessFilter.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Filters
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Defines the <see cref="PreventGeneralAccessFilter" />.
    /// </summary>
    public class PreventGeneralAccessFilter : ActionFilterAttribute
    {
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

            if (!(controller == "Account" && action == "InvalidUserAccount")
                && !(controller == "Home" && action == "Logout")
                && user.Identity.IsAuthenticated && !user.IsInRole("Administrator") && !user.IsInRole("BlueUser") && !user.IsInRole("ReadOnly") && (controller != "Tracking"))
            {
                context.Result = new RedirectToRouteResult(
                                        new RouteValueDictionary
                                        {
                                            { "Controller", "Account" },
                                            { "Action", "InvalidUserAccount" },
                                        });
            }
            else
            {
                await next();
            }
        }
    }
}
