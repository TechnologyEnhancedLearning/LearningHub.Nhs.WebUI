// <copyright file="StandardInputRecordValidator.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Migration.Interface.Validation;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Validation.Rules;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    ///  The input validator for the StandardInputModel class, as used for eLR and eWIN migrations, and possibly others in the future.
    /// </summary>
    public class StandardInputRecordValidator : IInputRecordValidator
    {
        private readonly UserIdExistsValidator userIdExistsValidator;
        private readonly AzureBlobFileValidator azureBlobFileValidator;
        private readonly FileExtensionIsAllowedValidator fileExtensionIsAllowedValidator;
        private readonly HtmlStringDeadLinkValidator htmlStringDeadLinkValidator;
        private readonly UrlExistsValidator urlExistsValidator;
        private readonly EsrLinkValidator esrLinkValidator;
        private readonly IOptions<Settings> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardInputRecordValidator"/> class.
        /// </summary>
        /// <param name="userIdExistsValidator">The userIdExistsValidator.</param>
        /// <param name="azureBlobFileValidator">The azureBlobFileValidator.</param>
        /// <param name="fileExtensionIsAllowedValidator">The fileExtensionIsAllowedValidator.</param>
        /// <param name="htmlStringDeadLinkValidator">The htmlStringDeadLinkValidator.</param>
        /// <param name="urlExistsValidator">The urlExistsValidator.</param>
        /// <param name="esrLinkValidator">The esrLinkValidator.</param>
        /// <param name="settings">The settings.</param>
        public StandardInputRecordValidator(
            UserIdExistsValidator userIdExistsValidator,
            AzureBlobFileValidator azureBlobFileValidator,
            FileExtensionIsAllowedValidator fileExtensionIsAllowedValidator,
            HtmlStringDeadLinkValidator htmlStringDeadLinkValidator,
            UrlExistsValidator urlExistsValidator,
            EsrLinkValidator esrLinkValidator,
            IOptions<Settings> settings)
        {
            this.userIdExistsValidator = userIdExistsValidator;
            this.azureBlobFileValidator = azureBlobFileValidator;
            this.fileExtensionIsAllowedValidator = fileExtensionIsAllowedValidator;
            this.htmlStringDeadLinkValidator = htmlStringDeadLinkValidator;
            this.urlExistsValidator = urlExistsValidator;
            this.esrLinkValidator = esrLinkValidator;
            this.settings = settings;
        }

        /// <summary>
        /// Validates the input record.
        /// </summary>
        /// <param name="jsonObject">The input record represented as a json string.</param>
        /// <param name="azureMigrationContainerName">The name of the blob container in Azure that contains the staged resource files.</param>
        /// <returns>The <see cref="MigrationInputRecordValidationResult"/>.</returns>
        public async Task<MigrationInputRecordValidationResult> ValidateAsync(string jsonObject, string azureMigrationContainerName)
        {
            var result = new MigrationInputRecordValidationResult();

            // If json data length exceeds the limit, immediately return an error;
            var overallLimit = this.settings.Value.MigrationTool.MetadataUploadSingleRecordCharacterLimit;
            if (jsonObject.Length > overallLimit)
            {
                return new MigrationInputRecordValidationResult(false, $"Overall input record length exceeds the {overallLimit} character maximum.");
            }

            // Populate a data model instance.
            StandardInputModel inputModel;
            try
            {
                inputModel = JsonConvert.DeserializeObject<StandardInputModel>(jsonObject);
            }
            catch (Exception ex)
            {
                return new MigrationInputRecordValidationResult(false, $"Unable to parse the JSON data. This could be caused by a property value using an incorrect data type. Error details: {ex.Message}");
            }

            try
            {
                // Validate all the fluent validation rules found on the model.
                var modelValidator = new StandardInputRecordModelValidator();
                var modelValidationResult = await modelValidator.ValidateAsync(inputModel);
                result = new MigrationInputRecordValidationResult(modelValidationResult)
                {
                    RecordTitle = inputModel.Title,
                    ScormEsrLinkUrl = inputModel.LmsLink,
                };

                // Other rules not possible via the fluent validation.
                this.userIdExistsValidator.Validate(inputModel.ElfhUserId, result, "Elfhuserid");
                await this.CheckAllResourceFilesExistInStagingContainer(azureMigrationContainerName, inputModel.ResourceFiles, result);

                var requestTimeout = this.settings.Value.MigrationTool.UrlDeadLinkTimeoutInSeconds;
                this.htmlStringDeadLinkValidator.Validate(inputModel.Description, requestTimeout, result, "Description");
                this.htmlStringDeadLinkValidator.Validate(inputModel.ArticleBody, requestTimeout, result, "Article Body");
                this.urlExistsValidator.Validate(inputModel.WebLinkUrl, requestTimeout, result, "Weblink URL");

                if (inputModel.ResourceType != null && inputModel.ResourceType.ToLower() == "scorm")
                {
                    this.esrLinkValidator.Validate(inputModel.LmsLink, requestTimeout, result, "LMS Link URL");
                }
            }
            catch (Exception ex)
            {
                result.AddError("N/A", $"An unexpected exception occurred when validating this resource. ERROR: {ex.ToString()}");
            }

            return result;
        }

        private async Task CheckAllResourceFilesExistInStagingContainer(string azureMigrationContainerName, ResourceFileModel[] resourceFiles, MigrationInputRecordValidationResult result)
        {
            // Check resource files exist in the Azure blob container for the migration.
            if (resourceFiles != null)
            {
                string migrationBlobContainerConnString = this.settings.Value.MigrationTool.AzureStorageAccountConnectionString;
                long maxFileSize = this.settings.Value.MigrationTool.ResourceFileSizeLimit;

                for (int i = 0; i < resourceFiles.Length; i++)
                {
                    if (resourceFiles[i] != null && !string.IsNullOrEmpty(resourceFiles[i].ResourceUrl))
                    {
                        await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, resourceFiles[i].ResourceUrl, maxFileSize, result, $"Resource Files[{i}]");
                        this.fileExtensionIsAllowedValidator.Validate(resourceFiles[i].ResourceUrl, result, $"Resource Files[{i}]");
                    }
                }
            }
        }
    }
}
