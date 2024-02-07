// <copyright file="CatalogueViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// CatalogueViewModel.
    /// </summary>
    public class CatalogueViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueViewModel"/> class.
        /// </summary>
        /// <param name="id"><see cref="Id"/>.</param>
        /// <param name="name"><see cref="Name"/>.</param>
        /// <param name="isRestricted"><see cref="IsRestricted"/>.</param>
        public CatalogueViewModel(int id, string? name, bool isRestricted)
        {
            this.Id = id;
            this.Name = name ?? string.Empty;
            this.IsRestricted = isRestricted;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueViewModel"/> class.
        /// </summary>
        /// <param name="catalogueNodeVersion"><see cref="CatalogueNodeVersion"/>.</param>
        public CatalogueViewModel(CatalogueNodeVersion catalogueNodeVersion)
        {
            this.Id = catalogueNodeVersion.NodeVersion.NodeId;
            this.Name = catalogueNodeVersion.Name;
            this.IsRestricted = catalogueNodeVersion.RestrictedAccess;
        }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the catalogue IsRestricted.
        /// </summary>
        public bool IsRestricted { get; set; }
    }
}