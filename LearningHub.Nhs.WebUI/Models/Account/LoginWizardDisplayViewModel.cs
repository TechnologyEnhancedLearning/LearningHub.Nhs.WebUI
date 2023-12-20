// <copyright file="LoginWizardDisplayViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using LearningHub.Nhs.WebUI.Models.UserProfile;

    /// <summary>
    /// Defines the <see cref="LoginWizardDisplayViewModel" />.
    /// </summary>
    public class LoginWizardDisplayViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWizardDisplayViewModel"/> class.
        /// </summary>
        /// <param name="loginWizard">Login wizard.</param>
        public LoginWizardDisplayViewModel(LoginWizardViewModel loginWizard)
        {
            this.LoginWizard = loginWizard;
        }

        /// <summary>
        /// Gets or sets the ChangePasswordViewModel.
        /// </summary>
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }

        /// <summary>
        /// Gets or sets the LoginWizard.
        /// </summary>
        public LoginWizardViewModel LoginWizard { get; set; }

        /// <summary>
        /// Gets or sets the PersonalDetailsViewModel.
        /// </summary>
        public PersonalDetailsViewModel PersonalDetailsViewModel { get; set; }

        /// <summary>
        /// Gets or sets the SecurityQuestionsModel.
        /// </summary>
        public SecurityViewModel SecurityQuestionsModel { get; set; }

        /// <summary>
        /// Gets or sets the TermsAndConditions.
        /// </summary>
        public TermsAndConditions TermsAndConditions { get; set; }

        /// <summary>
        /// Gets or sets the UserEmployment.
        /// </summary>
        public UserEmployment UserEmployment { get; set; }

        /// <summary>
        /// Gets or sets the URL to return to after the login wizard has been completed.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
