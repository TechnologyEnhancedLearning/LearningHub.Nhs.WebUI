// <copyright file="IVideoRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;

    /// <summary>
    /// The video repository interface.
    /// </summary>
    public interface IVideoRepository : IGenericRepository<Video>
    {
        /// <summary>
        /// The get by file Id async.
        /// </summary>
        /// <param name="fileId">The file Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Video> GetByFileIdAsync(int fileId);
    }
}