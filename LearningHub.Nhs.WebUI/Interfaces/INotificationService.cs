namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="INotificationService" />.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="userNotificationId">User notification id.</param>
        /// <param name="notificationid">Notification id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task Delete(int userNotificationId, int notificationid);

        /// <summary>
        /// The MarkAsRead.
        /// </summary>
        /// <param name="userNotificationId">User notification id.</param>
        /// <param name="notificationid">Notification id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task MarkAsRead(int userNotificationId, int notificationid);

        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">Paging request.</param>
        /// <param name="priorityType">Notification priority type.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PagedResultSet<UserNotificationViewModel>> GetPagedAsync(PagingRequestModel pagingRequestModel, NotificationPriorityEnum priorityType);

        /// <summary>
        /// The GetUserNotificationIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserNotification> GetUserNotificationIdAsync(int id);

        /// <summary>
        /// The GetUserUnreadNotificationCountAsync.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> GetUserUnreadNotificationCountAsync(int userId);
    }
}
