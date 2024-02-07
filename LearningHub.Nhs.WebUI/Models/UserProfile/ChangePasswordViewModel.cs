// <copyright file="ChangePasswordViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
  using System.ComponentModel.DataAnnotations;
  using LearningHub.Nhs.WebUI.Helpers;
  using LearningHub.Nhs.WebUI.Validation;

  /// <summary>
  /// Defines the <see cref="ChangePasswordViewModel" />.
  /// </summary>
  public class ChangePasswordViewModel
    {
        /// <summary>
        /// Gets or sets the NewPassword.
        /// </summary>
        [Required(ErrorMessage = CommonValidationErrorMessages.PasswordRequired)]
        [StringLength(
          CommonValidationValues.PasswordMaxLength,
          MinimumLength = CommonValidationValues.PasswordMinLength,
          ErrorMessage = CommonValidationErrorMessages.PasswordLengthErrorMessage)]
        [RegularExpression(
          CommonValidationValues.PasswordRegularExpression,
          ErrorMessage = CommonValidationErrorMessages.PasswordNotValid)]
        [NotEqual(
          "Username",
          ErrorMessage = CommonValidationErrorMessages.PasswordMatchesUsername)]
        [CommonPasswords(CommonValidationErrorMessages.PasswordTooCommon)]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the PasswordConfirmation.
        /// </summary>
        [Compare(
          "NewPassword",
          ErrorMessage = CommonValidationErrorMessages.PasswordConfirmationMismatch)]
        public string PasswordConfirmation { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }
    }
}
