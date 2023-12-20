// <copyright file="Interaction.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.LearningSessions
{
    /// <summary>
    /// Defines the <see cref="Interaction" />.
    /// </summary>
    public class Interaction
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
        /// Gets or sets the Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the Weighting.
        /// </summary>
        public string Weighting { get; set; }

        /// <summary>
        /// Gets or sets the Result.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the StudentResponse.
        /// </summary>
        public string StudentResponse { get; set; }

        /// <summary>
        /// Gets or sets the Latency.
        /// </summary>
        public string Latency { get; set; }
    }
}
