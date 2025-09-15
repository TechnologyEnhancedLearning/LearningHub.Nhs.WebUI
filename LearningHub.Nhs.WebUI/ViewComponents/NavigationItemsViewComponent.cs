namespace LearningHub.Nhs.WebUI.ViewComponents
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.WebUtilitiesInterfaces;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="NavigationItemsViewComponent" />.
    /// </summary>
    public class NavigationItemsViewComponent : ViewComponent
    {
        private readonly ICacheService cacheService;
        private INotificationService notificationService;
        private INavigationPermissionService permissionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItemsViewComponent"/> class.
        /// </summary>
        /// <param name="permissionService">Permission service.</param>
        /// <param name="notificationService">Notificatin service.</param>
        /// <param name="cacheService">The cacheService<see cref="ICacheService"/>.</param>
        public NavigationItemsViewComponent(INavigationPermissionService permissionService, INotificationService notificationService, ICacheService cacheService)
        {
            this.permissionService = permissionService;
            this.notificationService = notificationService;
            this.cacheService = cacheService;
        }

        /// <summary>
        /// The Display.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Display()
        {
            return true;
        }

        /// <summary>
        /// The InvokeAsync.
        /// </summary>
        /// <param name="navView">Navigation View - "Default", "Topnav" or "Searchbar".</param>
        /// <param name="controllerName">Controller name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IViewComponentResult> InvokeAsync(string navView = "Default", string controllerName = "")
        {
            NavigationModel model;

            if (!this.User.Identity.IsAuthenticated)
            {
                model = this.permissionService.NotAuthenticated();
            }
            else
            {
                var userId = this.User.Identity.GetCurrentUserId();

                var (cacheExists, _) = await this.cacheService.TryGetAsync<string>($"{userId}:LoginWizard");

                model = await this.permissionService.GetNavigationModelAsync(this.User, !cacheExists, controllerName);

                model.NotificationCount = await this.notificationService.GetUserUnreadNotificationCountAsync(userId);
            }

            return await Task.FromResult<IViewComponentResult>(this.View(navView, model));
        }
    }
}