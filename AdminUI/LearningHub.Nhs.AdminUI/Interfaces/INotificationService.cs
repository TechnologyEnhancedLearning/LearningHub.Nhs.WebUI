// <copyright file="INotificationService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="INotificationService" />.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="notification">The notification<see cref="NotificationViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> Create(NotificationViewModel notification);

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="notification">The notification<see cref="NotificationViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task Edit(NotificationViewModel notification);

        /// <summary>
        /// The GetIdAsync.
        /// </summary>
        /// <param name="id">The notification id.</param>
        /// <returns>The <see cref="Task{NotificationViewModel}"/>.</returns>
        Task<NotificationViewModel> GetIdAsync(int id);

        /// <summary>
        /// Copy existing notification.
        /// </summary>
        /// <param name="id">The notification id.</param>
        /// <returns>New niotification id.</returns>
        Task<int> CopyAsync(int id);

        /// <summary>
        /// Deletes a notification.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{NotificationViewModel}"/>.</returns>
        Task<PagedResultSet<NotificationViewModel>> GetPagedAsync(PagingRequestModel pagingRequestModel);
    }
}
