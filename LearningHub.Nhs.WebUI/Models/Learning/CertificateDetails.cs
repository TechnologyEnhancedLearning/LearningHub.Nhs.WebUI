// <copyright file="CertificateDetails.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Learning
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;

    /// <summary>
    /// CertificateDetails.
    /// </summary>
    public class CertificateDetails
    {
        /// <summary>
        /// Gets or sets UserViewModel.
        /// </summary>
        public UserViewModel UserViewModel { get; set; }

        /// <summary>
        /// Gets or sets ActivityDetailedItemViewModel.
        /// </summary>
        public ActivityDetailedItemViewModel ActivityDetailedItemViewModel { get; set; }

        /// <summary>
        /// Gets or sets ResourceItemViewModel.
        /// </summary>
        public ResourceItemViewModel ResourceItemViewModel { get; set; }

        /// <summary>
        /// Gets or sets NodeViewModels.
        /// </summary>
        public List<NodeViewModel> NodeViewModels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets DownloadCertificate.
        /// </summary>
        [DefaultValue(false)]
        public bool DownloadCertificate { get; set; }

        /// <summary>
        /// Gets or sets CertificateBase64Image.
        /// </summary>
        public string CertificateBase64Image { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets AccessCount.
        /// </summary>
        public int AccessCount { get; set; }

        /// <summary>
        /// Gets or sets ProfessionalRegistrationNumber.
        /// </summary>
        public string ProfessionalRegistrationNumber { get; set; }

        /// <summary>
        /// Gets or sets PageNo.
        /// </summary>
        public int PageNo { get; set; }
    }
}
