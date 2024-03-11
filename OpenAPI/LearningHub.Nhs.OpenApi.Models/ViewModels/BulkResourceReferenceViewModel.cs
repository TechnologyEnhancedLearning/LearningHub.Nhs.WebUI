namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Class.
    /// </summary>
    public class BulkResourceReferenceViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkResourceReferenceViewModel"/> class.
        /// </summary>
        /// <param name="resourceReferences"><see cref="ResourceReferences"/>>.</param>
        /// <param name="unmatchedResourceReferenceIds"><see cref="UnmatchedResourceReferenceIds"/>.</param>
        public BulkResourceReferenceViewModel(
            List<ResourceReferenceWithResourceDetailsViewModel> resourceReferences,
            List<int> unmatchedResourceReferenceIds)
        {
            this.ResourceReferences = resourceReferences;
            this.UnmatchedResourceReferenceIds = unmatchedResourceReferenceIds;
        }

        /// <summary>
        /// Gets <see cref="ResourceReferences"/>.
        /// </summary>
        public List<ResourceReferenceWithResourceDetailsViewModel> ResourceReferences { get; }

        /// <summary>
        /// Gets <see cref="UnmatchedResourceReferenceIds"/>.
        /// </summary>
        public List<int> UnmatchedResourceReferenceIds { get; }
    }
}
