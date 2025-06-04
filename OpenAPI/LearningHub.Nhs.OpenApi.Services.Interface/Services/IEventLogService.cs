namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using LearningHub.Nhs.Models.EventLog;

    /// <summary>
    /// The IEventLogService interface.
    /// </summary>
    public interface IEventLogService
    {
        /// <summary>
        /// Create event.
        /// </summary>
        /// <param name="eventViewModel">eventViewModel.</param>
        void CreateEvent(EventViewModel eventViewModel);
    }
}
