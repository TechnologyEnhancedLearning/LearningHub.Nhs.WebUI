﻿namespace LearningHub.Nhs.Repository.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource sync repository.
    /// </summary>
    public class ResourceSyncRepository : GenericRepository<ResourceSync>, IResourceSyncRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSyncRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbcontext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceSyncRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetSyncListForUser.
        /// </summary>
        /// <param name="userId">The userid.</param>
        /// <param name="includeResources">If the resource property should be populated.</param>
        /// <returns>The sync list for the user.</returns>
        public IQueryable<ResourceSync> GetSyncListForUser(int userId, bool includeResources)
        {
            var resourceSyncs = this.GetAll();
            if (includeResources)
            {
                resourceSyncs = resourceSyncs
                    .Include(x => x.Resource).ThenInclude(x => x.CreateUser)
                    .Include(x => x.Resource).ThenInclude(x => x.Resource).ThenInclude(r => r.ResourceReference)
                    .Include(x => x.Resource).ThenInclude(x => x.ResourceVersionKeyword)
                    .Include(x => x.Resource).ThenInclude(x => x.ResourceVersionAuthor)
                    .Include(x => x.Resource).ThenInclude(x => x.ResourceVersionRatingSummary)
                    .Include(x => x.Resource).ThenInclude(x => x.Publication);
            }

            return resourceSyncs.Where(x => x.UserId == userId);
        }

        /// <summary>
        /// The AddToSyncListAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        public async Task AddToSyncListAsync(int userId, List<int> resourceIds)
        {
            foreach (var resourceId in resourceIds)
            {
                var resourceSync = new ResourceSync { ResourceId = resourceId, UserId = userId };
                await this.CreateAsync(userId, resourceSync);
            }
        }

        /// <summary>
        /// The RemoveFromSyncListAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        public async Task RemoveFromSyncListAsync(int userId, List<int> resourceIds)
        {
            var syncsToRemove = this.GetAll().Where(x => x.UserId == userId && resourceIds.Contains(x.ResourceId)).ToList();
            foreach (var sync in syncsToRemove)
            {
                this.SetAuditFieldsForDelete(userId, sync);
                try
                {
                    await this.UpdateAsync(userId, sync);
                }
                catch (Exception ex)
                {
                    var a = ex;
                    throw a;
                }
            }
        }

        /// <summary>
        /// The SetSyncedForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The task.</returns>
        public async Task SetSyncedForUserAsync(int userId)
        {
            var resourceSyncs = this.GetSyncListForUser(userId, false).ToList();
            foreach (var sync in resourceSyncs)
            {
                this.SetAuditFieldsForDelete(userId, sync);
                await this.UpdateAsync(userId, sync);
            }
        }
    }
}
