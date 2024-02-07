// <copyright file="IEventService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Analytics;

    /// <summary>
    /// Defines the <see cref="IEventService" />.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="eventEntity">The event<see cref="Event"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> Create(Event eventEntity);
    }
}
