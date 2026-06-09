namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Search;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="SearchTelemetryService" />.
    /// </summary>
    public class SearchTelemetryService : ISearchTelemetryService
    {
        private readonly TelemetryClient telemetryClient;
        private readonly ILogger<SearchTelemetryService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTelemetryService"/> class.
        /// </summary>
        /// <param name="telemetryClient">Application Insights telemetry client.</param>
        /// <param name="logger">Logger instance.</param>
        public SearchTelemetryService(TelemetryClient telemetryClient, ILogger<SearchTelemetryService> logger)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Records search executed telemetry for zero-result rate analysis and latency measurement.
        /// </summary>
        /// <param name="search">The search request view model.</param>
        /// <param name="searchResult">The search result view model containing results.</param>
        /// <param name="latencyMs">The search execution latency in milliseconds.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RecordSearchExecutedAsync(SearchRequestViewModel search, SearchResultViewModel searchResult, long latencyMs)
        {
            if (searchResult == null || string.IsNullOrWhiteSpace(search?.Term))
            {
                return Task.CompletedTask;
            }

            try
            {
                // Calculate total result count from both resource and catalogue results
                int resultCount = (searchResult.ResourceSearchResult?.TotalHits ?? 0) +
                                 (searchResult.CatalogueSearchResult?.TotalHits ?? 0);

                // Use search ID or generate new correlation ID
                var correlationId = search.SearchId > 0
                    ? search.SearchId.ToString()
                    : Guid.NewGuid().ToString();

                var groupId = search.GroupId ?? Guid.NewGuid().ToString();

                var properties = new Dictionary<string, string>
                {
                    { "CorrelationId", correlationId },
                    { "SessionId", groupId },
                    { "QueryText", search.Term ?? string.Empty },
                    { "QueryMode", "standard" },
                    { "UseSemanticReranker", "false" },
                    { "ResultType", "combined" }, // Could be resource, catalogue, or combined
                };

                var metrics = new Dictionary<string, double>
                {
                    { "ResultCount", resultCount },
                    { "LatencyMs", latencyMs },
                };

                this.telemetryClient.TrackEvent("SearchExecutedTelemetry", properties, metrics);
            }
            catch (Exception ex)
            {
                // Log the exception but don't let telemetry errors impact search functionality
                this.logger.LogError(ex, "Failed to record SearchExecutedTelemetry for query: {QueryText}", search?.Term);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Records search result click telemetry for click-through analysis.
        /// </summary>
        /// <param name="model">The search result click telemetry model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RecordResultClickTelemetryAsync(SearchResultClickTelemetryModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ResultUrl))
            {
                return Task.CompletedTask;
            }

            try
            {
                var properties = new Dictionary<string, string>
                {
                    { "CorrelationId", model.CorrelationId ?? string.Empty },
                    { "SessionId", model.SessionId ?? string.Empty },
                    { "QueryText", model.QueryText ?? string.Empty },
                    { "QueryMode", model.QueryMode ?? string.Empty },
                    { "ResultUrl", model.ResultUrl ?? string.Empty },
                    { "ResultTitle", model.ResultTitle ?? string.Empty },
                    { "ResultType", model.ResultType ?? string.Empty },
                    { "OpenInNewTab", model.OpenInNewTab.ToString() },
                    { "InteractionType", model.InteractionType ?? string.Empty },
                };

                var metrics = new Dictionary<string, double>
                {
                    { "ResultRank", model.ResultRank },
                    { "ResourceReferenceId", model.ResourceReferenceId },
                    { "NodePathId", model.NodePathId },
                };

                this.telemetryClient.TrackEvent("SearchResultClickTelemetry", properties, metrics);
            }
            catch (Exception ex)
            {
                // Log the exception but don't let telemetry errors impact search functionality
                this.logger.LogError(ex, "Failed to record SearchResultClickTelemetry for result: {ResultTitle}", model?.ResultTitle);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Records search executed telemetry from API endpoints.
        /// </summary>
        /// <param name="model">The search executed telemetry model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        // public Task RecordSearchExecutedFromApiAsync(SearchExecutedTelemetryModel model)
        // {
        //    if (model == null || string.IsNullOrWhiteSpace(model.QueryText))
        //    {
        //        return Task.CompletedTask;
        //    }

        // try
        //    {
        //        var properties = new Dictionary<string, string>
        //        {
        //            { "CorrelationId", model.CorrelationId ?? string.Empty },
        //            { "SessionId", model.SessionId ?? string.Empty },
        //            { "QueryText", model.QueryText ?? string.Empty },
        //            { "QueryMode", model.QueryMode ?? string.Empty },
        //            { "UseSemanticReranker", model.UseSemanticReranker.ToString() },
        //            { "ResultType", model.ResultType ?? string.Empty },
        //        };

        // var metrics = new Dictionary<string, double>
        //        {
        //            { "ResultCount", model.ResultCount },
        //            { "LatencyMs", model.LatencyMs },
        //        };

        // this.telemetryClient.TrackEvent("SearchExecutedTelemetry", properties, metrics);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception but don't let telemetry errors impact search functionality
        //        this.logger.LogError(ex, "Failed to record SearchExecutedTelemetry from API for query: {QueryText}", model?.QueryText);
        //    }

        // return Task.CompletedTask;
        // }

        /// <summary>
        /// Records search facet applied telemetry for facet usage analysis.
        /// </summary>
        /// <param name="model">The search facet applied telemetry model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RecordFacetAppliedTelemetryAsync(SearchFacetAppliedTelemetryModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.FacetField))
            {
                return Task.CompletedTask;
            }

            try
            {
                var properties = new Dictionary<string, string>
                {
                    { "CorrelationId", model.CorrelationId ?? string.Empty },
                    { "SessionId", model.SessionId ?? string.Empty },
                    { "QueryText", model.QueryText ?? string.Empty },
                    { "QueryMode", model.QueryMode ?? string.Empty },
                    { "FacetField", model.FacetField ?? string.Empty },
                    { "FacetValue", model.FacetValue ?? string.Empty },
                    { "FacetAction", model.FacetAction ?? string.Empty },
                };

                this.telemetryClient.TrackEvent("SearchFacetAppliedTelemetry", properties);
            }
            catch (Exception ex)
            {
                // Log the exception but don't let telemetry errors impact search functionality
                this.logger.LogError(ex, "Failed to record SearchFacetAppliedTelemetry for facet: {FacetField}", model?.FacetField);
            }

            return Task.CompletedTask;
        }
    }
}
