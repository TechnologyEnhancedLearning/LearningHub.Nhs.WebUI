namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The article resource version file repository.
    /// </summary>
    public class ArticleResourceVersionFileRepository : GenericRepository<ArticleResourceVersionFile>, IArticleResourceVersionFileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleResourceVersionFileRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ArticleResourceVersionFileRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ArticleResourceVersionFile> GetByIdAsync(int id)
        {
            return await DbContext.ArticleResourceVersionFile.AsNoTracking()
                                .FirstOrDefaultAsync(arvf => arvf.Id == id);
        }

        /// <summary>
        /// The get by resourceVersionId and fileId async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="fileId">The fileId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ArticleResourceVersionFile> GetByResourceVersionAndFileAsync(int resourceVersionId, int fileId)
        {
            return await DbContext.ArticleResourceVersionFile.AsNoTracking()
                                .FirstOrDefaultAsync(arvf => arvf.ArticleResourceVersion.ResourceVersionId == resourceVersionId && arvf.FileId == fileId);
        }
    }
}
