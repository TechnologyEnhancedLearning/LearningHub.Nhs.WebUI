// <copyright file="IPublicationRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;

    /// <summary>
    /// The PublicationRepository interface.
    /// </summary>
    public interface IPublicationRepository : IGenericRepository<Publication>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Publication> GetByIdAsync(int id);

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Publication GetById(int id);

        /// <summary>
        /// The get by resourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The Publication.</returns>
        Task<Publication> GetByResourceVersionIdAsync(int resourceVersionId);

        /// <summary>
        /// Get cache operations for the supplied publication id.
        /// </summary>
        /// <param name="publicationId">The publicationId.</param>
        /// <returns>A list of <see cref="CacheOperationViewModel"/>.</returns>
        Task<List<CacheOperationViewModel>> GetCacheOperations(int publicationId);

        /// <summary>
        /// Get resource first publication record.
        /// </summary>
        /// <param name="resourceId">resource id.</param>
        /// <returns>publish view model.</returns>
        Task<Publication> GetResourceFirstPublication(int resourceId);
    }
}
