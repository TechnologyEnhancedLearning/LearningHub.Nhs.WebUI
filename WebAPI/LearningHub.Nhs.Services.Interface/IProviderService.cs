// <copyright file="IProviderService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Provider;

    /// <summary>
    /// The Provider Service interface.
    /// </summary>
    public interface IProviderService
    {
        /// <summary>
        /// Get all Providers.
        /// </summary>
        /// <returns>List of providers.</returns>
        Task<List<ProviderViewModel>> GetAllAsync();

        /// <summary>
        /// Get provider by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>provider.</returns>
        Task<ProviderViewModel> GetByIdAsync(int id);

        /// <summary>
        /// Get providers by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>provider.</returns>
        Task<List<ProviderViewModel>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Get providers by resource version id.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ProviderViewModel>> GetByResourceVersionIdAsync(int resourceVersionId);

        /// <summary>
        /// Get providers by node version id.
        /// </summary>
        /// <param name="nodeVersionId">node version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ProviderViewModel>> GetByCatalogueVersionIdAsync(int nodeVersionId);
    }
}
