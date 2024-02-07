// <copyright file="ILogService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Log;
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="ILogService" />.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// The GetIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LogViewModel}"/>.</returns>
        Task<LogViewModel> GetIdAsync(int id);

        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{LogBasicViewModel}"/>.</returns>
        Task<PagedResultSet<LogBasicViewModel>> GetPagedAsync(PagingRequestModel pagingRequestModel);
    }
}
