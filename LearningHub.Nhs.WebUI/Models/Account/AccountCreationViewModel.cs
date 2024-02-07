// <copyright file="AccountCreationViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using elfhHub.Nhs.Models.Entities;
    using LearningHub.Nhs.WebUI.Attributes;
    using LearningHub.Nhs.WebUI.Helpers;

    /// <summary>
    /// The AccountCreationViewModel.
    /// </summary>
    public class AccountCreationViewModel
    {
        /// <summary>
        /// Gets or sets the AccountCreationType.
        /// </summary>
        public AccountCreationTypeEnum AccountCreationType { get; set; } = AccountCreationTypeEnum.FullAccess;

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the PrimaryEmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the SecondaryEmailAddress.
        /// </summary>
        public string SecondaryEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the CountryId.
        /// </summary>
        public string CountryId { get; set; }

        /// <summary>
        /// Gets or sets the RegionId.
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// Gets or sets the PrimaryUserEmploymentId.
        /// </summary>
        public string PrimaryUserEmploymentId { get; set; }

        /// <summary>
        /// Gets or sets the CurrentRole.
        /// </summary>
        public string CurrentRole { get; set; }

        /// <summary>
        /// Gets or sets the CurrentRole.
        /// </summary>
        public string CurrentRoleName { get; set; }

        /// <summary>
        /// Gets or sets the CurrentRole.
        /// </summary>
        public int? MedicalCouncilId { get; set; }

        /// <summary>
        /// Gets or sets the CurrentRole.
        /// </summary>
        public string MedicalCouncilName { get; set; }

        /// <summary>
        /// Gets or sets the CurrentRole.
        /// </summary>
        public string MedicalCouncilCode { get; set; }

        /// <summary>
        /// Gets or sets the RegistrationNumber.
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// Gets or sets the Grade.
        /// </summary>
        public string GradeId { get; set; }

        /// <summary>
        /// Gets or sets the Grade.
        /// </summary>
        public string PrimarySpecialtyId { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the LocationId.
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets the filterText.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the AccountCreationPagingEnum.
        /// </summary>
        public AccountCreationPagingEnum AccountCreationPagingEnum { get; set; }

        /// <summary>
        /// Gets or sets the page item index.
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ReturnToConfirmation Page.
        /// </summary>
        [DefaultValue(false)]
        public bool? ReturnToConfirmation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account screens are being shown as part of the Login Wizard.
        /// </summary>
        public bool IsLoginWizard { get; set; }

        /// <summary>
        /// Gets or sets the list of stages that the LoginWizard needs to complete.
        /// </summary>
        public List<LoginWizardStage> WizardStages { get; set; }

        /// <summary>
        /// Gets or sets the returnUrl to use after the Login Wizard is complete.
        /// </summary>
        public string WizardReturnUrl { get; set; }
    }
}
