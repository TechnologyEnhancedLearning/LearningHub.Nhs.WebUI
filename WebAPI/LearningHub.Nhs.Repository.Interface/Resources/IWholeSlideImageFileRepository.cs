// <copyright file="IWholeSlideImageFileRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The FileRepository interface.
    /// </summary>
    public interface IWholeSlideImageFileRepository : IGenericRepository<WholeSlideImageFile>
    {
        /// <summary>
        /// The get by File Id async.
        /// </summary>
        /// <param name="fileId">The File Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<WholeSlideImageFile> GetByFileIdAsync(int fileId);
    }
}
