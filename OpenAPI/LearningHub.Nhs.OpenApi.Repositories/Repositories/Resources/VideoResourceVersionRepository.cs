namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The video resource version repository.
    /// </summary>
    public class VideoResourceVersionRepository : GenericRepository<VideoResourceVersion>, IVideoResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public VideoResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<VideoResourceVersion> GetByIdAsync(int id)
        {
            return await DbContext.VideoResourceVersion.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionid">The resource versionid.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<VideoResourceVersion> GetByResourceVersionIdAsync(int resourceVersionid, bool includeDeleted = false)
        {
            return await DbContext.VideoResourceVersion
                .Include(irv => irv.File).ThenInclude(f => f.FileType).AsNoTracking()
                .Include(irv => irv.TranscriptFile).ThenInclude(f => f.FileType).AsNoTracking()
                .Include(irv => irv.ClosedCaptionsFile).ThenInclude(f => f.FileType)
                .Include(irv => irv.File)
                .Include(irv => irv.TranscriptFile)
                .Include(irv => irv.ClosedCaptionsFile)
                .Include(irv => irv.ResourceAzureMediaAsset)
                .AsNoTracking().FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionid && (includeDeleted || !r.Deleted));
        }
    }
}
