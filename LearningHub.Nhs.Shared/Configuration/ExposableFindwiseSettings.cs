namespace LearningHub.Nhs.Shared.Configuration
{
    using LearningHub.Nhs.Shared.Interfaces.Configuration;
    /// <summary>
    /// Represents a public-facing set of configuration values for Findwise search,
    /// intended to be safely exposed to client-side applications or public APIs.
    /// 
    /// <para>
    /// Contains only non-sensitive data such as page sizes for various search types.
    /// </para>
    /// </summary>
    public class ExposableFindwiseSettings : IExposableFindwiseSettings
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
