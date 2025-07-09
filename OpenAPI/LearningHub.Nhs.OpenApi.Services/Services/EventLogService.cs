namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using LearningHub.Nhs.Models.EventLog;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The event log service.
    /// </summary>
    public class EventLogService : IEventLogService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<EventLogService> logger;

        /// <summary>
        /// The event log repository.
        /// </summary>
        private IEventLogRepository eventLogRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogService"/> class.
        /// </summary>
        /// <param name="eventLogRepository">The role repository.</param>
        /// <param name="logger">The logger.</param>
        public EventLogService(
            IEventLogRepository eventLogRepository,
            ILogger<EventLogService> logger)
        {
            this.eventLogRepository = eventLogRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Create event.
        /// </summary>
        /// <param name="eventViewModel">eventViewModel.</param>
        public void CreateEvent(EventViewModel eventViewModel)
        {
            eventLogRepository.CreateEvent(eventViewModel.EventLog, eventViewModel.EventType, eventViewModel.HierarchyEditId, eventViewModel.NodeId, eventViewModel.ResourceVersionId, eventViewModel.Details, eventViewModel.CreateUserId);
        }
    }
}
