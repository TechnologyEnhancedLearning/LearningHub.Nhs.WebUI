namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.WebUI.Attributes;
    using LearningHub.Nhs.WebUI.Helpers;

    /// <summary>
    /// Defines the <see cref="ForgotPasswordViewModel" />.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "You need to enter your email address")]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        public string EmailAddress { get; set; }
    }
}
