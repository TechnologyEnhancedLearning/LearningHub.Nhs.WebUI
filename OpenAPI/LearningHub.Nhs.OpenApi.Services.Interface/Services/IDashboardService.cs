namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.MyLearning;

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
        /// Get My in progress courses and Elearning.
        /// </summary>
        /// <param name="dashboardTrayLearningResourceType">The dashboardTrayLearningResource type.</param>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceType">The resourceType.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<DashboardMyLearningResponseViewModel> GetMyCoursesAndElearning(string dashboardTrayLearningResourceType, string dashboardType, int pageNumber, int userId, string resourceType);

        /// <summary>
        /// Gets the user in progrss my leraning activities..
        /// </summary>
        /// <param name="dashboardTrayLearningResourceType">The dashboardTrayLearningResourceType.</param>
        /// <param name="pageNumber">The pageNumber.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningActivitiesDetailedViewModel> GetMyInprogressLearningAsync(string dashboardTrayLearningResourceType, int pageNumber, int userId);

        /// <summary>
        /// Gets the resource certificate details.
        /// </summary>
        /// <param name="pageNumber">The pageNumber.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="dashboardTrayLearningResourceType">The resourceType.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningCertificatesDetailedViewModel> GetUserCertificateDetailsAsync(string dashboardTrayLearningResourceType, int pageNumber, int userId);

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
