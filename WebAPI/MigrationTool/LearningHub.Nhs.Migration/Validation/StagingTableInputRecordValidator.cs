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
    ///  The input validator for the StagingTableInputModel class, as used for staging table migrations.
    /// </summary>
    public class StagingTableInputRecordValidator : IInputRecordValidator
    {
        private readonly CatalogueNameExistsValidator catalogueNameExistsValidator;
        private readonly UserNameExistsValidator userNameExistsValidator;
        private readonly AzureBlobFileValidator azureBlobFileValidator;
        private readonly FileExtensionIsAllowedValidator fileExtensionIsAllowedValidator;
        private readonly HtmlStringDeadLinkValidator htmlStringDeadLinkValidator;
        private readonly UrlExistsValidator urlExistsValidator;
        private readonly EsrLinkValidator esrLinkValidator;
        private readonly IOptions<Settings> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StagingTableInputRecordValidator"/> class.
        /// </summary>
        /// <param name="catalogueNameExistsValidator">The catalogueNameExistsValidator.</param>
        /// <param name="userNameExistsValidator">The userNameExistsValidator.</param>
        /// <param name="azureBlobFileValidator">The azureBlobFileValidator.</param>
        /// <param name="fileExtensionIsAllowedValidator">The fileExtensionIsAllowedValidator.</param>
        /// <param name="htmlStringDeadLinkValidator">The htmlStringDeadLinkValidator.</param>
        /// <param name="urlExistsValidator">The urlExistsValidator.</param>
        /// <param name="esrLinkValidator">The esrLinkValidator.</param>
        /// <param name="settings">The settings.</param>
        public StagingTableInputRecordValidator(
            CatalogueNameExistsValidator catalogueNameExistsValidator,
            UserNameExistsValidator userNameExistsValidator,
            AzureBlobFileValidator azureBlobFileValidator,
            FileExtensionIsAllowedValidator fileExtensionIsAllowedValidator,
            HtmlStringDeadLinkValidator htmlStringDeadLinkValidator,
            UrlExistsValidator urlExistsValidator,
            EsrLinkValidator esrLinkValidator,
            IOptions<Settings> settings)
        {
            this.catalogueNameExistsValidator = catalogueNameExistsValidator;
            this.userNameExistsValidator = userNameExistsValidator;
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
            StagingTableInputModel inputModel;
            try
            {
                inputModel = JsonConvert.DeserializeObject<StagingTableInputModel>(jsonObject);
            }
            catch (Exception ex)
            {
                return new MigrationInputRecordValidationResult(false, $"Unable to parse the JSON data. This could be caused by a property value using an incorrect data type. Error details: {ex.Message}");
            }

            try
            {
                // Validate all the fluent validation rules found on the model.
                var modelValidator = new StagingTableInputRecordModelValidator();
                var modelValidationResult = await modelValidator.ValidateAsync(inputModel);
                result = new MigrationInputRecordValidationResult(modelValidationResult)
                {
                    RecordTitle = inputModel.Title,
                    RecordReference = inputModel.ResourceUniqueRef,
                    ScormEsrLinkUrl = inputModel.LMSLink,
                };

                // Other rules not possible via the fluent validation.
                this.userNameExistsValidator.Validate(inputModel.ContributorLearningHubUserName, result, "Contributor e-LfH User Name");
                this.catalogueNameExistsValidator.Validate(inputModel.CatalogueName, result, "Catalogue");
                await this.CheckAllResourceFilesExistInStagingContainer(inputModel, azureMigrationContainerName, result);
                this.CheckAllResourceFileExtensionsAreAllowed(inputModel, result);

                var requestTimeout = this.settings.Value.MigrationTool.UrlDeadLinkTimeoutInSeconds;
                this.htmlStringDeadLinkValidator.Validate(inputModel.Description, requestTimeout, result, "Description");
                this.htmlStringDeadLinkValidator.Validate(inputModel.ArticleBodyText, requestTimeout, result, "Article Body");
                this.urlExistsValidator.Validate(inputModel.WeblinkUrl, requestTimeout, result, "Weblink URL");

                if (inputModel.ResourceType != null && inputModel.ResourceType.ToLower() == "scorm")
                {
                    this.esrLinkValidator.Validate(inputModel.LMSLink, requestTimeout, result, "LMS Link URL");
                }
            }
            catch (Exception ex)
            {
                result.AddError("N/A", $"An unexpected exception occurred when validating this resource. ERROR: {ex.ToString()}");
            }

            return result;
        }

        private async Task CheckAllResourceFilesExistInStagingContainer(StagingTableInputModel inputModel, string azureMigrationContainerName, MigrationInputRecordValidationResult result)
        {
            string migrationBlobContainerConnString = this.settings.Value.MigrationTool.AzureStorageAccountConnectionString;
            long maxFileSize = this.settings.Value.MigrationTool.ResourceFileSizeLimit;

            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ServerFileName, maxFileSize, result, $"Server file name");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleContentFilename, maxFileSize, result, "Article Content file");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile1, maxFileSize, result, "Article file 1");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile2, maxFileSize, result, "Article file 2");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile3, maxFileSize, result, "Article file 3");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile4, maxFileSize, result, "Article file 4");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile5, maxFileSize, result, "Article file 5");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile6, maxFileSize, result, "Article file 6");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile7, maxFileSize, result, "Article file 7");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile8, maxFileSize, result, "Article file 8");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile9, maxFileSize, result, "Article file 9");
            await this.azureBlobFileValidator.Validate(migrationBlobContainerConnString, azureMigrationContainerName, inputModel.ArticleFile10, maxFileSize, result, $"Article file 10");
        }

        private void CheckAllResourceFileExtensionsAreAllowed(StagingTableInputModel inputModel, MigrationInputRecordValidationResult result)
        {
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ServerFileName, result, $"Server file name");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleContentFilename, result, "Article body content file");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile1, result, "Article file 1");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile2, result, "Article file 2");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile3, result, "Article file 3");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile4, result, "Article file 4");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile5, result, "Article file 5");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile6, result, "Article file 6");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile7, result, "Article file 7");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile8, result, "Article file 8");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile9, result, "Article file 9");
            this.fileExtensionIsAllowedValidator.Validate(inputModel.ArticleFile10, result, $"Article file 10");
        }
    }
}
