namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// Defines the <see cref="INavigationRequestService" />.
    /// </summary>
    public interface INavigationRequestService
    {
        /// <summary>
        /// GetNavigationModelAsync.
        /// </summary>
        /// <param name="user">user.</param>
        /// <param name="forceRefresh">forceRefresh.</param>
        /// <param name="controllerName">controllerName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<NavigationModel> GetNavigationModelAsync(IPrincipal user, bool forceRefresh, string controllerName);

        /// <summary>
        /// GetNotificationCountAsync.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> GetNotificationCountAsync(int userId);
    }
}
