namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Log;

    /// <summary>
    /// The LogService interface.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LogViewModel> GetByIdAsync(int id);

        /// <summary>
        /// The get page async.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<LogBasicViewModel>> GetPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string filter = "");
    }
}
