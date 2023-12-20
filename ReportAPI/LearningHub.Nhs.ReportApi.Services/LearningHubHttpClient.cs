// <copyright file="LearningHubHttpClient.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using LearningHub.Nhs.ReportApi.Shared.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The learning hub http client.
    /// </summary>
    public class LearningHubHttpClient : ILearningHubHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<Settings> settings;
        private readonly ILogger<LearningHubHttpClient> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubHttpClient"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="client">
        /// The http client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public LearningHubHttpClient(
            IOptions<Settings> settings,
            HttpClient client,
            ILogger<LearningHubHttpClient> logger)
        {
            this.settings = settings;
            this.httpClient = client;
            this.logger = logger;
            this.Initialise();
        }

        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<HttpClient> GetClientAsync()
        {
            return await Task.Run(() => this.httpClient);
        }

        /// <summary>
        /// The initialise.
        /// </summary>
        private void Initialise()
        {
            this.httpClient.BaseAddress = new Uri(this.settings.Value.LearningHubApiUrl);
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add(
                "Client-Identity-Key",
                this.settings.Value.ReportApiClientIdentityKey);
        }
    }
}
