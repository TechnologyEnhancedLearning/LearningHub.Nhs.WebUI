namespace LearningHub.Nhs.WebUI.Configuration
{
    /// <summary>
    /// The learning hub auth service config.
    /// </summary>
    public class LearningHubAuthServiceConfig
    {
        /// <summary>
        /// Gets or sets the authority.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the auth secret.
        /// </summary>
        public string AuthSecret { get; set; }

        /// <summary>
        /// Gets or sets the auth timeout.
        /// </summary>
        public int AuthTimeout { get; set; }

        /// <summary>
        /// Gets or sets the open athens config.
        /// </summary>
        public OpenAthens OpenAthens { get; set; }
    }
}
