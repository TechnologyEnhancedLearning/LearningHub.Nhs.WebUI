namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Activity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The activity repository.
    /// </summary>
    public class MediaResourcePlayedSegmentRepository : GenericRepository<MediaResourcePlayedSegment>, IMediaResourcePlayedSegmentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaResourcePlayedSegmentRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public MediaResourcePlayedSegmentRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<MediaResourcePlayedSegment>> GetPlayedSegmentsAsync(int userId, int resourceId, int majorVersion)
        {
            return await DbContext.MediaResourcePlayedSegment
                .Where(x => x.UserId == userId && x.ResourceId == resourceId && x.MajorVersion == majorVersion)
                .OrderBy(x => x.SegmentStartTime)
                .ToListAsync();
        }
    }
}
