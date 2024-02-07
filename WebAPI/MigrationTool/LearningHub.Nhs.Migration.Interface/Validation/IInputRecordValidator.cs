// <copyright file="IInputRecordValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Interface.Validation
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Models;

    /// <summary>
    /// Defines the methods exposed by an input record validator.
    /// </summary>
    public interface IInputRecordValidator
    {
        /// <summary>
        /// Validates the input record.
        /// </summary>
        /// <param name="modelData">The input record represented as a string, in whichever format suits the validator for the migration source.</param>
        /// <param name="azureMigrationContainerName">The name of the blob container in Azure that contains the staged resource files.</param>
        /// <returns>The <see cref="MigrationInputRecordValidationResult"/>.</returns>
        Task<MigrationInputRecordValidationResult> ValidateAsync(string modelData, string azureMigrationContainerName);
    }
}
