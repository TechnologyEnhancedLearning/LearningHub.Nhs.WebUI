// <copyright file="INavigationPermissionService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Security.Principal;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Models;

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
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<NavigationModel> GetNavigationModelAsync(IPrincipal user, bool loginWizardComplete, string controllerName);

        /// <summary>
        /// The NotAuthenticated.
        /// </summary>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        NavigationModel NotAuthenticated();
    }
}
