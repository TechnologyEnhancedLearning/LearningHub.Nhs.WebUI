namespace LearningHub.Nhs.WebUI.Models.SideMenu
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="SideNavigationGroup" />.
    /// </summary>
    public class SideNavigationGroup
    {
        /// <summary>
        /// Gets or sets a value indicating GroupTitle.
        /// </summary>
        public string GroupTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a Items.
        /// </summary>
        public List<SideNavigationItem> Items { get; set; } = [];
    }
}
