// <copyright file="BreadcrumbViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="BreadcrumbViewModel" />.
    /// </summary>
    public class BreadcrumbViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the "Back to parent" view, or the ellipsis view is displayed on mobile.
        /// </summary>
        public bool ShowBackToParentOnMobile { get; set; }

        /// <summary>
        /// Gets or sets the Breadcrumbs.
        /// </summary>
        public List<(string Title, string Url)> Breadcrumbs { get; set; }
    }
}
