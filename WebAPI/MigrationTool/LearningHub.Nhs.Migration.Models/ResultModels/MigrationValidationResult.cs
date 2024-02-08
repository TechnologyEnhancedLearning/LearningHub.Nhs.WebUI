namespace LearningHub.Nhs.Migration.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Data model class for representing the result of a migration validation operation on a single MigrationInputRecord.
    /// </summary>
    public class MigrationValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationValidationResult"/> class.
        /// </summary>
        public MigrationValidationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationValidationResult"/> class.
        /// </summary>
        /// <param name="success">Whether the validation passed or failed.</param>
        public MigrationValidationResult(bool success)
        {
            this.IsValid = success;
            this.InputRecordValidationResults = new List<MigrationInputRecordValidationResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationValidationResult"/> class.
        /// Use when a fundamental error prevents the validation even getting to the point where it validates individual input records.
        /// </summary>
        /// <param name="success">Whether the validation passed or failed.</param>
        /// <param name="fundamentalError">The error message.</param>
        public MigrationValidationResult(bool success, string fundamentalError)
        {
            this.IsValid = success;
            this.InputRecordValidationResults = new List<MigrationInputRecordValidationResult>();
            this.FundamentalError = fundamentalError;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the whole migration is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the FundamentalError.
        /// </summary>
        public string FundamentalError { get; set; }

        /// <summary>
        /// Gets or sets the InputRecordValidationResults.
        /// </summary>
        public List<MigrationInputRecordValidationResult> InputRecordValidationResults { get; set; }

        /// <summary>
        /// Gets a value indicating whether there were any valid input records in the migration.
        /// </summary>
        public bool AreAnyRecordsValid => this.InputRecordValidationResults.Any(x => x.IsValid);

        /// <summary>
        /// Adds a MigrationInputRecordValidationResult to the list.
        /// </summary>
        /// <param name="inputRecordValidationResult">The inputRecordValidationResult.</param>
        public void Add(MigrationInputRecordValidationResult inputRecordValidationResult)
        {
            this.IsValid = this.IsValid && inputRecordValidationResult.IsValid;
            this.InputRecordValidationResults.Add(inputRecordValidationResult);
        }
    }
}
