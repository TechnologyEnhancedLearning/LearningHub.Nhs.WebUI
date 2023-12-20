// <copyright file="LearningHubApiFacade.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.ReportApi.Services.Interface;
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
            var response = await client.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(result);
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
        /// <typeparam name="TBody">The body.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="TBody"/>.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        public async Task<T> PostAsync<T, TBody>(string url, TBody body)
            where TBody : class, new()
            where T : class, new()
        {
            var client = await this.client.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(result);
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
        /// <typeparam name="TBody">The type.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="TBody"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> PostAsync<TBody>(string url, TBody body)
            where TBody : class, new()
        {
            var client = await this.client.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse != null && apiResponse.Success)
                {
                    return apiResponse;
                }
                else
                {
                    string details = string.Empty;
                    if (apiResponse?.ValidationResult != null)
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
                if (apiResponse != null && apiResponse.Success)
                {
                    return apiResponse;
                }
                else
                {
                    string details = string.Empty;
                    if (apiResponse?.ValidationResult != null)
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
        /// <param name="body">The body<see cref="T"/>.</param>
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
                if (apiResponse != null && apiResponse.Success)
                {
                    return apiResponse;
                }
                else
                {
                    string details = string.Empty;
                    if (apiResponse?.ValidationResult != null)
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
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TBody">The body.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="TBody"/>.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        public async Task<T> PutAsync<T, TBody>(string url, TBody body)
            where TBody : class, new()
            where T : class, new()
        {
            var client = await this.client.GetClientAsync();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(result);
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
    }
}
