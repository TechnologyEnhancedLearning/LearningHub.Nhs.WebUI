namespace LearningHub.Nhs.WebUI.Filters
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.FeatureManagement;

    /// <summary>
    /// Defines the <see cref="ReporterPermissionFilter" />.
    /// </summary>
    public class ReporterPermissionFilter : ActionFilterAttribute
    {
        private readonly IReportService reportService;
        private IFeatureManager featureManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReporterPermissionFilter"/> class.
        /// </summary>
        /// <param name="reportService">reportService.</param>
        /// <param name="featureManager">featureManager.</param>
        public ReporterPermissionFilter(IReportService reportService, IFeatureManager featureManager)
        {
            this.reportService = reportService;
            this.featureManager = featureManager;
        }

        /// <inheritdoc/>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.ActionDescriptor.RouteValues["Controller"];
            var action = context.ActionDescriptor.RouteValues["Action"];

            // Check if in-platform report is active.
            var reportActive = Task.Run(() => this.featureManager.IsEnabledAsync(FeatureFlags.InPlatformReport)).Result;

            // Run your cached permission check
            var hasPermission = await this.reportService.GetReporterPermission();

            // If user does NOT have permission and they're not in safe routes
            if (!hasPermission && !reportActive
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
