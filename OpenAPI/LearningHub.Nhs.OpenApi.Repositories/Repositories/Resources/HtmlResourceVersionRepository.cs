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
    /// The html resource version repository.
    /// </summary>
    public class HtmlResourceVersionRepository : GenericRepository<HtmlResourceVersion>, IHtmlResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public HtmlResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HtmlResourceVersion> GetByIdAsync(int id)
        {
            return await DbContext.HtmlResourceVersion.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionid">The resource versionid.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HtmlResourceVersion> GetByResourceVersionIdAsync(int resourceVersionid, bool includeDeleted = false)
        {
            return await DbContext.HtmlResourceVersion
                .Include(gfrv => gfrv.File).ThenInclude(f => f.FileType)
                .AsNoTracking().FirstOrDefaultAsync(gfrv => gfrv.ResourceVersionId == resourceVersionid && (includeDeleted || !gfrv.Deleted));
        }

        /// <summary>
        /// The get by resource version id.
        /// </summary>
        /// <param name="resourceVersionid">The resource version id.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The html resource version.</returns>
        public HtmlResourceVersion GetByResourceVersionId(int resourceVersionid, bool includeDeleted = false)
        {
            return DbContext.HtmlResourceVersion
                .Include(gfrv => gfrv.File).ThenInclude(f => f.FileType)
                .AsNoTracking().FirstOrDefault(gfrv => gfrv.ResourceVersionId == resourceVersionid && (includeDeleted || !gfrv.Deleted));
        }
    }
}
