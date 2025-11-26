namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.DatabricksReport;

    /// <summary>
    /// The ProviderRepository interface.
    /// </summary>
    public interface IReportHistoryRepository : IGenericRepository<ReportHistory>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ReportHistory> GetByIdAsync(int id);

        /// <summary>
        /// The get by user id async.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        IQueryable<ReportHistory> GetByUserIdAsync(int userId);
    }
}
