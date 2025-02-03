namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dto;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The Scorm Activity interface.
    /// </summary>
    public interface IScormActivityRepository : IGenericRepository<ScormActivity>
    {
        /// <summary>
        /// Get Scorm Activity By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Scorm Activity.</returns>
        Task<ScormActivity> GetByIdAsync(int id);

        /// <summary>
        /// The create activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>Scorm Activity Id.</returns>
        int Create(int userId, int resourceVersionId);

        /// <summary>
        /// Complete scorm activity.
        /// Returns the resource activity id of the completion event.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="scormActivityId">The scorm activity id.</param>
        /// <returns>Resource Activity Id.</returns>
        int Complete(int userId, int scormActivityId);

        /// <summary>
        /// Resolve scorm activity.
        /// </summary>
        /// <param name="scormActivityId">The scorm activity id.</param>
        void Resolve(int scormActivityId);

        /// <summary>
        /// Gets the previously launched incomplete scrom activity id.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="resourceReferenceId">resourceReferenceId.</param>
        /// <returns>ScormActivitySummaryDto.</returns>
        ScormActivitySummaryDto GetScormActivitySummary(int userId, int resourceReferenceId);

        /// <summary>
        /// Clones the incomplete scrom activity session with the newly created session.
        /// </summary>
        /// <param name="incompleteScromActivityId">incompleteScromActivityId.</param>
        /// <param name="scromActivityId">scromActivityId.</param>
        /// <returns>ScormActivity.</returns>
        ScormActivity Clone(int incompleteScromActivityId, int scromActivityId);

        /// <summary>
        /// Check user scorm activity data suspend data need to be cleared.
        /// </summary>
        /// <param name="lastScormActivityId">last scorm activity id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>boolean.</returns>
        Task<bool> CheckUserScormActivitySuspendDataToBeCleared(int lastScormActivityId, int resourceVersionId);
    }
}
