namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version rating summary repository.
    /// </summary>
    public class ResourceVersionRatingSummaryRepository : GenericRepository<ResourceVersionRatingSummary>, IResourceVersionRatingSummaryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionRatingSummaryRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionRatingSummaryRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<ResourceVersionRatingSummary> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return DbContext.ResourceVersionRatingSummary.Where(r => r.ResourceVersionId == resourceVersionId && !r.Deleted).AsNoTracking()
                .SingleOrDefaultAsync();
        }
    }
}
