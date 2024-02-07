// <copyright file="MigrationSourceRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Migrations
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Migrations;
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
            return await this.DbContext.MigrationSource.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }
    }
}
