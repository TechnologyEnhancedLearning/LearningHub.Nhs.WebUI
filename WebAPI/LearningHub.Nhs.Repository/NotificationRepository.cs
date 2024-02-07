// <copyright file="NotificationRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The notification repository.
    /// </summary>
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NotificationRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<Notification> GetByIdAsync(int id)
        {
            return this.DbContext.Notification
                .Include(n => n.AmendUser)
                .Include(n => n.CreateUser)
                .Where(n => n.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// The get all full.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public IQueryable<Notification> GetAllFull()
        {
            return this.DbContext.Set<Notification>()
                .Include(n => n.AmendUser)
                .Include(n => n.CreateUser)
                .AsNoTracking();
        }
    }
}
