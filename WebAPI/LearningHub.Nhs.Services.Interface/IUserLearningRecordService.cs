namespace LearningHub.Nhs.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.MyLearning;

    /// <summary>
    /// The MyLearningService interface.
    /// </summary>
    public interface IUserLearningRecordService
    {
        /// <summary>
        /// GetUserLearningRecordsAsync.
        /// </summary>
        /// <param name="page">page.</param>
        /// <param name="pageSize">pageSize.</param>
        /// <param name="sortColumn">sortColumn.</param>
        /// <param name="sortDirection">sortDirection.</param>
        /// <param name="presetFilter">presetFilter.</param>
        /// <param name="filter">filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<MyLearningDetailedItemViewModel>> GetUserLearningRecordsAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "");
    }
}
