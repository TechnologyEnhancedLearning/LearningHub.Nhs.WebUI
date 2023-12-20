// <copyright file="StagingTableSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Shared.Configuration
{
    /// <summary>
    /// The StagingTable migration settings for the Migration Tool.
    /// </summary>
    public class StagingTableSettings
    {
        /// <summary>
        /// Gets or sets the staging table ADF resource group.
        /// </summary>
        public string AdfResourceGroup { get; set; }

        /// <summary>
        /// Gets or sets the staging table ADF factory name.
        /// </summary>
        public string AdfFactoryName { get; set; }

        /// <summary>
        /// Gets or sets the staging table ADF pipeline name.
        /// </summary>
        public string AdfPipelineName { get; set; }

        /// <summary>
        /// Gets or sets the name of the blob container that the ADF pipeline reads from.
        /// </summary>
        public string AdfPipelineAzureBlobContainerName { get; set; }
    }
}
