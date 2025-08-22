namespace LearningHub.Nhs.WebUI.Models.SideMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Defines the <see cref="SideNavigationConfiguration" />.
    /// </summary>
    public static class SideNavigationConfiguration
    {
        /// <summary>
        /// GetGroupedMenus.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable<SideNavigationGroup> GetGroupedMenus()
        {
            return new List<SideNavigationGroup>
        {
            new SideNavigationGroup
            {
                GroupTitle = "Account",
                Items = new List<SideNavigationItem>
                {
                    new SideNavigationItem
                    {
                        Text = "Personal details",
                        Controller = "MyAccount",
                        Action = "Index",
                        IsActive = route => MatchRoute(route, "MyAccount", "Index"),
                    },
                    new SideNavigationItem
                    {
                        Text = "My employment",
                        Controller = "MyAccount",
                        Action = "MyEmploymentDetails",
                        IsActive = route => MatchRoute(route, "MyAccount", "MyEmploymentDetails"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Security",
                        Controller = "MyAccount",
                        Action = "MyAccountSecurity",
                        IsActive = route => MatchRoute(route, "MyAccount", "MyAccountSecurity"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Notification",
                        Controller = "Notification",
                        Action = "Index",
                        IsActive = route => MatchRoute(route, "Notification", "Index"),
                    },
                },
            },
            new SideNavigationGroup
            {
                GroupTitle = "Activity",
                Items = new List<SideNavigationItem>
                {
                    new SideNavigationItem
                    {
                        Text = "Recent learning",
                        Controller = "MyLearning",
                        Action = "Index",
                        IsActive = route => MatchRoute(route, "MyLearning", "Index"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Bookmarks",
                        Controller = "Bookmark",
                        Action = "Index",
                        IsActive = route => MatchRoute(route, "Bookmark", "Index"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Certificates",
                        Controller = "MyLearning",
                        Action = "Certificates",
                        IsActive = route => MatchRoute(route, "MyLearning", "Certificates"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Learning history",
                        Controller = "MyLearning",
                        Action = "LearningHistory",
                        IsActive = route => MatchRoute(route, "MyLearning", "LearningHistory"),
                    },
                },
            },
        };
        }

        /// <summary>
        /// GetMenuGroupByTitle.
        /// </summary>
        /// <param name="title">title.</param>
        /// <returns>string.</returns>
        public static SideNavigationGroup? GetMenuGroupByTitle(string title)
        {
            return GetGroupedMenus().FirstOrDefault(g =>
                string.Equals(g.GroupTitle, title, StringComparison.OrdinalIgnoreCase));
        }

        private static bool MatchRoute(RouteValueDictionary route, string controller, string action)
        {
            var currentController = route["controller"]?.ToString();
            var currentAction = route["action"]?.ToString();

            return string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase);
        }
    }
}
