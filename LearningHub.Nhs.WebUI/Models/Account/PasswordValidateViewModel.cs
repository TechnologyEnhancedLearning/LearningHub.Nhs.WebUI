// <copyright file="PasswordValidateViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    /// <summary>
    /// Defines the <see cref="PasswordValidateViewModel" />.
    /// </summary>
    public class PasswordValidateViewModel : ChangePasswordViewModel
    {
        /// <summary>
        /// Gets or sets the Loctoken.
        /// </summary>
        public string Loctoken { get; set; }

        /// <summary>
        /// Gets or sets the Token.
        /// </summary>
        public string Token { get; set; }
    }
}
