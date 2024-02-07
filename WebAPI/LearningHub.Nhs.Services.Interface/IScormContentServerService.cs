// <copyright file="IScormContentServerService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

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
        /// Gets the SCORM content details for a particular external url (LH or historic).
        /// </summary>
        /// <param name="externalUrl">The externalUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        ScormContentServerViewModel GetScormContentDetailsByExternalUrl(string externalUrl);

        /// <summary>
        /// The GetScormContentDetailsByExternalReference.
        /// </summary>
        /// <param name="externalReference">The externalReference<see cref="Guid"/>.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        ScormContentServerViewModel GetScormContentDetailsByExternalReference(string externalReference);

        /// <summary>
        /// The LogScormResourceReferenceEventAsync.
        /// </summary>
        /// <param name="scormResourceReferenceEventViewModel">The scormResourceReferenceEventViewModel<see cref="ScormResourceReferenceEventViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task LogScormResourceReferenceEventAsync(ScormResourceReferenceEventViewModel scormResourceReferenceEventViewModel);
    }
}
