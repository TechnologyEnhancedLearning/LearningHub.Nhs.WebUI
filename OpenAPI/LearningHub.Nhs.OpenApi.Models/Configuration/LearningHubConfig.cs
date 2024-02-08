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
    }
}
