// <copyright file="IProviderService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>
namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Validation;

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
        /// Get providers by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<ProviderViewModel>> GetProvidersByUserIdAsync(int userId);

        /// <summary>
        /// The update providers by user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="providerIdList">The provider id list.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserProviderAsync(int userId, string providerIdList);
    }
}
