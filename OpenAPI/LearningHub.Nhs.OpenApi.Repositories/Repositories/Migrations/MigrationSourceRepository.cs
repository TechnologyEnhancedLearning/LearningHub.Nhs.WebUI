namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Migrations
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Migrations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The migration source repository.
    /// </summary>
    public class MigrationSourceRepository : GenericRepository<MigrationSource>, IMigrationSourceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationSourceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public MigrationSourceRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MigrationSource> GetByIdAsync(int id)
        {
            return await DbContext.MigrationSource.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }
    }
}
