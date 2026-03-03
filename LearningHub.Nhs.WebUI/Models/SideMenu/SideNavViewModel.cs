namespace LearningHub.Nhs.WebUI.Models.SideMenu
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Defines the <see cref="SideNavViewModel" />.
    /// </summary>
    public class SideNavViewModel
    {
        /// <summary>
        /// Gets or sets the Groups.
        /// </summary>
        public List<SideNavigationGroup> Groups { get; set; } = [];

        /// <summary>
        /// Gets or sets the RouteData.
        /// </summary>
        public RouteValueDictionary RouteData { get; set; } = [];
    }
}
