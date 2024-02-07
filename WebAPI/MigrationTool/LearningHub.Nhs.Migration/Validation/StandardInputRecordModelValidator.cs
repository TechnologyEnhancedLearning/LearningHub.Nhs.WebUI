// <copyright file="StandardInputRecordModelValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation
{
    using FluentValidation;
    using LearningHub.Nhs.Migration.Models;

    /// <summary>
    ///  The input validator for the "standard" type of input model. This is the input model used
    ///  for eLR and eWIN migrations.
    /// </summary>
    public class StandardInputRecordModelValidator : InputRecordModelValidatorBase<StandardInputModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardInputRecordModelValidator"/> class.
        /// </summary>
        public StandardInputRecordModelValidator()
        {
            this.RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is mandatory.")
            .MaximumLength(TitleMaxLength)
            .WithMessage($"Title cannot exceed {TitleMaxLength} characters.");

            this.RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is mandatory.")
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {DescriptionMaxLength} characters.");

            this.RuleFor(x => x.Authors)
            .NotEmpty()
            .WithMessage("Authors cannot be null or empty.");

            this.RuleForEach(x => x.Authors)
            .NotNull()
            .WithMessage("Authors cannot be null.")
            .ChildRules(x =>
            {
                x.RuleFor(y => y.AuthorIndex)
                    .NotEmpty()
                    .WithMessage("Author Index is mandatory.")
                    .GreaterThan(-1)
                    .WithMessage($"Author Index must be zero or greater.");
                x.RuleFor(y => y.Author)
                    .MaximumLength(AuthorMaxLength)
                    .WithMessage($"Author cannot exceed {AuthorMaxLength} characters.");
                x.RuleFor(y => y.Organisation)
                    .MaximumLength(OrganisationMaxLength)
                    .WithMessage($"Author organisation cannot exceed {OrganisationMaxLength} characters.");
                x.RuleFor(y => y.Role)
                    .MaximumLength(RoleMaxLength)
                    .WithMessage($"Author role cannot exceed {RoleMaxLength} characters.");
                x.RuleFor(y => y)
                    .Must(z => !string.IsNullOrEmpty(z.Author) || !string.IsNullOrEmpty(z.Organisation))
                    .WithMessage("Author or author organisation is mandatory.");
            });

            this.RuleFor(x => x.Keywords)
            .NotEmpty()
            .WithMessage("Keywords cannot be null or empty.");

            this.RuleForEach(x => x.Keywords)
            .NotEmpty()
            .WithMessage("Keyword cannot be null or empty.")
            .MaximumLength(KeywordMaxLength)
            .WithMessage($"Each Keyword cannot exceed {KeywordMaxLength} characters. " + "Keyword: '{PropertyValue}'");

            this.RuleFor(x => x.ElfhUserId)
            .NotEmpty()
            .WithMessage("ElfhUserId is mandatory.")
            .Matches(@"^\d+$")
            .WithMessage("ElfhUserId must contain an integer.");

            this.RuleFor(x => x.CreateDate)
            .NotEmpty()
            .WithMessage("CreateDate is mandatory.");

            this.RuleFor(x => x.ResourceFiles)
            .Must((model, value) => (model.ResourceType == null) ||
                                    (model.ResourceType.ToLower() == "weblink" && (value == null || value.Length == 0)) ||
                                    (model.ResourceType.ToLower() == "video" && value != null && value.Length == 1) ||
                                    (model.ResourceType.ToLower() == "audio" && value != null && value.Length == 1) ||
                                    (model.ResourceType.ToLower() == "genericfile" && value != null && value.Length == 1) ||
                                    (model.ResourceType.ToLower() == "image" && value != null && value.Length == 1) ||
                                    (model.ResourceType.ToLower() == "scorm" && value != null && value.Length == 1) ||
                                    (model.ResourceType.ToLower() != "weblink" && // This last clause prevents the rule giving an error if the resource type is not valid.
                                        model.ResourceType.ToLower() != "video" &&
                                        model.ResourceType.ToLower() != "audio" &&
                                        model.ResourceType.ToLower() != "genericfile" &&
                                        model.ResourceType.ToLower() != "image" &&
                                        model.ResourceType.ToLower() != "scorm"))
            .WithMessage("The number of resource files doesn't meet the requirement for the resource type.");

            this.RuleForEach(x => x.ResourceFiles)
            .NotNull()
            .WithMessage("Resource Files cannot be null.")
            .SetValidator(model => new ResourceFileModelValidator(model));

            this.RuleFor(x => x.ResourceType)
            .NotEmpty()
            .WithMessage("Resource Type is mandatory.")
            .Must(x => x == null || this.AllowedResourceTypes.Contains(x.Trim().ToLower()))
            .WithMessage("Resource Type '{PropertyValue}' is not valid");

            this.RuleFor(x => x.Version)
            .NotEmpty()
            .WithMessage("Version is mandatory.")
            .Matches(@"^\d+\.\d+$")
            .WithMessage("Version is not valid. It must be a two part version number, e.g. 1.0");

            this.RuleFor(x => x.LicenceType)
            .NotEmpty().When(x => x.ResourceType != null && x.ResourceType.ToLower() != "weblink")
            .WithMessage("Licence Type is mandatory for this resource type.")
            .Must((model, value) => value == null || model.ResourceType.ToLower() == "weblink" || this.AllowedLicenceTypes.Contains(value.Trim().ToLower()))
            .WithMessage("Licence Type '{PropertyValue}' is not valid. It must be one of the 4-tier Creative Commons licence types.");

            this.RuleFor(x => x.HasCost)
            .NotNull()
            .WithMessage("Has Cost is mandatory.");

            this.RuleFor(x => x.ArticleBody)
            .Must((model, value) => model.ResourceType == null || model.ResourceType.ToLower() != "article" || !string.IsNullOrEmpty(value))
            .WithMessage("Article Body is mandatory when Resource Type is Article.");

            this.RuleFor(x => x.WebLinkUrl)
            .Must((model, value) => model.ResourceType == null || model.ResourceType.ToLower() != "weblink" || !string.IsNullOrEmpty(value))
            .WithMessage("Web Link URL is mandatory when Resource Type is Web Link.")
            .MaximumLength(WebLinkUrlMaxLength)
            .WithMessage($"Web Link URL cannot exceed {WebLinkUrlMaxLength} characters.");

            this.RuleFor(x => x.LmsLink)
            .MaximumLength(LmsLinkMaxLength)
            .WithMessage($"LMS Link cannot exceed {LmsLinkMaxLength} characters.");
        }

        /// <summary>
        /// Validator for the ResourceFiles property. This had to be split out into a child validator so that the check for a zip file on the scorm resource type could be done.
        /// Access to the resource type in the parent model was required, and was not possible when using the ChildRules(...) method.
        /// </summary>
        public class ResourceFileModelValidator : AbstractValidator<ResourceFileModel>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ResourceFileModelValidator"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public ResourceFileModelValidator(StandardInputModel parent)
            {
                this.RuleFor(y => y.ResourceIndex)
                    .NotEmpty()
                    .WithMessage("Resource Index is mandatory.")
                    .GreaterThan(-1)
                    .WithMessage($"Resource Index must be zero or greater.");
                this.RuleFor(y => y.ResourceUrl)
                    .NotEmpty()
                    .WithMessage("Resource URL is mandatory.")
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"Resource URL cannot exceed {ResourceFileMaxLength} characters.")
                    .Must(z => z == null || z.EndsWith(".zip") || parent.ResourceType == null || parent.ResourceType.ToLower() != "scorm")
                    .WithMessage("A SCORM package resource file must be a zip file. '{PropertyValue}' is not valid.");
            }
        }
    }
}
