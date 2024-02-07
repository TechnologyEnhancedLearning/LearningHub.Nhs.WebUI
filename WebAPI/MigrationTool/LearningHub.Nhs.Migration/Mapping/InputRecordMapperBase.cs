// <copyright file="InputRecordMapperBase.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Mapping
{
    using System;
    using Ganss.XSS;
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// The base class for all input record mapping classes. Contains simple common properties and methods.
    /// The subclasses are used to map a migration-specific input data model class to a common class that the
    /// MigrationService uses for the rest of the migration process (ResourceParamsModel).
    /// </summary>
    public abstract class InputRecordMapperBase
    {
        private IHtmlSanitizer sanitizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputRecordMapperBase"/> class.
        /// </summary>
        public InputRecordMapperBase()
        {
            this.sanitizer = new HtmlSanitizer();
        }

        /// <summary>
        /// Sanitizes a HTML string by removing unsafe scripts, etc. Uses the HtmlSanitizer third party library to do so.
        /// </summary>
        /// <param name="html">The HTML string to sanitize.</param>
        /// <returns>The sanitized HTML string.</returns>
        protected string SanitizeHtml(string html)
        {
            return this.sanitizer.Sanitize(html);
        }

        /// <summary>
        /// Returns the resource type ID for a particular resource type name.
        /// </summary>
        /// <param name="resourceType">The resource type name. e.g. "article". Case-insensitive.</param>
        /// <returns>The resource type ID.</returns>
        protected int GetResourceTypeId(string resourceType)
        {
            switch (resourceType.Trim().ToLower())
            {
                case "article":
                    return (int)ResourceTypeEnum.Article;
                case "audio":
                    return (int)ResourceTypeEnum.Audio;
                case "embedded":
                    return (int)ResourceTypeEnum.Embedded;
                case "equipment":
                    return (int)ResourceTypeEnum.Equipment;
                case "image":
                    return (int)ResourceTypeEnum.Image;
                case "scorm":
                    return (int)ResourceTypeEnum.Scorm;
                case "video":
                    return (int)ResourceTypeEnum.Video;
                case "weblink":
                    return (int)ResourceTypeEnum.WebLink;
                case "genericfile":
                    return (int)ResourceTypeEnum.GenericFile;
                default:
                    throw new NotSupportedException($"Resource Type '{resourceType}' is not supported by the StandardInputRecordMapper.");
            }
        }

        /// <summary>
        /// Returns the licence type ID for a particular licence type string. It handles slight variations in input text between different migration types.
        /// </summary>
        /// <param name="licenceType">The licence type string. e.g. "creative commons (ew) attribution, non-commercial". Case-insensitive.</param>
        /// <returns>The licence type ID.</returns>
        protected int GetResourceLicenceId(string licenceType)
        {
            if (licenceType == null)
            {
                return 0;
            }

            switch (licenceType.Trim().ToLower())
            {
                case "creative commons (e&w) attribution, non-commercial":
                case "creative commons attribution-non commercial 4.0 international":
                    return 1;
                case "creative commons (e&w) attribution, non-commercial, sharealike":
                case "creative commons (e&w) attribution, non-commercial, share-a-like":
                case "creative commons attribution-non commercial-share-alike 4.0 international":
                    return 2;
                case "creative commons (e&w) attribution, non-commercial, no derivatives":
                case "creative commons attribution-non commercial no derivatives- 4.0 international":
                    return 3;
                case "all rights reserved":
                    return 4;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Convert a string to bool. Possible values - Yes/yes/True/true/1. Anything else is false.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>The resulting boolean value.</returns>
        protected bool ConvertStringToBool(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.Trim().ToLower();
                return input == "yes" || input == "1" || input == "true";
            }

            return false;
        }
    }
}
