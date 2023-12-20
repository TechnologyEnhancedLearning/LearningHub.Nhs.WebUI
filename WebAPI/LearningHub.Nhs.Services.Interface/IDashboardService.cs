// <copyright file="IDashboardService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;

    /// <summary>
    /// IDashboardService.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// GetMyAccessLearnings.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<DashboardMyLearningResponseViewModel> GetMyAccessLearnings(string dashboardType, int pageNumber, int userId);

        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<DashboardCatalogueResponseViewModel> GetCatalogues(string dashboardType, int pageNumber, int userId);

        /// <summary>
        /// GetCataloguessAsync.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<DashboardResourceResponseViewModel> GetResources(string dashboardType, int pageNumber, int userId);
    }
}
