namespace LearningHub.Nhs.AdminUI.Models
{
    using System;

    /// <summary>
    /// View model for Azure Search indexer status.
    /// </summary>
    public class IndexerStatusViewModel
    {
        /// <summary>
        /// Gets or sets the name of the indexer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current status of the indexer.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the last run time of the indexer.
        /// </summary>
        public DateTimeOffset? LastRunTime { get; set; }

        /// <summary>
        /// Gets or sets the last run status (success/failed/inProgress).
        /// </summary>
        public string LastRunStatus { get; set; }

        /// <summary>
        /// Gets or sets the error message if the last run failed.
        /// </summary>
        public string LastRunErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the number of documents indexed in the last run.
        /// </summary>
        public int? ItemsProcessed { get; set; }

        /// <summary>
        /// Gets or sets the number of documents that failed in the last run.
        /// </summary>
        public int? ItemsFailed { get; set; }
    }
}