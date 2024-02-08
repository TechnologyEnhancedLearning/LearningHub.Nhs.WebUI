namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.WebUI.Attributes;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Validation;

    /// <summary>
    /// Defines the <see cref="PersonalDetails" />.
    /// </summary>
    public class PersonalDetails
    {
        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        public string PrimaryEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        [Required(ErrorMessage = "Please provide a first name")]
        [MaxLength(50, ErrorMessage ="First name length cannot exceed 50 characters")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        [Required(ErrorMessage = "Please provide a last name")]
        [MaxLength(50, ErrorMessage = "Last name length cannot exceed 50 characters")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the SecondaryEmailAddress.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [NotEqual("PrimaryEmailAddress", ErrorMessage = CommonValidationErrorMessages.SecondaryEmailShouldNotBeSame)]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        public string SecondaryEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ReturnToConfirmation Page.
        /// </summary>
        [DefaultValue(false)]
        public bool? ReturnToConfirmation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is being shown as part of the Login Wizard.
        /// </summary>
        public bool IsLoginWizard { get; set; }
    }
}
