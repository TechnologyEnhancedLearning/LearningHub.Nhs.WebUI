// <copyright file="EventService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Analytics;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The event service.
    /// </summary>
    public class EventService : IEventService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<RoleService> logger;

        /// <summary>
        /// The event repository.
        /// </summary>
        private IEventRepository eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="eventRepository">The role repository.</param>
        /// <param name="logger">The logger.</param>
        public EventService(
            IEventRepository eventRepository,
            ILogger<RoleService> logger)
        {
            this.eventRepository = eventRepository;
            this.logger = logger;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Event> GetByIdAsync(int id)
        {
            return await this.eventRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="eventEntity">The event.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAsync(int userId, Event eventEntity)
        {
            var retVal = await this.ValidateAsync(eventEntity);

            if (retVal.IsValid)
            {
                retVal.CreatedId = await this.eventRepository.CreateAsync(userId, eventEntity);
            }

            return retVal;
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="eventEntity">The event.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateAsync(int userId, Event eventEntity)
        {
            var retVal = await this.ValidateAsync(eventEntity);

            if (retVal.IsValid)
            {
                await this.eventRepository.UpdateAsync(userId, eventEntity);
            }

            return retVal;
        }

        /// <summary>
        /// The validate async.
        /// </summary>
        /// <param name="eventEntity">The event.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> ValidateAsync(Event eventEntity)
        {
            var eventValidator = new EventValidator();
            var clientValidationResult = await eventValidator.ValidateAsync(eventEntity);

            var retVal = new LearningHubValidationResult(clientValidationResult);

            return retVal;
        }
    }
}
