// <copyright file="IArticleResourceVersionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ArticleResourceVersionRepository interface.
    /// </summary>
    public interface IArticleResourceVersionRepository : IGenericRepository<ArticleResourceVersion>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ArticleResourceVersion> GetByIdAsync(int id);

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ArticleResourceVersion> GetByResourceVersionIdAsync(int id, bool includeDeleted = false);
    }
}
