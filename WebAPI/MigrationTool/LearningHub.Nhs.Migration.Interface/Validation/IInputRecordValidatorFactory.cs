// <copyright file="IInputRecordValidatorFactory.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Interface.Validation
{
    /// <summary>
    /// Defines the methods exposed by the validator factory.
    /// </summary>
    public interface IInputRecordValidatorFactory
    {
        /// <summary>
        /// Returns the correct validator implementation based on the migration source id.
        /// </summary>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <returns>The correct validator implementation for the source.</returns>
        IInputRecordValidator GetValidator(int migrationSourceId);
    }
}
