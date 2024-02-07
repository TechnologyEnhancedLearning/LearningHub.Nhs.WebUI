// <copyright file="AzureBlobStorageSettings.cs" company="NHS England">
// Copyright (c) NHS England.
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
