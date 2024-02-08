namespace LearningHub.Nhs.WebUI.Models.Learning
{
    using System.Collections.Generic;

    /// <summary>
    /// ActivityViewProgress.
    /// </summary>
    public class ActivityViewProgress
    {
        /// <summary>
        /// Gets or sets ResourceReferenceId.
        /// </summary>
        public int ResourceReferenceId { get; set; }

        /// <summary>
        /// Gets or sets Segments.
        /// </summary>
        public IEnumerable<ResourcePlayedSegment> Segments { get; set; }

        /// <summary>
        /// Gets or sets MediaLength.
        /// </summary>
        public string MediaLength { get; set; }
    }
}