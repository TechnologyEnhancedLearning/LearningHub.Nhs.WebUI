namespace LearningHub.Nhs.Migration.Interface.Mapping
{
    using LearningHub.Nhs.Migration.Models;

    /// <summary>
    /// Defines the methods exposed by a migration input record mapper.
    /// </summary>
    public interface IInputRecordMapper
    {
        /// <summary>
        /// Populates a ResourceParamsModel from the migration input record data.
        /// </summary>
        /// <param name="modelData">The input record represented as a string, in whichever format suits the validator for the migration source.</param>
        /// <returns>The <see cref="ResourceParamsModel"/>.</returns>
        ResourceParamsModel GetResourceParamsModel(string modelData);
    }
}
