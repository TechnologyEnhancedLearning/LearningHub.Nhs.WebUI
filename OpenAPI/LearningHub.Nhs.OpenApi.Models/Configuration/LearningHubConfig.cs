namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// <see cref="LearningHubConfig"/>.
    /// </summary>
    public class LearningHubConfig
    {
        /// <summary>
        /// Gets or sets <see cref="BaseUrl"/>.
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="LaunchResourceEndpoint"/>.
        /// </summary>
        public string LaunchResourceEndpoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ContentServerUrl"/>.
        /// </summary>
        public string ContentServerUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="EmailConfirmationTokenExpiryMinutes"/>.
        /// </summary>
        public int EmailConfirmationTokenExpiryMinutes { get; set; } = 0;

        /// <summary>
        /// Gets or sets <see cref="LearningHubTenantId"/>.
        /// </summary>
        public int LearningHubTenantId { get; set; } = 0;

        /// <summary>
        /// Gets or sets <see cref="SupportPages"/>.
        /// </summary>
        public string SupportPages { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="SupportForm"/>.
        /// </summary>
        public string SupportForm { get; set; } = null!;

        /// <summary>
        ///  Gets or sets a value indicating whether <see cref="UseRedisCache"/>.
        /// </summary>
        public bool UseRedisCache { get; set; } = false;

        /// <summary>
        /// Gets or sets <see cref="HierarchyEditPublishQueueName"/>.
        /// </summary>
        public string HierarchyEditPublishQueueName { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ResourcePublishQueueRouteName"/>.
        /// </summary>
        public string ResourcePublishQueueRouteName { get; set; } = null!;

    }
}
