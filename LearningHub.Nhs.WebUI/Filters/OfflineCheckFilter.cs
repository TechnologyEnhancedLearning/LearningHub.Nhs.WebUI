// <copyright file="OfflineCheckFilter.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Filters
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Defines the <see cref="OfflineCheckFilter" />.
    /// </summary>
    public class OfflineCheckFilter : ActionFilterAttribute
    {
        private readonly IInternalSystemService internalSystemService;
        private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfflineCheckFilter"/> class.
        /// </summary>
        /// <param name="internalSystemService">The cacheService<see cref="ICacheService"/>.</param>
        /// <param name="userGroupService">The userGroupService<see cref="IUserGroupService"/>.</param>
        public OfflineCheckFilter(IInternalSystemService internalSystemService, IUserGroupService userGroupService)
        {
            this.internalSystemService = internalSystemService;
            this.userGroupService = userGroupService;
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

            var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.LearningHub);

            if (controller != "Offline"
                && !(controller == "Home" && action == "Logout")
                && internalSystem.IsOffline
                && (!user.Identity.IsAuthenticated || !(await this.userGroupService.UserHasPermissionAsync("System_Offline_Bypass"))))
            {
                context.Result = new RedirectToRouteResult(
                                        new RouteValueDictionary
                                        {
                                            { "Controller", "Offline" },
                                            { "Action", "Index" },
                                        });
                executeNextAction = false;
            }

            if (executeNextAction)
            {
                await next();
            }
        }
    }
}
