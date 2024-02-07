// <copyright file="MigrationResourceCreationResult.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Data model class for representing the result of a migration validation operation on a single MigrationInputRecord.
    /// </summary>
    public class MigrationResourceCreationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationResourceCreationResult"/> class.
        /// </summary>
        public MigrationResourceCreationResult()
        {
            this.Errors = new Dictionary<int, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationResourceCreationResult"/> class, after a fundamental error has occurred.
        /// </summary>
        /// <param name="fundamentalError">The fundamental error details.</param>
        public MigrationResourceCreationResult(string fundamentalError)
        {
            this.FundamentalError = fundamentalError;
        }

        /// <summary>
        /// Gets or sets the number of input records not yet processed.
        /// </summary>
        public int NotYetProcessedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of input records successfully created.
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Gets or sets the number of input records that failed due to an error.
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of input records in the migration.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the Errors.
        /// </summary>
        public Dictionary<int, string> Errors { get; set; }

        /// <summary>
        /// Gets or sets the FundamentalError.
        /// </summary>
        public string FundamentalError { get; set; }

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
