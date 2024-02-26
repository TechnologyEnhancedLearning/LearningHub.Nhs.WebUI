namespace LearningHub.Nhs.WebUI.Models
{
    using System;

    /// <summary>
    /// Defines the <see cref="UserViewModel" />.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether Active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the ActiveFromDate.
        /// </summary>
        public DateTimeOffset? ActiveFromDate { get; set; }

        /// <summary>
        /// Gets or sets the ActiveToDate.
        /// </summary>
        public DateTimeOffset? ActiveToDate { get; set; }

        /// <summary>
        /// Gets or sets the AltEmailAddress.
        /// </summary>
        public string AltEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the CountryId.
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the LoginTimes.
        /// </summary>
        public int LoginTimes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether LoginWizardInProgress.
        /// </summary>
        public bool LoginWizardInProgress { get; set; }

        /// <summary>
        /// Gets or sets the PreferredName.
        /// </summary>
        public string PreferredName { get; set; }

        /// <summary>
        /// Gets or sets the PreferredTenantId.
        /// </summary>
        public int PreferredTenantId { get; set; }

        /// <summary>
        /// Gets or sets the PrimaryUserEmploymentId.
        /// </summary>
        public int? PrimaryUserEmploymentId { get; set; }

        /// <summary>
        /// Gets or sets the RegionId.
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// Gets or sets the RegistrationCode.
        /// </summary>
        public string RegistrationCode { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}
