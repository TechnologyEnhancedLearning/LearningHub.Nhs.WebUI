namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="UserPersonalDetailsViewModel" />.
    /// </summary>
    public class UserPersonalDetailsViewModel
    {
        /// <summary>
        /// Gets  or sets the UserName.
        /// </summary>
        [DisplayName("Username")]
        public string UserName { get; set; }

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
    }
}
