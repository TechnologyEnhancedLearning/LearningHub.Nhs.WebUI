// <copyright file="StagingTableInputRecordValidatorTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.UnitTests.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Migration.Interface.Validation;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Validation;
    using LearningHub.Nhs.Migration.Validation.Helpers;
    using LearningHub.Nhs.Migration.Validation.Rules;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Options;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// The stagingTableInputRecordValidator tests.
    /// </summary>
    public class StagingTableInputRecordValidatorTests
    {
        /// <summary>
        /// The mock ICatalogueNodeVersionRepository.
        /// </summary>
        private readonly Mock<ICatalogueNodeVersionRepository> mockCatalogueNodeVersionRepository;

        /// <summary>
        /// The mock migration repository.
        /// </summary>
        private readonly Mock<IUserRepository> mockUserRepository;

        /// <summary>
        /// The mock UrlRewriting repository.
        /// </summary>
        private readonly Mock<IUrlRewritingRepository> mockUrlRewritingRepository;

        /// <summary>
        /// The mock azure blob service.
        /// </summary>
        private readonly Mock<IAzureBlobService> mockAzureBlobService;

        /// <summary>
        /// The mock file type service.
        /// </summary>
        private readonly Mock<IFileTypeService> mockFileTypeService;

        /// <summary>
        /// The mock settings.
        /// </summary>
        private readonly Mock<IOptions<Settings>> mockSettings;

        /// <summary>
        /// The standard input record validator.
        /// </summary>
        private readonly IInputRecordValidator stagingTableInputRecordValidator;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StagingTableInputRecordValidatorTests"/> class.
        /// </summary>
        public StagingTableInputRecordValidatorTests()
        {
            this.mockCatalogueNodeVersionRepository = new Mock<ICatalogueNodeVersionRepository>(MockBehavior.Strict);
            this.mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            this.mockUrlRewritingRepository = new Mock<IUrlRewritingRepository>(MockBehavior.Strict);
            this.mockAzureBlobService = new Mock<IAzureBlobService>(MockBehavior.Strict);
            this.mockFileTypeService = new Mock<IFileTypeService>(MockBehavior.Strict);
            this.mockSettings = new Mock<IOptions<Settings>>(MockBehavior.Loose);

            // This needs to be set up before the constructor is called to avoid test errors.
            this.mockFileTypeService.Setup(x => x.GetAllDisallowedFileExtensions())
                .Returns(new List<string> { "asp" });

            this.stagingTableInputRecordValidator = new StagingTableInputRecordValidator(
                new CatalogueNameExistsValidator(this.mockCatalogueNodeVersionRepository.Object),
                new UserNameExistsValidator(this.mockUserRepository.Object),
                new AzureBlobFileValidator(this.mockAzureBlobService.Object),
                new FileExtensionIsAllowedValidator(this.mockFileTypeService.Object),
                new HtmlStringDeadLinkValidator(new UrlChecker()),
                new UrlExistsValidator(new UrlChecker()),
                new EsrLinkValidator(new UrlChecker(), this.mockUrlRewritingRepository.Object),
                this.mockSettings.Object);

            // Set up expected invocations for checks made during happy path. Avoids duplication across test cases.
            this.settings = new Settings
            {
                MigrationTool = new MigrationToolSettings()
                {
                    MetadataUploadSingleRecordCharacterLimit = 100000,
                    ResourceFileSizeLimit = 10000,

                    AzureStorageAccountConnectionString = "migration-connection-string",
                    AzureBlobContainerNameForMetadataFiles = "metadata-blob-container-name",

                    UrlDeadLinkTimeoutInSeconds = 5,
                },
            };

            this.mockSettings.Setup(x => x.Value).Returns(this.settings);
            this.mockAzureBlobService.Setup(x => x.GetBlobMetadata(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container", It.IsAny<string>())).ReturnsAsync(new BlobMetadataViewModel() { SizeInBytes = 1000 });
            this.mockUserRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.FromResult(new User()));
            this.mockCatalogueNodeVersionRepository.Setup(x => x.ExistsAsync("my catalogue")).Returns(Task.FromResult(true));
            this.mockUrlRewritingRepository.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
        }

        /// <summary>
        /// Validate returns error if record length is greater than maximum allowed.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfRecordLengthIsGreaterThanMaximumAllowed()
        {
            // Arrange
            string jsonData = this.GetString(this.settings.MigrationTool.MetadataUploadSingleRecordCharacterLimit + 1);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Equal("Overall input record length exceeds the 100000 character maximum.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if json deserialization fails.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfJsonDeserialisationFails()
        {
            // Arrange
            string jsonData = "this is not valid json!";

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Unable to parse the JSON data. This could be caused by a property value using an incorrect data type. Error details: ", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if user name does not exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfUserNameDoesNotExist()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockUserRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), false))
                                    .Returns(Task.FromResult<User>(null));

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: Contributor e-LfH User Name. Error: Username 'username1' not found in Learning Hub database.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if resource file does not exist in azure container.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfResourceFileDoesNotExistInAzureContainer()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ServerFileName = "resource.pdf";
            inputRecord.ArticleContentFilename = "articleBody.html";
            inputRecord.ArticleFile1 = "articlefile1.pdf";
            inputRecord.ArticleFile2 = "articlefile2.pdf";
            inputRecord.ArticleFile3 = "articlefile3.pdf";
            inputRecord.ArticleFile4 = "articlefile4.pdf";
            inputRecord.ArticleFile5 = "articlefile5.pdf";
            inputRecord.ArticleFile6 = "articlefile6.pdf";
            inputRecord.ArticleFile7 = "articlefile7.pdf";
            inputRecord.ArticleFile8 = "articlefile8.pdf";
            inputRecord.ArticleFile9 = "articlefile9.pdf";
            inputRecord.ArticleFile10 = "articlefile10.pdf";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockAzureBlobService.Setup(x => x.GetBlobMetadata(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container", It.IsAny<string>())).ReturnsAsync((BlobMetadataViewModel)null);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Equal(12, result.Errors.Count());
            Assert.Contains("Property Name: Server file name. Error: Resource File 'resource.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article Content file. Error: Resource File 'articleBody.html' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 1. Error: Resource File 'articlefile1.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 2. Error: Resource File 'articlefile2.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 3. Error: Resource File 'articlefile3.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 4. Error: Resource File 'articlefile4.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 5. Error: Resource File 'articlefile5.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 6. Error: Resource File 'articlefile6.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 7. Error: Resource File 'articlefile7.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 8. Error: Resource File 'articlefile8.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 9. Error: Resource File 'articlefile9.pdf' does not exist in the Azure migration blob container.", result.Errors);
            Assert.Contains("Property Name: Article file 10. Error: Resource File 'articlefile10.pdf' does not exist in the Azure migration blob container.", result.Errors);
        }

        /// <summary>
        /// Validate returns error if resource file is over the maximum size limit.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfResourceFileSizeTooLarge()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ServerFileName = "resource.pdf";
            inputRecord.ArticleFile1 = "articlefile1.pdf";
            inputRecord.ArticleFile2 = "articlefile2.pdf";
            inputRecord.ArticleFile3 = "articlefile3.pdf";
            inputRecord.ArticleFile4 = "articlefile4.pdf";
            inputRecord.ArticleFile5 = "articlefile5.pdf";
            inputRecord.ArticleFile6 = "articlefile6.pdf";
            inputRecord.ArticleFile7 = "articlefile7.pdf";
            inputRecord.ArticleFile8 = "articlefile8.pdf";
            inputRecord.ArticleFile9 = "articlefile9.pdf";
            inputRecord.ArticleFile10 = "articlefile10.pdf";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockAzureBlobService.Setup(x => x.GetBlobMetadata(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container", It.IsAny<string>()))
                                        .ReturnsAsync(new BlobMetadataViewModel { SizeInBytes = 10001 });

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Equal(12, result.Errors.Count());
            Assert.Contains("Property Name: Server file name. Error: Resource File 'resource.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article Content file. Error: Resource File 'articleBody.html' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 1. Error: Resource File 'articlefile1.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 2. Error: Resource File 'articlefile2.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 3. Error: Resource File 'articlefile3.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 4. Error: Resource File 'articlefile4.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 5. Error: Resource File 'articlefile5.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 6. Error: Resource File 'articlefile6.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 7. Error: Resource File 'articlefile7.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 8. Error: Resource File 'articlefile8.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 9. Error: Resource File 'articlefile9.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
            Assert.Contains("Property Name: Article file 10. Error: Resource File 'articlefile10.pdf' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
        }

        /// <summary>
        /// Validate returns warning if body contains dead link.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsWarningIfBodyContainsDeadLink()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ArticleBodyText = "<p><a href=\"http://www.nonexistentsite.com/nonexistentpage\">My link</a></p>";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Single(result.Warnings);
            Assert.StartsWith("Property Name: Article Body. Warning: Article Body contains an invalid link - \"http://www.nonexistentsite.com/nonexistentpage\"", result.Warnings[0]);
        }

        /// <summary>
        /// Validate returns error if weblink URL contains dead link.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfWeblinkUrlContainsDeadLink()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.WeblinkUrl = "http://www.nonexistentsite.com/nonexistentpage";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: Weblink URL. Error: Weblink URL contains an invalid link - \"http://www.nonexistentsite.com/nonexistentpage\"", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Resource File Has Disallowed Extension.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfResourceFileHasDisallowedExtension()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ServerFileName = "disallowed.asp";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: Server file name. Error: The file extension of resource file 'disallowed.asp' is not allowed by the Learning Hub.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Scorm Resource Doesnt Have Zip File.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfScormResourceDoesntHaveZipFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "SCORM";
            inputRecord.ServerFileName = "not-a-zip.txt";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ServerFileName. Error: Server file name for a SCORM resource must be a zip file", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if model validation returns an error.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfModelValidationReturnsAnError()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.Title = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: Title. Error: Title is mandatory.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns success if all validation checks pass.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfAllValidationChecksPass()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns error if scorm LMS link URL is invalid (doesn't return 200 HTTP code).
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfScormLmsLinkUrlIsInvalid()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "scorm";
            inputRecord.ServerFileName = "scormfile.zip";
            inputRecord.LMSLink = "http://www.nonexistentsite.com/nonexistentpage";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: LMS Link URL. Error: LMS Link URL contains an invalid ESR link - \"http://www.nonexistentsite.com/nonexistentpage\"", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if scorm LMS link URL has been used before.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfScormLmsLinkUrlHasBeenUsedElsewhereInLH()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "scorm";
            inputRecord.ServerFileName = "scormfile.zip";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockUrlRewritingRepository.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: LMS Link URL. Error: LMS Link URL contains a link that is already used by an existing Learning Hub resource.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if scorm LMS link URL has been used before.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfScormLmsLinkUrlHasBeenUsedElsewhereInThisMigration()
        {
            // Arrange
            var inputRecord1 = this.GetInputRecord();
            inputRecord1.ResourceType = "scorm";
            inputRecord1.ServerFileName = "scormfile.zip";
            string jsonData1 = JsonConvert.SerializeObject(inputRecord1);

            var inputRecord2 = this.GetInputRecord();
            inputRecord2.ResourceType = "scorm";
            inputRecord2.ServerFileName = "scormfile.zip";
            string jsonData2 = JsonConvert.SerializeObject(inputRecord2);

            // Act
            var result1 = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData1, "my-container");
            var result2 = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData2, "my-container");

            // Assert
            Assert.False(result2.IsValid);
            Assert.Empty(result2.Warnings);
            Assert.Single(result2.Errors);
            Assert.StartsWith("Property Name: LMS Link URL. Error: LMS Link URL contains a link that is already used by another resource in this migration.", result2.Errors[0]);
        }

        #region Mandatory Field Test

        /// <summary>
        /// Validate returns errors if mandatory model properties are not populated.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorsForMissingMandatoryFields()
        {
            // Arrange

            // Clear the mandatory fields.
            var inputRecord = this.GetInputRecord();
            inputRecord.Title = null;
            inputRecord.Description = null;
            inputRecord.AuthorName1 = null;
            inputRecord.Keywords = null;
            inputRecord.ContributorLearningHubUserName = null;
            inputRecord.PublishedDate = null;
            inputRecord.ResourceType = null;

            // These aren't mandatory - shouldn't give errors.
            inputRecord.CatalogueName = null;
            inputRecord.Licence = null;
            inputRecord.ServerFileName = null;
            inputRecord.ArticleContentFilename = null;
            inputRecord.ArticleFile1 = null;
            inputRecord.ArticleFile2 = null;
            inputRecord.ArticleFile3 = null;
            inputRecord.ArticleFile4 = null;
            inputRecord.ArticleFile5 = null;
            inputRecord.ArticleFile6 = null;
            inputRecord.ArticleFile7 = null;
            inputRecord.ArticleFile8 = null;
            inputRecord.ArticleFile9 = null;
            inputRecord.ArticleFile10 = null;
            inputRecord.WeblinkUrl = null;
            inputRecord.LMSLink = null;
            inputRecord.LMSLinkVisibility = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Equal(7, result.Errors.Count);
            Assert.Contains("Property Name: Title. Error: Title is mandatory.", result.Errors);
            Assert.Contains("Property Name: Description. Error: Description is mandatory.", result.Errors);
            Assert.Contains("Property Name: AuthorName1. Error: AuthorName1 or Organisation1 is mandatory.", result.Errors);
            Assert.Contains("Property Name: Keywords. Error: Keywords cannot be null or empty.", result.Errors);
            Assert.Contains("Property Name: ContributorLearningHubUserName. Error: Contributor ElfhUserName is mandatory.", result.Errors);
            Assert.Contains("Property Name: PublishedDate. Error: PublishedDate is mandatory.", result.Errors);
            Assert.Contains("Property Name: ResourceType. Error: Resource Type is mandatory.", result.Errors);
            Assert.Contains("Property Name: PublishedDate. Error: PublishedDate is mandatory.", result.Errors);
        }

        /// <summary>
        /// Validate returns ok if licence type is not populated and resource type is weblink.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsOkForMissingLicenceIfResourceTypeIsWeblink()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Weblink";
            inputRecord.Licence = null;
            inputRecord.ServerFileName = null;
            inputRecord.WeblinkUrl = "http://www.bbc.co.uk";

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns error if licence type is not populated and resource type is not weblink.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorForMissingLicenceIfResourceTypeIsNotWeblink()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.Licence = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: Licence. Error: Licence is mandatory for this resource type.", result.Errors);
        }

        /// <summary>
        /// Validate returns error if article body is not populated and resource type is article.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorForMissingArticleBodyIfResourceTypeIsArticle()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ArticleContentFilename = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: ArticleContentFilename. Error: Article Content file name is mandatory for Article resources.", result.Errors);
        }

        /// <summary>
        /// Validate returns error if weblink url is not populated and resource type is weblink.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorForMissingWeblinkUrlIfResourceTypeIsWeblink()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Weblink";
            inputRecord.ServerFileName = null;
            inputRecord.WeblinkUrl = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: WeblinkUrl. Error: Web Link URL is mandatory when Resource Type is Web Link.", result.Errors);
        }

        #endregion

        #region Field Length Tests

        /// <summary>
        /// Validate returns errors if model properties too long.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorsIfDataTooLong()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceUniqueRef = this.GetString(256);
            inputRecord.Title = this.GetString(256);
            inputRecord.Description = this.GetString(1025);
            inputRecord.AuthorName1 = this.GetString(101);
            inputRecord.AuthorName2 = this.GetString(101);
            inputRecord.AuthorName3 = this.GetString(101);
            inputRecord.AuthorOrganisation1 = this.GetString(101);
            inputRecord.AuthorOrganisation2 = this.GetString(101);
            inputRecord.AuthorOrganisation3 = this.GetString(101);
            inputRecord.AuthorRole1 = this.GetString(101);
            inputRecord.AuthorRole2 = this.GetString(101);
            inputRecord.AuthorRole3 = this.GetString(101);
            inputRecord.Keywords = this.GetString(51);
            inputRecord.ServerFileName = this.GetString(1025);
            inputRecord.ArticleContentFilename = this.GetString(1025);
            inputRecord.ArticleFile1 = this.GetString(1025);
            inputRecord.ArticleFile2 = this.GetString(1025);
            inputRecord.ArticleFile3 = this.GetString(1025);
            inputRecord.ArticleFile4 = this.GetString(1025);
            inputRecord.ArticleFile5 = this.GetString(1025);
            inputRecord.ArticleFile6 = this.GetString(1025);
            inputRecord.ArticleFile7 = this.GetString(1025);
            inputRecord.ArticleFile8 = this.GetString(1025);
            inputRecord.ArticleFile9 = this.GetString(1025);
            inputRecord.ArticleFile10 = this.GetString(1025);
            inputRecord.WeblinkUrl = this.GetString(1025);
            inputRecord.LMSLink = this.GetString(256);
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Contains("Property Name: ResourceUniqueRef. Error: ResourceUniqueRef cannot exceed 255 characters.", result.Errors);
            Assert.Contains("Property Name: Title. Error: Title cannot exceed 255 characters.", result.Errors);
            Assert.Contains("Property Name: Description. Error: Description cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorName1. Error: AuthorName1 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorName2. Error: AuthorName2 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorName3. Error: AuthorName3 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorOrganisation1. Error: Organisation1 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorOrganisation2. Error: Organisation2 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorOrganisation3. Error: Organisation3 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorRole1. Error: Role1 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorRole2. Error: Role2 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: AuthorRole3. Error: Role3 cannot exceed 100 characters.", result.Errors);
            Assert.Contains("Property Name: Keywords. Error: Each Keyword cannot exceed 50 characters. Keywords: 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'", result.Errors);
            Assert.Contains("Property Name: ServerFileName. Error: Server file name cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleContentFilename. Error: Article Content file name cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile1. Error: ArticleFile1 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile2. Error: ArticleFile2 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile3. Error: ArticleFile3 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile4. Error: ArticleFile4 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile5. Error: ArticleFile5 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile6. Error: ArticleFile6 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile7. Error: ArticleFile7 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile8. Error: ArticleFile8 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile9. Error: ArticleFile9 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: ArticleFile10. Error: ArticleFile10 cannot exceed 1024 characters.", result.Errors);
            Assert.Contains("Property Name: LMSLink. Error: LMS Link cannot exceed 255 characters.", result.Errors);
        }

        /// <summary>
        /// Validate returns success if model properties are all within limits.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfDataLengthsWithinLimits()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceUniqueRef = this.GetString(255);
            inputRecord.Title = this.GetString(255);
            inputRecord.Description = this.GetString(1024);
            inputRecord.AuthorName1 = this.GetString(100);
            inputRecord.AuthorName2 = this.GetString(100);
            inputRecord.AuthorName3 = this.GetString(100);
            inputRecord.AuthorOrganisation1 = this.GetString(100);
            inputRecord.AuthorOrganisation2 = this.GetString(100);
            inputRecord.AuthorOrganisation3 = this.GetString(100);
            inputRecord.AuthorRole1 = this.GetString(100);
            inputRecord.AuthorRole2 = this.GetString(100);
            inputRecord.AuthorRole3 = this.GetString(100);
            inputRecord.Keywords = this.GetString(50);
            inputRecord.ServerFileName = this.GetString(1024);
            inputRecord.ArticleContentFilename = this.GetString(1019) + ".html"; // Needs to end in .html to avoid error.
            inputRecord.ArticleFile1 = this.GetString(1024);
            inputRecord.ArticleFile2 = this.GetString(1024);
            inputRecord.ArticleFile3 = this.GetString(1024);
            inputRecord.ArticleFile4 = this.GetString(1024);
            inputRecord.ArticleFile5 = this.GetString(1024);
            inputRecord.ArticleFile6 = this.GetString(1024);
            inputRecord.ArticleFile7 = this.GetString(1024);
            inputRecord.ArticleFile8 = this.GetString(1024);
            inputRecord.ArticleFile9 = this.GetString(1024);
            inputRecord.ArticleFile10 = this.GetString(1024);
            inputRecord.WeblinkUrl = "https://www.google.co.uk/?" + this.GetString(997); // 1024 chars in total.
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
    }

        #endregion

        #region Resource File Count Tests

        // Scorm

        /// <summary>
        /// Validate returns error if Scorm Resource has more than one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfScormResourceMoreThanOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "SCORM";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: ServerFileName. Error: Server file name is mandatory for this resource type.", result.Errors);
        }

        /// <summary>
        /// Validate returns success if Scorm Resource has one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfScormResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "SCORM";
            inputRecord.ServerFileName = "scormfile.zip";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // Article

        /// <summary>
        /// Validate returns error if Article Resource does not have a content file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfNoArticleContentFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ArticleContentFilename = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Contains("Property Name: ArticleContentFilename. Error: Article Content file name is mandatory for Article resources.", result.Errors);
        }

        /// <summary>
        /// Validate returns success if Article Resource does have a content file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfArticleContentFileIsPresent()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ArticleContentFilename = "article.html";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns success if Article Resource has multiple file attachments.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfArticleHasMultipleFileAttachments()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ArticleContentFilename = "ArticleContent.html";
            inputRecord.ArticleFile1 = "file.pdf";
            inputRecord.ArticleFile2 = "file.pdf";
            inputRecord.ArticleFile3 = "file.pdf";
            inputRecord.ArticleFile4 = "file.pdf";
            inputRecord.ArticleFile5 = "file.pdf";
            inputRecord.ArticleFile6 = "file.pdf";
            inputRecord.ArticleFile7 = "file.pdf";
            inputRecord.ArticleFile8 = "file.pdf";
            inputRecord.ArticleFile9 = "file.pdf";
            inputRecord.ArticleFile10 = "file.pdf";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns success if Article Resource zero file attachments.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfArticleHasZeroFileAttachments()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ArticleContentFilename = "ArticleContent.html";
            inputRecord.ArticleFile1 = null;
            inputRecord.ArticleFile2 = null;
            inputRecord.ArticleFile3 = null;
            inputRecord.ArticleFile4 = null;
            inputRecord.ArticleFile5 = null;
            inputRecord.ArticleFile6 = null;
            inputRecord.ArticleFile7 = null;
            inputRecord.ArticleFile8 = null;
            inputRecord.ArticleFile9 = null;
            inputRecord.ArticleFile10 = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns success if Article Resource has one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfArticleHasOneFileAttachment()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ArticleContentFilename = "ArticleContent.html";
            inputRecord.ArticleFile1 = "file.pdf";
            inputRecord.ArticleFile2 = null;
            inputRecord.ArticleFile3 = null;
            inputRecord.ArticleFile4 = null;
            inputRecord.ArticleFile5 = null;
            inputRecord.ArticleFile6 = null;
            inputRecord.ArticleFile7 = null;
            inputRecord.ArticleFile8 = null;
            inputRecord.ArticleFile9 = null;
            inputRecord.ArticleFile10 = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // Weblink

        /// <summary>
        /// Validate returns success if Weblink Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfWeblinkResourceHasEmptyResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Weblink";
            inputRecord.WeblinkUrl = "http://www.bbc.co.uk";
            inputRecord.ServerFileName = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // File  - i.e. Video/Image/Audio/GenericFile.

        /// <summary>
        /// Validate returns error if File Resource has no server file name.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfFileResourceHasNoServerFileName()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "File";
            inputRecord.ServerFileName = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: ServerFileName. Error: Server file name is mandatory for this resource type.", result.Errors);
        }

        /// <summary>
        /// Validate returns success if File Resource has server file name.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfVideoResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "File";
            inputRecord.ServerFileName = "file.pdf";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns error if unknown resource type.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfUnknownResourceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "ABCDEF";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceType. Error: Resource Type 'ABCDEF' is not valid", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if licence type is invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfUnknownLicence()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.Licence = "ABCDEF";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: Licence. Error: Licence 'ABCDEF' is not valid. It must be one of the 4-tier Creative Commons licence types.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if ESR link visibility type is invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfUnknownEsrLinkVisibility()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "scorm";
            inputRecord.ServerFileName = "scorm.zip";
            inputRecord.LMSLinkVisibility = "ABCDEF";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.stagingTableInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: LMSLinkVisibility. Error: LMS Link Visibility 'ABCDEF' is not valid. It must be one of the three LMS link visibility types.", result.Errors[0]);
        }

        #endregion

        private StagingTableInputModel GetInputRecord()
        {
            var model = new StagingTableInputModel()
            {
                ResourceUniqueRef = "ref",
                Title = "title",
                Description = "description",
                SensitiveContentFlag = "Yes",
                Keywords = "keyword1, keyword2",
                LMSLink = "https://www.elearningrepository.nhs.uk/sites/default/files/lms/5733/index_lms.html",
                LMSLinkVisibility = "Display only to me",
                CatalogueName = "my catalogue",
                Licence = "All Rights Reserved",
                AuthorName1 = "author1",
                AuthorName2 = "author2",
                ContributorLearningHubUserName = "username1",
                ResourceType = "Article",
                ArticleContentFilename = "articleBody.html",
                PublishedDate = DateTime.Now,
            };

            return model;
        }

        private string GetString(int length)
        {
            string str = string.Empty;
            for (int i = 0; i < length; i++)
            {
                str += "a";
            }

            return str;
        }
    }
}
