// <copyright file="AccountCreationConfirmation.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    /// <summary>
    /// The AccountCreationConfirmation.
    /// </summary>
    public class AccountCreationConfirmation
    {
        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the Region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the Grade.
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the Specialty.
        /// </summary>
        public string Specialty { get; set; }

        /// <summary>
        /// Gets or sets the Employer.
        /// </summary>
        public string Employer { get; set; }

        /// <summary>
        /// Gets or sets the AccountCreationViewModel.
        /// </summary>
        public AccountCreationViewModel AccountCreationViewModel { get; set; }
    }
}
