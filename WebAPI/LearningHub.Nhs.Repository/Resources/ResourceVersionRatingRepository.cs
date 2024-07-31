namespace LearningHub.Nhs.Repository.Resources
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version Event repository.
    /// </summary>
    public class ResourceVersionRatingRepository : GenericRepository<ResourceVersionRating>, IResourceVersionRatingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionRatingRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionRatingRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Gets a user's previous rating for any minor version of the same major resource version.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersionRating> GetUsersPreviousRatingForSameMajorVersionAsync(int resourceVersionId, int userId)
        {
            var minorVersionIds = await this.GetAllResourceVersionIdsForSameMajorVersion(resourceVersionId);

            return await this.DbContext.ResourceVersionRating.FirstOrDefaultAsync(x => minorVersionIds.Contains(x.ResourceVersionId) && x.UserId == userId && !x.Deleted);
        }

        /// <summary>
        /// Gets the total rating counts across all users for a particular resource version.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>An array of integers, which are the count for each star value, starting at 1 star and ending with 5 stars.</returns>
        public async Task<int[]> GetRatingCountsForResourceVersionAsync(int resourceVersionId)
        {
            var minorVersionIds = await this.GetAllResourceVersionIdsForSameMajorVersion(resourceVersionId);

            var allMajorVersionRatings = this.DbContext.ResourceVersionRating.Where(x => minorVersionIds.Contains(x.ResourceVersionId));

            int[] starCounts = new int[5];

            starCounts[0] = await allMajorVersionRatings.CountAsync(x => x.Rating == 1);
            starCounts[1] = await allMajorVersionRatings.CountAsync(x => x.Rating == 2);
            starCounts[2] = await allMajorVersionRatings.CountAsync(x => x.Rating == 3);
            starCounts[3] = await allMajorVersionRatings.CountAsync(x => x.Rating == 4);
            starCounts[4] = await allMajorVersionRatings.CountAsync(x => x.Rating == 5);

            return starCounts;
        }

        /// <summary>
        /// Gets a list of resource version Ids for ALL minor versions of the same major resource version.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>A list of resoruce verison ids.</returns>
        private async Task<List<int>> GetAllResourceVersionIdsForSameMajorVersion(int resourceVersionId)
        {
            var majorVersionInfo = await this.DbContext.ResourceVersion.Where(x => x.Id == resourceVersionId).Select(x => new { ResourceId = x.ResourceId, MajorVersion = x.MajorVersion }).FirstOrDefaultAsync();

            var minorVersionIds = await this.DbContext.ResourceVersion
                .Where(x => x.ResourceId == majorVersionInfo.ResourceId && x.MajorVersion == majorVersionInfo.MajorVersion).Select(x => x.Id).ToListAsync();

            return minorVersionIds;
        }
    }
}
