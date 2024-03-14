namespace LearningHub.Nhs.AdminUI.Models
{
    /// <summary>
    /// Defines the CatalogueNavPage.
    /// </summary>
    public enum CatalogueNavPage
    {
        /// <summary>
        /// Defines the Add.
        /// </summary>
        Add,

        /// <summary>
        /// Defines the Edit.
        /// </summary>
        Edit,

        /// <summary>
        /// Defines the Resources.
        /// </summary>
        Resources,

        /// <summary>
        /// Defines the Folders.
        /// </summary>
        Folders,

        /// <summary>
        /// Defines the UserGroups.
        /// </summary>
        UserGroups,

        /// <summary>
        /// Defines the Catalogue Owner.
        /// </summary>
        CatalogueOwner,
    }

    /// <summary>
    /// Defines the <see cref="CatalogueNavViewModel" />.
    /// </summary>
    public class CatalogueNavViewModel
    {
        /// <summary>
        /// Gets or sets the CatalogueId.
        /// </summary>
        public int CatalogueId { get; set; }

        /// <summary>
        /// Gets or sets the Page.
        /// </summary>
        public CatalogueNavPage Page { get; set; }
    }
}
