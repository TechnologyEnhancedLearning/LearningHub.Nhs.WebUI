using System.Collections.Generic;

namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The JiraApiConfig.
    /// </summary>
    public class JiraApiConfig
    {
        /// <summary>
        /// Gets or sets the base url for the Jira issues search.
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the search url.
        /// </summary>
        public string SearchEndpoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets component url.
        /// </summary>
        public string ComponentEndpoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string ApiToken { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ComponentIds.
        /// </summary>
        public List<string> ComponentIds { get; set; } = new();

        /// <summary>
        /// Gets or sets max results count.
        /// </summary>
        public int MaxResults { get; set; }
    }
}
