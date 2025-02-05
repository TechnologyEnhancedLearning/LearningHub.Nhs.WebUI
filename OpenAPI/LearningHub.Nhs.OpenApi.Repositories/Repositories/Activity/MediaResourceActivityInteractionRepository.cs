namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Activity
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The media resource activity interaction repository.
    /// </summary>
    public class MediaResourceActivityInteractionRepository : GenericRepository<MediaResourceActivityInteraction>, IMediaResourceActivityInteractionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaResourceActivityInteractionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public MediaResourceActivityInteractionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<MediaResourceActivityInteraction> GetByIdAsync(int id)
        {
            return DbContext.MediaResourceActivityInteraction.Where(n => n.Id == id).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Performs the analysis of media resource activity to populate the played segment data.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="mediaResourceActivityId">The mediaResourceActivityId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CalculatePlayedMediaSegments(int userId, int resourceVersionId, int mediaResourceActivityId)
        {
            var resourceVersion = DbContext.ResourceVersion.FirstOrDefault(x => x.Id == resourceVersionId);

            if (resourceVersion == null)
            {
                throw new ArgumentException($"ResourceVersionId {resourceVersionId} not found.");
            }

            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersion.ResourceId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceVersion.MajorVersion };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = mediaResourceActivityId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = userId }; // This is not a mistake. SP has two different user Id params and in this scenario they are both the same value.
            var param5 = new SqlParameter("@p5", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            await DbContext.Database.ExecuteSqlRawAsync("activity.CalculatePlayedMediaSegments @p0, @p1, @p2, @p3, @p4, @p5", param0, param1, param2, param3, param4, param5);
        }
    }
}
