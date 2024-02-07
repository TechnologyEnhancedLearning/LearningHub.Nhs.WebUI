// <copyright file="EventLogService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using LearningHub.Nhs.Models.EventLog;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
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
            this.eventLogRepository.CreateEvent(eventViewModel.EventLog, eventViewModel.EventType, eventViewModel.HierarchyEditId, eventViewModel.NodeId, eventViewModel.ResourceVersionId, eventViewModel.Details, eventViewModel.CreateUserId);
        }
    }
}
