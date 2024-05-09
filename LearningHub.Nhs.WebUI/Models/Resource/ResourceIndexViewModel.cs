namespace LearningHub.Nhs.WebUI.Models.Resource
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;

    /// <summary>
    /// Defines the <see cref="ResourceIndexViewModel" />.
    /// </summary>
    public class ResourceIndexViewModel
    {
        /// <summary>
        /// Gets or sets the ResourceReferenceId.
        /// </summary>
        public int ResourceReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the ResourceItemViewModel.
        /// </summary>
        public ResourceItemViewModel ResourceItem { get; set; }

        /// <summary>
        /// Gets or sets the ResourceRatingViewModel.
        /// </summary>
        public ResourceRatingViewModel ResourceRating { get; set; }

        /// <summary>
        /// Gets or sets the ScormContentDetails.
        /// </summary>
        public ExternalContentDetailsViewModel ExternalContentDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has access to the catalogue that the resource is located in.
        /// </summary>
        public bool HasCatalogueAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has access a certificate.
        /// </summary>
        public bool UserHasCertificate { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueAccessRequest.
        /// </summary>
        public CatalogueAccessRequestViewModel CatalogueAccessRequest { get; set; }

        /// <summary>
        /// Gets or sets the node path details for each node in the node path.
        /// </summary>
        public List<NodePathViewModel> NodePathNodes { get; set; }
    }
}
