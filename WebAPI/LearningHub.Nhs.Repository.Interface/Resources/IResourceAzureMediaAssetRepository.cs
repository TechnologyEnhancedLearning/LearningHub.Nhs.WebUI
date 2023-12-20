// <copyright file="IResourceAzureMediaAssetRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    ///  The ResourceAzureMediaAssetRepository interface.
    /// </summary>
    public interface IResourceAzureMediaAssetRepository : IGenericRepository<ResourceAzureMediaAsset>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceAzureMediaAsset> GetByIdAsync(int id);
    }
}
