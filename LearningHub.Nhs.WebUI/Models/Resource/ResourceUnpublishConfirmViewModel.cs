// <copyright file="ResourceUnpublishConfirmViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Resource
{
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// Defines the <see cref="ResourceUnpublishConfirmViewModel" />.
    /// </summary>
    public class ResourceUnpublishConfirmViewModel
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
        /// Gets or sets the ResourceType.
        /// </summary>
        public ResourceTypeEnum ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueNodeVersionId.
        /// </summary>
        public int CatalogueNodeVersionId { get; set; }

        /// <summary>
        /// Gets or sets the ScormEsrLinkType.
        /// </summary>
        public EsrLinkType ScormEsrLinkType { get; set; }

        /// <summary>
        /// Gets or sets the ResourceTitle.
        /// </summary>
        public string ResourceTitle { get; set; }
    }
}
