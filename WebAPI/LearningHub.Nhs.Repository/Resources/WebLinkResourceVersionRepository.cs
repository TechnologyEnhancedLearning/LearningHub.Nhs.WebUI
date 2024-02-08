namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The web link resource version repository.
    /// </summary>
    public class WebLinkResourceVersionRepository : GenericRepository<WebLinkResourceVersion>, IWebLinkResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebLinkResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public WebLinkResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<WebLinkResourceVersion> GetByResourceVersionIdAsync(int id)
        {
            return await this.DbContext.WebLinkResourceVersion.AsNoTracking()
                                .Include(rv => rv.ResourceVersion)
                                .ThenInclude(rv => rv.Resource).AsNoTracking()
                                .FirstOrDefaultAsync(r => r.ResourceVersionId == id && !r.ResourceVersion.Deleted);
        }
    }
}
