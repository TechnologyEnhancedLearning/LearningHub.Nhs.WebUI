namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System;

    /// <summary>
    /// Defines the <see cref="MyAccountEmploymentDetailsViewModel" />.
    /// </summary>
    public class MyAccountEmploymentDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the Region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the CountryName.
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the RegionName.
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// Gets or sets the user employment id.
        /// </summary>
        public int EmploymentId { get; set; }

        /// <summary>
        /// Gets or sets the job role id.
        /// </summary>
        public int? JobRoleId { get; set; }

        /// <summary>
        /// Gets or sets the CurrentRole.
        /// </summary>
        public string JobRole { get; set; }

        /// <summary>
        /// Gets or sets the medical council id.
        /// </summary>
        public int? MedicalCouncilId { get; set; }

        /// <summary>
        /// Gets or sets the ProfessionalRegistrationNumber.
        /// </summary>
        public string MedicalCouncilNo { get; set; }

        /// <summary>
        /// Gets or sets the grade id.
        /// </summary>
        public int? GradeId { get; set; }

        /// <summary>
        /// Gets or sets the Grade.
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the specialty id.
        /// </summary>
        public int? SpecialtyId { get; set; }

        /// <summary>
        /// Gets or sets the PrimarySpecialty.
        /// </summary>
        public string PrimarySpecialty { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        public DateTimeOffset? JobStartDate { get; set; }

        /// <summary>
        /// Gets or sets the location id.
        /// </summary>
        public int LocationId { get; set; }

        /// <summary>
        /// Gets or sets the PlaceOfWork.
        /// </summary>
        public string PlaceOfWork { get; set; }
    }
}
