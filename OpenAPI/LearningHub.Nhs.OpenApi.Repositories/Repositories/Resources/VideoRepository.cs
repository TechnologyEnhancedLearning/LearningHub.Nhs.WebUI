namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The Video repository.
    /// </summary>
    public class VideoRepository : GenericRepository<Video>, IVideoRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public VideoRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by file Id async.
        /// </summary>
        /// <param name="fileId"> The file Id. </param>
        /// <returns>The <see cref="Task{TResult}"/>. </returns>
        public async Task<Video> GetByFileIdAsync(int fileId)
        {
            return await DbContext.Video.Include(v => v.MediaBlocks)
                .ThenInclude(mb => mb.Block)
                .ThenInclude(b => b.BlockCollection)
                .ThenInclude(bc => bc.CaseResourceVersion)
                .AsNoTracking().FirstOrDefaultAsync(v => v.FileId == fileId);
        }
    }
}