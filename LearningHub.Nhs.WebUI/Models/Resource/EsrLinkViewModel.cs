namespace LearningHub.Nhs.WebUI.Models.Resource
{
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// Defines the <see cref="EsrLinkViewModel" />.
    /// </summary>
    public class EsrLinkViewModel
    {
        /// <summary>
        /// Gets or sets the ScormContentDetails.
        /// </summary>
        public ScormContentDetailsViewModel ScormContentDetails { get; set; }

        /// <summary>
        /// Gets or sets the ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
