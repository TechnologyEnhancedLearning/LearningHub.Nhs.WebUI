namespace LearningHub.Nhs.Repository.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource reference repository.
    /// </summary>
    public class ResourceReferenceRepository : GenericRepository<ResourceReference>, IResourceReferenceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceReferenceRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeNodePath">The include NodePath.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceReference> GetByIdAsync(int id, bool includeNodePath)
        {
            if (includeNodePath)
            {
                return await this.DbContext.ResourceReference
                    .Include(r => r.NodePath)
                    .ThenInclude(r => r.CatalogueNode)
                    .ThenInclude(r => r.CurrentNodeVersion)
                    .ThenInclude(r => r.CatalogueNodeVersion).AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
            }
            else
            {
                return await this.DbContext.ResourceReference
                    .Include(r => r.Resource)
                    .AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
            }
        }

        /// <summary>
        /// The get by original resource reference id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeNodePath">The include NodePath.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceReference> GetByOriginalResourceReferenceIdAsync(int id, bool includeNodePath)
        {
            if (includeNodePath)
            {
                return await this.DbContext.ResourceReference
                    .Include(r => r.NodePath)
                    .ThenInclude(r => r.CatalogueNode)
                    .ThenInclude(r => r.CurrentNodeVersion)
                    .ThenInclude(r => r.CatalogueNodeVersion)
                    .OrderByDescending(r => r.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.OriginalResourceReferenceId == id && !r.Deleted);
            }
            else
            {
                return await this.DbContext.ResourceReference
                    .Include(r => r.Resource)
                    .OrderByDescending(r => r.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.OriginalResourceReferenceId == id && !r.Deleted);
            }
        }

        /// <summary>
        /// The get default by resource id async.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceReference> GetDefaultByResourceIdAsync(int resourceId)
        {
            return await this.DbContext.ResourceReference
                .Include(r => r.NodePath)
                .ThenInclude(r => r.CatalogueNode)
                .AsNoTracking()
                .Where(rr => rr.ResourceId == resourceId && rr.NodePath.CatalogueNode != null).OrderByDescending(r => r.Id).FirstOrDefaultAsync();
        }
    }
}
