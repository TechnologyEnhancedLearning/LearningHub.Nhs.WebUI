namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.MyLearning;

    /// <summary>
    /// The MyLearningService interface.
    /// </summary>
    public interface IMyLearningService
    {
        /// <summary>
        /// Gets the activity records for the detailed activity tab of My Learning screen.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningDetailedViewModel> GetActivityDetailed(MyLearningRequestModel requestModel);

        /// <summary>
        /// Gets the user recent my leraning activities..
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningActivitiesDetailedViewModel> GetUserRecentMyLearningActivities(MyLearningRequestModel requestModel);

        /// <summary>
        /// Gets the user leraning history.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MyLearningActivitiesDetailedViewModel> GetUserLearningHistory(MyLearningRequestModel requestModel);

        /// <summary>
        /// Gets the played segment data for the progress modal in My Learning screen.
        /// </summary>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<PlayedSegmentViewModel>> GetPlayedSegments(int resourceId, int majorVersion);

        /// <summary>
        /// Gets the resource certificate details of a resource reference.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <param name="minorVersion">The minorVersion.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Tuple<int, MyLearningDetailedItemViewModel>> GetResourceCertificateDetails(int resourceReferenceId, int? majorVersion = 0, int? minorVersion = 0, int? userId = 0);

        /// <summary>
        /// Gets the resource URL for a given resource reference ID.
        /// </summary>
        /// <param name="resourceReferenceId">resourceReferenceId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        string GetResourceUrl(int resourceReferenceId);
    }
}