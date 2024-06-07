namespace LearningHub.Nhs.Services.Interface
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// The ScormContentServerService interface.
    /// </summary>
    public interface IScormContentServerService
    {
        /// <summary>
        /// Gets the content details for a particular external url (LH or historic).
        /// </summary>
        /// <param name="externalUrl">The externalUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ContentServerViewModel"/>.</returns>
        ContentServerViewModel GetContentDetailsByExternalUrl(string externalUrl);

        /// <summary>
        /// The GetContentDetailsByExternalReference.
        /// </summary>
        /// <param name="externalReference">The externalReference<see cref="Guid"/>.</param>
        /// <returns>The <see cref="ContentServerViewModel"/>.</returns>
        ContentServerViewModel GetContentDetailsByExternalReference(string externalReference);

        /// <summary>
        /// The LogResourceReferenceEventAsync.
        /// </summary>
        /// <param name="resourceReferenceEventViewModel">The ResourceReferenceEventViewModel<see cref="ResourceReferenceEventViewModel"/>.</param>
        void LogResourceReferenceEventAsync(ResourceReferenceEventViewModel resourceReferenceEventViewModel);
    }
}
