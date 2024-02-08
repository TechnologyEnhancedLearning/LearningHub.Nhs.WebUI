namespace LearningHub.Nhs.WebUI.Models.LearningSessions
{
    /// <summary>
    /// Defines the <see cref="Objective" />.
    /// </summary>
    public class Objective
    {
        /// <summary>
        /// Gets or sets the SequenceNumber.
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ScoreRaw.
        /// </summary>
        public string ScoreRaw { get; set; }

        /// <summary>
        /// Gets or sets the ScoreMax.
        /// </summary>
        public string ScoreMax { get; set; }

        /// <summary>
        /// Gets or sets the ScoreMin.
        /// </summary>
        public string ScoreMin { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }
    }
}
