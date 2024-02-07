// <copyright file="IActivityService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The IActivityService interface.
    /// </summary>
    public interface IActivityService
    {
        /// <summary>
        /// This method cleans up incomplete media activities. Required if for any reason, the end of the user's activity was not recorded normally. For example - browser crash, power loss, connection loss.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task CleanUpIncompleteActivitiesAsync(int userId);

        /// <summary>
        /// Create an activity record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createResourceActivityViewModel">The create Resource Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateResourceActivity(int userId, CreateResourceActivityViewModel createResourceActivityViewModel);

        /// <summary>
        /// Create assessment resource activity record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createAssessmentResourceActivityViewModel">The assessment resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAssessmentResourceActivity(int userId, CreateAssessmentResourceActivityViewModel createAssessmentResourceActivityViewModel);

        /// <summary>
        /// Create assessment resource interaction record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createAssessmentResourceActivityInteractionViewModel">The assessment resource activity interaction view model.</param>
        /// <param name="answerInOrder">Whether the user should answer questions in order.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAssessmentResourceActivityInteraction(int userId, CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionViewModel, bool answerInOrder);

        /// <summary>
        /// Gets the resource version id of the assessment, linked to the given activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId">The Id for the assessment resource activity.</param>
        /// <returns>Returns the corresponding resource version id.</returns>
        Task<int> GetAssessmentResourceIdByActivity(int assessmentResourceActivityId);

        /// <summary>
        /// Gets the latest assessment resource activity for the given user and resource version id's.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The assessment resource activity task.</returns>
        Task<AssessmentResourceActivity> GetLatestAssessmentResourceActivityByResourceVersionAndUserId(int resourceVersionId, int userId);

        /// <summary>
        /// Gets the attempts the user has made for the given assessment.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="resourceVersionId">The Id for the assessment resource.</param>
        /// <returns>Returns how many attempts the user has made.</returns>
        int GetAttempts(int userId, int resourceVersionId);

        /// <summary>
        /// Create media resource activity record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createMediaResourceActivityViewModel">The media resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateMediaResourceActivity(int userId, CreateMediaResourceActivityViewModel createMediaResourceActivityViewModel);

        /// <summary>
        /// Create media resource interaction record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createMediaResourceActivityInteractionViewModel">The media resource activity interaction view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateMediaResourceActivityInteraction(int userId, CreateMediaResourceActivityInteractionViewModel createMediaResourceActivityInteractionViewModel);

        /// <summary>
        /// Update a scorm activity record for the supplied params.
        /// </summary>
        /// <param name="currentUserId">The user Id.</param>
        /// <param name="updateScormActivityViewModel">The update Scorm Resource Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ScormUpdateResponseViewModel> UpdateScormActivity(int currentUserId, ScormActivityViewModel updateScormActivityViewModel);

        /// <summary>
        /// Launches a scrom activity session.
        /// </summary>
        /// <param name="currentUserId">The user Id.</param>
        /// <param name="launchScormActivityViewModel">The update Scorm Resource Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ScormActivityViewModel> LaunchScormActivity(int currentUserId, LaunchScormActivityViewModel launchScormActivityViewModel);

        /// <summary>
        /// Complete scorm activity.
        /// </summary>
        /// <param name="currentUserId">The user Id.</param>
        /// <param name="completeScormActivityViewModel">The update scorm Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CompleteScormActivity(int currentUserId, ScormActivityViewModel completeScormActivityViewModel);

        /// <summary>
        /// The resolve scorm activity.
        /// Resolves any completed active content that does not have associated completion events.
        /// Required when LMS has not received an LMSFinish event yet has received an LMSCommit with
        /// a "Completed" status - scenario may arise due to lost connection etc.
        /// </summary>
        /// <param name="userId">The user id.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ResolveScormActivity(int userId);

        /// <summary>
        /// Gets the user Id for a given assessment activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId"> The assessment resource activity Id.</param>
        /// <returns> The corresponding user Id.</returns>
        int GetUserIdForActivity(int assessmentResourceActivityId);

        /// <summary>
        /// Gets the answers for all the activities of a given assessment.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="resourceVersionId">The Id for the assessment resource.</param>
        /// <returns>Returns the answers for all the activities of a given assessment.</returns>
        Task<Dictionary<int, Dictionary<int, IEnumerable<int>>>> GetAnswersForAllTheAssessmentResourceActivities(int userId, int resourceVersionId);

        /// <summary>
        /// Check user scorm activity data suspend data need to be cleared.
        /// </summary>
        /// <param name="lastScormActivityId">last scorm activity id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>boolean.</returns>
        Task<bool> CheckUserScormActivitySuspendDataToBeCleared(int lastScormActivityId, int resourceVersionId);
    }
}
