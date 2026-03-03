namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.DatabricksReport;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The provider repository.
    /// </summary>
    public class ReportHistoryRepository : GenericRepository<ReportHistory>, IReportHistoryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ReportHistoryRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <inheritdoc />
        public async Task<ReportHistory> GetByIdAsync(int id)
        {
            return await DbContext.ReportHistory.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
        }

        /// <inheritdoc />
        public IQueryable<ReportHistory> GetByUserIdAsync(int userId)
        {
            return DbContext.ReportHistory.AsNoTracking().Where(n => n.CreateUserId == userId && !n.Deleted);
        }

        /// <summary>
        /// The get by user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<Provider> GetProvidersByUserIdAsync(int userId)
        {
            return DbContext.Set<UserProvider>()
             .Include(up => up.Provider)
                .Where(up => up.UserId == userId && !up.Deleted).AsNoTracking()
                .Select(up => up.Provider);
        }

       
    }
}
