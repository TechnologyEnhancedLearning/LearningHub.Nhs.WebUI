namespace LearningHub.Nhs.Repository
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user notification repository.
    /// </summary>
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UserNotificationRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserNotification> GetByIdAsync(int id)
        {
            return await this.DbContext.UserNotification
                .Where(n => n.Id == id).SingleOrDefaultAsync();
        }

        /// <summary>
        /// The get user unread notification count async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> GetUserUnreadNotificationCountAsync(int userId)
        {
            return await this.GetActiveNotifications(userId, DateTimeOffset.Now)
                            .Where(n => !n.UserNotification.ReadOnDate.HasValue)
                            .CountAsync();
        }

        /// <summary>
        /// The get all non dismissed.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="priorityType">Notification priority type.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public IQueryable<UserSpecificNotification> GetAllNonDismissed(int userId, NotificationPriorityEnum priorityType, string sortColumn = "", string sortDirection = "")
        {
            var query = this.GetActiveNotifications(userId, DateTimeOffset.Now)
                            .Where(n => n.Notification.NotificationPriorityEnum == priorityType);

            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                switch (sortColumn)
                {
                    case "date":
                        return sortDirection == "A" ? query.OrderBy(n => n.Notification.StartDate) : query.OrderByDescending(n => n.Notification.StartDate);
                    default:
                        throw new NotImplementedException($"Sorting for column:{sortColumn} on notifications not implemented.");
                }
            }

            return query.OrderByDescending(n => n.Notification.StartDate);
        }

        /// <summary>
        /// The get by notification and user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notificationId">The notification id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserNotification> GetByNotificationAndUserIdAsync(int userId, int notificationId)
        {
            return await this.DbContext.UserNotification
            .Include(n => n.AmendUser)
            .Include(n => n.CreateUser)
            .Include(n => n.User)
            .Include(n => n.Notification)
            .Where(n => n.NotificationId == notificationId && n.UserId == userId).SingleOrDefaultAsync();
        }

        private IQueryable<UserSpecificNotification> GetActiveNotifications(int userId, DateTimeOffset now)
        {
            return from n in this.DbContext.Notification
                   join un in this.DbContext.UserNotification
                                   on new { n.Id, UserId = userId }
                               equals new { Id = un.NotificationId, un.UserId } into list
                   from un in list.DefaultIfEmpty()
                   where now >= n.StartDate && now <= n.EndDate
                       && (n.IsUserSpecific == false || un.UserId == userId)
                       && (un == null || un.Dismissed == false)
                   select new UserSpecificNotification { Notification = n, UserNotification = un };
        }
    }
}