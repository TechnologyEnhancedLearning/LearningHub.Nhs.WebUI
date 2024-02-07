// <copyright file="IActivityService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The ActivityService interface.
    /// </summary>
    public interface IActivityService
    {
        /// <summary>
        /// The create resource activity async.
        /// </summary>
        /// <param name="createResourceActivityViewModel">The create resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateResourceActivityAsync(CreateResourceActivityViewModel createResourceActivityViewModel);

        /// <summary>
        /// The create assessment resource activity.
        /// </summary>
        /// <param name="createAssessmentResourceActivityViewModel">The create assessment resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAssessmentResourceActivityAsync(CreateAssessmentResourceActivityViewModel createAssessmentResourceActivityViewModel);

        /// <summary>
        /// Validates and creates an assessment resource activity interaction.
        /// </summary>
        /// <param name="createAssessmentResourceActivityInteractionViewModel">The createAssessmentResourceActivityInteractionViewModel<see cref="CreateAssessmentResourceActivityViewModel"/>.</param>
        /// <returns>The result of the API request.</returns>
        Task<AssessmentViewModel> CreateAssessmentResourceActivityInteractionAsync(CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionViewModel);

        /// <summary>
        /// The create media resource activity.
        /// </summary>
        /// <param name="createMediaResourceActivityViewModel">The create media resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateMediaResourceActivityAsync(CreateMediaResourceActivityViewModel createMediaResourceActivityViewModel);

        /// <summary>
        /// The create media resource activity interaction.
        /// </summary>
        /// <param name="createMediaResourceActivityInteractionViewModel">The create media resource activity interaction view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateMediaResourceActivityInteractionAsync(CreateMediaResourceActivityInteractionViewModel createMediaResourceActivityInteractionViewModel);

        /// <summary>
        /// The launch scorm activity async.
        /// </summary>
        /// <param name="launchScormActivityViewModel">The launch resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ScormActivityViewModel> LaunchScormActivityAsync(LaunchScormActivityViewModel launchScormActivityViewModel);

        /// <summary>
        /// The update scorm activity async.
        /// </summary>
        /// <param name="updateScormActivityViewModel">The update scorm activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ScormUpdateResponseViewModel> UpdateScormActivityAsync(ScormActivityViewModel updateScormActivityViewModel);

        /// <summary>
        /// The CompleteScormActivity.
        /// </summary>
        /// <param name="scormActivityViewModel">The updateScormActivityViewModel<see cref="ScormActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CompleteScormActivity(ScormActivityViewModel scormActivityViewModel);

        /// <summary>
        /// The ResolveScormActivity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ResolveScormActivity(int userId);

        /// <summary>
        /// Check user scorm activity data suspend data need to be cleared.
        /// </summary>
        /// <param name="lastScormActivityId">last scorm activity id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>boolean.</returns>
        Task<bool> CheckSuspendDataToBeCleared(int lastScormActivityId, int resourceVersionId);
    }
}
