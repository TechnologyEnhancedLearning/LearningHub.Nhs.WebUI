// <copyright file="ICatalogueRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// Catalogue repository interface.
    /// </summary>
    public interface ICatalogueRepository
    {
        /// <summary>
        /// Gets all catalogues.
        /// </summary>
        /// <returns>CatalogueNodeVersions.</returns>
        public Task<IEnumerable<CatalogueNodeVersion>> GetAllCatalogues();
    }
}