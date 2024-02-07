// <copyright file="AzureBlobSettings.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Configuration
{
    /// <summary>
    /// Config AzureBlobSettings.
    /// </summary>
    public class AzureBlobSettings
    {
        /// <summary>
        /// Gets or sets connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets upload container name.
        /// </summary>
        public string UploadContainer { get; set; }
    }
}
