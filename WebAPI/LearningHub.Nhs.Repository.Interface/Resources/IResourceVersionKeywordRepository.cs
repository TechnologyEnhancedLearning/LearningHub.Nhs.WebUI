// <copyright file="IResourceVersionKeywordRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ResourceVersionKeywordRepository interface.
    /// </summary>
    public interface IResourceVersionKeywordRepository : IGenericRepository<ResourceVersionKeyword>
    {
        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionKeywordId">The resource version keyword id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int userId, int resourceVersionKeywordId);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="keywordId">The keyword id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersionKeyword> GetByResourceVersionAndKeywordAsync(int resourceVersionId, int keywordId);

        /// <summary>
        /// Check if a specific keyword exists for the Resource Version.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> DoesResourceVersionKeywordAlreadyExistAsync(int resourceVersionId, string keyword);
    }
}
