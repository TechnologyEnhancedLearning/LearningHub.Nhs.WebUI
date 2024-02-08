namespace LearningHub.Nhs.Services.Interface.Messaging
{
    using System.Threading.Tasks;

    /// <summary>
    /// The INotificationSenderSerivce interface.
    /// </summary>
    public interface INotificationSenderService
    {
        /// <summary>
        /// The SendCatalogueAccessRequestAcceptedNotification.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <param name="recipientUserId">The recipientUserId.</param>
        /// <returns>The task.</returns>
        Task SendCatalogueAccessRequestAcceptedNotification(int userId, string catalogueName, string catalogueUrl, int recipientUserId);

        /// <summary>
        /// The SendCatalogueAccessRequestAcceptedNotification.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <param name="rejectionReason">The rejectionReason.</param>
        /// <param name="recipientUserId">The recipientUserId.</param>
        /// <returns>The task.</returns>
        Task SendCatalogueAccessRequestRejectedNotification(int userId, string catalogueName, string catalogueUrl, string rejectionReason, int recipientUserId);
    }
}
