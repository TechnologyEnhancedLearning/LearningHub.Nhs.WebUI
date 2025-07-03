namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Content
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Content;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the <see cref="VideoAssetRepository" />.
    /// </summary>
    public class VideoAssetRepository : GenericRepository<VideoAsset>, IVideoAssetRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoAssetRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="LearningHubDbContext"/>.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public VideoAssetRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetByPageSectionDetailId.
        /// </summary>
        /// <param name="pageSectionDetailId">The pageSectionDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VideoAsset}"/>.</returns>
        public async Task<VideoAsset> GetByPageSectionDetailId(int pageSectionDetailId)
        {
            return await DbContext.VideoAsset.SingleOrDefaultAsync(va => va.PageSectionDetailId == pageSectionDetailId);
        }

        /// <summary>
        /// The GetById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VideoAsset}"/>.</returns>
        public async Task<VideoAsset> GetById(int id)
        {
            return await DbContext.VideoAsset.SingleOrDefaultAsync(va => va.Id == id);
        }
    }
}
