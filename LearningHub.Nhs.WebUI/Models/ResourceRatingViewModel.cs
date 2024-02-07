// <copyright file="ResourceRatingViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using LearningHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="ResourceRatingViewModel" />.
    /// </summary>
    public class ResourceRatingViewModel
    {
        /// <summary>
        /// Gets or sets the ResourceVersionId.
        /// </summary>
        public int ResourceVersionId { get; set; }

        /// <summary>
        /// Gets or sets the ResourceReferenceId.
        /// </summary>
        public int ResourceReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the return Url.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has access to the catalogue that the resource is located in.
        /// </summary>
        public bool HasCatalogueAccess { get; set; }

        /// <summary>
        /// Gets or sets the rating summary.
        /// </summary>
        public RatingSummaryViewModel RatingSummary { get; set; }
    }
}
