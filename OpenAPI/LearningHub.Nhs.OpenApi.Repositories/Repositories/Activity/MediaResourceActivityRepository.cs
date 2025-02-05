namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Activity
{
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
    public class MediaResourceActivityRepository : GenericRepository<MediaResourceActivity>, IMediaResourceActivityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaResourceActivityRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public MediaResourceActivityRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<MediaResourceActivity> GetByIdAsync(int id)
        {
            return DbContext.MediaResourceActivity.Where(n => n.Id == id).SingleOrDefaultAsync();
        }
    }
}
