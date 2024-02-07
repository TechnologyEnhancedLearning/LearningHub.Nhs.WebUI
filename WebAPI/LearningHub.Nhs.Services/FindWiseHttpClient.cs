// <copyright file="FindWiseHttpClient.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// The FindWise Http Client.
    /// </summary>
    public class FindWiseHttpClient : IFindWiseHttpClient, IDisposable
    {
        private readonly HttpClient httpClient = new ();
        //// private readonly LearningHubAuthServiceConfig authConfig;
        private bool initialised = false;

        /// <summary>
        /// The Get Client method.
        /// </summary>
        /// <param name="httpClientUrl">The url of the client.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HttpClient> GetClient(string httpClientUrl)
        {
            this.Initialise(httpClientUrl);
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
                this.initialised = true;
            }
        }
    }
}
