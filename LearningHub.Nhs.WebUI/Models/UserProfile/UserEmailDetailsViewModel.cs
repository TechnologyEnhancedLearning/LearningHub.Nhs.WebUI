// <copyright file="UserEmailDetailsViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.WebUI.Attributes;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Validation;

    /// <summary>
    /// Defines the <see cref="UserEmailDetailsViewModel" />.
    /// </summary>
    public class UserEmailDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the Primary Email Address.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Enter a primary email address")]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        [DisplayName("Primary email address")]
        public string PrimaryEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the Secondary Email Address.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [NotEqual("PrimaryEmailAddress", ErrorMessage = CommonValidationErrorMessages.SecondaryEmailShouldNotBeSame)]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        [DisplayName("Secondary email address")]
        public string SecondaryEmailAddress { get; set; }
    }
}
