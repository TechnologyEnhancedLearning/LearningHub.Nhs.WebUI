namespace LearningHub.Nhs.Services.Interface
{
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The INotificationTemplateService interface.
    /// </summary>
    public interface INotificationTemplateService
    {
        /// <summary>
        /// The GetCatalogueAccessRequestAccepted.
        /// </summary>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <returns>The notification with message and title set.</returns>
        Notification GetCatalogueAccessRequestAccepted(string catalogueName, string catalogueUrl);

        /// <summary>
        /// The GetCatalogueAccessRequestRejected.
        /// </summary>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <param name="rejectionReason">The rejectionReason.</param>
        /// <returns>The notification with message and title set.</returns>
        Notification GetCatalogueAccessRequestFailure(string catalogueName, string catalogueUrl, string rejectionReason);

        /// <summary>
        /// The GetCatalogueEditorAdded.
        /// </summary>
        /// <param name="supportUrl">The support url.</param>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <param name="catalogueUrl">The catalogue url.</param>
        /// <returns>The notification with message and title set.</returns>
        Notification GetCatalogueEditorAdded(string supportUrl, string catalogueName, string catalogueUrl);

        /// <summary>
        /// The GetCatalogueEditorRemoved.
        /// </summary>
        /// <param name="supportUrl">The support url.</param>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <param name="catalogueUrl">The catalogue url.</param>
        /// <returns>The notification with message and title set.</returns>
        Notification GetCatalogueEditorRemoved(string supportUrl, string catalogueName, string catalogueUrl);
    }
}
