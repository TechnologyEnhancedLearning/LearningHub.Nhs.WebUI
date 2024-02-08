namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The article resource version repository.
    /// </summary>
    public class ArticleResourceVersionRepository : GenericRepository<ArticleResourceVersion>, IArticleResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ArticleResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ArticleResourceVersion> GetByIdAsync(int id)
        {
            return await this.DbContext.ArticleResourceVersion.AsNoTracking()
                                .Include(rv => rv.ResourceVersion)
                                .ThenInclude(rv => rv.Resource).AsNoTracking()
                                .Include(rv => rv.ArticleResourceVersionFile)
                                .ThenInclude(rv => rv.File)
                                .ThenInclude(rv => rv.FileType)
                                .FirstOrDefaultAsync(r => r.ResourceVersionId == id && !r.ResourceVersion.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionid">The resource versionid.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ArticleResourceVersion> GetByResourceVersionIdAsync(int resourceVersionid, bool includeDeleted = false)
        {
            return await this.DbContext.ArticleResourceVersion
                                .Include(arv => arv.ArticleResourceVersionFile)
                                .ThenInclude(arvf => arvf.File)
                                .ThenInclude(rv => rv.FileType).AsNoTracking()
                                .AsNoTracking().FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionid && (includeDeleted || !r.Deleted));
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ArticleResourceVersion> GetByResourceVersionIdAsync(int id)
        {
            return await this.DbContext.ArticleResourceVersion.AsNoTracking()
                               .Include(rv => rv.ResourceVersion)
                               .ThenInclude(rv => rv.Resource).AsNoTracking()
                               .FirstOrDefaultAsync(r => r.ResourceVersionId == id && !r.ResourceVersion.Deleted);
        }
    }
}
