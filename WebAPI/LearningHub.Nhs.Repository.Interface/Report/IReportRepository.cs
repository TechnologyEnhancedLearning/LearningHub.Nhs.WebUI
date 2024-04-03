namespace LearningHub.Nhs.Repository.Interface.Report
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Reporting;

    /// <summary>
    /// The ReportRepository interface.
    /// </summary>
    public interface IReportRepository : IGenericRepository<Report>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeChildren">If the children entities should be loaded.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Report> GetByIdAsync(int id, bool includeChildren = false);

        /// <summary>
        /// The get by filename and hash.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="hash">The hash.</param>
        /// <param name="includeChildren">Include children.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Report> GetByFileDetailAsync(string fileName, string hash, bool includeChildren = false);
    }
}
