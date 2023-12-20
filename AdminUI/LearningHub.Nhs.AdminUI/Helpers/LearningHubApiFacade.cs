// <copyright file="LearningHubApiFacade.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Helpers
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="LearningHubApiFacade" />.
    /// </summary>
    public class LearningHubApiFacade : ILearningHubApiFacade
    {
        /// <summary>
        /// Defines the _client.
        /// </summary>
        private readonly ILearningHubHttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubApiFacade"/> class.
        /// </summary>
        /// <param name="client">The client<see cref="ILearningHubHttpClient"/>.</param>
        public LearningHubApiFacade(ILearningHubHttpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// The GetAsync.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        public async Task<T> GetAsync<T>(string url)
            where T : class, new()
        {
            var client = await this.client.GetClientAsync();

            var vm = new T();

            var response = await client.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                vm = JsonConvert.DeserializeObject<T>(result);

                return vm;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }
        }

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task PostAsync<T>(string url, T body)
            where T : class, new()
        {
            var client = await this.client.GetClientAsync();

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }
        }

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TBody">The body type.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        public async Task<ApiResponse> PostAsync<T, TBody>(string url, TBody body)
            where TBody : class, new()
            where T : class, new()
        {
            var client = await this.client.GetClientAsync();

            var vm = new T();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.Success)
                {
                    return apiResponse;
                }
                else
                {
                    string details = string.Empty;
                    if (apiResponse.ValidationResult != null)
                    {
                        if (apiResponse.ValidationResult.Details != null)
                        {
                            details = $"::ValidationResult: {string.Join(",", apiResponse.ValidationResult.Details)}";
                        }
                    }

                    throw new Exception($"PostAsync ApiResponse returned False: {details}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Access Denied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }
        }

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> PutAsync(string url)
        {
            var client = await this.client.GetClientAsync();

            var response = await client.PutAsync(url, null).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.Success)
                {
                    return apiResponse;
                }
                else
                {
                    string details = string.Empty;
                    if (apiResponse.ValidationResult != null)
                    {
                        if (apiResponse.ValidationResult.Details != null)
                        {
                            details = $"::ValidationResult: {string.Join(",", apiResponse.ValidationResult.Details)}";
                        }
                    }

                    throw new Exception($"PutAsync ApiResponse returned False: {details}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Access Denied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }
        }

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> PutAsync<T>(string url, T body)
            where T : class, new()
        {
            var client = await this.client.GetClientAsync();

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.Success)
                {
                    return apiResponse;
                }
                else
                {
                    string details = string.Empty;
                    if (apiResponse.ValidationResult != null)
                    {
                        if (apiResponse.ValidationResult.Details != null)
                        {
                            details = $"::ValidationResult: {string.Join(",", apiResponse.ValidationResult.Details)}";
                        }
                    }

                    throw new Exception($"PutAsync ApiResponse returned False: {details}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Access Denied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
            }
        }
    }
}
