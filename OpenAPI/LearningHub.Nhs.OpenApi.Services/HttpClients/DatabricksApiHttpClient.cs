namespace LearningHub.Nhs.OpenApi.Services.HttpClients
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Http client for Databricks.
    /// </summary>
    public class DatabricksApiHttpClient : IDatabricksApiHttpClient, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly DatabricksOAuthTokenService tokenService;


        /// <summary>
        /// Initializes a new instance of the <see cref="DatabricksApiHttpClient"/> class.
        /// </summary>
        /// <param name="config">Configuration details for the databricks.</param>
        public DatabricksApiHttpClient(IOptions<DatabricksConfig> config)
        {
            tokenService = new DatabricksOAuthTokenService(
                config.Value.ClientId,
                config.Value.ClientSecret,
                config.Value.InstanceUrl);

            httpClient = new HttpClient { BaseAddress = new Uri(config.Value.InstanceUrl) };
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        /// <inheritdoc />
        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        /// <summary>
        /// The Get Client method.
        /// </summary>
        /// <returns>The <see cref="HttpClient"/>.</returns>
        public async Task<HttpClient> GetClient()
        {
            string token = await tokenService.GetAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return this.httpClient;
        }


        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetData(string requestUrl)
        {
            string token = await tokenService.GetAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await httpClient.GetAsync(requestUrl);
        }

    }
}
