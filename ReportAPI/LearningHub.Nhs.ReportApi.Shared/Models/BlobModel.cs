// <copyright file="BlobModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Shared.Models
{
    using System.IO;

    /// <summary>
    /// The BlobModel.
    /// </summary>
    public class BlobModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public Stream Content { get; set; }
    }
}
