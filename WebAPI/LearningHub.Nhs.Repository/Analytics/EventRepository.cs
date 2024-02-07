// <copyright file="EventRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Analytics
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Analytics;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The event repository.
    /// </summary>
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public EventRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Get event by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The task.</returns>
        public async Task<Event> GetByIdAsync(int id)
        {
            try
            {
                return await this.DbContext.Event.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
