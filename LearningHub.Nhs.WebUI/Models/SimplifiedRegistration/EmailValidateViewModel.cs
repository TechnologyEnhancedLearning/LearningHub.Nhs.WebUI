namespace LearningHub.Nhs.WebUI.Models.SimplifiedRegistration
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.WebUI.Attributes;
    using LearningHub.Nhs.WebUI.Helpers;

    /// <summary>
    /// Defines the <see cref="EmailValidateViewModel" />.
    /// </summary>
    public class EmailValidateViewModel
    {
        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        [Required(ErrorMessage = "You need to enter your email address")]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ReturnToConfirmation Page.
        /// </summary>
        [DefaultValue(false)]
        public bool? ReturnToConfirmation { get; set; }
    }
}
