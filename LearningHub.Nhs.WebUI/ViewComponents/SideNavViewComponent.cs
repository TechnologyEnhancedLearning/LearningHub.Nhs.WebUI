namespace LearningHub.Nhs.WebUI.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.WebUI.Models.SideMenu;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Initializes a new instance of the <see cref="SideNavViewComponent"/> class.
    /// </summary>
    public class SideNavViewComponent : ViewComponent
    {
        /// <summary>
        /// The Invoke.
        /// </summary>
        /// <param name="groupTitle">group Title.</param>
        /// <returns>A representing the result of the synchronous operation.</returns>
        public IViewComponentResult Invoke(string? groupTitle = null)
        {
            var routeData = this.ViewContext.RouteData.Values;
            var groups = string.IsNullOrEmpty(groupTitle)
                ? SideNavigationConfiguration.GetGroupedMenus().ToList()
                : SideNavigationConfiguration.GetMenuGroupByTitle(groupTitle) is { } singleGroup
                    ? new List<SideNavigationGroup> { singleGroup }
                    : new List<SideNavigationGroup>();

            var viewModel = new SideNavViewModel
            {
                Groups = groups,
                RouteData = routeData,
            };
            return this.View(viewModel);
        }
    }
}
