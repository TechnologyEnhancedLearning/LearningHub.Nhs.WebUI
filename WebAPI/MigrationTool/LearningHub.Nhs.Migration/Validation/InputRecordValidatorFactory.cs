namespace LearningHub.Nhs.Migration.Validation
{
    using System;
    using LearningHub.Nhs.Migration.Interface.Validation;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Returns the correct validator implementation based on the migration source id.
    /// </summary>
    public class InputRecordValidatorFactory : IInputRecordValidatorFactory
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputRecordValidatorFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The serviceProvider.</param>
        public InputRecordValidatorFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns the correct validator implementation based on the migration source id.
        /// </summary>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <returns>The IInputRecordValidator.</returns>
        public IInputRecordValidator GetValidator(int migrationSourceId)
        {
            // Using ranges of IDs here so that new migration sources can be added via the database without requiring a code change (assuming the required input format already exists).
            if (migrationSourceId >= 1 && migrationSourceId <= 99)
            {
                // IDs 1 to 99 have been reserved for migrations using the "StandardInputRecordValidator". This was the first input format, currently used for eLR (1) and eWIN (2).
                return this.serviceProvider.GetService<StandardInputRecordValidator>();
            }
            else if (migrationSourceId >= 100 && migrationSourceId <= 199)
            {
                // IDs 100 to 199 have been reserved for migrations using the spreadsheet template/staging tables. New ones can be added in the database using these IDs without requiring a code change.
                return this.serviceProvider.GetService<StagingTableInputRecordValidator>();
            }
            else
            {
                throw new ArgumentException($"Unexpected MigrationSourceId - {migrationSourceId}. This Id has not been set up in LearningHub.Nhs.Migration.Validation.InputRecordValidatorFactory.cs");
            }
        }
    }
}
