namespace LearningHub.NHS.OpenAPI.Auth
{
    using System.Collections.Generic;

    /// <summary>
    /// API key client.
    /// </summary>
    public class ApiKeyClient
    {
        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the client's keys.
        /// </summary>
        public IEnumerable<string> Keys { get; set; } = null!;
    }
}
