namespace LearningHub.Nhs.Repository.Migrations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Migrations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The migration input record repository.
    /// </summary>
    public class MigrationInputRecordRepository : GenericRepository<MigrationInputRecord>, IMigrationInputRecordRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationInputRecordRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public MigrationInputRecordRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MigrationInputRecord> GetByIdAsync(int id)
        {
            return await this.DbContext.MigrationInputRecord.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="migrationId">The migrationId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<MigrationInputRecord>> GetByMigrationIdAsync(int migrationId)
        {
            return await this.DbContext.MigrationInputRecord.AsNoTracking().Where(r => r.MigrationId == migrationId && !r.Deleted).ToListAsync();
        }
    }
}
