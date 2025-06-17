namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// The UserNotificationRepository interface.
    /// </summary>
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserNotification> GetByIdAsync(int id);

        /// <summary>
        /// The get by notification and user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notificationId">The notification id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserNotification> GetByNotificationAndUserIdAsync(int userId, int notificationId);

        /// <summary>
        /// The get user unread notification count async.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> GetUserUnreadNotificationCountAsync(int userid);

        /// <summary>
        /// The get all non dismissed.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="priorityType">Notification priority type.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        IQueryable<UserSpecificNotification> GetAllNonDismissed(int userId, NotificationPriorityEnum priorityType, string sortColumn = "", string sortDirection = "");
    }
}