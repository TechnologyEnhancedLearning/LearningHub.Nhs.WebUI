namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.WebUI.Attributes;
    using LearningHub.Nhs.WebUI.Helpers;

    /// <summary>
    /// Defines the <see cref="MyAccountPersonalDetailsViewModel" />.
    /// </summary>
    public class MyAccountPersonalDetailsViewModel
    {
        /// <summary>
        /// Gets  or sets the UserName.
        /// </summary>
        [DisplayName("Username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets  or sets the Name.
        /// </summary>
        [DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        [Required(ErrorMessage = "Enter a first name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be less than 50 characters.")]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        [Required(ErrorMessage = "Enter a last name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be less than 50 characters.")]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the PreferredName.
        /// </summary>
        [StringLength(50, MinimumLength = 0, ErrorMessage = "Preferred name must be less than 50 characters.")]
        [DisplayName("Preferred name")]
        public string PreferredName { get; set; }

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
        /// Gets or sets the new primary email address.
        /// </summary>
        public string NewPrimaryEmailAddress { get; set; }
    }
}
