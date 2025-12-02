namespace LearningHub.Nhs.WebUI.Services
{
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// NavigationRequestService.
    /// </summary>
    public class NavigationRequestService : INavigationRequestService
    {
        private NavigationModel navigationModel;
        private int? notificationCount;

        private INotificationService notificationService;
        private INavigationPermissionService permissionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationRequestService"/> class.
        /// </summary>
        /// <param name="permissionService">permissionService.</param>
        /// <param name="notificationService">notificationService.</param>
        public NavigationRequestService(
            INavigationPermissionService permissionService,
            INotificationService notificationService)
        {
            this.permissionService = permissionService;
            this.notificationService = notificationService;
        }

        /// <summary>
        /// The GetNavigationModelAsync method.
        /// </summary>
        /// <param name="user">user.</param>
        /// <param name="forceRefresh">forceRefresh.</param>
        /// <param name="controllerName">controllerName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<NavigationModel> GetNavigationModelAsync(
            IPrincipal user,
            bool forceRefresh,
            string controllerName)
        {
            if (this.navigationModel != null)
            {
                return this.navigationModel;
            }

            this.navigationModel = await this.permissionService.GetNavigationModelAsync(user, forceRefresh, controllerName);
            return this.navigationModel;
        }

        /// <summary>
        /// The GetNotificationCountAsync.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<int> GetNotificationCountAsync(int userId)
        {
            if (this.notificationCount.HasValue)
            {
                return this.notificationCount.Value;
            }

            this.notificationCount = await this.notificationService.GetUserUnreadNotificationCountAsync(userId);
            return this.notificationCount.Value;
        }
    }
}
