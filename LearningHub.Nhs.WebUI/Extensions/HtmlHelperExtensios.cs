// <copyright file="HtmlHelperExtensios.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Defines the <see cref="HtmlHelperExtensios" />.
    /// </summary>
    public static class HtmlHelperExtensios
    {
        private const string SelectedCssClass = "nhsuk-header__navigation-item--current";

        /// <summary>
        /// Set item selected.
        /// </summary>
        /// <param name="htmlHelper">htmlhelper.</param>
        /// <param name="controller">controller.</param>
        /// <param name="action">action.</param>
        /// <returns>string.</returns>
        public static string IsSelected(
            this IHtmlHelper htmlHelper,
            string? controller = null,
            string? action = null)
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            List<string> controllerList = controller?.Split(',').ToList();

            return (controllerList != null && controllerList.Any(s => s.Trim().ToLower() == currentController.ToLower()) && (action == null || action == currentAction))
                ? SelectedCssClass
                : string.Empty;
        }
    }
}
