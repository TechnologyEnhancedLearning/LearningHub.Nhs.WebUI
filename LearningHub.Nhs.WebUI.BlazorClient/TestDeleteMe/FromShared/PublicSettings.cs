namespace LearningHub.Nhs.WebUI.BlazorClient.TestDeleteMe.FromShared
{
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
        public string UserApiUrl { get; set; }
        /// <inheritdoc/>
        public string LearningHubAdminUrl { get; set; }
        /// <summary>
        /// Gets or sets the OpenApiUrl.
        /// </summary>
        public string OpenApiUrl { get; set; }
        /// <summary>
        /// Backend for Frontend (BFF) URL for the Learning Hub API accessed by samesite cookie and uses httpclients with bearers to access external apis.
        /// </summary>
        public string LearningHubApiBFFUrl { get; set; }
        /// <inheritdoc/>
        public IFindwiseSettingsPublic FindwiseSettings { get; set; }
        
    }
}
