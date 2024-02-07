// <copyright file="IVideoResourceVersionRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The VideoResourceVersionRepository interface.
    /// </summary>
    public interface IVideoResourceVersionRepository : IGenericRepository<VideoResourceVersion>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<VideoResourceVersion> GetByIdAsync(int id);

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<VideoResourceVersion> GetByResourceVersionIdAsync(int id, bool includeDeleted = false);
    }
}
