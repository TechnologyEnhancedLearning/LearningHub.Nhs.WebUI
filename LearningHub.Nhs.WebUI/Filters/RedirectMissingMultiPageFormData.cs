namespace LearningHub.Nhs.WebUI.Filters
{
    using System;
    using System.Threading.Tasks;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Redirects to the Index action of the current controller if there is no
    ///     TempData Guid or MultiPageFormData for the feature set.
    /// </summary>
    public class RedirectMissingMultiPageFormData : ActionFilterAttribute
    {
        private readonly MultiPageFormDataFeature feature;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ILogger<RedirectMissingMultiPageFormData> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectMissingMultiPageFormData"/> class.
        /// </summary>
        /// <param name="multiPageFormService">multiPageFormService.</param>
        /// <param name="feature">feature.</param>
        /// <param name="logger">Logger.</param>
        public RedirectMissingMultiPageFormData(
            IMultiPageFormService multiPageFormService,
            string feature,
            ILogger<RedirectMissingMultiPageFormData> logger)
        {
            this.feature = feature;
            this.multiPageFormService = multiPageFormService;
            this.logger = logger;
        }

        /// <summary>
        /// The OnActionExecutionAsync.
        /// </summary>
        /// <param name="context">The context<see cref="ActionExecutingContext"/>.</param>
        /// <param name="next">The next<see cref="ActionExecutionDelegate"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is Controller controller)
            {
                var tempDataObject = controller.TempData.Peek(this.feature.TempDataKey);
                if (tempDataObject == null || !(tempDataObject is Guid tempDataGuid))
                {
                    RedirectToIndex(context, controller);
                    return;
                }

                try
                {
                    if (!await this.multiPageFormService.FormDataExistsForGuidAndFeature(this.feature, tempDataGuid))
                    {
                        RedirectToIndex(context, controller);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError("Multiformpage error: " + ex.Message);
                    RedirectToIndex(context, controller);
                    return;
                }

                _ = await next();
            }
        }

        private static void RedirectToIndex(ActionExecutingContext context, Controller controller) =>
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = controller.ControllerContext.ActionDescriptor.ControllerName, action = "Index", }));
    }
}
