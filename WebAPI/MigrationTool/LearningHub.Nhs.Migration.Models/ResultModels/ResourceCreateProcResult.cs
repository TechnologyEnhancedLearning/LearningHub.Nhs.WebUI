// <copyright file="ResourceCreateProcResult.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Models.ResultModels
{
    /// <summary>
    /// Stores Ids returned by the migrations.ResourceCreate stored proc.
    /// </summary>
    public class ResourceCreateProcResult
    {
        /// <summary>
        /// Gets or sets the ResourceVersionId.
        /// </summary>
        public int ResourceVersionId { get; set; }

        /// <summary>
        /// Gets or sets the ArticleResourceVersionId.
        /// </summary>
        public int ArticleResourceVersionId { get; set; }
    }
}
