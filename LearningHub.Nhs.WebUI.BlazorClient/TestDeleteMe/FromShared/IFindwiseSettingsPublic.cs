namespace LearningHub.Nhs.WebUI.BlazorClient.TestDeleteMe.FromShared
{
    /// <summary>
    /// Represents configuration values related to Findwise search that are safe to expose
    /// to client-side applications or public-facing APIs.
    /// 
    /// <para>
    /// This includes non-sensitive values such as page sizes for different types of search results.
    /// It does not contain any secure credentials or internal service configuration.
    /// </para>
    /// </summary>
    public interface IFindwiseSettingsPublic
    {
        /// <summary>
        /// Gets or sets the page size for resource search results.
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
