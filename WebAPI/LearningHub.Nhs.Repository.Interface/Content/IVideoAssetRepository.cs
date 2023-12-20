// <copyright file="IVideoAssetRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Content
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Content;

    /// <summary>
    /// Defines the <see cref="IVideoAssetRepository" />.
    /// </summary>
    public interface IVideoAssetRepository : IGenericRepository<VideoAsset>
    {
        /// <summary>
        /// The GetByPageSectionDetailId.
        /// </summary>
        /// <param name="pageSectionDetailId">The pageSectionDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VideoAsset}"/>.</returns>
        Task<VideoAsset> GetByPageSectionDetailId(int pageSectionDetailId);

        /// <summary>
        /// The GetByPageSectionDetailId.
        /// </summary>
        /// <param name="id">The videoAssetId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VideoAsset}"/>.</returns>
        Task<VideoAsset> GetById(int id);
    }
}
