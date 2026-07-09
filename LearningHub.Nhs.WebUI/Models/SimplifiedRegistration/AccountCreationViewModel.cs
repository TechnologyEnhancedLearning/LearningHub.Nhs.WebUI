namespace LearningHub.Nhs.WebUI.Models.SimplifiedRegistration
{
    using System.ComponentModel;

    /// <summary>
    /// The AccountCreationViewModel.
    /// </summary>
    public class AccountCreationViewModel
    {
        /// <summary>
        /// Gets or sets the AccountCreationType.
        /// </summary>
        public AccountCreationTypeEnum AccountCreationType { get; set; } = AccountCreationTypeEnum.FullAccess;

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the PrimaryEmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the SecondaryEmailAddress.
        /// </summary>
        public string SecondaryEmailAddress { get; set; }
    }
}
