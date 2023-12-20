// <copyright file="IMigrationService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The MigrationService interface.
    /// </summary>
    public interface IMigrationService
    {
        /// <summary>
        /// Creates a new migration from an input json string.
        /// </summary>
        /// <param name="jsonData">The json data.</param>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <param name="migrationAzureContainerName">The migration azure container name.</param>
        /// <param name="destinationNodeId">The destination node id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateFromJsonString(string jsonData, int migrationSourceId, string migrationAzureContainerName, int destinationNodeId, int migratorUserId);

        /// <summary>
        /// Creates a new migration from metadata provided in an Excel template file and processed via the staging tables ADF pipeline.
        /// </summary>
        /// <param name="excelFileContent">The Excel metadata template file content.</param>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <param name="migrationAzureContainerName">The migration azure container name.</param>
        /// <param name="migratorUserId">The user id of the user performing the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateFromStagingTables(byte[] excelFileContent, int migrationSourceId, string migrationAzureContainerName, int migratorUserId);

        /// <summary>
        /// Validates an existing migration.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <param name="migratorUserId">TThe user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MigrationValidationResult> Validate(int migrationId, int migratorUserId);

        /// <summary>
        /// Creates the resource metadata for an existing migration, and transfers the resource files from the staging area to the final content area within Azure.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> BeginCreateMetadata(int migrationId, int migratorUserId);

        /// <summary>
        /// Creates the LH draft resource for a single migration input record.
        /// </summary>
        /// <param name="migrationInputRecordId">The migrationInputRecordId.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateMetadataForSingleInputRecord(int migrationInputRecordId, int migratorUserId);

        /// <summary>
        /// Returns the results of a prior call to the BeginCreateMetadata method. This method is intended to be polled by the migration console app in order to track progress/completion.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MigrationResourceCreationResult> CheckStatusOfCreateMetadata(int migrationId);

        /// <summary>
        /// Begins the process of publishing the resources for an existing migration. This can be called after successful completion of CreateMetadata.
        /// This method updates the status of the migration and returns a list of MigrationInputRecord Ids that need to be published - i.e it doesn't actually publish anything!
        /// The PublishResourceForSingleInputRecord method should then be called for each MigrationInputRecord Id, and that publishes the resource. This used to all happen in a
        /// single web service call but has been split out to avoid timeout errors on large migrations.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MigrationBeginPublishResult> BeginPublishResources(int migrationId, int migratorUserId);

        /// <summary>
        /// Publishes the resource for a single input record. This method needs to be called for each input record after the BeginPublishResources method has been called for the migration.
        /// This used to all happen in a single web service call but has been split out to avoid timeout errors on large migrations.
        /// </summary>
        /// <param name="migrationInputRecordId">The migration input record id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> PublishResourceForSingleInputRecord(int migrationInputRecordId, int migratorUserId);

        /// <summary>
        /// Returns the results of a prior call to the PublishResources method. This method is intended to be polled by the migration console app in order to track progress/completion.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MigrationPublishResult> CheckStatusOfPublishResources(int migrationId, int migratorUserId);

        /// <summary>
        /// Gets a list of ResourceVersionIds that were published as part of a migration. Intended to be used by the migration tool to unpublish each resource.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MigrationBeginPublishResult> GetPublishedResourceVersionIds(int migrationId);

        /// <summary>
        /// The GetMigrationSourcesAsync.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{MigrationSourceViewModel}"/>.</returns>
        Task<IEnumerable<MigrationSourceViewModel>> GetMigrationSourcesAsync();
    }
}
