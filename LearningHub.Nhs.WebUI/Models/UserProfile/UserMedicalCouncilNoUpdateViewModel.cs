// <copyright file="UserMedicalCouncilNoUpdateViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    /// <summary>
    /// Defines the <see cref="UserMedicalCouncilNoUpdateViewModel" />.
    /// </summary>
    public class UserMedicalCouncilNoUpdateViewModel
    {
        /// <summary>
        /// Gets or sets the medical council code.
        /// </summary>
        public string MedicalCouncilCode { get; set; }

        /// <summary>
        /// Gets or sets the medical council.
        /// </summary>
        public string MedicalCouncil { get; set; }

        /// <summary>
        /// Gets or sets the medical council number.
        /// </summary>
        public string MedicalCouncilNo { get; set; }

        /// <summary>
        /// Gets or sets the selected job role id.
        /// </summary>
        public int SelectedJobRoleId { get; set; }

        /// <summary>
        /// Gets or sets the selected job role.
        /// </summary>
        public string SelectedJobRole { get; set; }

        /// <summary>
        /// Gets or sets the medical council id.
        /// </summary>
        public int SelectedMedicalCouncilId { get; set; }

        /// <summary>
        /// Gets or sets the medical council code.
        /// </summary>
        public string SelectedMedicalCouncilCode { get; set; }

        /// <summary>
        /// Gets or sets the selected medical council number.
        /// </summary>
        public string SelectedMedicalCouncilNo { get; set; }
    }
}