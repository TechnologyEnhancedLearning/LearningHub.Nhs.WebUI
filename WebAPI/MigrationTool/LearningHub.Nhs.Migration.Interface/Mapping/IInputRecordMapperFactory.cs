// <copyright file="IInputRecordMapperFactory.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Interface.Mapping
{
    /// <summary>
    /// Defines the methods exposed by the input record mapper factory.
    /// </summary>
    public interface IInputRecordMapperFactory
    {
        /// <summary>
        /// Returns the correct resource input record mapper implementation based on the migration source id.
        /// </summary>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <returns>The correct input record mapper implementation for the source.</returns>
        IInputRecordMapper GetMapper(int migrationSourceId);
    }
}
