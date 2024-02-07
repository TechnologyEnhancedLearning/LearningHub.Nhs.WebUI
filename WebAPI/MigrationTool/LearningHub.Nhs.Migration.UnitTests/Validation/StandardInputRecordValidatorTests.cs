// <copyright file="StandardInputRecordValidatorTests.cs" company="NHS England">
// Copyright (c) NHS England.
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
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Options;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// The StandardInputRecordValidator tests.
    /// </summary>
    public class StandardInputRecordValidatorTests
    {
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
        private readonly IInputRecordValidator standardInputRecordValidator;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardInputRecordValidatorTests"/> class.
        /// </summary>
        public StandardInputRecordValidatorTests()
        {
            this.mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            this.mockUrlRewritingRepository = new Mock<IUrlRewritingRepository>(MockBehavior.Strict);
            this.mockAzureBlobService = new Mock<IAzureBlobService>(MockBehavior.Strict);
            this.mockFileTypeService = new Mock<IFileTypeService>(MockBehavior.Strict);
            this.mockSettings = new Mock<IOptions<Settings>>(MockBehavior.Loose);

            // This needs to be set up before the constructor is called to avoid test errors.
            this.mockFileTypeService.Setup(x => x.GetAllDisallowedFileExtensions())
                .Returns(new List<string> { "asp" });

            this.standardInputRecordValidator = new StandardInputRecordValidator(
                new UserIdExistsValidator(this.mockUserRepository.Object),
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
            this.mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User()));
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
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Unable to parse the JSON data. This could be caused by a property value using an incorrect data type. Error details: ", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if user id does not exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfUserIdDoesNotExist()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                    .Returns(Task.FromResult<User>(null));

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: Elfhuserid. Error: User ID '999' not found in Learning Hub database.", result.Errors[0]);
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
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockAzureBlobService.Setup(x => x.GetBlobMetadata(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container", "resource file name 0.zip"))
                                        .ReturnsAsync((BlobMetadataViewModel)null);

            this.mockAzureBlobService.Setup(x => x.GetBlobMetadata(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container", "resource file name 1.zip"))
                                        .ReturnsAsync(new BlobMetadataViewModel { SizeInBytes = 10000 });

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Equal("Property Name: Resource Files[0]. Error: Resource File 'resource file name 0.zip' does not exist in the Azure migration blob container.", result.Errors[0]);
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
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockAzureBlobService.Setup(x => x.GetBlobMetadata(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container", "resource file name 0.zip"))
                                        .ReturnsAsync(new BlobMetadataViewModel { SizeInBytes = 10001 });

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: Resource Files[0]. Error: Resource File 'resource file name 0.zip' is too large. Size limit is 10000 bytes. Size of file is 10001 bytes.", result.Errors);
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
            inputRecord.ArticleBody = "<p><a href=\"http://www.nonexistentsite.com/nonexistentpage\">My link</a></p>";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            inputRecord.WebLinkUrl = "http://www.nonexistentsite.com/nonexistentpage";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            inputRecord.ResourceFiles[0].ResourceUrl = "disallowed.asp";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: Resource Files[0]. Error: The file extension of resource file 'disallowed.asp' is not allowed by the Learning Hub.", result.Errors[0]);
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
            inputRecord.ResourceFiles = inputRecord.ResourceFiles.Take(1).ToArray();
            inputRecord.ResourceFiles[0].ResourceUrl = "not-a-zip.txt";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles[0].ResourceUrl. Error: A SCORM package resource file must be a zip file. 'not-a-zip.txt' is not valid.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if ElfhuserId doesn't contain an integer.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfElfhuserIdDoesntContainAnInteger()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ElfhUserId = "ABCDEF";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ElfhUserId. Error: ElfhUserId must contain an integer.", result.Errors[0]);
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
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            inputRecord.ResourceFiles = inputRecord.ResourceFiles.Take(1).ToArray();
            inputRecord.LmsLink = "http://www.nonexistentsite.com/nonexistentpage";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            inputRecord.ResourceFiles = inputRecord.ResourceFiles.Take(1).ToArray();
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockUrlRewritingRepository.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            inputRecord1.ResourceFiles = inputRecord1.ResourceFiles.Take(1).ToArray();
            string jsonData1 = JsonConvert.SerializeObject(inputRecord1);

            var inputRecord2 = this.GetInputRecord();
            inputRecord2.ResourceType = "scorm";
            inputRecord2.ResourceFiles = inputRecord1.ResourceFiles.Take(1).ToArray();
            string jsonData2 = JsonConvert.SerializeObject(inputRecord2);

            // Act
            var result1 = await this.standardInputRecordValidator.ValidateAsync(jsonData1, "my-container");
            var result2 = await this.standardInputRecordValidator.ValidateAsync(jsonData2, "my-container");

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
            inputRecord.Authors = null;
            inputRecord.Keywords = null;
            inputRecord.ElfhUserId = null;
            inputRecord.CreateDate = null;
            inputRecord.ResourceFiles = null;
            inputRecord.ResourceType = null;
            inputRecord.Version = null;
            inputRecord.HasCost = null;

            // These aren't mandatory - shouldn't give errors.
            inputRecord.LicenceType = null;
            inputRecord.ArticleBody = null;
            inputRecord.WebLinkUrl = null;
            inputRecord.LmsLink = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Equal(9, result.Errors.Count);
            Assert.Contains("Property Name: Title. Error: Title is mandatory.", result.Errors);
            Assert.Contains("Property Name: Description. Error: Description is mandatory.", result.Errors);
            Assert.Contains("Property Name: Authors. Error: Authors cannot be null or empty.", result.Errors);
            Assert.Contains("Property Name: Keywords. Error: Keywords cannot be null or empty.", result.Errors);
            Assert.Contains("Property Name: ElfhUserId. Error: ElfhUserId is mandatory.", result.Errors);
            Assert.Contains("Property Name: CreateDate. Error: CreateDate is mandatory.", result.Errors);
            Assert.Contains("Property Name: ResourceType. Error: Resource Type is mandatory.", result.Errors);
            Assert.Contains("Property Name: Version. Error: Version is mandatory.", result.Errors);
            Assert.Contains("Property Name: HasCost. Error: Has Cost is mandatory.", result.Errors);
        }

        /// <summary>
        /// Validate returns errors if mandatory model properties are not populated.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorsForEmptyMandatoryArrays()
        {
            // Arrange

            // Clear the mandatory fields.
            var inputRecord = this.GetInputRecord();
            inputRecord.Authors = new AuthorModel[] { };
            inputRecord.Keywords = new string[] { };

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Equal(2, result.Errors.Count);
            Assert.Contains("Property Name: Authors. Error: Authors cannot be null or empty.", result.Errors);
            Assert.Contains("Property Name: Keywords. Error: Keywords cannot be null or empty.", result.Errors);
        }

        /// <summary>
        /// Validate returns errors if mandatory model child properties are not populated.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorsForMissingMandatoryFieldsInChildObjects()
        {
            // Arrange

            // Clear the mandatory child fields.
            var inputRecord = this.GetInputRecord();
            inputRecord.Authors[0].AuthorIndex = null;
            inputRecord.Authors[0].Author = null;
            inputRecord.Authors[0].Organisation = null;
            inputRecord.ResourceFiles[0].ResourceIndex = null;
            inputRecord.ResourceFiles[0].ResourceUrl = null;
            inputRecord.Keywords[0] = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Equal(5, result.Errors.Count);
            Assert.Contains("Property Name: Authors[0].AuthorIndex. Error: Author Index is mandatory.", result.Errors);
            Assert.Contains("Property Name: Authors[0]. Error: Author or author organisation is mandatory.", result.Errors);
            Assert.Contains("Property Name: ResourceFiles[0].ResourceIndex. Error: Resource Index is mandatory.", result.Errors);
            Assert.Contains("Property Name: ResourceFiles[0].ResourceUrl. Error: Resource URL is mandatory.", result.Errors);
            Assert.Contains("Property Name: Keywords[0]. Error: Keyword cannot be null or empty.", result.Errors);
        }

        /// <summary>
        /// Validate returns ok if licence type is not populated and resource type is weblink.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsOkForMissingLicenceTypeIfResourceTypeIsWeblink()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Weblink";
            inputRecord.LicenceType = null;
            inputRecord.ResourceFiles = null;
            inputRecord.WebLinkUrl = "http://www.bbc.co.uk";

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
        public async Task ValidateReturnsErrorForMissingLicenceTypeIfResourceTypeIsNotWeblink()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.LicenceType = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: LicenceType. Error: Licence Type is mandatory for this resource type.", result.Errors);
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
            inputRecord.ArticleBody = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: ArticleBody. Error: Article Body is mandatory when Resource Type is Article.", result.Errors);
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
            inputRecord.ResourceFiles = null;
            inputRecord.WebLinkUrl = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.Contains("Property Name: WebLinkUrl. Error: Web Link URL is mandatory when Resource Type is Web Link.", result.Errors);
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
            inputRecord.Title = this.GetString(256);
            inputRecord.Description = this.GetString(1025);
            inputRecord.Authors[0].Author = this.GetString(101);
            inputRecord.Authors[0].Organisation = this.GetString(101);
            inputRecord.Authors[0].Role = this.GetString(101);
            inputRecord.Keywords[0] = this.GetString(51);
            inputRecord.ResourceFiles[0].ResourceUrl = this.GetString(1025);
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Equal(7, result.Errors.Count);
            Assert.StartsWith("Property Name: Title. Error: Title cannot exceed 255 characters.", result.Errors[0]);
            Assert.StartsWith("Property Name: Description. Error: Description cannot exceed 1024 characters.", result.Errors[1]);
            Assert.StartsWith("Property Name: Authors[0].Author. Error: Author cannot exceed 100 characters.", result.Errors[2]);
            Assert.StartsWith("Property Name: Authors[0].Organisation. Error: Author organisation cannot exceed 100 characters.", result.Errors[3]);
            Assert.StartsWith("Property Name: Authors[0].Role. Error: Author role cannot exceed 100 characters.", result.Errors[4]);
            Assert.StartsWith("Property Name: Keywords[0]. Error: Each Keyword cannot exceed 50 characters. Keyword: 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'", result.Errors[5]);
            Assert.StartsWith("Property Name: ResourceFiles[0].ResourceUrl. Error: Resource URL cannot exceed 1024 characters.", result.Errors[6]);
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
            inputRecord.Title = this.GetString(255);
            inputRecord.Description = this.GetString(1024);
            inputRecord.Authors[0].Author = this.GetString(100);
            inputRecord.Authors[0].Organisation = this.GetString(100);
            inputRecord.Authors[0].Role = this.GetString(100);
            inputRecord.Keywords[0] = this.GetString(50);
            inputRecord.ResourceFiles[0].ResourceUrl = this.GetString(1024);
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Scorm Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfScormResourceHasEmptyResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "SCORM";
            inputRecord.ResourceFiles = new ResourceFileModel[] { };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Scorm Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfScormResourceHasNullResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "SCORM";
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
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
            inputRecord.ResourceFiles = new ResourceFileModel[] { new ResourceFileModel { ResourceIndex = 0, ResourceUrl = "scormfile.zip" } };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // Article

        /// <summary>
        /// Validate returns success if Article Resource has more than one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfArticleResourceMoreThanOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns success if Article Resource has empty resource files array.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfArticleResourceHasEmptyResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ResourceFiles = new ResourceFileModel[] { };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns success if Article Resource has null resource files array.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfArticleResourceHasNullResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
        public async Task ValidateReturnsSuccessIfArticleResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            inputRecord.ResourceFiles = new ResourceFileModel[] { new ResourceFileModel { ResourceIndex = 0, ResourceUrl = "scormfile.zip" } };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // Weblink

        /// <summary>
        /// Validate returns error if Weblink Resource has one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfWeblinkResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Weblink";
            inputRecord.WebLinkUrl = "http://www.bbc.co.uk";
            inputRecord.ResourceFiles = new ResourceFileModel[] { new ResourceFileModel { ResourceIndex = 0, ResourceUrl = "file.txt" } };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

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
            inputRecord.WebLinkUrl = "http://www.bbc.co.uk";
            inputRecord.ResourceFiles = new ResourceFileModel[] { };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// Validate returns success if Weblink Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfWeblinkResourceHasNullResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Weblink";
            inputRecord.WebLinkUrl = "http://www.bbc.co.uk";
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // Video

        /// <summary>
        /// Validate returns error if Video Resource has more than one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfVideoResourceMoreThanOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Video";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Video Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfVideoResourceHasEmptyResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Video";
            inputRecord.ResourceFiles = new ResourceFileModel[] { };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Video Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfVideoResourceHasNullResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Video";
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns success if Video Resource has one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfVideoResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Video";
            inputRecord.ResourceFiles = new ResourceFileModel[] { new ResourceFileModel { ResourceIndex = 0, ResourceUrl = "video.mp4" } };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // Audio

        /// <summary>
        /// Validate returns error if Audio Resource has more than one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfAudioResourceMoreThanOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Audio";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Audio Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfAudioResourceHasEmptyResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Audio";
            inputRecord.ResourceFiles = new ResourceFileModel[] { };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Audio Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfAudioResourceHasNullResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Audio";
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns success if Video Resource has one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfAudioResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Audio";
            inputRecord.ResourceFiles = new ResourceFileModel[] { new ResourceFileModel { ResourceIndex = 0, ResourceUrl = "Audio.mp3" } };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // GenericFile

        /// <summary>
        /// Validate returns error if GenericFile Resource has more than one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfGenericFileResourceMoreThanOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "GenericFile";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if GenericFile Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfGenericFileResourceHasEmptyResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "GenericFile";
            inputRecord.ResourceFiles = new ResourceFileModel[] { };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if GenericFile Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfGenericFileResourceHasNullResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "GenericFile";
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns success if GenericFile Resource has one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfGenericFileResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "GenericFile";
            inputRecord.ResourceFiles = new ResourceFileModel[] { new ResourceFileModel { ResourceIndex = 0, ResourceUrl = "Audio.mp3" } };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Empty(result.Errors);
        }

        // Image

        /// <summary>
        /// Validate returns error if Image Resource has more than one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfImageResourceMoreThanOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Image";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Image Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfImageResourceHasEmptyResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Image";
            inputRecord.ResourceFiles = new ResourceFileModel[] { };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Image Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfImageResourceHasNullResourceFilesArray()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Image";
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceFiles. Error: The number of resource files doesn't meet the requirement for the resource type.", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns success if GenericFile Resource has one resource file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsSuccessIfImageResourceHasOneResourceFile()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Image";
            inputRecord.ResourceFiles = new ResourceFileModel[] { new ResourceFileModel { ResourceIndex = 0, ResourceUrl = "image.jpg" } };
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

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
            inputRecord.ResourceFiles = null;
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: ResourceType. Error: Resource Type 'ABCDEF' is not valid", result.Errors[0]);
        }

        /// <summary>
        /// Validate returns error if Image Resource has no resource files.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ValidateReturnsErrorIfUnknownLicenceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.LicenceType = "ABCDEF";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var result = await this.standardInputRecordValidator.ValidateAsync(jsonData, "my-container");

            // Assert
            Assert.False(result.IsValid);
            Assert.Empty(result.Warnings);
            Assert.Single(result.Errors);
            Assert.StartsWith("Property Name: LicenceType. Error: Licence Type 'ABCDEF' is not valid. It must be one of the 4-tier Creative Commons licence types.", result.Errors[0]);
        }

        #endregion

        private StandardInputModel GetInputRecord()
        {
            var model = new StandardInputModel()
            {
                Title = "title",
                Authors = new AuthorModel[]
                {
                    new AuthorModel() { AuthorIndex = 0, Author = "author1" },
                    new AuthorModel() { AuthorIndex = 1, Author = "author2" },
                },
                HasCost = false,
                Version = "1.0",
                Keywords = new string[] { "keyword1", "keyword2", "keyword3" },
                LmsLink = "https://www.elearningrepository.nhs.uk/sites/default/files/lms/5733/index_lms.html",
                CreateDate = DateTime.Now,
                ElfhUserId = "999",
                Description = "description",
                WebLinkUrl = null,
                ArticleBody = "article body",
                LicenceType = "All Rights Reserved",
                ResourceType = "Article",
                ResourceFiles = new ResourceFileModel[]
                {
                    new ResourceFileModel() { ResourceIndex = 0, ResourceUrl = "resource file name 0.zip" },
                    new ResourceFileModel() { ResourceIndex = 1, ResourceUrl = "resource file name 1.zip" },
                },
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
