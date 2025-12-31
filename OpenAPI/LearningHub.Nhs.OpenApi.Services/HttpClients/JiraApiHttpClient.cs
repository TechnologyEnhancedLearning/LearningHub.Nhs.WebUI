namespace LearningHub.Nhs.OpenApi.Services.HttpClients
{
    using LearningHub.Nhs.Models.JiraRoadmap;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The JiraApiHttpClient.
    /// </summary>
    public class JiraApiHttpClient : IJiraApiHttpClient
    {
        private readonly JiraApiConfig jiraApiConfig;
        private readonly HttpClient httpClient = new();
        private readonly ILogger<JiraApiHttpClient> logger;

        private bool initialised = false;
        private string jiraApiBaseUrl;
        private string jiraApiEmail;
        private string jiraApiToken;
        private string jiraSearchEndpoint;
        private string jiraComponentEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraApiHttpClient"/> class.
        /// </summary>
        /// <param name="httpClient">The http client.</param>
        /// <param name="jiraApiConfig">the config.</param>
        public JiraApiHttpClient(HttpClient httpClient,
            IOptions<JiraApiConfig> jiraApiConfig, ILogger<JiraApiHttpClient> logger)
        {
            this.jiraApiConfig = jiraApiConfig.Value;
            this.httpClient = httpClient;
            this.logger = logger;

            this.jiraApiBaseUrl = this.jiraApiConfig.BaseUrl;
            this.jiraApiEmail = this.jiraApiConfig.Email;
            this.jiraApiToken = this.jiraApiConfig.ApiToken;
            this.jiraSearchEndpoint = this.jiraApiConfig.SearchEndpoint;
            this.jiraComponentEndpoint = this.jiraApiConfig.ComponentEndpoint;
        }

        /// <summary>
        /// Api call to get Component Metadata.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<ComponentMetadata>> GetComponentMetadata()
        {
            List<ComponentMetadata> viewModel = new List<ComponentMetadata>();
            this.Initialise();
            var requestUrl = this.jiraComponentEndpoint;
            var response = await this.httpClient.GetAsync(requestUrl).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewModel = JsonConvert.DeserializeObject<List<ComponentMetadata>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                this.logger.LogError($"Get Components Request failed, HTTP Status Code:{response.StatusCode}");
                throw new Exception("AccessDenied to Jira endpoint");
            }
            else
            {
                var error = response.Content.ReadAsStringAsync().Result.ToString();
                this.logger.LogError($"Get Components Request failed, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
                throw new Exception("Error with Jira endpoint request");
            }

            return viewModel;
        }

        /// <summary>
        /// To Get All Public Issues.
        /// </summary>
        /// <returns></returns>
        public async Task<JiraSearchResponse> GetAllPublicIssues(string jql)
        {
            JiraSearchResponse viewModel = new JiraSearchResponse();
            this.Initialise();

            var requestUrl = this.jiraSearchEndpoint + jql;

            var response = await this.httpClient.GetAsync(requestUrl).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewModel = JsonConvert.DeserializeObject<JiraSearchResponse>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                this.logger.LogError($"Get Jira Search Result failed, HTTP Status Code:{response.StatusCode}");
                throw new Exception("AccessDenied to Jira endpoint");
            }
            else
            {
                var error = response.Content.ReadAsStringAsync().Result.ToString();
                this.logger.LogError($"Get Jira Search Result failed, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
                throw new Exception("Error with Jira endpoint request");
            }

            return viewModel;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        private void Initialise()
        {
            if (this.initialised == false)
            {
                this.httpClient.BaseAddress = new Uri(jiraApiBaseUrl);
                this.httpClient.DefaultRequestHeaders.Accept.Clear();
                var authBytes = Encoding.ASCII.GetBytes($"{jiraApiEmail}:{jiraApiToken}");
                var authValue = Convert.ToBase64String(authBytes);
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
                this.httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                this.initialised = true;
            }
        }
    }
}
