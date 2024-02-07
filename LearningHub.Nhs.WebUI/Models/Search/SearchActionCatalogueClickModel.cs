// <copyright file="SearchActionCatalogueClickModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using LearningHub.Nhs.Models.Search;

    /// <summary>
    /// Defines the <see cref="SearchActionCatalogueClickModel" />.
    /// </summary>
    public class SearchActionCatalogueClickModel : SearchActionCatalogueModel
    {
        /// <summary>
        /// Gets or sets the catalogue url.
        /// </summary>
        public string CatalogueUrl { get; set; }
    }
}
