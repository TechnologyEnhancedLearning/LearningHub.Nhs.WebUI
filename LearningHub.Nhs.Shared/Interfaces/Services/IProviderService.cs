namespace LearningHub.Nhs.Shared.Interfaces.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Provider;

    /// <summary>
    /// Defines the <see cref="IProviderService" />.
    /// </summary>
    public interface IProviderService
    {
        /// <summary>
        /// Get Providers.
        /// </summary>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<ProviderViewModel>> GetProviders();

        /// <summary>
        /// Get providers for the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<ProviderViewModel>> GetProvidersForUserAsync(int userId);

        /// <summary>
        /// Get providers for the resource.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<ProviderViewModel>> GetProvidersForResourceAsync(int resourceVersionId);

        /// <summary>
        /// Get provider by id.
        /// </summary>
        /// <param name="providerId">The provider id.</param>
        /// <returns>The <see cref="Task{ProviderViewModel}"/>.</returns>
        ProviderViewModel GetProviderById(int providerId);
    }
}
