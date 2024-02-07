// <copyright file="LoginWizardAccountConfirmation.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System.Collections.Generic;
    using elfhHub.Nhs.Models.Entities;

    /// <summary>
    /// Defines the data required for the confirmation screen displayed at the end of the login wizard if any of the
    /// PersonalData, JobRole and PlaceOfWork stages were displayed during the wizard. Same as for the Create Account
    /// process plus it needs to know which wizard stages were shown.
    /// </summary>
    public class LoginWizardAccountConfirmation : AccountCreationConfirmation
    {
        /// <summary>
        /// Gets or sets the login wizard stages that are required.
        /// </summary>
        public List<LoginWizardStage> WizardStages { get; set; }
    }
}
