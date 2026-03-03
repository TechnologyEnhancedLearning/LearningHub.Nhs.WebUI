using Newtonsoft.Json;

namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    /// <summary>
    /// DatabricksNotification
    /// </summary>
    public class DatabricksNotification
    {
        /// <summary>
        /// Gets or sets <see cref="EventType"/>.
        /// </summary>
        [JsonProperty("event_type")]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Run"/>.
        /// </summary>
        [JsonProperty("run")]
        public RunInfo Run { get; set; }

        /// <summary>
        /// RunInfo
        /// </summary>
        public class RunInfo
        {
            /// <summary>
            /// Gets or sets <see cref="RunId"/>.
            /// </summary>
            [JsonProperty("run_id")]
            public long RunId { get; set; }

            /// <summary>
            /// Gets or sets <see cref="ParentRunId"/>.
            /// </summary>
            [JsonProperty("parent_run_id")]
            public long ParentRunId { get; set; }
        }
    }
}
