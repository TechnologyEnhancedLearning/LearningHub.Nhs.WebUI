// <copyright file="MigrationInputRecordValidationResult.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation.Results;

    /// <summary>
    /// Data model class for representing the result of a migration validation operation.
    /// </summary>
    public class MigrationInputRecordValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationInputRecordValidationResult"/> class.
        /// </summary>
        public MigrationInputRecordValidationResult()
        {
            this.Errors = new List<string>();
            this.Warnings = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationInputRecordValidationResult"/> class.
        /// </summary>
        /// <param name="success">Whether the validation passed or failed.</param>
        /// <param name="errorMessage">The error message.</param>
        public MigrationInputRecordValidationResult(bool success, string errorMessage)
        {
            this.IsValid = success;
            this.Errors = new List<string> { errorMessage };
            this.Warnings = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationInputRecordValidationResult"/> class.
        /// </summary>
        /// <param name="result">The ValidationResult.</param>
        public MigrationInputRecordValidationResult(ValidationResult result)
        {
            this.IsValid = result.IsValid;
            this.Errors = new List<string>();
            this.Warnings = new List<string>();
            this.Errors.AddRange(result.Errors.Select(x => $"Property Name: {x.PropertyName}. Error: {x.ErrorMessage}"));
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input record is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the RecordReference.
        /// </summary>
        public string RecordReference { get; set; }

        /// <summary>
        /// Gets or sets the RecordTitle.
        /// </summary>
        public string RecordTitle { get; set; }

        /// <summary>
        /// Gets or sets the ScormEsrLinkUrl.
        /// </summary>
        public string ScormEsrLinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the index (order) or the record in the original input file.
        /// </summary>
        public int RecordIndex { get; set; }

        /// <summary>
        /// Gets or sets the Errors.
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets the Warnings.
        /// </summary>
        public List<string> Warnings { get; set; }

        /// <summary>
        /// Adds a validation error.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="errorMessage">The validation error message.</param>
        public void AddError(string propertyName, string errorMessage)
        {
            this.IsValid = false;
            this.Errors.Add($"Property Name: {propertyName}. Error: {errorMessage}");
        }

        /// <summary>
        /// Adds a validation warning.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="warningMessage">The validation error message.</param>
        public void AddWarning(string propertyName, string warningMessage)
        {
            this.Warnings.Add($"Property Name: {propertyName}. Warning: {warningMessage}");
        }
    }
}
