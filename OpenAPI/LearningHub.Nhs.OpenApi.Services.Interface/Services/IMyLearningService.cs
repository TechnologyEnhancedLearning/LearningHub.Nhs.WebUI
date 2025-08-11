namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.MyLearning;

    /// <summary>
    /// The MyLearningService interface.
    /// </summary>
    public interface IMyLearningService
    {
        /// <summary>
        /// Gets the activity records for the detailed activity tab of My Learning screen.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningDetailedViewModel> GetActivityDetailed(int userId, MyLearningRequestModel requestModel);

        /// <summary>
        /// Gets the user recent my leraning activities.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningActivitiesDetailedViewModel> GetUserRecentMyLearningActivitiesAsync(int userId, MyLearningRequestModel requestModel);

        /// <summary>
        /// Gets history of users my leraning activities.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningActivitiesDetailedViewModel> GetUserLearningHistoryAsync(int userId, MyLearningRequestModel requestModel);

        /// <summary>
        /// Gets the played segment data for the progress modal in My Learning screen.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<PlayedSegmentViewModel>> GetPlayedSegments(int userId, int resourceId, int majorVersion);

        /// <summary>
        /// Gets the resource certificate details of a resource reference.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <param name="minorVersion">The minorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Tuple<int, MyLearningDetailedItemViewModel>> GetResourceCertificateDetails(int userId, int resourceReferenceId, int majorVersion, int minorVersion);

        /// <summary>
        /// Populate MyLearning Detailed ItemViewModels.
        /// </summary>
        /// <param name="resourceActivities">The resource activities.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<MyLearningDetailedItemViewModel>> PopulateMyLearningDetailedItemViewModels(List<ResourceActivity> resourceActivities, int userId);
    }
}
