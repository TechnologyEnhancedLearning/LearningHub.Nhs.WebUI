namespace LearningHub.Nhs.WebUI.Models.Resource
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;

    /// <summary>
    /// Defines the <see cref="ResourceValidationResultViewModel" />.
    /// </summary>
    public class ResourceValidationResultViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user has access.
        /// </summary>
        public bool HasAccess { get; set; }

        /// <summary>
        /// Gets or sets the ResourceVersionValidationResult.
        /// </summary>
        public ResourceVersionValidationResultViewModel ResourceVersionValidationResult { get; set; }
    }
}