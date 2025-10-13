namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Security.Principal;
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// Defines the <see cref="INavigationPermissionService" />.
    /// </summary>
    public interface INavigationPermissionService
    {
        /// <summary>
        /// The GetNavigationModelAsync.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="loginWizardComplete">Login wizard complete.</param>
        /// <param name="controllerName">Controller name.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<NavigationModel> GetNavigationModelAsync(IPrincipal user, bool loginWizardComplete, string controllerName, int currentUserId);

        /// <summary>
        /// The NotAuthenticated.
        /// </summary>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        NavigationModel NotAuthenticated();
    }
}
