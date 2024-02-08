namespace LearningHub.Nhs.Repository.Interface.Migrations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Migration;

    /// <summary>
    /// The MigrationInputRecordRepository interface.
    /// </summary>
    public interface IMigrationInputRecordRepository : IGenericRepository<MigrationInputRecord>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MigrationInputRecord> GetByIdAsync(int id);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="migrationId">The migrationId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<MigrationInputRecord>> GetByMigrationIdAsync(int migrationId);
    }
}
