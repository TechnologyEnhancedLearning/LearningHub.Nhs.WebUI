namespace LearningHub.Nhs.WebUI.Helpers
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="OpenApiFacade" />.
    /// </summary>
    public class OpenApiFacade : IOpenApiFacade
    {
        /// <summary>
        /// Defines the _client.
        /// </summary>
        private readonly IOpenApiHttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiFacade"/> class.
        /// </summary>
        /// <param name="client">The client<see cref="IOpenApiHttpClient"/>.</param>
        public OpenApiFacade(IOpenApiHttpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// The GetAsync.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        public async Task<T> GetAsync<T>(string url)
            where T : class, new()
        {
            var client = await this.client.GetClientAsync();

            var vm = new T();

            var response = await client.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
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
        /// <param name="url">The url.</param>
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
        /// <typeparam name="T">The return type.</typeparam>
        /// <typeparam name="TBody">The type of body parameter.</typeparam>
        /// <param name="url">The url.</param>
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
                var result = await response.Content.ReadAsStringAsync();
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
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> PutAsync(string url)
        {
            var client = await this.client.GetClientAsync();

            var response = await client.PutAsync(url, null).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
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
        /// <param name="url">The url.</param>
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
                var result = await response.Content.ReadAsStringAsync();
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
