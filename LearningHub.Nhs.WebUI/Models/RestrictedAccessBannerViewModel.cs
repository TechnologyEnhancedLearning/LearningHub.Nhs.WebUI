namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.User;

     /// <summary>
    /// Defines the <see cref="RestrictedAccessBannerViewModel" />.
    /// </summary>
    public class RestrictedAccessBannerViewModel
    {
        /// <summary>
        /// Gets or sets the banner title text.
        /// </summary>
        public string TitleText { get; set; }

        /// <summary>
        /// Gets or sets the banner body text.
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        /// Gets or sets the catalogue node version Id.
        /// </summary>
        public int CatalogueNodeVersionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the access to the catalogue is restricted.
        /// </summary>
        public bool RestrictedAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has access to the catalogue.
        /// </summary>
        public bool HasCatalogueAccess { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueAccessRequest if there is one.
        /// </summary>
        public CatalogueAccessRequestViewModel CatalogueAccessRequest { get; set; }

        /// <summary>
        /// Gets or sets the current user's usergroups.
        /// </summary>
        public List<RoleUserGroupViewModel> UserGroups { get; set; }
    }
}
