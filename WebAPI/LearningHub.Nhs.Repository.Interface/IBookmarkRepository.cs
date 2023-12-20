// <copyright file="IBookmarkRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The IRoadmapRepository.
    /// </summary>
    public interface IBookmarkRepository : IGenericRepository<UserBookmark>
    {
        /// <summary>
        /// The GetById.
        /// </summary>
        /// <param name="bookmarkId">The bookmarkId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserBookmark}"/>.</returns>
        Task<UserBookmark> GetById(int bookmarkId);

        /// <summary>
        /// DeleteFolder.
        /// </summary>
        /// <param name="bookmarkId">bookmarkId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteFolder(int bookmarkId, int userId);
    }
}
