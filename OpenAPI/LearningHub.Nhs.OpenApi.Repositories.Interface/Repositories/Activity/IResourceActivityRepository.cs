namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.MyLearning;

    /// <summary>
    /// The ResourceActivity interface.
    /// </summary>
    public interface IResourceActivityRepository : IGenericRepository<ResourceActivity>
    {
        /// <summary>
        /// Get Resource Activity By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ResourceActivity.</returns>
        Task<ResourceActivity> GetByIdAsync(int id);

        /// <summary>
        /// Create activity record against a ResourceVersion.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="launchResourceActivityId">The launch resource activity id.</param>
        /// <param name="activityStatusEnum">The activity Status Enum.</param>
        /// <param name="activityStart">The activity Start.</param>
        /// <param name="activityEnd">The activity End.</param>
        /// <returns>Activity Id.</returns>
        int CreateActivity(
           int userId,
           int resourceVersionId,
           int nodePathId,
           int? launchResourceActivityId,
           ActivityStatusEnum activityStatusEnum,
           DateTimeOffset? activityStart,
           DateTimeOffset? activityEnd);

        /// <summary>
        /// Get Resource Activity By user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>ResourceActivity.</returns>
        IQueryable<ResourceActivity> GetByUserId(int userId);

        /// <summary>
        /// Gets a list of incomplete media activities. Those that for any reason, the end of the user's activity was not recorded normally. For example - browser crash, power loss, connection loss.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<ResourceActivity>> GetIncompleteMediaActivities(int userId);

        /// <summary>
        /// Gets a list of all the user's activities for a given resource.
        /// </summary>
        /// <param name="userId">The user id.</param>>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<ResourceActivity>> GetAllTheActivitiesFor(int userId, int resourceId);

        /// <summary>
        /// Check if scorm activity has been completed.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="scormActivityId">The scormActivityId id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> IsScormActivityFinished(int userId, int scormActivityId);

        /// <summary>
        /// Get Resource Activity By user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">requestModel.</param>
        /// <param name="detailedMediaActivityRecordingStartDate">detailedMediaActivityRecordingStartDate.</param>
        /// <returns>ResourceActivity.</returns>
        Task<IQueryable<ResourceActivity>> GetByUserIdFromSP(int userId, Nhs.Models.MyLearning.MyLearningRequestModel requestModel, DateTimeOffset detailedMediaActivityRecordingStartDate);

        /// <summary>
        /// Check if scorm activity has been completed.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">requestModel.</param>
        /// <param name="detailedMediaActivityRecordingStartDate">detailedMediaActivityRecordingStartDate.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        int GetTotalCount(int userId, MyLearningRequestModel requestModel, DateTimeOffset detailedMediaActivityRecordingStartDate);

        /// <summary>
        /// Gets a list of all the user's activities for a given resource.
        /// </summary>
        /// <param name="userId">The user id.</param>>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<ResourceActivity>> GetAllTheActivitiesFromSP(int userId, int resourceId);

        /// <summary>
        /// Get the assessment activity completion details.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="activityId">The activityId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentActivityCompletionViewModel> GetAssessmentActivityCompletionPercentage(int userId, int resourceVersionId, int activityId);
    }
}
