// <copyright file="ICatalogueService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// Defines the <see cref="ICatalogueService" />.
    /// </summary>
    public interface ICatalogueService
    {
        /// <summary>
        /// The CreateCatalogueAsync.
        /// </summary>
        /// <param name="catalogue">The catalogue<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<ApiResponse> CreateCatalogueAsync(CatalogueViewModel catalogue);

        /// <summary>
        /// The GetCatalogueAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{CatalogueViewModel}"/>.</returns>
        Task<CatalogueViewModel> GetCatalogueAsync(int id);

        /// <summary>
        /// The GetCataloguesAsync.
        /// </summary>
        /// <param name="searchTerm">The searchTerm<see cref="string"/>.</param>
        /// <returns>The <see cref="List{CatalogueViewModel}"/>.</returns>
        Task<List<CatalogueViewModel>> GetCataloguesAsync(string searchTerm);

        /// <summary>
        /// The GetResourcesAsync.
        /// </summary>
        /// <param name="catalogueId">The catalogueId<see cref="int"/>.</param>
        /// <param name="request">The request<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="Task{CatalogueResourcesViewModel}"/>.</returns>
        Task<CatalogueResourcesViewModel> GetResourcesAsync(int catalogueId, PagingRequestModel request);

        /// <summary>
        /// The HideCatalogueAsync.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HideCatalogueAsync(int nodeId);

        /// <summary>
        /// The ShowCatalogueAsync.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> ShowCatalogueAsync(int nodeId);

        /// <summary>
        /// The UpdateCatalogueAsync.
        /// </summary>
        /// <param name="catalogue">The catalogue<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> UpdateCatalogueAsync(CatalogueViewModel catalogue);

        /// <summary>
        /// The UpdateCatalogueOwnerAsync.
        /// </summary>
        /// <param name="catalogueOwner">The catalogue owner<see cref="CatalogueOwnerViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> UpdateCatalogueOwnerAsync(CatalogueOwnerViewModel catalogueOwner);
    }
}
