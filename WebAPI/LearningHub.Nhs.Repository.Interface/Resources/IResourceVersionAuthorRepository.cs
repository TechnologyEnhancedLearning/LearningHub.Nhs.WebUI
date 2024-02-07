// <copyright file="IResourceVersionAuthorRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ResourceVersionAuthorRepository interface.
    /// </summary>
    public interface IResourceVersionAuthorRepository : IGenericRepository<ResourceVersionAuthor>
    {
        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionAuthorId">The resource version author id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int userId, int resourceVersionAuthorId);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="authorId">The author id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersionAuthor> GetByResourceVersionAndAuthorAsync(int resourceVersionId, int authorId);
    }
}
