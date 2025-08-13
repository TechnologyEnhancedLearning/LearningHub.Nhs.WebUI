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
    public class MoodleHttpClient : IMoodleHttpClient, IDisposable
    {
        private readonly MoodleConfig moodleConfig;
        private readonly HttpClient httpClient = new();

        private bool initialised = false;
        private string moodleAPIBaseUrl;
        private string moodleAPIMoodleWSRestFormat;
        private string moodleAPIWSToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleHttpClient"/> class.
        /// </summary>
        /// <param name="httpClient">httpClient.</param>
        /// <param name="moodleConfig">config.</param>
        public MoodleHttpClient(HttpClient httpClient, IOptions<MoodleConfig> moodleConfig)
        {
            this.moodleConfig = moodleConfig.Value;
            this.httpClient = httpClient;

            this.moodleAPIBaseUrl = this.moodleConfig.APIBaseUrl + "webservice/rest/server.php";
            this.moodleAPIMoodleWSRestFormat = this.moodleConfig.APIWSRestFormat;
            this.moodleAPIWSToken = this.moodleConfig.APIWSToken;
        }

        /// <summary>
        /// The Get Client method.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HttpClient> GetClient()
        {
            this.Initialise(this.moodleAPIBaseUrl);
            return this.httpClient;
        }

        /// <summary>
        /// GetDefaultParameters.
        /// </summary>
        /// <returns>defaultParameters.</returns>
        public string GetDefaultParameters()
        {
            string defaultParameters = $"wstoken={this.moodleAPIWSToken}"
                              + $"&moodlewsrestformat={this.moodleAPIMoodleWSRestFormat}";

            return defaultParameters;
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
                this.initialised = true;
            }
        }
    }
}
