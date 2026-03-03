namespace LearningHub.Nhs.WebUI.Models.SideMenu
{
    using System;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Defines the <see cref="SideNavigationItem" />.
    /// </summary>
    public class SideNavigationItem
    {
        /// <summary>
        /// Gets or sets a value indicating Text.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating Controller.
        /// </summary>
        public string Controller { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating Action.
        /// </summary>
        public string Action { get; set; } = "Index";

        /// <summary>
        /// Gets or sets a value indicating IsActiven.
        /// </summary>
        public Func<RouteValueDictionary, bool> IsActive { get; set; }
    }
}
