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
    }
}
