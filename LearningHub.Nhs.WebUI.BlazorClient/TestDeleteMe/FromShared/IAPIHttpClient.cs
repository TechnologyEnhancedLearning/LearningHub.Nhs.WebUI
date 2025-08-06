namespace LearningHub.Nhs.WebUI.BlazorClient.TestDeleteMe.FromShared
{
    /// <summary>
    /// Represents an HTTP client for a specific API.
    /// </summary>
    public interface IAPIHttpClient
    {
        /// <summary>
        /// Gets the configured <see cref="HttpClient"/> for the API.
        /// </summary>
        Task<HttpClient> GetClientAsync();

        /// <summary>
        /// Gets the base URL of the API.
        /// </summary>
        string ApiUrl { get; }
    }
}
