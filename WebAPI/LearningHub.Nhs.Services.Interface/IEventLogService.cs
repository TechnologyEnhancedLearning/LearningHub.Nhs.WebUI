namespace LearningHub.Nhs.Services.Interface
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
