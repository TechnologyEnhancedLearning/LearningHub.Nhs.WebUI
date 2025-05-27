namespace LearningHub.Nhs.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The Moodle Http Client.
    /// </summary>
    public class MoodleHttpClient : IMoodleHttpClient, IDisposable
    {
        private readonly HttpClient httpClient = new ();
        private bool initialised = false;
        private string moodleAPIBaseUrl;
        private string moodleAPIMoodleWSRestFormat;
        private string moodleAPIWSToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleHttpClient"/> class.
        /// </summary>
        /// <param name="httpClient">httpClient.</param>
        /// <param name="config">config.</param>
        public MoodleHttpClient(HttpClient httpClient, IConfiguration config)
        {
            this.httpClient = httpClient;
            this.moodleAPIBaseUrl = config["MoodleAPIConfig:BaseUrl"] + "webservice/rest/server.php";
            this.moodleAPIMoodleWSRestFormat = config["MoodleAPIConfig:MoodleWSRestFormat"];
            this.moodleAPIWSToken = config["MoodleAPIConfig:WSToken"];
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
