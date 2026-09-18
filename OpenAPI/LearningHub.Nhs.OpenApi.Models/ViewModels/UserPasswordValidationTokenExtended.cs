namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The user password validation token extended.
    /// </summary>
    public class UserPasswordValidationTokenExtended : UserPasswordValidationToken
    {
        /// <summary>
        /// Gets or sets the validate url.
        /// </summary>
        public string ValidateUrl { get; set; }

        /// <summary>
        /// Gets or sets the log on url.
        /// </summary>
        public string LogOnUrl { get; set; }

        /// <summary>
        /// Gets or sets the password reset url.
        /// </summary>
        public string PasswordResetUrl { get; set; }
    }
}
