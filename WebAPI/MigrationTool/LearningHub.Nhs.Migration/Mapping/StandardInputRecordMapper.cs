// <copyright file="StandardInputRecordMapper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Migration.Interface.Mapping;
    using LearningHub.Nhs.Migration.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// The input record mapper for populating an instance of the ResourceParamsModel class (the model class used by the MigrationService to create the LH resources)
    /// from a StandardInputModel (the data model that the eLR and eWIN migrations adhere to - and possibly other migration sources in the future).
    /// </summary>
    public class StandardInputRecordMapper : InputRecordMapperBase, IInputRecordMapper
    {
        /// <summary>
        /// Populates a ResourceParamsModel from the migration input record.
        /// </summary>
        /// <param name="jsonObject">The input record represented as a string. This will be deserialized into an instance of the StandInputRecordModel class.</param>
        /// <returns>The <see cref="ResourceParamsModel"/>.</returns>
        public ResourceParamsModel GetResourceParamsModel(string jsonObject)
        {
            // Populate a data model instance.
            StandardInputModel inputModel = JsonConvert.DeserializeObject<StandardInputModel>(jsonObject);

            var resourceParams = new ResourceParamsModel()
            {
                ResourceTypeId = this.GetResourceTypeId(inputModel.ResourceType),
                Title = inputModel.Title,
                Description = this.SanitizeHtml(inputModel.Description),
                ResourceLicenceId = this.GetResourceLicenceId(inputModel.LicenceType),
                UserId = Convert.ToInt32(inputModel.ElfhUserId),
                CreateDate = inputModel.CreateDate.Value,
                ResourceFileUrls = inputModel.ResourceFiles != null ? inputModel.ResourceFiles.OrderBy(x => x.ResourceIndex).Select(x => x.ResourceUrl).ToList() : new List<string>(),
                Authors = inputModel.Authors.OrderBy(x => x.AuthorIndex).Select(x =>
                    new AuthorParamsModel
                    {
                        Author = x.Author,
                        Organisation = x.Organisation,
                        Role = x.Role,
                    }).ToList(),
                Keywords = inputModel.Keywords.Select(x => x.Trim()).Where(x => x.Length > 0).Distinct().ToList(),

                ArticleBody = this.SanitizeHtml(inputModel.ArticleBody),

                WebLinkDisplayText = string.Empty,
                WebLinkUrl = inputModel.WebLinkUrl,
            };

            return resourceParams;
        }
    }
}
