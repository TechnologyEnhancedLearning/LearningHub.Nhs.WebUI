namespace LearningHub.Nhs.Shared.Configuration
{
    using LearningHub.Nhs.Shared.Interfaces.Configuration;
    /// <summary>
    /// Represents configuration values that are safe to expose to clientside frontend applications
    /// (such as Blazor WebAssembly) or public-facing APIs.
    /// 
    /// <para>
    /// Implements <see cref="IPublicSettings"/> and contains only non-sensitive, non-secret
    /// values such as public API endpoints and pagination settings. This separation ensures
    /// that secure or private configuration data is not inadvertently exposed to clients.
    /// </para>
    /// </summary>
    public class PublicSettings : IPublicSettings
    {
        /// <inheritdoc/>
        public string LearningHubApiUrl { get; set; }

        /// <inheritdoc/>
        public int ResourceSearchPageSize { get; set; }

        /// <inheritdoc/>
        public int CatalogueSearchPageSize { get; set; }

        /// <inheritdoc/>
        public int AllCatalogueSearchPageSize { get; set; }

        /// <inheritdoc/>
        public IFindwiseSettingsPublic FindwiseSettings { get; set; }
    }
}
