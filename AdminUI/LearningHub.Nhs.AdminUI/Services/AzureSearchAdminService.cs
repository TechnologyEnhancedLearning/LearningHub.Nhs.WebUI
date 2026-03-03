namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Service for Azure Search administration operations.
    /// </summary>
    public class AzureSearchAdminService : IAzureSearchAdminService
    {
        /// <summary>
        /// The Azure Search REST API version.
        /// </summary>
        private const string ApiVersion = "2024-07-01";

        private readonly AzureSearchConfig config;
        private readonly ILogger<AzureSearchAdminService> logger;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchAdminService"/> class.
        /// </summary>
        /// <param name="config">The Azure Search configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public AzureSearchAdminService(
            IOptions<AzureSearchConfig> config,
            ILogger<AzureSearchAdminService> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.config = config.Value;
            this.logger = logger;
            this.httpClient = httpClientFactory.CreateClient("AzureSearch");
            this.ConfigureHttpClient();
        }

        /// <inheritdoc/>
        public async Task<List<IndexerStatusViewModel>> GetIndexersStatusAsync()
        {
            var result = new List<IndexerStatusViewModel>();

            try
            {
                if (string.IsNullOrEmpty(this.config.ServiceEndpoint) || string.IsNullOrEmpty(this.config.AdminApiKey))
                {
                    this.logger.LogWarning("Azure Search configuration is not set");
                    return result;
                }

                // Get list of indexers
                var indexersResponse = await this.httpClient.GetAsync($"indexers?api-version={ApiVersion}");
                if (!indexersResponse.IsSuccessStatusCode)
                {
                    this.logger.LogError("Failed to get indexers list: {StatusCode}", indexersResponse.StatusCode);
                    return result;
                }

                var indexersJson = await indexersResponse.Content.ReadAsStringAsync();
                using var indexersDoc = JsonDocument.Parse(indexersJson);

                if (indexersDoc.RootElement.TryGetProperty("value", out var indexersArray))
                {
                    foreach (var indexer in indexersArray.EnumerateArray())
                    {
                        var indexerName = indexer.GetProperty("name").GetString();
                        var encodedIndexerName = Uri.EscapeDataString(indexerName);

                        // Get status for each indexer
                        var statusResponse = await this.httpClient.GetAsync($"indexers/{encodedIndexerName}/status?api-version={ApiVersion}");
                        if (statusResponse.IsSuccessStatusCode)
                        {
                            var statusJson = await statusResponse.Content.ReadAsStringAsync();
                            var statusViewModel = this.ParseIndexerStatus(indexerName, statusJson);
                            result.Add(statusViewModel);
                        }
                        else
                        {
                            result.Add(new IndexerStatusViewModel
                            {
                                Name = indexerName,
                                Status = "Unknown",
                                LastRunStatus = "Error retrieving status",
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting indexers status");
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<IndexStatusViewModel>> GetIndexesStatusAsync()
        {
            var result = new List<IndexStatusViewModel>();

            try
            {
                if (string.IsNullOrEmpty(this.config.ServiceEndpoint) || string.IsNullOrEmpty(this.config.AdminApiKey))
                {
                    this.logger.LogWarning("Azure Search configuration is not set");
                    return result;
                }

                // Get list of indexes
                var indexesResponse = await this.httpClient.GetAsync($"indexes?api-version={ApiVersion}");
                if (!indexesResponse.IsSuccessStatusCode)
                {
                    this.logger.LogError("Failed to get indexes list: {StatusCode}", indexesResponse.StatusCode);
                    return result;
                }

                var indexesJson = await indexesResponse.Content.ReadAsStringAsync();
                using var indexesDoc = JsonDocument.Parse(indexesJson);

                if (indexesDoc.RootElement.TryGetProperty("value", out var indexesArray))
                {
                    foreach (var index in indexesArray.EnumerateArray())
                    {
                        var indexName = index.GetProperty("name").GetString();
                        var encodedIndexName = Uri.EscapeDataString(indexName);

                        // Get statistics for each index
                        var statsResponse = await this.httpClient.GetAsync($"indexes/{encodedIndexName}/stats?api-version={ApiVersion}");
                        if (statsResponse.IsSuccessStatusCode)
                        {
                            var statsJson = await statsResponse.Content.ReadAsStringAsync();
                            using var statsDoc = JsonDocument.Parse(statsJson);

                            result.Add(new IndexStatusViewModel
                            {
                                Name = indexName,
                                DocumentCount = statsDoc.RootElement.TryGetProperty("documentCount", out var docCount) ? docCount.GetInt64() : null,
                                StorageSize = statsDoc.RootElement.TryGetProperty("storageSize", out var storageSize) ? storageSize.GetInt64() : null,
                            });
                        }
                        else
                        {
                            result.Add(new IndexStatusViewModel
                            {
                                Name = indexName,
                                DocumentCount = null,
                                StorageSize = null,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting indexes status");
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> RunIndexerAsync(string indexerName)
        {
            try
            {
                if (string.IsNullOrEmpty(this.config.ServiceEndpoint) || string.IsNullOrEmpty(this.config.AdminApiKey))
                {
                    this.logger.LogWarning("Azure Search configuration is not set");
                    return false;
                }

                var encodedIndexerName = Uri.EscapeDataString(indexerName);
                var response = await this.httpClient.PostAsync($"indexers/{encodedIndexerName}/run?api-version={ApiVersion}", null);

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    this.logger.LogInformation("Successfully triggered indexer: {IndexerName}", indexerName);
                    return true;
                }

                this.logger.LogError("Failed to run indexer {IndexerName}: {StatusCode}", indexerName, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error running indexer {IndexerName}", indexerName);
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ResetIndexerAsync(string indexerName)
        {
            try
            {
                if (string.IsNullOrEmpty(this.config.ServiceEndpoint) || string.IsNullOrEmpty(this.config.AdminApiKey))
                {
                    this.logger.LogWarning("Azure Search configuration is not set");
                    return false;
                }

                var encodedIndexerName = Uri.EscapeDataString(indexerName);
                var response = await this.httpClient.PostAsync($"indexers/{encodedIndexerName}/reset?api-version={ApiVersion}", null);

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    this.logger.LogInformation("Successfully reset indexer: {IndexerName}", indexerName);
                    return true;
                }

                this.logger.LogError("Failed to reset indexer {IndexerName}: {StatusCode}", indexerName, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error resetting indexer {IndexerName}", indexerName);
                return false;
            }
        }

        private void ConfigureHttpClient()
        {
            if (!string.IsNullOrEmpty(this.config.ServiceEndpoint))
            {
                this.httpClient.BaseAddress = new Uri(this.config.ServiceEndpoint.TrimEnd('/') + "/");
            }

            if (!string.IsNullOrEmpty(this.config.AdminApiKey))
            {
                this.httpClient.DefaultRequestHeaders.Add("api-key", this.config.AdminApiKey);
            }

            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private IndexerStatusViewModel ParseIndexerStatus(string indexerName, string statusJson)
        {
            var viewModel = new IndexerStatusViewModel
            {
                Name = indexerName,
            };

            try
            {
                using var doc = JsonDocument.Parse(statusJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("status", out var status))
                {
                    viewModel.Status = status.GetString();
                }

                if (root.TryGetProperty("lastResult", out var lastResult))
                {
                    if (lastResult.TryGetProperty("status", out var lastStatus))
                    {
                        viewModel.LastRunStatus = lastStatus.GetString();
                    }

                    if (lastResult.TryGetProperty("endTime", out var endTime))
                    {
                        viewModel.LastRunTime = endTime.GetDateTimeOffset();
                    }

                    if (lastResult.TryGetProperty("errorMessage", out var errorMessage))
                    {
                        viewModel.LastRunErrorMessage = errorMessage.GetString();
                    }

                    if (lastResult.TryGetProperty("itemsProcessed", out var itemsProcessed))
                    {
                        viewModel.ItemsProcessed = itemsProcessed.GetInt32();
                    }

                    if (lastResult.TryGetProperty("itemsFailed", out var itemsFailed))
                    {
                        viewModel.ItemsFailed = itemsFailed.GetInt32();
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error parsing indexer status for {IndexerName}", indexerName);
                viewModel.Status = "Error parsing status";
            }

            return viewModel;
        }
    }
}