namespace LearningHub.Nhs.WebUI.Filters
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Defines the <see cref="ReporterPermissionFilter" />.
    /// </summary>
    public class ReporterPermissionFilter : ActionFilterAttribute
    {
        private readonly IReportService reportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReporterPermissionFilter"/> class.
        /// </summary>
        /// <param name="reportService">reportService.</param>
        public ReporterPermissionFilter(IReportService reportService)
        {
            this.reportService = reportService;
        }

        /// <inheritdoc/>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.ActionDescriptor.RouteValues["Controller"];
            var action = context.ActionDescriptor.RouteValues["Action"];

            // Run your cached permission check
            var hasPermission = await this.reportService.GetReporterPermission();

            // If user does NOT have permission and they're not in safe routes
            if (!hasPermission
                && !(controller == "Account" && action == "AccessRestricted")
                && !(controller == "Home" && action == "Logout"))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                    { "Controller", "Account" },
                    { "Action", "AccessRestricted" },
                    });

                return;
            }

            await next();
        }
    }
}
