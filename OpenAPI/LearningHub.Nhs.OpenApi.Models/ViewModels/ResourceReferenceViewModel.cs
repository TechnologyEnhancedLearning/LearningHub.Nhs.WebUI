namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    /// <summary>
    /// ResourceReferenceDetailsViewModel.
    /// </summary>
    public class ResourceReferenceViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceViewModel"/> class.
        /// </summary>
        /// <param name="refId"><see cref="RefId"/>.</param>
        /// <param name="catalogue"><see cref="Catalogue"/>.</param>
        /// <param name="link"><see cref="Link"/>.</param>
        public ResourceReferenceViewModel(int refId, CatalogueViewModel catalogue, string link)
        {
            this.RefId = refId;
            this.Catalogue = catalogue;
            this.Link = link;
        }

        /// <summary>
        /// Gets or sets RefId.
        /// </summary>
        public int RefId { get; set; }

        /// <summary>
        /// Gets or sets catalogue.
        /// </summary>
        public CatalogueViewModel Catalogue { get; set; }

        /// <summary>
        /// Gets or sets link.
        /// </summary>
        public string Link { get; set; }
    }
}
