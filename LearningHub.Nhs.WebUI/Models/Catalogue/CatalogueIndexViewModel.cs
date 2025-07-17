namespace LearningHub.Nhs.WebUI.Models.Catalogue
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.WebUI.Shared.Models.Search;

    /// <summary>
    /// Defines the <see cref="CatalogueIndexViewModel" />.
    /// </summary>
    public class CatalogueIndexViewModel
    {
        /// <summary>
        /// Gets or sets the catalogue view model.
        /// </summary>
        public CatalogueViewModel Catalogue { get; set; }

        /// <summary>
        /// Gets or sets the node details view model.
        /// </summary>
        public NodeViewModel NodeDetails { get; set; }

        /// <summary>
        /// Gets or sets the node details for each node in the node path.
        /// </summary>
        public List<NodeViewModel> NodePathNodes { get; set; }

        /// <summary>
        /// Gets or sets the contents of the node currently being displayed in the UI.
        /// The node will either be the catalogue root or a folder.
        /// </summary>
        public List<NodeContentBrowseViewModel> NodeContents { get; set; }

        /// <summary>
        /// Gets or sets the current user's usergroups.
        /// </summary>
        public List<RoleUserGroupViewModel> UserGroups { get; set; }

        /// <summary>
        /// Gets or sets the latest catalogue access request for the current user.
        /// </summary>
        public CatalogueAccessRequestViewModel CatalogueAccessRequest { get; set; }

        /// <summary>
        /// Gets or sets the search result view model.
        /// </summary>
        public SearchResultViewModel SearchResults { get; set; }
    }
}
