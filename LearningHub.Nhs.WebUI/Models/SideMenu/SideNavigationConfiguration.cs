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
                        Controller = "Account",
                        Action = "PersonalDetails",
                        IsActive = route => MatchRoute(route, "Account", "PersonalDetails"),
                    },
                    new SideNavigationItem
                    {
                        Text = "My employment",
                        Controller = "Account",
                        Action = "MyEmployment",
                        IsActive = route => MatchRoute(route, "Account", "MyEmployment"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Security",
                        Controller = "Account",
                        Action = "Security",
                        IsActive = route => MatchRoute(route, "Account", "Security"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Notification",
                        Controller = "Account",
                        Action = "Notification",
                        IsActive = route => MatchRoute(route, "Account", "Notification"),
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
                        IsActive = route => MatchRoute(route, "Activity", "Recent"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Bookmarks",
                        Controller = "MyLearning",
                        Action = "Bookmark",
                        IsActive = route => MatchRoute(route, "Activity", "Bookmark"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Certificates",
                        Controller = "MyLearning",
                        Action = "Certificates",
                        IsActive = route => MatchRoute(route, "Activity", "Certificates"),
                    },
                    new SideNavigationItem
                    {
                        Text = "Learning history",
                        Controller = "MyLearning",
                        Action = "LearningHistory",
                        IsActive = route => MatchRoute(route, "Activity", "LearningHistory"),
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
