using LearningHub.Nhs.Shared.Interfaces.Http;

namespace LearningHub.Nhs.WebUI.BlazorClient.Services 
{
    public class GenericAPIHttpClient : IAPIHttpClient, ILearningHubHttpClient, IUserApiHttpClient, IOpenApiHttpClient
    {
        private readonly HttpClient _httpClient; // Private field to hold the injected HttpClient

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericAPIHttpClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance provided by dependency injection.</param>
        public GenericAPIHttpClient(HttpClient httpClient) // Inject HttpClient
        {
            _httpClient = httpClient;
        }

        public string ApiUrl => _httpClient.BaseAddress.AbsoluteUri;

        /// <summary>
        /// Retrieves the configured HttpClient instance.
        /// </summary>
        /// <returns>A Task that resolves to the HttpClient instance.</returns>
        public Task<HttpClient> GetClientAsync()
        {
            // Return the injected HttpClient instance wrapped in a completed Task
            return Task.FromResult(_httpClient);
        }
    }
}
