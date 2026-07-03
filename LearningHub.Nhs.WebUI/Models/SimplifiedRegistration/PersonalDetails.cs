namespace LearningHub.Nhs.WebUI.Models.SimplifiedRegistration
{
    using System.ComponentModel.DataAnnotations;

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
    }
}
