// <copyright file="IDashboardService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// IDashboardService.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<DashboardMyLearningResponseViewModel> GetMyAccessLearningsAsync(string dashboardType, int pageNumber);

        /// <summary>
        /// GetCataloguesAsync.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<DashboardCatalogueResponseViewModel> GetCataloguesAsync(string dashboardType, int pageNumber);

        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<DashboardResourceResponseViewModel> GetResourcesAsync(string dashboardType, int pageNumber);

        /// <summary>
        /// Records dashboard event.
        /// </summary>
        /// <param name="dashboardEventViewModel">dashboardEventViewModel.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RecordDashBoardEventAsync(DashboardEventViewModel dashboardEventViewModel);
    }
}
