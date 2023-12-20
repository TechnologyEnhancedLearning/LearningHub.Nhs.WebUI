// <copyright file="IPageRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Content
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.Repository.Interface;

    /// <summary>
    /// The IPageRepository.
    /// </summary>
    public interface IPageRepository : IGenericRepository<Page>
    {
        /// <summary>
        /// GetPagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<Page>> GetPagesAsync();

        /// <summary>
        /// The GetPageById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="publishedOnly">The published only<see cref="bool"/>.</param>
        /// <param name="preview">Preview mode.</param>
        /// <returns>The <see cref="Task{Page}"/>.</returns>
        Task<Page> GetPageByIdAsync(int id, bool publishedOnly, bool preview);

        /// <summary>
        /// The DiscardAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DiscardAsync(int pageId, int currentUserId);

        /// <summary>
        /// The PublishAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task PublishAsync(int pageId, int currentUserId);
    }
}
