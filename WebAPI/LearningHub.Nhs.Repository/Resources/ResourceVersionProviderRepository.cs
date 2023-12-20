// <copyright file="ResourceVersionProviderRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// The resource version provider repository.
    /// </summary>
    public class ResourceVersionProviderRepository : GenericRepository<ResourceVersionProvider>, IResourceVersionProviderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionProviderRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionProviderRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Delete resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="providerId">The providerId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int resourceVersionId, int providerId, int userId)
        {
            var amendDate = DateTimeOffset.Now;

            var resourceVersionProvider = this.DbContext.ResourceVersionProvider
                    .Where(r => r.ResourceVersionId == resourceVersionId && r.ProviderId == providerId)
                    .SingleOrDefault();

            if (resourceVersionProvider != null)
            {
                this.SetAuditFieldsForDelete(userId, resourceVersionProvider);

                await this.DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Delete all resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAllAsync(int resourceVersionId,  int userId)
        {
            var amendDate = DateTimeOffset.Now;

            var resourceVersionProviderList = this.DbContext.ResourceVersionProvider
                    .Where(r => r.ResourceVersionId == resourceVersionId && !r.Deleted).ToList();

            if (resourceVersionProviderList != null)
            {
                foreach (var resourceVersionProvider in resourceVersionProviderList)
                {
                    this.SetAuditFieldsForDelete(userId, resourceVersionProvider);
                    await this.UpdateAsync(userId, resourceVersionProvider);
                }
            }
        }
    }
}
