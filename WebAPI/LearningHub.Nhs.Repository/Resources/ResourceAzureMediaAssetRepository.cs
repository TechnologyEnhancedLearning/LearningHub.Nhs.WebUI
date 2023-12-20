// <copyright file="ResourceAzureMediaAssetRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resourceazuremediaasset repository.
    /// </summary>
    public class ResourceAzureMediaAssetRepository : GenericRepository<ResourceAzureMediaAsset>, IResourceAzureMediaAssetRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAzureMediaAssetRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceAzureMediaAssetRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceAzureMediaAsset> GetByIdAsync(int id)
        {
            return await this.DbContext.ResourceAzureMediaAsset.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id && !f.Deleted);
        }
    }
}
