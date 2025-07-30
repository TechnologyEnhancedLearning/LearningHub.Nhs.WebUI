namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// The IEventLogRepository interface.
    /// </summary>
    public interface IEventLogRepository
    {
        /// <summary>
        /// Create  event.
        /// </summary>
        /// <param name="eventLog">eventLog.</param>
        /// <param name="eventType">eventType.</param>
        /// <param name="hierarchyEditId">hierarchyEditId.</param>
        /// <param name="nodeId">nodeId.</param>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <param name="details">details.</param>
        /// <param name="userId">user id.</param>
        void CreateEvent(EventLogEnum eventLog, EventLogEventTypeEnum eventType, int? hierarchyEditId, int? nodeId, int? resourceVersionId, string details, int userId);
    }
}
