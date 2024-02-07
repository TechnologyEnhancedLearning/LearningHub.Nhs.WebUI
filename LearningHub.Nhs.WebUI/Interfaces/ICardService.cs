// <copyright file="ICardService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;

    /// <summary>
    /// The card service interface.
    /// </summary>
    public interface ICardService
    {
        /// <summary>
        /// Get contributions totals.
        /// </summary>
        /// <param name="catalogueId">The catalogue Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyContributionsTotalsViewModel> GetMyContributionsTotalsAsync(int catalogueId);

        /// <summary>
        /// Get contributions based on user and catalogue.
        /// </summary>
        /// <param name="resourceContributionsRequestViewModel">The resource contributions request viewmodel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<ContributedResourceCardViewModel>> GetContributionsAsync(ResourceContributionsRequestViewModel resourceContributionsRequestViewModel);

        /// <summary>
        /// Get my resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyResourceViewModel> GetMyResourceViewModelAsync();

        /// <summary>
        /// Get resource for extended card.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceCardExtendedViewModel> GetResourceCardExtendedViewModelAsync(int id);
    }
}
