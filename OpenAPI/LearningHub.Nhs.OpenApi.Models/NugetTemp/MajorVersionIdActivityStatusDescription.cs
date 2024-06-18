namespace LearningHub.Nhs.OpenApi.Models.NugetTemp
{
    using LearningHub.Nhs.Models.Enums;

    // Note activity status not in the ORM and name column not used directly

    /// <summary>
    /// Represents a combination of Major Version ID and Activity Status Description.
    /// </summary>
    public struct MajorVersionIdActivityStatusDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MajorVersionIdActivityStatusDescription"/> struct.
        /// </summary>
        /// <param name="majorVersionId">The major version ID.</param>
        /// <param name="activityStatusDescription">The activity status description.</param>
        public MajorVersionIdActivityStatusDescription(int majorVersionId, string activityStatusDescription)
        {
            this.MajorVersionId = majorVersionId;
            this.ActivityStatusDescription = activityStatusDescription;
        }

        /// <summary>
        /// Gets or sets the MajorVersionId.
        /// </summary>
        public int MajorVersionId { get; set; }
        /// <summary>
        /// Gets or sets the ActivityStatusDescription.
        /// </summary>
        public string ActivityStatusDescription { get; set; }
    }
}