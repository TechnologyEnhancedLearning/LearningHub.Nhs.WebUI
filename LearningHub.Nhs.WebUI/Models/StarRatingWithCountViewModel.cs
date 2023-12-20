// <copyright file="StarRatingWithCountViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="StarRatingWithCountViewModel" />.
    /// </summary>
    public class StarRatingWithCountViewModel
    {
        /// <summary>
        /// Gets or sets the AverageRating.
        /// </summary>
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Gets or sets the RatingCount.
        /// </summary>
        public int RatingCount { get; set; }

        /// <summary>
        /// Gets or sets the ResourceReference.
        /// </summary>
        public int ResourceReferenceId { get; set; }
    }
}
