namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.Data.SqlClient;
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
            return await DbContext.UserNotification
                .Where(n => n.Id == id).SingleOrDefaultAsync();
        }

        /// <summary>
        /// The get user unread notification count async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> GetUserUnreadNotificationCountAsync(int userId)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userId };
                var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

                var result = await this.DbContext
                    .NotificationCount
                    .FromSqlRaw("EXEC hub.GetActiveNotificationCount @p0, @p1", param0, param1)
                    .ToListAsync();

                return result.FirstOrDefault()?.UserNotificationCount ?? 0;
            }
            catch (Exception ex)
            {
                // Optional: log ex
                throw new Exception("Failed to get unread notification count: " + ex.Message);
            }
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
            var query = GetActiveNotifications(userId, DateTimeOffset.Now)
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
            return await DbContext.UserNotification
            .Include(n => n.AmendUser)
            .Include(n => n.CreateUser)
            .Include(n => n.User)
            .Include(n => n.Notification)
            .Where(n => n.NotificationId == notificationId && n.UserId == userId).SingleOrDefaultAsync();
        }

        private IQueryable<UserSpecificNotification> GetActiveNotifications(int userId, DateTimeOffset now)
        {
            return from n in DbContext.Notification
                   join un in DbContext.UserNotification
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