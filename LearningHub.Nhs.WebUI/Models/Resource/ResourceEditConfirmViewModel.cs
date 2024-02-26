namespace LearningHub.Nhs.WebUI.Models.Resource
{
    /// <summary>
    /// Defines the <see cref="ResourceEditConfirmViewModel" />.
    /// </summary>
    public class ResourceEditConfirmViewModel
    {
        /// <summary>
        /// Gets or sets the ResourceId.
        /// </summary>
        public int ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the ResourceReferenceId.
        /// </summary>
        public int ResourceReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the ResourceTitle.
        /// </summary>
        public string ResourceTitle { get; set; }
    }
}
