namespace LearningHub.Nhs.WebUI.Models.Search
{
    /// <summary>
    /// Defines telemetry data for a search executed event.
    /// </summary>
    public class SearchExecutedTelemetryModel
    {
        /// <summary>
        /// Gets or sets the correlation id for the search request.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the session id for the search request.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the query text.
        /// </summary>
        public string QueryText { get; set; }

        /// <summary>
        /// Gets or sets the query mode (keyword, hybrid, semantic).
        /// </summary>
        public string QueryMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether semantic reranker was used.
        /// </summary>
        public bool UseSemanticReranker { get; set; }

        /// <summary>
        /// Gets or sets the result type.
        /// </summary>
        public string ResultType { get; set; }

        /// <summary>
        /// Gets or sets the count of results returned.
        /// </summary>
        public int ResultCount { get; set; }

        /// <summary>
        /// Gets or sets the latency in milliseconds.
        /// </summary>
        public long LatencyMs { get; set; }
    }
}
