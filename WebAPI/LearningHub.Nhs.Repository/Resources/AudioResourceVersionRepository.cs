namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The audio resource version repository.
    /// </summary>
    public class AudioResourceVersionRepository : GenericRepository<AudioResourceVersion>, IAudioResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public AudioResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AudioResourceVersion> GetByIdAsync(int id)
        {
            return await this.DbContext.AudioResourceVersion.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionid">The resource versionid.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AudioResourceVersion> GetByResourceVersionIdAsync(int resourceVersionid, bool includeDeleted = false)
        {
            return await this.DbContext.AudioResourceVersion
                .Include(irv => irv.File)
                .Include(irv => irv.TranscriptFile)
                .Include(irv => irv.ResourceAzureMediaAsset)
                .Include(irv => irv.File).ThenInclude(f => f.FileType).AsNoTracking()
                .Include(irv => irv.TranscriptFile).ThenInclude(f => f.FileType).AsNoTracking()
                .AsNoTracking().FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionid && (includeDeleted || !r.Deleted));
        }
    }
}
