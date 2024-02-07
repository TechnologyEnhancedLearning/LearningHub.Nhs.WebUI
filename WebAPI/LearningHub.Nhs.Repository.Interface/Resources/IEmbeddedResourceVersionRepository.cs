// <copyright file="IEmbeddedResourceVersionRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The EmbeddedResourceVersionRepository interface.
    /// </summary>
    public interface IEmbeddedResourceVersionRepository : IGenericRepository<EmbeddedResourceVersion>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<EmbeddedResourceVersion> GetByIdAsync(int id);
    }
}
