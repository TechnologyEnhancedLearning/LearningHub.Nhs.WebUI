namespace LearningHub.Nhs.WebUI.Models.Search
{
    /// <summary>
    /// Defines telemetry data for a search facet applied event.
    /// </summary>
    public class SearchFacetAppliedTelemetryModel
    {
        /// <summary>
        /// Gets or sets the correlation id for the originating search request.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the session id for the search session.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the query text.
        /// </summary>
        public string QueryText { get; set; }

        /// <summary>
        /// Gets or sets the query mode.
        /// </summary>
        public string QueryMode { get; set; }

        /// <summary>
        /// Gets or sets the facet field name (e.g. "ResultType", "Category").
        /// </summary>
        public string FacetField { get; set; }

        /// <summary>
        /// Gets or sets the facet value (e.g. "Guidance", "Policy").
        /// </summary>
        public string FacetValue { get; set; }

        /// <summary>
        /// Gets or sets the facet action ("applied", "removed", or "cleared").
        /// </summary>
        public string FacetAction { get; set; }
    }
}
