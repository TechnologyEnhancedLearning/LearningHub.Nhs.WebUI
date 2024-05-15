namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    public class ResourceRepository : IResourceRepository
    {
        private LearningHubDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRepository"/> class.
        /// </summary>
        /// <param name="dbContext"><see cref="dbContext"/>.</param>
        public ResourceRepository(LearningHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceIds">.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds)
        {
            var resources = await this.dbContext.Resource
                                                    .AsNoTracking()
                                                    .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
                                                    .Include(r => r.ResourceReference)
                                                        .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
                                                    .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary)
                                                    .Include(r => r.ResourceReference)
                                                        .ThenInclude(rr => rr.NodePath.Node)
                                                    .ToListAsync();

            resources.ForEach(r =>
            {
                var nonExternalReferences = r.ResourceReference
                    .Where(rr => rr?.NodePath?.Node?.NodeTypeEnum != null && (int)rr.NodePath.Node.NodeTypeEnum != 4)
                    .ToList();

                r.ResourceReference = nonExternalReferences;
            });

            return resources;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds, int userId)
        {
            var resources = await this.dbContext.Resource
                                                    .AsNoTracking()
                                                    .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
                                                    .Include(r => r.ResourceReference)
                                                        .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
                                                    .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary)
                                                    .Include(r => r.ResourceReference)
                                                        .ThenInclude(rr => rr.NodePath.Node)
                                                    .Include(r => r.ResourceActivity.Where(ra => ra.UserId == userId && !ra.Deleted))
                                                    .ToListAsync();

            resources.ForEach(r =>
            {
                var nonExternalReferences = r.ResourceReference
                    .Where(rr => rr?.NodePath?.Node?.NodeTypeEnum != null && (int)rr.NodePath.Node.NodeTypeEnum != 4)
                    .ToList();

                r.ResourceReference = nonExternalReferences;
            });

            return resources;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
            IEnumerable<int> originalResourceReferenceIds, int userId)
        {
            return await this.dbContext.ResourceReference
        .AsNoTracking()
        .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
                     !rr.Deleted &&
                     (int)rr.NodePath.Node.NodeTypeEnum != 4)
        .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
        .Include(rr => rr.Resource.ResourceActivity
            .Where(ra => ra.UserId == userId && !ra.Deleted))
            .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary)
            .ToListAsync();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="originalResourceReferenceIds">.</param>
        /// <returns>A <see cref="Task{ResourceReference}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
          IEnumerable<int> originalResourceReferenceIds)
        {
            return await this.dbContext.ResourceReference
                .AsNoTracking()
                .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
                         !rr.Deleted &&
                         (int)rr.NodePath.Node.NodeTypeEnum != 4)
                .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
                .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary)
                .ToListAsync();
        }
    }
}
