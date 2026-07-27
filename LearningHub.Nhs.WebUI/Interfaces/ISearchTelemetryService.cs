namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Models.Search;

    /// <summary>
    /// Defines the <see cref="ISearchTelemetryService" />.
    /// </summary>
    public interface ISearchTelemetryService
    {
        /// <summary>
        /// Records search executed telemetry for zero-result rate analysis and latency measurement.
        /// </summary>
        /// <param name="search">The search request view model.</param>
        /// <param name="searchResult">The search result view model containing results.</param>
        /// <param name="latencyMs">The search execution latency in milliseconds.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RecordSearchExecutedAsync(SearchRequestViewModel search, SearchResultViewModel searchResult, long latencyMs);

        /// <summary>
        /// Records search executed telemetry for API search endpoints.
        /// </summary>
        /// <param name="searchViewModel">The search view model returned from the search service.</param>
        /// <param name="queryText">The search query text.</param>
        /// <param name="groupId">The group ID used as session identifier.</param>
        /// <param name="latencyMs">The search execution latency in milliseconds.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RecordApiSearchExecutedAsync(SearchViewModel searchViewModel, string queryText, System.Guid groupId, long latencyMs);

        /// <summary>
        /// Records search result click telemetry for click-through analysis.
        /// </summary>
        /// <param name="model">The search result click telemetry model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RecordResultClickTelemetryAsync(SearchResultClickTelemetryModel model);

        /// <summary>
        /// Records search executed telemetry from API endpoints.
        /// </summary>
        /// <param name="model">The search executed telemetry model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RecordSearchExecutedFromApiAsync(SearchExecutedTelemetryModel model);

        /// <summary>
        /// Records search facet applied telemetry for facet usage analysis.
        /// </summary>
        /// <param name="model">The search facet applied telemetry model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RecordFacetAppliedTelemetryAsync(SearchFacetAppliedTelemetryModel model);
    }
}
