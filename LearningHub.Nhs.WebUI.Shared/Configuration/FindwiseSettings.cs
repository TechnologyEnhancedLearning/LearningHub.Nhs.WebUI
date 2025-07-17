using LearningHub.Nhs.WebUI.Shared.Configuration.ConfigurationSeperatingInterfaces;

namespace LearningHub.Nhs.WebUI.Shared.Configuration { 
    /// <summary>
    /// Defines the <see cref="FindwiseSettings" />.
    /// </summary>
    public class FindwiseSettings : IFindwiseSettings
    {
        /// <summary>
        /// Gets or sets the ResourceSearchPageSize.
        /// </summary>
        public int ResourceSearchPageSize { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueSearchPageSize.
        /// </summary>
        public int CatalogueSearchPageSize { get; set; }

        /// <summary>
        /// Gets or sets the AllCatalogueSearchPageSize.
        /// </summary>
        public int AllCatalogueSearchPageSize { get; set; }
    }
}
