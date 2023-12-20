// <copyright file="AzureBlobSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Shared.Configuration
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
