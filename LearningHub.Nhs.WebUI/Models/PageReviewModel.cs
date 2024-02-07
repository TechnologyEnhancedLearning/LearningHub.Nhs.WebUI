// <copyright file="PageReviewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System;

    /// <summary>
    /// Defines the <see cref="PageReviewModel" />.
    /// </summary>
    public class PageReviewModel
    {
        /// <summary>
        /// Gets or sets the LastReviewedDate.
        /// </summary>
        public DateTimeOffset LastReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the NextReviewDate.
        /// </summary>
        public DateTimeOffset NextReviewDate { get; set; }
    }
}
