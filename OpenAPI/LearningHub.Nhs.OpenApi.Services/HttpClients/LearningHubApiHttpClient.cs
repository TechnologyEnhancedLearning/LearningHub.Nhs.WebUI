// <copyright file="LearningHubApiHttpClient.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

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

    /// <summary>
    /// Http client for LearningHub bookmarks.
    /// </summary>
    public class LearningHubApiHttpClient : ILearningHubApiHttpClient
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubApiHttpClient"/> class.
        /// </summary>
        /// <param name="learningHubApiConfig">Configuration details for the LearningHubApi.</param>
        public LearningHubApiHttpClient(IOptions<LearningHubApiConfig> learningHubApiConfig)
        {
            this.httpClient = new HttpClient { BaseAddress = new Uri(learningHubApiConfig.Value.BaseUrl) };

            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.httpClient.Dispose();
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
