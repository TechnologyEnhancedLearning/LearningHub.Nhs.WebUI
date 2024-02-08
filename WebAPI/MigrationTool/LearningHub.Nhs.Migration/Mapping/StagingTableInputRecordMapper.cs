namespace LearningHub.Nhs.Migration.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Migration.Interface.Mapping;
    using LearningHub.Nhs.Migration.Mapping.Helpers;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using Newtonsoft.Json;

    /// <summary>
    /// The input record mapper for mapping an instance of StagingTableInputModel (the input data model that the staging table migration source uses)
    /// to an instance of the ResourceParamsModel class (the model class used by the MigrationService to create the LH resources).
    /// </summary>
    public class StagingTableInputRecordMapper : InputRecordMapperBase, IInputRecordMapper
    {
        private const int CommunityContributionsCatalogueNodeId = 1;

        private readonly UserLookup userLookup;
        private readonly NodeIdLookup nodeIdLookup;
        private readonly FileTypeLookup fileTypeLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="StagingTableInputRecordMapper"/> class.
        /// </summary>
        /// <param name="userIdLookup">The userIdLookup.</param>
        /// <param name="nodeIdLookup">The nodeIdLookup.</param>
        /// <param name="fileTypeLookup">The fileTypeLookup.</param>
        public StagingTableInputRecordMapper(UserLookup userIdLookup, NodeIdLookup nodeIdLookup, FileTypeLookup fileTypeLookup)
        {
            this.userLookup = userIdLookup;
            this.nodeIdLookup = nodeIdLookup;
            this.fileTypeLookup = fileTypeLookup;
        }

        /// <summary>
        /// Populates a ResourceParamsModel from the migration input record.
        /// </summary>
        /// <param name="jsonObject">The input record represented as a string. This will be deserialized into an instance of the StandInputRecordModel class.</param>
        /// <returns>The <see cref="ResourceParamsModel"/>.</returns>
        public ResourceParamsModel GetResourceParamsModel(string jsonObject)
        {
            // Populate a data model instance.
            StagingTableInputModel inputModel = JsonConvert.DeserializeObject<StagingTableInputModel>(jsonObject);

            UserProfile authorUser = this.userLookup.GetByUserName(inputModel.ContributorLearningHubUserName);

            var resourceParams = new ResourceParamsModel()
            {
                ResourceTypeId = this.GetResourceTypeId(inputModel.ResourceType, inputModel.ServerFileName),
                Title = inputModel.Title,
                Description = this.SanitizeHtml(inputModel.Description),
                ResourceLicenceId = this.GetResourceLicenceId(inputModel.Licence),
                SensitiveContentFlag = this.ConvertStringToBool(inputModel.SensitiveContentFlag),
                UserId = authorUser.Id,
                CreateDate = inputModel.PublishedDate.Value,
                DestinationNodeId = (!string.IsNullOrEmpty(inputModel.CatalogueName)) ? this.nodeIdLookup.GetNodeIdByCatalogueName(inputModel.CatalogueName) : CommunityContributionsCatalogueNodeId,
                ResourceFileUrls = this.BuildResourceFileList(inputModel),
                Authors = this.BuildAuthorList(inputModel, authorUser),
                Keywords = this.BuildKeywordList(inputModel),
                AdditionalInformation = inputModel.AdditionalInformation,
                YearAuthored = inputModel.YearAuthored.GetValueOrDefault(),
                MonthAuthored = inputModel.MonthAuthored.GetValueOrDefault(),
                DayAuthored = inputModel.DayAuthored.GetValueOrDefault(),
                ArticleBody = this.SanitizeHtml(inputModel.ArticleBodyText),
                WebLinkDisplayText = inputModel.WeblinkText ?? string.Empty,
                WebLinkUrl = inputModel.WeblinkUrl,
                EsrLinkTypeId = this.GetEsrLinkTypeId(inputModel.LMSLinkVisibility),
            };

            return resourceParams;
        }

        private List<string> BuildResourceFileList(StagingTableInputModel inputModel)
        {
            var resourceFiles = new List<string>();

            if (inputModel.ServerFileName != null)
            {
                resourceFiles.Add(inputModel.ServerFileName);
            }

            if (inputModel.ArticleFile1 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile1);
            }

            if (inputModel.ArticleFile2 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile2);
            }

            if (inputModel.ArticleFile3 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile3);
            }

            if (inputModel.ArticleFile4 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile4);
            }

            if (inputModel.ArticleFile5 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile5);
            }

            if (inputModel.ArticleFile6 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile6);
            }

            if (inputModel.ArticleFile7 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile7);
            }

            if (inputModel.ArticleFile8 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile8);
            }

            if (inputModel.ArticleFile9 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile9);
            }

            if (inputModel.ArticleFile10 != null)
            {
                resourceFiles.Add(inputModel.ArticleFile10);
            }

            return resourceFiles;
        }

        private List<AuthorParamsModel> BuildAuthorList(StagingTableInputModel inputModel, UserProfile authorUser)
        {
            var authors = new List<AuthorParamsModel>();

            // If IAmTheAuthor is true, need to use their first and last name from the ELFH database rather than use the supplied values.
            if (this.ConvertStringToBool(inputModel.IAmTheAuthorFlag))
            {
                authors.Add(new AuthorParamsModel { IAmTheAuthor = true, Author = $"{authorUser.FirstName} {authorUser.LastName}", Organisation = inputModel.AuthorOrganisation1, Role = inputModel.AuthorRole1 });
            }
            else
            {
                authors.Add(new AuthorParamsModel { Author = inputModel.AuthorName1, Organisation = inputModel.AuthorOrganisation1, Role = inputModel.AuthorRole1 });
            }

            if (!string.IsNullOrEmpty(inputModel.AuthorName2) || !string.IsNullOrEmpty(inputModel.AuthorOrganisation2))
            {
                authors.Add(new AuthorParamsModel { Author = inputModel.AuthorName2, Organisation = inputModel.AuthorOrganisation2, Role = inputModel.AuthorRole2 });
            }

            if (!string.IsNullOrEmpty(inputModel.AuthorName3) || !string.IsNullOrEmpty(inputModel.AuthorOrganisation3))
            {
                authors.Add(new AuthorParamsModel { Author = inputModel.AuthorName3, Organisation = inputModel.AuthorOrganisation3, Role = inputModel.AuthorRole3 });
            }

            return authors;
        }

        private List<string> BuildKeywordList(StagingTableInputModel inputModel)
        {
            var keywords = inputModel.Keywords.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).Distinct().ToList();
            return keywords;
        }

        /// <summary>
        /// Returns the resource type ID for a particular resource type name. Hides the one in the base class as potential options are different.
        /// </summary>
        /// <param name="resourceType">The resource type name. e.g. "article". Case-insensitive.</param>
        /// <param name="resourceFileName">The resource file name. e.g. "x-ray.jpeg".</param>
        /// <returns>The resource type ID.</returns>
        private int GetResourceTypeId(string resourceType, string resourceFileName)
        {
            switch (resourceType.Trim().ToLower())
            {
                case "article":
                    return (int)ResourceTypeEnum.Article;
                case "scorm":
                    return (int)ResourceTypeEnum.Scorm;
                case "weblink":
                    return (int)ResourceTypeEnum.WebLink;
                case "file":
                    return this.fileTypeLookup.GetFileTypeByFileName(resourceFileName).DefaultResourceTypeId;
                default:
                    throw new NotSupportedException($"Resource Type '{resourceType}' is not supported by the StagingTableInputRecordMapper.");
            }
        }

        /// <summary>
        /// Returns the ESR link type ID for a particular LMS link visibilty type.
        /// </summary>
        /// <param name="lmsLinkVisibility">The LMS link visibility type. e.g. "display only to me". Case-insensitive.</param>
        /// <returns>The ESR link type ID.</returns>
        private int GetEsrLinkTypeId(string lmsLinkVisibility)
        {
            // Return zero for non-scorm resources.
            if (string.IsNullOrEmpty(lmsLinkVisibility))
            {
                return 0;
            }

            switch (lmsLinkVisibility.Trim().ToLower())
            {
                case "don't display":
                    return (int)EsrLinkTypeEnum.NotAvailable;
                case "display only to me":
                    return (int)EsrLinkTypeEnum.CreatedUserAndOtherEditors;
                case "display to everyone":
                    return (int)EsrLinkTypeEnum.Everyone;
                default:
                    throw new NotSupportedException($"LMS Link Visibility '{lmsLinkVisibility}' is not supported by the StagingTableInputRecordMapper.");
            }
        }
    }
}
