// <copyright file="ControllerExtensions.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Extensions
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Defines the <see cref="ControllerExtensions" />.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// The ViewExists.
        /// </summary>
        /// <param name="controller">The controller<see cref="ControllerBase"/>.</param>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool ViewExists(this ControllerBase controller, string name)
        {
            var services = controller.HttpContext.RequestServices;
            var viewEngine = services.GetRequiredService<ICompositeViewEngine>();
            var result = viewEngine.GetView(null, name, true);
            if (!result.Success)
            {
                result = viewEngine.FindView(controller.ControllerContext, name, true);
            }

            return result.Success;
        }
    }
}
