// <copyright file="CatalogueRequestAccessViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Catalogue
{
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.Models.Catalogue;

    /// <summary>
    /// Defines the <see cref="CatalogueRequestAccessViewModel" />.
    /// </summary>
    public class CatalogueRequestAccessViewModel
    {
        /// <summary>
        /// Gets or sets the CatalogueNodeId.
        /// </summary>
        public int CatalogueNodeId { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueName.
        /// </summary>
        public string CatalogueName { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueUrl.
        /// </summary>
        public string CatalogueUrl { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueAccessRequest.
        /// </summary>
        public CatalogueAccessRequestViewModel CatalogueAccessRequest { get; set; }

        /// <summary>
        /// Gets or sets the CurrentUser.
        /// </summary>
        public UserBasicViewModel CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the AccessRequestMessage.
        /// </summary>
        [Required(ErrorMessage = "Message is required")]
        public string AccessRequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
