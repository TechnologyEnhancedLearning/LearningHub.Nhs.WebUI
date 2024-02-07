// <copyright file="TermsAndConditionsViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Policies
{
    using System;

    /// <summary>
    /// Defines the <see cref="TermsAndConditionsViewModel" />.
    /// </summary>
    public class TermsAndConditionsViewModel
    {
        /// <summary>
        /// Gets or sets the CreatedDate.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the TermsAndConditions.
        /// </summary>
        public string TermsAndConditions { get; set; }
    }
}
