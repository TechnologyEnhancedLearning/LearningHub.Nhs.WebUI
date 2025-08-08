namespace LearningHub.Nhs.Shared.Interfaces.Configuration
{
    /// <summary>
    /// Defines a contract for configuration data that is non-sensitive and safe to expose publicly
    ///
    /// <para>
    /// This interface exposes only data that is safe to be publicly consumed or shared,
    /// such as API endpoint URLs or non-sensitive configuration values.
    /// It explicitly excludes any private or sensitive information (e.g., authentication tokens,
    /// credentials, or secret keys), which should be handled via separate interfaces or services.
    /// </para>
    ///
    /// <para>
    /// The data provided by this interface can be safely used in frontend technologies,
    /// such as Blazor WebAssembly, JavaScript frameworks, or other client-side applications,
    /// without risking exposure of sensitive information.
    /// </para>
    /// </summary>
    public interface IPublicSettings
    {
        /// <summary>
        /// Gets or sets the LearningHubApiUrl.
        /// </summary>
        public string LearningHubApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the UserApiUrl.
        /// </summary>
        public string UserApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the OpenApiUrl.
        /// </summary>
        public string OpenApiUrl { get; set; }
        /// <summary>
        /// Gets or sets the LearningHubApiBFFUrl used to proxy via same domain cookie to the BFF LearningHubAPI calls.
        /// </summary>
        public string LearningHubApiBFFUrl { get; set; }

        public IFindwiseSettingsPublic FindwiseSettings { get; set; }
    }
}
