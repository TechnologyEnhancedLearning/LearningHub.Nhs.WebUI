namespace LearningHub.Nhs.WebUI.Models.SideMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public Func<RouteValueDictionary, bool>? IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating Children.
        /// </summary>
        public List<SideNavigationItem> Children { get; set; } = [];

        /// <summary>
        /// Gets a value indicating whether gets a value indicating value indicating HasChildren.
        /// </summary>
        public bool HasChildren => this.Children?.Any() == true;
    }
}
