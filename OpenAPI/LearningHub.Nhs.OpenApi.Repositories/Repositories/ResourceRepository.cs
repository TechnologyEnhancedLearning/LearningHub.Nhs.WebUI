namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
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

        /// <inheritdoc />
        public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds)
        {
            var resources = await this.dbContext.Resource
                .Where(r => resourceIds.Contains(r.Id))
                .Where(r => !r.Deleted)
                .Include(r => r.ResourceReference)
                .ThenInclude(rr => rr.NodePath)
                .ThenInclude(np => np.CatalogueNode)
                .ThenInclude(n => n.CurrentNodeVersion)
                .ThenInclude(n => n.CatalogueNodeVersion)
                .Include(r => r.CurrentResourceVersion)
                .ThenInclude(r => r.ResourceVersionRatingSummary)
                .Include(r => r.ResourceReference)
                .ThenInclude(rr => rr.NodePath)
                .ThenInclude(np => np.Node)
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
            IEnumerable<int> originalResourceReferenceIds)
        {
            return await this.dbContext.ResourceReference
                .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId))
                .Where(rr => !rr.Deleted)
                .Where(rr => (int)rr.NodePath.Node.NodeTypeEnum != 4)
                .Include(rr => rr.NodePath)
                .ThenInclude(np => np.CatalogueNode)
                .ThenInclude(n => n.CurrentNodeVersion)
                .ThenInclude(n => n.CatalogueNodeVersion)
                .Include(rr => rr.Resource)
                .ThenInclude(r => r.CurrentResourceVersion)
                .ThenInclude(r => r.ResourceVersionRatingSummary)
                .ToListAsync();
        }
    }
}
