namespace LearningHub.Nhs.WebUI.Models.Search
{
    /// <summary>
    /// Defines telemetry data for a search result click event.
    /// </summary>
    public class SearchResultClickTelemetryModel
    {
        /// <summary>
        /// Gets or sets the correlation id for the originating search request.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the session id for the originating search request.
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
        /// Gets or sets the clicked result url.
        /// </summary>
        public string ResultUrl { get; set; }

        /// <summary>
        /// Gets or sets the clicked result title.
        /// </summary>
        public string ResultTitle { get; set; }

        /// <summary>
        /// Gets or sets the clicked result rank.
        /// </summary>
        public int ResultRank { get; set; }

        /// <summary>
        /// Gets or sets the resource reference id.
        /// </summary>
        public int ResourceReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the node path id.
        /// </summary>
        public int NodePathId { get; set; }

        /// <summary>
        /// Gets or sets the result type.
        /// </summary>
        public string ResultType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether result opened in new tab.
        /// </summary>
        public bool OpenInNewTab { get; set; }

        /// <summary>
        /// Gets or sets the interaction type (click/keyboard/auxclick).
        /// </summary>
        public string InteractionType { get; set; }
    }
}
