namespace LearningHub.Nhs.WebUI.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration options for validating BFF paths.
    /// </summary>
    public class BFFPathValidationOptions
    {
        /// <summary>
        /// Gets the section name for BFF path validation options.
        /// </summary>
        public const string SectionName = "BFFPathValidation";

        /// <summary>
        /// Gets or sets fine tuning. Allows for fine tuning of what paths the BFF can be used to access and what not to.
        /// </summary>
        public List<string> AllowedPathPrefixes { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets fine tuning of what paths the BFF can be used to access and what not to.
        /// </summary>
        public List<string> BlockedPathSegments { get; set; } = new List<string>();
    }
}
