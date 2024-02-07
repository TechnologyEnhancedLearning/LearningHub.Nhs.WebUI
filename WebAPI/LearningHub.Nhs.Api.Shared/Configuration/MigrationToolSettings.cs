// <copyright file="MigrationToolSettings.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Shared.Configuration
{
    /// <summary>
    /// The FindwiseSettings.
    /// </summary>
    public class MigrationToolSettings
    {
        /// <summary>
        /// Gets or sets the migration azure storage account connection string.
        /// Migration Tool Settings.
        /// </summary>
        public string AzureStorageAccountConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the migration azure blob container name for metadata files.
        /// </summary>
        public string AzureBlobContainerNameForMetadataFiles { get; set; }

        /// <summary>
        /// Gets or sets the migration metadata upload single record character limit.
        /// </summary>
        public int MetadataUploadSingleRecordCharacterLimit { get; set; }

        /// <summary>
        /// Gets or sets the migration metadata creation azure queue name.
        /// </summary>
        public string MetadataCreationQueueName { get; set; }

        /// <summary>
        /// Gets or sets the migration resource file size limit (per file). This is limited by what the LH publish process is capable of.
        /// </summary>
        public long ResourceFileSizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the request timeout that the migration tool uses when checking if URLs are valid.
        /// </summary>
        public int UrlDeadLinkTimeoutInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the staging table settings.
        /// </summary>
        public StagingTableSettings StagingTables { get; set; }
    }
}
