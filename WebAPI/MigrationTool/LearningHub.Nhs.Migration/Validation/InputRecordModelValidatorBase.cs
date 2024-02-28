namespace LearningHub.Nhs.Migration.Validation
{
    using System.Collections.Generic;
    using FluentValidation;

    /// <summary>
    ///  The base class for input model validators. Defines properties common to all model validators.
    /// </summary>
    /// <typeparam name="T">The input model class type.</typeparam>
    public class InputRecordModelValidatorBase<T> : AbstractValidator<T>
    {
        /// <summary>
        /// The ResourceUniqueRef max length.
        /// </summary>
        public const int ResourceUniqueRefMaxLength = 255;

        /// <summary>
        /// The title max length.
        /// </summary>
        public const int TitleMaxLength = 255;

        /// <summary>
        /// The description max length.
        /// </summary>
        public const int DescriptionMaxLength = 1024;

        /// <summary>
        /// The author max length.
        /// </summary>
        public const int AuthorMaxLength = 100;

        /// <summary>
        /// The organisation max length.
        /// </summary>
        public const int OrganisationMaxLength = 100;

        /// <summary>
        /// The role max length.
        /// </summary>
        public const int RoleMaxLength = 100;

        /// <summary>
        /// The keyword max length.
        /// </summary>
        public const int KeywordMaxLength = 50;

        /// <summary>
        /// The resource file max length.
        /// </summary>
        public const int ResourceFileMaxLength = 1024;

        /// <summary>
        /// The web link url max length.
        /// </summary>
        public const int WebLinkUrlMaxLength = 1024;

        /// <summary>
        /// The web link display text max length.
        /// </summary>
        public const int WebLinkDisplayTextMaxLength = 50;

        /// <summary>
        /// The lms link max length.
        /// </summary>
        public const int LmsLinkMaxLength = 255;

        /// <summary>
        /// Gets a list of allowed resource types.
        /// </summary>
        public List<string> AllowedResourceTypes { get; } = new List<string>() { "article", "scorm", "video", "audio", "weblink", "genericfile", "image" };

        /// <summary>
        /// Gets a list of allowed licence types.
        /// </summary>
        public List<string> AllowedLicenceTypes { get; } = new List<string>()
        {
            "creative commons (e&w) attribution, non-commercial",
            "creative commons attribution-non commercial 4.0 international",

            "creative commons (e&w) attribution, non-commercial, sharealike",
            "creative commons (e&w) attribution, non-commercial, share-a-like",
            "creative commons attribution-non commercial-share-alike 4.0 international",

            "creative commons (e&w) attribution, non-commercial, no derivatives",
            "creative commons attribution-non commercial no derivatives- 4.0 international",

            "all rights reserved",
        };
    }
}
