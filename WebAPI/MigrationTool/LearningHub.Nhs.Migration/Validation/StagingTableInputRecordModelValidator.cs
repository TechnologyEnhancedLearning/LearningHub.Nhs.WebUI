// <copyright file="StagingTableInputRecordModelValidator.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using LearningHub.Nhs.Migration.Models;

    /// <summary>
    ///  The input validator for the "statging table" type of input model. This is the input model used
    ///  for South Central Content Server migrations (at time of writing).
    /// </summary>
    public class StagingTableInputRecordModelValidator : InputRecordModelValidatorBase<StagingTableInputModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StagingTableInputRecordModelValidator"/> class.
        /// </summary>
        public StagingTableInputRecordModelValidator()
        {
            this.RuleFor(x => x.ResourceUniqueRef)
                .MaximumLength(ResourceUniqueRefMaxLength)
                .WithMessage($"ResourceUniqueRef cannot exceed {ResourceUniqueRefMaxLength} characters.");

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

            // Author 1.
            this.RuleFor(x => x.AuthorName1)
                    .MaximumLength(AuthorMaxLength)
                    .WithMessage($"AuthorName1 cannot exceed {AuthorMaxLength} characters.");
            this.RuleFor(x => x.AuthorOrganisation1)
                .MaximumLength(OrganisationMaxLength)
                .WithMessage($"Organisation1 cannot exceed {OrganisationMaxLength} characters.");
            this.RuleFor(x => x.AuthorRole1)
                .MaximumLength(RoleMaxLength)
                .WithMessage($"Role1 cannot exceed {RoleMaxLength} characters.");
            this.RuleFor(x => x.AuthorName1)
                .Must((model, value) => !string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(model.AuthorOrganisation1))
                    .WithMessage("AuthorName1 or Organisation1 is mandatory.");

            // Author 2.
            this.RuleFor(x => x.AuthorName2)
                    .MaximumLength(AuthorMaxLength)
                    .WithMessage($"AuthorName2 cannot exceed {AuthorMaxLength} characters.");
            this.RuleFor(x => x.AuthorOrganisation2)
                .MaximumLength(OrganisationMaxLength)
                .WithMessage($"Organisation2 cannot exceed {OrganisationMaxLength} characters.");
            this.RuleFor(x => x.AuthorRole2)
                .MaximumLength(RoleMaxLength)
                .WithMessage($"Role2 cannot exceed {RoleMaxLength} characters.");
            this.RuleFor(x => x)
                    .Must(y => string.IsNullOrEmpty(y.AuthorRole2) ||
                                (!string.IsNullOrEmpty(y.AuthorRole2) && (!string.IsNullOrEmpty(y.AuthorName2) || !string.IsNullOrEmpty(y.AuthorOrganisation2))))
                    .WithMessage("Role2 cannot be specified without either AuthorName2 or Organisation2 as well.");

            // Author 3.
            this.RuleFor(x => x.AuthorName3)
                    .MaximumLength(AuthorMaxLength)
                    .WithMessage($"AuthorName3 cannot exceed {AuthorMaxLength} characters.");
            this.RuleFor(x => x.AuthorOrganisation3)
                .MaximumLength(OrganisationMaxLength)
                .WithMessage($"Organisation3 cannot exceed {OrganisationMaxLength} characters.");
            this.RuleFor(x => x.AuthorRole3)
                .MaximumLength(RoleMaxLength)
                .WithMessage($"Role3 cannot exceed {RoleMaxLength} characters.");
            this.RuleFor(x => x)
                    .Must(y => string.IsNullOrEmpty(y.AuthorRole3) ||
                                (!string.IsNullOrEmpty(y.AuthorRole3) && (!string.IsNullOrEmpty(y.AuthorName3) || !string.IsNullOrEmpty(y.AuthorOrganisation3))))
                    .WithMessage("Role3 cannot be specified without either AuthorName3 or Organisation3 as well.");

            this.RuleFor(x => x.Keywords)
                .NotEmpty()
                .WithMessage("Keywords cannot be null or empty.")
                .Must(y => string.IsNullOrEmpty(y) || y.Split(',').All(z => z.Length <= KeywordMaxLength))
                .WithMessage($"Each Keyword cannot exceed {KeywordMaxLength} characters. " + "Keywords: '{PropertyValue}'");

            this.RuleFor(x => x.ContributorLearningHubUserName)
                .NotEmpty()
                .WithMessage("Contributor ElfhUserName is mandatory.");

            this.RuleFor(x => x.PublishedDate)
                .NotEmpty()
                .WithMessage("PublishedDate is mandatory.");

            this.RuleFor(x => x.ServerFileName)
            .Must((model, value) => (model.ResourceType == null) ||
                                    (model.ResourceType.ToLower() == "weblink") ||
                                    (model.ResourceType.ToLower() == "article") ||
                                    (model.ResourceType.ToLower() == "file" && !string.IsNullOrEmpty(value)) ||
                                    (model.ResourceType.ToLower() == "scorm" && !string.IsNullOrEmpty(value)) ||

                                    // This last clause prevents the rule giving an error if the resource type is not valid.
                                    (model.ResourceType.ToLower() != "article" && model.ResourceType.ToLower() != "weblink" && model.ResourceType.ToLower() != "file" && model.ResourceType.ToLower() != "scorm"))
            .WithMessage("Server file name is mandatory for this resource type.");

            this.RuleFor(x => x.ServerFileName)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"Server file name cannot exceed {ResourceFileMaxLength} characters.")
                    .Must((model, value) => model.ResourceType == null || model.ResourceType.ToLower() != "scorm" || value == null || (value != null && value.EndsWith(".zip")))
                    .WithMessage("Server file name for a SCORM resource must be a zip file.");

            this.RuleFor(x => x.ArticleContentFilename)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"Article Content file name cannot exceed {ResourceFileMaxLength} characters.")
                    .Must((model, value) => model.ResourceType == null || model.ResourceType.ToLower() != "article" || value == null || (value != null && value.EndsWith(".html")))
                    .WithMessage("Article Content file name must be a html file.")
                    .Must((model, value) => model.ResourceType == null || model.ResourceType.ToLower() != "article" || !string.IsNullOrEmpty(value))
                    .WithMessage("Article Content file name is mandatory for Article resources.");

            this.RuleFor(x => x.ArticleFile1)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile1 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile2)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile2 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile3)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile3 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile4)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile4 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile5)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile5 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile6)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile6 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile7)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile7 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile8)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile8 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile9)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile9 cannot exceed {ResourceFileMaxLength} characters.");
            this.RuleFor(x => x.ArticleFile10)
                    .MaximumLength(ResourceFileMaxLength)
                    .WithMessage($"ArticleFile10 cannot exceed {ResourceFileMaxLength} characters.");

            this.RuleFor(x => x.ResourceType)
                .NotEmpty()
                .WithMessage("Resource Type is mandatory.")
                .Must(x => x == null || this.AllowedResourceTypes.Contains(x.Trim().ToLower()))
                .WithMessage("Resource Type '{PropertyValue}' is not valid");

            this.RuleFor(x => x.Licence)
                .NotEmpty().When(x => x.ResourceType != null && x.ResourceType.ToLower() != "weblink")
                .WithMessage("Licence is mandatory for this resource type.")
                .Must((model, value) => value == null || model.ResourceType.ToLower() == "weblink" || this.AllowedLicenceTypes.Contains(value.Trim().ToLower()))
                .WithMessage("Licence '{PropertyValue}' is not valid. It must be one of the 4-tier Creative Commons licence types.");

            this.RuleFor(x => x.WeblinkUrl)
                .Must((model, value) => model.ResourceType == null || model.ResourceType.ToLower() != "weblink" || !string.IsNullOrEmpty(value))
                .WithMessage("Web Link URL is mandatory when Resource Type is Web Link.")
                .MaximumLength(WebLinkUrlMaxLength)
                .WithMessage($"Web Link URL cannot exceed {WebLinkUrlMaxLength} characters.");

            this.RuleFor(x => x.WeblinkText)
                .MaximumLength(WebLinkDisplayTextMaxLength)
                .WithMessage($"Web Link Display Text cannot exceed {WebLinkDisplayTextMaxLength} characters.");

            this.RuleFor(x => x.LMSLink)
                .MaximumLength(LmsLinkMaxLength)
                .WithMessage($"LMS Link cannot exceed {LmsLinkMaxLength} characters.");

            this.RuleFor(x => x.LMSLinkVisibility)
                .Must((model, value) => value == null || model.ResourceType.ToLower() != "scorm" || this.AllowedEsrLinkVisibilityTypes.Contains(value.Trim().ToLower()))
                .WithMessage("LMS Link Visibility '{PropertyValue}' is not valid. It must be one of the three LMS link visibility types.");

            this.RuleFor(x => x.MonthAuthored)
                .InclusiveBetween(1, 12)
                .WithMessage("MonthAuthored is not a valid month.")
                .Must((model, value) => !value.HasValue || (model.YearAuthored.HasValue && model.YearAuthored.Value > 0))
                .WithMessage("If MonthAuthored is populated then YearAuthored must be populated as well.");

            this.RuleFor(x => x.DayAuthored)
                .Must((model, value) => !value.HasValue || (model.MonthAuthored.HasValue && model.MonthAuthored.Value > 0))
                .WithMessage("If DayAuthored is populated then MonthAuthored must be populated as well.")
                .Must((model, value) => !model.DayAuthored.HasValue || !model.MonthAuthored.HasValue || !model.YearAuthored.HasValue ||
                                        DateTime.TryParse($"{model.DayAuthored}/{model.MonthAuthored}/{model.YearAuthored}", out var result))
                .WithMessage("The YearAuthored, MonthAuthored and DayAuthored fields do not represent a valid date.");
        }

        /// <summary>
        /// Gets a list of allowed resource types. Hides the default list in the base class because for the Staging Tables import, the input data contains the "File" resource type,
        /// which covers all of video, audio, image and genericfile together. Later on when the resources are created, the tool decides which resource type to create based on
        /// file extension, same as the website does.
        /// </summary>
        public new List<string> AllowedResourceTypes { get; } = new List<string>() { "article", "scorm", "file", "weblink" };

        /// <summary>
        /// Gets a list of allowed ESR link visibility types.
        /// </summary>
        private List<string> AllowedEsrLinkVisibilityTypes { get; } = new List<string>()
        {
            "don't display",
            "display only to me",
            "display to everyone",
        };
    }
}
