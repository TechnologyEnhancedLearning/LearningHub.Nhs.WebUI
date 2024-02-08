namespace LearningHub.Nhs.Migration.ConsoleApp
{
    /// <summary>
    /// The create params model.
    /// </summary>
    public class CreateParamsModel
    {
        /// <summary>
        /// Gets or sets the migration data source type.
        /// </summary>
        public string MigrationDataSourceType { get; set; }

        /// <summary>
        /// Gets or sets the metadata file path.
        /// </summary>
        public string MetadataFilePath { get; set; }

        /// <summary>
        /// Gets or sets the migration source id.
        /// </summary>
        public int MigrationSourceId { get; set; }

        /// <summary>
        /// Gets or sets the destination node id.
        /// </summary>
        public int DestinationNodeId { get; set; }

        /// <summary>
        /// Gets or sets the azure migration container name.
        /// </summary>
        public string AzureMigrationContainerName { get; set; }
    }
}
