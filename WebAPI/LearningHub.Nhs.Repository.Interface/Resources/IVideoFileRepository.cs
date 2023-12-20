// <copyright file="IVideoFileRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The FileRepository interface.
    /// </summary>
    public interface IVideoFileRepository : IGenericRepository<VideoFile>
    {
        /// <summary>
        /// The get by File Id async.
        /// </summary>
        /// <param name="fileId">The File Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<VideoFile> GetByFileIdAsync(int fileId);
    }
}
