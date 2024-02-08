namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Class.
    /// </summary>
    public class BulkCatalogueViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkCatalogueViewModel"/> class.
        /// </summary>
        /// <param name="catalogues"><see cref="Catalogues"/>>.</param>
        public BulkCatalogueViewModel(List<CatalogueViewModel> catalogues)
        {
            this.Catalogues = catalogues;
        }

        /// <summary>
        /// Gets <see cref="Catalogues"/>.
        /// </summary>
        public List<CatalogueViewModel> Catalogues { get; }
    }
}