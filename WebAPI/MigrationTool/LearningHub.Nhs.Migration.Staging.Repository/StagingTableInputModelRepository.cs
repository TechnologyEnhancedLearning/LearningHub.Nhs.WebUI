namespace LearningHub.Nhs.Migration.Staging.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The StagingTableInputModel repository.
    /// </summary>
    public class StagingTableInputModelRepository : IStagingTableInputModelRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StagingTableInputModelRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        public StagingTableInputModelRepository(MigrationStagingDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        protected MigrationStagingDbContext DbContext { get; }

        /// <summary>
        /// Gets all staging table input models from the staging database.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<StagingTableInputModel>> GetAllStagingTableInputModels()
        {
            return await this.DbContext.StagingTableInputModels.FromSqlRaw("Migration.GetStagingTableResources").AsNoTracking().ToListAsync();
        }
    }
}
