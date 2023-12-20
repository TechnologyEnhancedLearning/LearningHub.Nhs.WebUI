// <copyright file="FindwiseCollectionIdSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The FindwiseCollectionIdSettings.
    /// </summary>
    public class FindwiseCollectionIdSettings
    {
        /// <summary>
        /// Gets or sets the resource collection id.
        /// </summary>
        public string Resource { get; set; } = null!;

        /// <summary>
        /// Gets or sets the catalogue collection id.
        /// </summary>
        public string Catalogue { get; set; } = null!;
    }
}
