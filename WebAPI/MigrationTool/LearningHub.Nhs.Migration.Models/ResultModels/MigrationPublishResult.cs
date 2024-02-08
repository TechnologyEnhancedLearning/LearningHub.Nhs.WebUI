namespace LearningHub.Nhs.Migration.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Data model class for representing the result of a migration publish operation.
    /// </summary>
    public class MigrationPublishResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationPublishResult"/> class.
        /// </summary>
        public MigrationPublishResult()
        {
            this.Errors = new Dictionary<int, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationPublishResult"/> class, after a fundamental error has occurred.
        /// </summary>
        /// <param name="fundamentalError">The fundamental error details.</param>
        public MigrationPublishResult(string fundamentalError)
        {
            this.FundamentalError = fundamentalError;
        }

        /// <summary>
        /// Gets the total number of input records processed in the publish operation.
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.PublishedCount + this.QueuedForPublishCount + this.PublishFailedCount;
            }
        }

        /// <summary>
        /// Gets or sets the Errors.
        /// </summary>
        public Dictionary<int, string> Errors { get; set; }

        /// <summary>
        /// Gets or sets the FundamentalError.
        /// </summary>
        public string FundamentalError { get; set; }

        /// <summary>
        /// Gets or sets the count of migration input records that have been queued for publish.
        /// </summary>
        public int QueuedForPublishCount { get; set; }

        /// <summary>
        /// Gets or sets the count of migration input records that have been published successfully.
        /// </summary>
        public int PublishedCount { get; set; }

        /// <summary>
        /// Gets or sets the count of migration input records that have failed publishing.
        /// </summary>
        public int PublishFailedCount { get; set; }

        /// <summary>
        /// Adds an error to the result.
        /// </summary>
        /// <param name="migrationInputRecordId">The migrationInputRecordId.</param>
        /// <param name="error">The result.</param>
        public void AddError(int migrationInputRecordId, string error)
        {
            this.Errors.Add(migrationInputRecordId, error);
        }
    }
}
