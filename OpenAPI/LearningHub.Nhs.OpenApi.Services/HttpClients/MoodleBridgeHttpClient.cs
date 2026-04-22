using IdentityModel.Client;
using LearningHub.Nhs.Models.Entities.Reporting;
using LearningHub.Nhs.OpenApi.Models.Configuration;
using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.HttpClients
{
    /// <summary>
    /// The Moodle Http Client.
    /// </summary>
    public class MoodleBridgeHttpClient : IMoodleBridgeHttpClient, IDisposable
    {
        private readonly MoodleBridgeAPIConfig moodleBridgeApiConfig;
        private readonly HttpClient httpClient = new();

        private bool initialised = false;
        private string moodleBridgeApiUrl;
        private string moodleBridgeApiToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleBridgeHttpClient"/> class.
        /// </summary>
        /// <param name="httpClient">httpClient.</param>
        /// <param name="moodleBridgeApiConfig">config.</param>
        public MoodleBridgeHttpClient(HttpClient httpClient, IOptions<MoodleBridgeAPIConfig> moodleBridgeApiConfig)
        {
            this.moodleBridgeApiConfig = moodleBridgeApiConfig.Value;
            this.httpClient = httpClient;

            this.moodleBridgeApiUrl = this.moodleBridgeApiConfig.BaseUrl;
            this.moodleBridgeApiToken = this.moodleBridgeApiConfig.Token;
        }

        /// <summary>
        /// The Get Client method.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HttpClient> GetClient()
        {
            this.Initialise(this.moodleBridgeApiUrl);
            return this.httpClient;
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispoase.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.httpClient.Dispose();
            }
        }

        private void Initialise(string httpClientUrl)
        {
            if (this.initialised == false)
            {
                this.httpClient.BaseAddress = new Uri(httpClientUrl);
                this.httpClient.DefaultRequestHeaders.Accept.Clear();
                this.httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                this.httpClient.DefaultRequestHeaders.Add("X-API-KEY", moodleBridgeApiToken);
                this.initialised = true;
            }
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
