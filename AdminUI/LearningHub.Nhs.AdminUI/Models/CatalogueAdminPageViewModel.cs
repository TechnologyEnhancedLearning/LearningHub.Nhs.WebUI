// <copyright file="CatalogueAdminPageViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Models
{
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="CatalogueAdminPageViewModel" />.
    /// </summary>
    public class CatalogueAdminPageViewModel
    {
        /// <summary>
        /// Gets or sets the result set of admins for the add table..
        /// </summary>
        public TablePagingViewModel<CatalogueAdminViewModel> AddingCatalogueAdmins { get; set; }

        /// <summary>
        /// Gets or sets the admins for the catalogue..
        /// </summary>
        public TablePagingViewModel<CatalogueAdminViewModel> CatalogueAdmins { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueName.
        /// </summary>
        public string CatalogueName { get; set; }
    }
}
