namespace LearningHub.Nhs.Repository.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version Event repository.
    /// </summary>
    public class ResourceVersionEventRepository : GenericRepository<ResourceVersionEvent>, IResourceVersionEventRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionEventRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionEventRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<ResourceVersionEvent> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return this.DbContext.ResourceVersionEvent.Where(r => r.ResourceVersionId == resourceVersionId && !r.Deleted).AsNoTracking()
                .Include(r => r.CreateUser)
                .OrderBy(r => r.Id)
                .AsNoTracking();
        }

        /// <inheritdoc/>
        public IQueryable<ResourceVersionEvent> GetByResourceVersionIdAndEventTypeAsync(int resourceVersionId, ResourceVersionEventTypeEnum resourceVersionEventType)
        {
            return this.DbContext.ResourceVersionEvent.Where(r => r.ResourceVersionId == resourceVersionId
                                                            && r.ResourceVersionEventType == resourceVersionEventType
                                                            && !r.Deleted).AsNoTracking()
                .Include(r => r.CreateUser)
                .OrderBy(r => r.Id)
                .AsNoTracking();
        }
    }
}
