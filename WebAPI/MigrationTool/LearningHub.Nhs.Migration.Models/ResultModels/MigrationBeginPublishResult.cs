// <copyright file="MigrationBeginPublishResult.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Data model class for representing the result of a migration begin publish operation.
    /// This contains either a list of MigrationInputRecord Ids to publish, or error details.
    /// </summary>
    public class MigrationBeginPublishResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationBeginPublishResult"/> class. Parameterless version Needed for the JSON serialiser.
        /// </summary>
        public MigrationBeginPublishResult()
        {
            this.MigrationInputRecordIdsToPublish = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationBeginPublishResult"/> class, after a fundamental error has occurred.
        /// </summary>
        /// <param name="migrationInputRecordIdsToPublish">The list of MigrationInputRecord Ids to publish.</param>
        public MigrationBeginPublishResult(List<int> migrationInputRecordIdsToPublish)
        {
            this.MigrationInputRecordIdsToPublish = migrationInputRecordIdsToPublish;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationBeginPublishResult"/> class, after a fundamental error has occurred.
        /// </summary>
        /// <param name="fundamentalError">The fundamental error details.</param>
        public MigrationBeginPublishResult(string fundamentalError)
        {
            this.FundamentalError = fundamentalError;
        }

        /// <summary>
        /// Gets or sets the MigrationInputRecordIdsToPublish.
        /// </summary>
        public List<int> MigrationInputRecordIdsToPublish { get; set; }

        /// <summary>
        /// Gets or sets the FundamentalError.
        /// </summary>
        public string FundamentalError { get; set; }
    }
}
