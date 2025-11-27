namespace LearningHub.Nhs.OpenApi.Services.HttpClients
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    /// <summary>
    /// Http client for Databricks.
    /// </summary>
    public class DatabricksApiHttpClient : IDatabricksApiHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<DatabricksConfig> databricksConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabricksApiHttpClient"/> class.
        /// </summary>
        /// <param name="databricksConfig">Configuration details for the databricks.</param>
        public DatabricksApiHttpClient(IOptions<DatabricksConfig> databricksConfig)
        {
            this.databricksConfig = databricksConfig;
            this.httpClient = new HttpClient { BaseAddress = new Uri(databricksConfig.Value.InstanceUrl) };
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
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
        public HttpClient GetClient()
        {
            string accessToken = this.databricksConfig.Value.Token;
            this.httpClient.SetBearerToken(accessToken);
            return this.httpClient;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetData(string requestUrl, string? authHeader)
        {
            if (!string.IsNullOrEmpty(authHeader))
            {
                this.httpClient.SetBearerToken(authHeader);
            }

            var message = await this.httpClient.GetAsync(requestUrl).ConfigureAwait(false);
            return message;
        }
    }
}
