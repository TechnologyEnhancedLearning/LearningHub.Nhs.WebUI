// <copyright file="AzureBlobStorageSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Shared.Configuration
{
    /// <summary>
    /// Config AzureBlobStorageSettings.
    /// </summary>
    public class AzureBlobStorageSettings
    {
        /// <summary>
        /// Gets or sets connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets container name.
        /// </summary>
        public string Container { get; set; }
    }
}
