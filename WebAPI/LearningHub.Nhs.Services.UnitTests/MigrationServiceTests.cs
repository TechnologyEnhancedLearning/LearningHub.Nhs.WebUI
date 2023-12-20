// <copyright file="MigrationServiceTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Migration.Interface.Mapping;
    using LearningHub.Nhs.Migration.Interface.Validation;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Staging.Repository;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Migration;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Migrations;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// The migration service tests.
    /// </summary>
    public class MigrationServiceTests
    {
        /// <summary>
        /// The mock migration repository.
        /// </summary>
        private readonly Mock<IMigrationRepository> mockMigrationRepository;

        /// <summary>
        /// The mock migration input record repository.
        /// </summary>
        private readonly Mock<IMigrationInputRecordRepository> mockMigrationInputRecordRepository;

        /// <summary>
        /// The mock migration source repository.
        /// </summary>
        private readonly Mock<IMigrationSourceRepository> mockMigrationSourceRepository;

        /// <summary>
        /// The mock staging table input model repository.
        /// </summary>
        private readonly Mock<IStagingTableInputModelRepository> mockStagingTableInputModelRepository;

        /// <summary>
        /// The mock node repository.
        /// </summary>
        private readonly Mock<INodeRepository> mockNodeRepository;

        /// <summary>
        /// The mock resource version repository.
        /// </summary>
        private readonly Mock<IResourceVersionRepository> mockResourceVersionRepository;

        /// <summary>
        /// The mock azure blob service.
        /// </summary>
        private readonly Mock<IAzureBlobService> mockAzureBlobService;

        /// <summary>
        /// The mock azure media service.
        /// </summary>
        private readonly Mock<IAzureMediaService> mockAzureMediaService;

        /// <summary>
        /// The mock azure data factory service.
        /// </summary>
        private readonly Mock<IAzureDataFactoryService> mockAzureDataFactoryService;

        /// <summary>
        /// The mock file type service.
        /// </summary>
        private readonly Mock<IFileTypeService> mockFileTypeService;

        /// <summary>
        /// The mock queue communicator service.
        /// </summary>
        private readonly Mock<IQueueCommunicatorService> mockQueueCommunicatorService;

        /// <summary>
        /// The mock queue communicator service.
        /// </summary>
        private readonly Mock<IResourceService> mockResourceService;

        /// <summary>
        /// The mock InputRecordValidatorFactory.
        /// </summary>
        private readonly Mock<IInputRecordValidatorFactory> mockInputRecordValidatorFactory;

        /// <summary>
        /// The mock IInputRecordMapperFactory.
        /// </summary>
        private readonly Mock<IInputRecordMapperFactory> mockInputRecordMapperFactory;

        /// <summary>
        /// The mock InputRecordValidator.
        /// </summary>
        private readonly Mock<IInputRecordValidator> mockInputRecordValidator;

        /// <summary>
        /// The mock IInputRecordMapper.
        /// </summary>
        private readonly Mock<IInputRecordMapper> mockInputRecordMapper;

        /// <summary>
        /// The mock settings.
        /// </summary>
        private readonly Mock<IOptions<Settings>> mockSettings;

        /// <summary>
        /// The mock logger.
        /// </summary>
        private readonly Mock<ILogger<MigrationService>> mockLogger;

        /// <summary>
        /// The migration service.
        /// </summary>
        private readonly IMigrationService migrationService;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        private Mock<IMapper> mockMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationServiceTests"/> class.
        /// </summary>
        public MigrationServiceTests()
        {
            this.mockMigrationRepository = new Mock<IMigrationRepository>(MockBehavior.Strict);
            this.mockMigrationInputRecordRepository = new Mock<IMigrationInputRecordRepository>(MockBehavior.Strict);
            this.mockMigrationSourceRepository = new Mock<IMigrationSourceRepository>(MockBehavior.Strict);
            this.mockStagingTableInputModelRepository = new Mock<IStagingTableInputModelRepository>(MockBehavior.Strict);
            this.mockNodeRepository = new Mock<INodeRepository>(MockBehavior.Strict);
            this.mockResourceVersionRepository = new Mock<IResourceVersionRepository>(MockBehavior.Strict);
            this.mockAzureBlobService = new Mock<IAzureBlobService>(MockBehavior.Strict);
            this.mockAzureMediaService = new Mock<IAzureMediaService>(MockBehavior.Strict);
            this.mockAzureDataFactoryService = new Mock<IAzureDataFactoryService>(MockBehavior.Strict);
            this.mockFileTypeService = new Mock<IFileTypeService>(MockBehavior.Strict);
            this.mockQueueCommunicatorService = new Mock<IQueueCommunicatorService>(MockBehavior.Strict);
            this.mockResourceService = new Mock<IResourceService>(MockBehavior.Strict);
            this.mockInputRecordValidatorFactory = new Mock<IInputRecordValidatorFactory>(MockBehavior.Strict);
            this.mockInputRecordMapperFactory = new Mock<IInputRecordMapperFactory>(MockBehavior.Strict);
            this.mockInputRecordValidator = new Mock<IInputRecordValidator>(MockBehavior.Strict);
            this.mockInputRecordMapper = new Mock<IInputRecordMapper>(MockBehavior.Strict);
            this.mockSettings = new Mock<IOptions<Settings>>(MockBehavior.Loose);
            this.mockLogger = new Mock<ILogger<MigrationService>>(MockBehavior.Loose);
            this.mockMapper = new Mock<IMapper>(MockBehavior.Loose);

            this.migrationService = new MigrationService(
                this.mockMigrationRepository.Object,
                this.mockMigrationInputRecordRepository.Object,
                this.mockMigrationSourceRepository.Object,
                this.mockStagingTableInputModelRepository.Object,
                this.mockNodeRepository.Object,
                this.mockResourceVersionRepository.Object,
                this.mockAzureBlobService.Object,
                this.mockAzureMediaService.Object,
                this.mockAzureDataFactoryService.Object,
                this.mockFileTypeService.Object,
                this.mockQueueCommunicatorService.Object,
                this.mockResourceService.Object,
                this.mockInputRecordValidatorFactory.Object,
                this.mockInputRecordMapperFactory.Object,
                this.mockSettings.Object,
                this.mockLogger.Object,
                this.mockMapper.Object);

            // Set up expected invocations for checks made during happy path. Avoids duplication across test cases.
            this.settings = new Settings
            {
                MigrationTool = new MigrationToolSettings()
                {
                    AzureStorageAccountConnectionString = "migration-connection-string",
                    AzureBlobContainerNameForMetadataFiles = "metadata-blob-container-name",
                    MetadataCreationQueueName = "metadataCreateQueueName",
                    StagingTables = new StagingTableSettings
                    {
                        AdfPipelineAzureBlobContainerName = "adf-blob-container-name",
                        AdfFactoryName = "factory",
                        AdfPipelineName = "pipeline",
                        AdfResourceGroup = "resourceGroup",
                    },
                },
                AzureFileStorageConnectionString = "lh-azure-connection-string",
                AzureFileStorageResourceShareName = "lh-azure-share-name",
            };

            this.mockSettings.Setup(x => x.Value).Returns(this.settings);

            this.mockMigrationSourceRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .Returns(Task.FromResult<MigrationSource>(new MigrationSource()));

            this.mockNodeRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .Returns(Task.FromResult<Node>(new Node()));

            this.mockAzureBlobService.Setup(x => x.BlobContainerExists(this.settings.MigrationTool.AzureStorageAccountConnectionString, this.settings.MigrationTool.AzureBlobContainerNameForMetadataFiles)).ReturnsAsync(true);
            this.mockAzureBlobService.Setup(x => x.BlobContainerExists(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container")).ReturnsAsync(true);
            this.mockAzureBlobService.Setup(x => x.BlobContainerExists(this.settings.MigrationTool.AzureStorageAccountConnectionString, this.settings.MigrationTool.StagingTables.AdfPipelineAzureBlobContainerName)).ReturnsAsync(true);

            this.mockAzureBlobService.Setup(x => x.UploadBlobToContainer(
                    this.settings.MigrationTool.AzureStorageAccountConnectionString,
                    this.settings.MigrationTool.AzureBlobContainerNameForMetadataFiles,
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()))
                .ReturnsAsync("azure-metadata-file-url");

            this.mockInputRecordValidatorFactory.Setup(x => x.GetValidator(It.IsAny<int>()))
                .Returns(this.mockInputRecordValidator.Object);

            this.mockInputRecordMapperFactory.Setup(x => x.GetMapper(It.IsAny<int>()))
                .Returns(this.mockInputRecordMapper.Object);
        }

        #region Migration Creation Tests - CreateFromJsonString

        /// <summary>
        /// The create from json string_ returns error if destination node id doesnt exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonString_ReturnsErrorIfDestinationNodeIdDoesntExist()
        {
            // Arrange
            this.mockNodeRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .Returns(Task.FromResult<Node>(null));

            // Act
            var result = await this.migrationService.CreateFromJsonString(this.GetJsonData(), 9, "my-container", 99, 999);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Destination Node ID '99' does not exist in the database", result.Details[0]);
        }

        /// <summary>
        /// The create from json string_ returns error if migration source id doesnt exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonString_ReturnsErrorIfMigrationSourceIdDoesntExist()
        {
            // Arrange
            this.mockMigrationSourceRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .Returns(Task.FromResult<MigrationSource>(null));

            // Act
            var result = await this.migrationService.CreateFromJsonString(this.GetJsonData(), 9, "my-container", 99, 999);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Migration Source ID '9' does not exist in the database", result.Details[0]);
        }

        /// <summary>
        /// The create from json string_ returns error if azure metadata file container doesnt exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonString_ReturnsErrorIfAzureMetadataFileContainerDoesntExist()
        {
            // Arrange
            this.mockAzureBlobService.Setup(x => x.BlobContainerExists(this.settings.MigrationTool.AzureStorageAccountConnectionString, this.settings.MigrationTool.AzureBlobContainerNameForMetadataFiles))
                                        .ReturnsAsync(false);

            // Act
            var result = await this.migrationService.CreateFromJsonString(this.GetJsonData(), 9, "my-container", 99, 999);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal($"Blob container '{this.settings.MigrationTool.AzureBlobContainerNameForMetadataFiles}' does not exist in the Migration Azure Storage Account", result.Details[0]);
        }

        /// <summary>
        /// The create from json string_ returns error if azure resource file container doesnt exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonString_ReturnsErrorIfAzureResourceFileContainerDoesntExist()
        {
            // Arrange
            this.mockAzureBlobService.Setup(x => x.BlobContainerExists(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container"))
                                        .ReturnsAsync(false);

            // Act
            var result = await this.migrationService.CreateFromJsonString(this.GetJsonData(), 9, "my-container", 99, 999);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Blob container 'my-container' does not exist in the Migration Azure Storage Account", result.Details[0]);
        }

        /// <summary>
        /// The create from json string_ returns success if all checks passed.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonString_ReturnsSuccessIfAllChecksPassed()
        {
            // Arrange

            // Migration Entity.
            this.mockMigrationRepository.Setup(x => x.CreateAsync(999, It.Is<Migration>(y =>
                        y.AzureMigrationContainerName == "my-container" &&
                        y.DestinationNodeId == 99 &&
                        y.MetadataFilePath == "azure-metadata-file-url" &&
                        y.MigrationSourceId == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Created)))
                .Returns(Task.FromResult<int>(123));

            // Migration Input Record entities.
            this.mockMigrationInputRecordRepository.Setup(x => x.CreateAsync(999, It.Is<MigrationInputRecord>(y =>
                        y.Data.Length > 0 &&
                        y.MigrationId == 123 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.Created)))
                .Returns(Task.FromResult<int>(1));

            // Act
            var result = await this.migrationService.CreateFromJsonString(this.GetJsonData(), 9, "my-container", 99, 999);

            // Assert
            this.mockMigrationRepository.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<Migration>()), Times.Once());
            this.mockMigrationInputRecordRepository.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<MigrationInputRecord>()), Times.Exactly(3));
            Assert.True(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Finished migration creation successfully. 3 input records staged. MigrationId: 123", result.Details[0]);
        }

        #endregion

        #region Migration Creation Tests - CreateFromStagingTables

        /// <summary>
        /// The create from staging tables returns error if destination node id doesnt exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromStagingTables_ReturnsErrorIfMigrationSourceIdDoesntExist()
        {
            // Arrange
            this.mockMigrationSourceRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .Returns(Task.FromResult<MigrationSource>(null));

            // Act
            var result = await this.migrationService.CreateFromStagingTables(new byte[0], 9, "my-container", 999);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Migration Source ID '9' does not exist in the database", result.Details[0]);
        }

        /// <summary>
        /// The CreateFromStagingTables returns error if azure resource file container doesnt exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromStagingTables_ReturnsErrorIfAzureResourceFileContainerDoesntExist()
        {
            // Arrange
            this.mockAzureBlobService.Setup(x => x.BlobContainerExists(this.settings.MigrationTool.AzureStorageAccountConnectionString, "my-container"))
                                        .ReturnsAsync(false);

            // Act
            var result = await this.migrationService.CreateFromStagingTables(new byte[0], 9, "my-container", 999);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Blob container 'my-container' does not exist in the Migration Azure Storage Account", result.Details[0]);
        }

        /// <summary>
        /// The CreateFromStagingTables_ReturnsErrorIfNoStagingTableData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromStagingTables_ReturnsErrorIfNoStagingTableData()
        {
            // Arrange
            this.mockAzureBlobService.Setup(x => x.UploadBlobToContainer(
                    this.settings.MigrationTool.AzureStorageAccountConnectionString,
                    this.settings.MigrationTool.StagingTables.AdfPipelineAzureBlobContainerName,
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()))
                .ReturnsAsync("azure-pipeline-file-url");

            this.mockAzureDataFactoryService.Setup(x => x.RunPipeline("resourceGroup", "factory", "pipeline", It.IsAny<Dictionary<string, object>>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            this.mockStagingTableInputModelRepository.Setup(x => x.GetAllStagingTableInputModels())
                .Returns(Task.FromResult(new List<StagingTableInputModel>()));

            // Act
            var result = await this.migrationService.CreateFromStagingTables(new byte[0], 9, "my-container", 999);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("The staging tables did not contain any resource data.", result.Details[0]);
        }

        /// <summary>
        /// The CreateFromStagingTables returns success if all checks passed.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromStagingTables_ReturnsSuccessIfAllChecksPassed()
        {
            // Arrange
            this.mockAzureBlobService.Setup(x => x.UploadBlobToContainer(
                    this.settings.MigrationTool.AzureStorageAccountConnectionString,
                    this.settings.MigrationTool.StagingTables.AdfPipelineAzureBlobContainerName,
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()))
                .ReturnsAsync("azure-pipeline-file-url");

            this.mockAzureDataFactoryService.Setup(x => x.RunPipeline("resourceGroup", "factory", "pipeline", It.IsAny<Dictionary<string, object>>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Staging table data.
            this.mockStagingTableInputModelRepository.Setup(x => x.GetAllStagingTableInputModels())
                .Returns(Task.FromResult(new List<StagingTableInputModel>()
                {
                    new StagingTableInputModel { Title = "Article 1", Description = "Article description 1", ResourceType = "Article", ArticleContentFilename = "article1.html" },
                    new StagingTableInputModel { Title = "Weblink 1", Description = "Weblink description 1", ResourceType = "Weblink", },
                }));

            // Calls to azure blob service to get article body file contents.
            this.mockAzureBlobService.Setup(x => x.GetBlobMetadata("migration-connection-string", "my-container", "article1.html"))
                .ReturnsAsync(new BlobMetadataViewModel());
            this.mockAzureBlobService.Setup(x => x.DownloadBlobAsText("migration-connection-string", "my-container", "article1.html"))
                .ReturnsAsync("article content 1");

            // Migration Entity.
            this.mockMigrationRepository.Setup(x => x.CreateAsync(999, It.Is<Migration>(y =>
                        y.AzureMigrationContainerName == "my-container" &&
                        y.DestinationNodeId == null &&
                        y.MetadataFilePath == "azure-pipeline-file-url" &&
                        y.MigrationSourceId == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Created)))
                .Returns(Task.FromResult<int>(123));

            // Migration Input Record entities.
            this.mockMigrationInputRecordRepository.Setup(x => x.CreateAsync(999, It.Is<MigrationInputRecord>(y =>
                        y.Data.Length > 0 &&
                        y.MigrationId == 123 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.Created)))
                .Returns(Task.FromResult<int>(1));

            // Act
            var result = await this.migrationService.CreateFromStagingTables(new byte[0], 9, "my-container", 999);

            // Assert
            this.mockMigrationRepository.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<Migration>()), Times.Once());
            this.mockMigrationInputRecordRepository.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<MigrationInputRecord>()), Times.Exactly(2));
            Assert.True(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Finished migration creation successfully. 2 input records staged. MigrationId: 123", result.Details[0]);
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Validate returns error if migration id doesn't exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsErrorIfMigrationIdDoesntExist()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(null));

            // Act
            var result = await this.migrationService.Validate(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.False(result.IsValid);
            Assert.Equal("Migration '9' was not found in the database.", result.FundamentalError);
        }

        /// <summary>
        /// Validate returns error if migration status is CreatingLHMetadata.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsErrorIfMigrationStatusIsCreatingLHMetadata()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new Migration()
                {
                    Id = 9,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                }));

            // Act
            var result = await this.migrationService.Validate(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.False(result.IsValid);
            Assert.Equal("Migration '9' has already been progressed onto the resource creation stage and cannot be validated again.", result.FundamentalError);
        }

        /// <summary>
        /// Validate returns valid if input record is valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsValidIfInputRecordIsValid()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.Created,
                }));

            this.mockMigrationInputRecordRepository.Setup(x => x.GetByMigrationIdAsync(9))
                .Returns(Task.FromResult(new List<MigrationInputRecord>
                {
                    new MigrationInputRecord()
                        {
                            Id = 1,
                            MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.Created,
                        },
                }));

            // Initial call to update migration status.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Validating)))
                .Returns(Task.CompletedTask);

            // Call to validator.
            this.mockInputRecordValidator.Setup(x => x.ValidateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new MigrationInputRecordValidationResult()
                {
                    IsValid = true,
                    RecordIndex = 1,
                    RecordTitle = "title",
                    Errors = new List<string>(),
                    Warnings = new List<string>(),
                }));

            // Call to update status on input record.
            this.mockMigrationInputRecordRepository.Setup(x => x.UpdateAsync(99, It.Is<MigrationInputRecord>(y =>
                        y.Id == 1 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationComplete &&
                        y.RecordTitle == "title")))
                .Returns(Task.CompletedTask);

            // Second call to update migration status again.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Validated)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.Validate(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            this.mockMigrationRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Migration>()), Times.Exactly(2));
            this.mockMigrationInputRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<MigrationInputRecord>()), Times.Once());
            Assert.True(result.IsValid);
            Assert.Null(result.FundamentalError);
            Assert.Single(result.InputRecordValidationResults);
        }

        /// <summary>
        /// Validate returns invalid if input record is invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsInvalidIfInputRecordIsInvalid()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.Created,
                }));

            this.mockMigrationInputRecordRepository.Setup(x => x.GetByMigrationIdAsync(9))
                .Returns(Task.FromResult(new List<MigrationInputRecord>
                {
                    new MigrationInputRecord()
                        {
                            Id = 1,
                            MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.Created,
                        },
                }));

            // Initial call to update migration status.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Validating)))
                .Returns(Task.CompletedTask);

            // Call to validator.
            this.mockInputRecordValidator.Setup(x => x.ValidateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new MigrationInputRecordValidationResult()
                {
                    IsValid = false,
                    RecordIndex = 1,
                    RecordTitle = "title",
                    Errors = new List<string>
                    {
                        "validation error 1",
                        "validation error 2",
                    },
                    Warnings = new List<string>
                    {
                        "validation warning 1",
                        "validation warning 2",
                    },
                }));

            // Call to update status on input record.
            this.mockMigrationInputRecordRepository.Setup(x => x.UpdateAsync(99, It.Is<MigrationInputRecord>(y =>
                        y.Id == 1 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationFailed &&
                        y.RecordTitle == "title" &&
                        y.ValidationErrors == $"validation error 1{Environment.NewLine}validation error 2" &&
                        y.ValidationWarnings == $"validation warning 1{Environment.NewLine}validation warning 2")))
                .Returns(Task.CompletedTask);

            // Second call to update migration status again.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Validated)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.Validate(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            this.mockMigrationRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Migration>()), Times.Exactly(2));
            this.mockMigrationInputRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<MigrationInputRecord>()), Times.Once());
            Assert.False(result.IsValid);
            Assert.Null(result.FundamentalError);
            Assert.Single(result.InputRecordValidationResults);
        }

        /// <summary>
        /// Validate returns invalid if any input record is invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsInvalidIfAnyInputRecordIsInvalid()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.Created,
                }));

            this.mockMigrationInputRecordRepository.Setup(x => x.GetByMigrationIdAsync(9))
                .Returns(Task.FromResult(new List<MigrationInputRecord>
                {
                    new MigrationInputRecord()
                        {
                            Id = 1,
                            MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.Created,
                            Data = "rec1",
                        },
                    new MigrationInputRecord()
                        {
                            Id = 2,
                            MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.Created,
                            Data = "rec2",
                        },
                }));

            // Initial call to update migration status.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Validating)))
                .Returns(Task.CompletedTask);

            // Call to validator for input record #1 - invalid.
            this.mockInputRecordValidator.Setup(x => x.ValidateAsync("rec1", It.IsAny<string>()))
                .Returns(Task.FromResult(new MigrationInputRecordValidationResult()
                {
                    IsValid = false,
                    RecordIndex = 1,
                    RecordTitle = "title1",
                    Errors = new List<string>
                    {
                        "validation error 1",
                        "validation error 2",
                    },
                    Warnings = new List<string>
                    {
                        "validation warning 1",
                        "validation warning 2",
                    },
                }));

            // Call to validator for inpute record #2 - valid.
            this.mockInputRecordValidator.Setup(x => x.ValidateAsync("rec2", It.IsAny<string>()))
                .Returns(Task.FromResult(new MigrationInputRecordValidationResult()
                {
                    IsValid = true,
                    RecordIndex = 2,
                    RecordTitle = "title2",
                    Errors = new List<string>(),
                    Warnings = new List<string>(),
                }));

            // Call to update status on input record #1.
            this.mockMigrationInputRecordRepository.Setup(x => x.UpdateAsync(99, It.Is<MigrationInputRecord>(y =>
                        y.Id == 1 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationFailed &&
                        y.RecordTitle == "title1" &&
                        y.ValidationErrors == $"validation error 1{Environment.NewLine}validation error 2" &&
                        y.ValidationWarnings == $"validation warning 1{Environment.NewLine}validation warning 2")))
                .Returns(Task.CompletedTask);

            // Call to update status on input record #2.
            this.mockMigrationInputRecordRepository.Setup(x => x.UpdateAsync(99, It.Is<MigrationInputRecord>(y =>
                        y.Id == 2 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationComplete &&
                        y.RecordTitle == "title2" &&
                        y.ValidationErrors == null &&
                        y.ValidationWarnings == null)))
                .Returns(Task.CompletedTask);

            // Second call to update migration status again.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.Validated)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.Validate(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            this.mockMigrationRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Migration>()), Times.Exactly(2));
            this.mockMigrationInputRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<MigrationInputRecord>()), Times.Exactly(2));
            Assert.False(result.IsValid);
            Assert.Null(result.FundamentalError);
            Assert.Equal(2, result.InputRecordValidationResults.Count);
        }

        #endregion

        #region Draft Resource Creation Tests

        /// <summary>
        /// BeginCreateMetadata returns error if migration id doesn't exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task BeginCreateMetadata_ReturnsErrorIfMigrationIdDoesntExist()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(null));

            // Act
            var result = await this.migrationService.BeginCreateMetadata(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Migration '9' was not found in the database.", result.Details[0]);
        }

        /// <summary>
        /// BeginCreateMetadata returns error if migration status is not correct.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task BeginCreateMetadata_ReturnsErrorIfMigrationStatusIsNotCorrect()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new Migration()
                {
                    Id = 9,
                    MigrationStatusEnum = MigrationStatusEnum.Created,
                }));

            // Act
            var result = await this.migrationService.BeginCreateMetadata(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Cannot create resource metadata for Migration '9' as its status is 'Created'. This operation can only be called on migrations with a status of Validated, CreatingLHMetadata or CreatedLHMetadata.", result.Details[0]);
        }

        /// <summary>
        /// BeginCreateMetadata creates a queue message for each resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task BeginCreateMetadata_CreatesQueueMessageForEachInputRecord()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByMigrationIdAsync(9))
                .Returns(Task.FromResult(new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        Id = 1,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    },
                    new MigrationInputRecord
                    {
                        Id = 2,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    },
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.Validated,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Initial call to update migration status.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.CreatingLHMetadata)))
                .Returns(Task.CompletedTask);

            // Calls to add messages to queue.
            this.mockQueueCommunicatorService.Setup(x => x.SendAsync(this.settings.MigrationTool.MetadataCreationQueueName, It.Is<MigrationInputRecordRequestModel>(y => y.UserId == 99 && y.MigrationInputRecordId == 1))).Returns(Task.CompletedTask);
            this.mockQueueCommunicatorService.Setup(x => x.SendAsync(this.settings.MigrationTool.MetadataCreationQueueName, It.Is<MigrationInputRecordRequestModel>(y => y.UserId == 99 && y.MigrationInputRecordId == 2))).Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.BeginCreateMetadata(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            this.mockMigrationRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Migration>()), Times.Once());
            Assert.True(result.IsValid);
            Assert.Empty(result.Details);
        }

        /// <summary>
        /// CheckStatusOfCreateMetadata returns error if migration id doesn't exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CheckStatusOfCreateMetadata_ReturnsErrorIfMigrationIdDoesntExist()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(null));

            // Act
            var result = await this.migrationService.CheckStatusOfCreateMetadata(9);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.Equal(0, result.SuccessCount);
            Assert.Equal(0, result.ErrorCount);
            Assert.Equal("Migration '9' was not found in the database.", result.FundamentalError);
        }

        /// <summary>
        /// CheckStatusOfCreateMetadata returns correct counts for success, error, not yet processed and total.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CheckStatusOfCreateMetadata_ReturnsCorrectCounts()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.Validated,
                    AzureMigrationContainerName = "migration-container-name",
                    MigrationInputRecords = new List<MigrationInputRecord>
                    {
                        new MigrationInputRecord()
                        {
                            Id = 1,
                            MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                            Data = "rec1",
                        },
                    },
                }));

            this.mockMigrationInputRecordRepository.Setup(x => x.GetByMigrationIdAsync(9))
                .Returns(Task.FromResult(new List<MigrationInputRecord>
                {
                    new MigrationInputRecord() { Id = 1, MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete },
                    new MigrationInputRecord() { Id = 2, MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationComplete },
                    new MigrationInputRecord() { Id = 3, MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationComplete },
                    new MigrationInputRecord() { Id = 4, MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationFailed },
                    new MigrationInputRecord() { Id = 5, MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationFailed },
                    new MigrationInputRecord() { Id = 6, MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationFailed },
                    new MigrationInputRecord() { Id = 7, MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationFailed },
                }));

            // Act
            var result = await this.migrationService.CheckStatusOfCreateMetadata(9);

            // Assert
            Assert.Equal(1, result.NotYetProcessedCount);
            Assert.Equal(2, result.SuccessCount);
            Assert.Equal(3, result.ErrorCount);
            Assert.Equal(6, result.TotalCount);
            Assert.Null(result.FundamentalError);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord returns error if input record doesn't exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_ReturnsErrorIfInputRecordDoesntExist()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<MigrationInputRecord>(null));

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Migration input record '9' was not found in the database.", result.Details[0]);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord returns error if migration doesn't have the correct status.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_ReturnsErrorIfMigrationDoesntHaveCorrectStatus()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 123,
                    MigrationStatusEnum = MigrationStatusEnum.Validated, // Invalid if not CreatingLHMetadata.
                }));

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Cannot create resource metadata for MigrationInputRecord '9' as the Migration it belongs to (123) does not have the status CreatingLHMetadata.", result.Details[0]);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord returns error if migration input record doesn't have the correct status.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_ReturnsErrorIfMigrationInputRecordDoesntHaveCorrectStatus()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationFailed, // Invalid if not ValidationComplete.
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 123,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                }));

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
            Assert.Equal("Migration input record '9' has incorrect status for metadata creation. Current status is 'ValidationFailed'.", result.Details[0]);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord updates the migration status if all records have finished.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_UpdatesMigrationStatusIfAllRecordsFinished()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    Data = "rec1",
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Call to mapper for input record - an article with no attachments.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Article,
                    ResourceFileUrls = new List<string>(),
                });

            // Call to create resource.
            this.mockMigrationRepository.Setup(x => x.CreateResourceAsync(It.IsAny<ResourceParamsModel>(), It.Is<List<ResourceFileParamsModel>>(y => y.Count == 0)))
                .Returns(Task.FromResult<int>(123));

            // Resultset for checking whether the migration has completed for all records.
            var inputsRecordsAfterMigration = new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        MigrationId = 9,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationComplete,
                    },
                };
            this.mockMigrationInputRecordRepository.Setup(x => x.GetAll())
                .Returns(inputsRecordsAfterMigration.AsQueryable());

            // Call to update migration status because all input records were complete.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.CreatedLHMetadata))) // ALL RECORDS COMPLETE.
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord doesn't update the migration status if all records have finished.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_DoesntUpdatesMigrationStatusIfSomeRecordsNotProcessedYet()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    Data = "rec1",
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Call to mapper for input record - an article with no attachments.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Article,
                    ResourceFileUrls = new List<string>(),
                });

            // Call to create resource.
            this.mockMigrationRepository.Setup(x => x.CreateResourceAsync(It.IsAny<ResourceParamsModel>(), It.Is<List<ResourceFileParamsModel>>(y => y.Count == 0)))
                .Returns(Task.FromResult<int>(123));

            // Resultset for checking whether the migration has completed for all records.
            var inputsRecordsAfterMigration = new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        MigrationId = 9,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete, // ALL RECORDS NOT COMPLETE.
                    },
                };
            this.mockMigrationInputRecordRepository.Setup(x => x.GetAll())
                .Returns(inputsRecordsAfterMigration.AsQueryable());

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord calls the AzureMediaService if it needs to create a video resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_CallsAzureMediaServiceForVideo()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    Data = "rec1",
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Call to mapper for input record - a video.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Video,
                    ResourceFileUrls = new List<string> { "public://video.mp4" },
                });

            // Call to azure media service to copy resource from migration blob to azure media services.
            this.mockAzureMediaService.Setup(x => x.CreateMediaInputAssetFromBlob("video.mp4", "migration-connection-string", "migration-container-name", "video.mp4"))
                .Returns(Task.FromResult(new BlobCopyResult() { Name = "video file name", FileSizeKb = 111 }));

            // Call to create resource.
            this.mockMigrationRepository.Setup(x => x.CreateResourceAsync(It.IsAny<ResourceParamsModel>(), It.Is<List<ResourceFileParamsModel>>(y =>
                    y.Count == 1 &&
                    y.ElementAt(0).FileName == "video.mp4" &&
                    y.ElementAt(0).FileSizeKb == 111 &&
                    y.ElementAt(0).FileTypeId == 1 &&
                    y.ElementAt(0).FilePath == "video file name")))
                .Returns(Task.FromResult<int>(123));

            // FileType
            this.mockFileTypeService.Setup(x => x.GetByFilename(It.IsAny<string>()))
                .Returns(Task.FromResult(new FileType() { Id = 1 }));

            // Resultset for checking whether the migration has completed for all records.
            var inputsRecordsAfterMigration = new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        MigrationId = 9,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    },
                };
            this.mockMigrationInputRecordRepository.Setup(x => x.GetAll())
                .Returns(inputsRecordsAfterMigration.AsQueryable());

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord calls the AzureMediaService if it needs to create an audio resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_CallsAzureMediaServiceForAudio()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    Data = "rec1",
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Call to mapper for input record - a video.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Audio,
                    ResourceFileUrls = new List<string> { "public://audio.mp3" },
                });

            // Call to azure media service to copy resource from migration blob to azure media services.
            this.mockAzureMediaService.Setup(x => x.CreateMediaInputAssetFromBlob("audio.mp3", "migration-connection-string", "migration-container-name", "audio.mp3"))
                .Returns(Task.FromResult(new BlobCopyResult() { Name = "audio file name", FileSizeKb = 111 }));

            // Call to create resource.
            this.mockMigrationRepository.Setup(x => x.CreateResourceAsync(It.IsAny<ResourceParamsModel>(), It.Is<List<ResourceFileParamsModel>>(y =>
                    y.Count == 1 &&
                    y.ElementAt(0).FileName == "audio.mp3" &&
                    y.ElementAt(0).FileSizeKb == 111 &&
                    y.ElementAt(0).FileTypeId == 1 &&
                    y.ElementAt(0).FilePath == "audio file name")))
                .Returns(Task.FromResult<int>(123));

            // FileType
            this.mockFileTypeService.Setup(x => x.GetByFilename(It.IsAny<string>()))
                .Returns(Task.FromResult(new FileType() { Id = 1 }));

            // Resultset for checking whether the migration has completed for all records.
            var inputsRecordsAfterMigration = new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        MigrationId = 9,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    },
                };
            this.mockMigrationInputRecordRepository.Setup(x => x.GetAll())
                .Returns(inputsRecordsAfterMigration.AsQueryable());

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord calls the AzureBlobService if it needs to create an image/generic file resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_CallsAzureBlobServiceForImages()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    Data = "rec1",
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Call to mapper for input record - a video.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Image,
                    ResourceFileUrls = new List<string> { "public://image.jpg" },
                });

            // Call to azure blob service to copy resource from migration blob to LH content blob container.
            this.mockAzureBlobService.Setup(x => x.CopyBlobToFileShare("migration-connection-string", "migration-container-name", "image.jpg", "lh-azure-connection-string", "lh-azure-share-name", It.IsAny<string>(), "image.jpg"))
                .Returns(Task.FromResult(222));

            // Call to create resource.
            this.mockMigrationRepository.Setup(x => x.CreateResourceAsync(It.IsAny<ResourceParamsModel>(), It.Is<List<ResourceFileParamsModel>>(y =>
                    y.Count == 1 &&
                    y.ElementAt(0).FileName == "image.jpg" &&
                    y.ElementAt(0).FileSizeKb == 222 &&
                    y.ElementAt(0).FileTypeId == 1 &&
                    y.ElementAt(0).FilePath.Length > 0)))
                .Returns(Task.FromResult<int>(123));

            // FileType
            this.mockFileTypeService.Setup(x => x.GetByFilename(It.IsAny<string>()))
                .Returns(Task.FromResult(new FileType() { Id = 1 }));

            // Resultset for checking whether the migration has completed for all records.
            var inputsRecordsAfterMigration = new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        MigrationId = 9,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    },
                };
            this.mockMigrationInputRecordRepository.Setup(x => x.GetAll())
                .Returns(inputsRecordsAfterMigration.AsQueryable());

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// CreateMetadataForSingleInputRecord calls the AzureBlobService for each article attachment.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMetadataForSingleInputRecord_CallsAzureBlobServiceForEachArticleAttachment()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new MigrationInputRecord
                {
                    MigrationId = 123,
                    MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    Data = "rec1",
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Call to mapper for input record - article with multiple attachments.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Article,
                    ResourceFileUrls = new List<string> { "public://image1.jpg", "public://image2.jpg", "public://image3.jpg" },
                });

            // Calls to azure blob service to copy resources from migration blob to LH content blob container.
            this.mockAzureBlobService.Setup(x => x.CopyBlobToFileShare("migration-connection-string", "migration-container-name", "image1.jpg", "lh-azure-connection-string", "lh-azure-share-name", It.IsAny<string>(), "image1.jpg"))
                .Returns(Task.FromResult(111));
            this.mockAzureBlobService.Setup(x => x.CopyBlobToFileShare("migration-connection-string", "migration-container-name", "image2.jpg", "lh-azure-connection-string", "lh-azure-share-name", It.IsAny<string>(), "image2.jpg"))
                .Returns(Task.FromResult(222));
            this.mockAzureBlobService.Setup(x => x.CopyBlobToFileShare("migration-connection-string", "migration-container-name", "image3.jpg", "lh-azure-connection-string", "lh-azure-share-name", It.IsAny<string>(), "image3.jpg"))
                .Returns(Task.FromResult(333));

            // Call to create resource.
            this.mockMigrationRepository.Setup(x => x.CreateResourceAsync(It.IsAny<ResourceParamsModel>(), It.Is<List<ResourceFileParamsModel>>(y =>
                    y.Count == 3 &&
                    y.ElementAt(0).FileName == "image1.jpg" &&
                    y.ElementAt(0).FileSizeKb == 111 &&
                    y.ElementAt(0).FileTypeId == 1 &&
                    y.ElementAt(0).FilePath.Length > 0 &&
                    y.ElementAt(1).FileName == "image2.jpg" &&
                    y.ElementAt(1).FileSizeKb == 222 &&
                    y.ElementAt(1).FileTypeId == 1 &&
                    y.ElementAt(1).FilePath.Length > 0 &&
                    y.ElementAt(2).FileName == "image3.jpg" &&
                    y.ElementAt(2).FileSizeKb == 333 &&
                    y.ElementAt(2).FileTypeId == 1 &&
                    y.ElementAt(2).FilePath.Length > 0)))
                .Returns(Task.FromResult<int>(123));

            // FileType
            this.mockFileTypeService.Setup(x => x.GetByFilename(It.IsAny<string>()))
                .Returns(Task.FromResult(new FileType() { Id = 1 }));

            // Resultset for checking whether the migration has completed for all records.
            var inputsRecordsAfterMigration = new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        MigrationId = 9,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationComplete,
                    },
                };
            this.mockMigrationInputRecordRepository.Setup(x => x.GetAll())
                .Returns(inputsRecordsAfterMigration.AsQueryable());

            // Act
            var result = await this.migrationService.CreateMetadataForSingleInputRecord(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.True(result.IsValid);
        }

        #endregion

        #region BeginPublishResources Tests

        /// <summary>
        /// BeginPublishResources returns error if migration id doesn't exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task BeginPublishResources_ReturnsErrorIfMigrationIdDoesntExist()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(null));

            // Act
            var result = await this.migrationService.BeginPublishResources(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.Equal("Migration '9' was not found in the database.", result.FundamentalError);
        }

        /// <summary>
        /// BeginPublishResources returns error if migration status is not correct.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task BeginPublishResources_ReturnsErrorIfMigrationStatusIsNotCorrect()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new Migration()
                {
                    Id = 9,
                    MigrationStatusEnum = MigrationStatusEnum.Created,
                }));

            // Act
            var result = await this.migrationService.BeginPublishResources(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.Equal("Cannot publish resources for Migration '9' as its status is 'Created'. This operation can only be called on migrations with a status of CreatedLHMetadata or PublishingLHResources.", result.FundamentalError);
        }

        /// <summary>
        /// BeginPublishResources creates a queue message for each resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task BeginPublishResources_ReturnsIDsOfAllMigrationInputRecords()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByMigrationIdAsync(9))
                .Returns(Task.FromResult(new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        Id = 1,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationComplete,
                        ResourceVersionId = 456,
                        Data = "rec1",
                    },
                    new MigrationInputRecord
                    {
                        Id = 2,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationComplete,
                        ResourceVersionId = 789,
                        Data = "rec2",
                    },
                    new MigrationInputRecord
                    {
                        Id = 3,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.ValidationFailed,
                        ResourceVersionId = 789,
                        Data = "rec3",
                    },
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.CreatedLHMetadata,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Initial call to update migration status.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.PublishingLHResources)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.BeginPublishResources(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            this.mockMigrationRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Migration>()), Times.Once());

            Assert.Null(result.FundamentalError);
            Assert.Equal(2, result.MigrationInputRecordIdsToPublish.Count());
            Assert.Equal(1, result.MigrationInputRecordIdsToPublish.First());
            Assert.Equal(2, result.MigrationInputRecordIdsToPublish.Last());
        }

        /// <summary>
        /// BeginPublishResources creates a queue message for each resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceForSingleInputRecord_ReturnsTrueForValidInputRecord()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(1))
                .Returns(Task.FromResult(
                    new MigrationInputRecord
                    {
                        Id = 1,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationComplete,
                        ResourceVersionId = 456,
                        Data = "rec1",
                    }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 123,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.PublishingLHResources,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Initial call to update migration status.
            this.mockMigrationRepository.Setup(x => x.UpdateAsync(99, It.Is<Migration>(y =>
                        y.Id == 9 &&
                        y.MigrationStatusEnum == MigrationStatusEnum.PublishingLHResources)))
                .Returns(Task.CompletedTask);

            // Call to input record mapper.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Image,
                    CreateDate = new DateTime(2020, 10, 13),
                });

            // Call to add message to queue.
            this.mockResourceService.Setup(x => x.SubmitResourceVersionForPublish(It.IsAny<PublishViewModel>()))
                .ReturnsAsync(new LearningHubValidationResult { IsValid = true });

            // Call to update input record status.
            this.mockMigrationInputRecordRepository.Setup(x => x.UpdateAsync(99, It.Is<MigrationInputRecord>(y =>
                        y.Id == 1 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHQueuedForPublish)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.PublishResourceForSingleInputRecord(1, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());

            Assert.True(result.IsValid);
        }

        /// <summary>
        /// BeginPublishResources returns errors if publish request fails.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishResourceForSingleInputRecord_ReturnsErrorIfPublishRequestFails()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByIdAsync(1))
                .Returns(Task.FromResult(
                    new MigrationInputRecord
                    {
                        Id = 1,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationComplete,
                        ResourceVersionId = 456,
                        Data = "rec1",
                    }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(123))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.PublishingLHResources,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Calls to input record mapper.
            this.mockInputRecordMapper.Setup(x => x.GetResourceParamsModel("rec1"))
                .Returns(new ResourceParamsModel()
                {
                    ResourceTypeId = (int)ResourceTypeEnum.Image,
                    CreateDate = new DateTime(2020, 10, 13),
                });

            // Calls to add messages to queue.
            this.mockResourceService.Setup(x => x.SubmitResourceVersionForPublish(It.IsAny<PublishViewModel>()))
                .ReturnsAsync(new LearningHubValidationResult { IsValid = false, Details = new List<string> { "Failed!" } });

            // Calls to update input record status.
            this.mockMigrationInputRecordRepository.Setup(x => x.UpdateAsync(99, It.Is<MigrationInputRecord>(y =>
                        y.Id == 1 &&
                        y.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHPublishFailed)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this.migrationService.PublishResourceForSingleInputRecord(1, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());

            Assert.False(result.IsValid);
            Assert.Equal("Failed to queue for publish: MigrationInputRecord '1' - ResourceVersion '456' - Failed!", result.Details.First());
        }

        #endregion

        #region CheckStatusOfPublishResources Tests

        /// <summary>
        /// CheckStatusOfPublishResources returns error if migration id doesn't exist.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CheckStatusOfPublishResources_ReturnsErrorIfMigrationIdDoesntExist()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(null));

            // Act
            var result = await this.migrationService.CheckStatusOfPublishResources(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.Equal("Migration '9' was not found in the database.", result.FundamentalError);
        }

        /// <summary>
        /// CheckStatusOfPublishResources returns error if migration status is not correct.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CheckStatusOfPublishResources_ReturnsErrorIfMigrationStatusIsNotCorrect()
        {
            // Arrange
            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult(new Migration()
                {
                    Id = 9,
                    MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata,
                }));

            // Act
            var result = await this.migrationService.CheckStatusOfPublishResources(9, 99);

            // Assert
            this.mockMigrationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.Equal("Migration '9' is not in the correct status for checking publish progress. Migration status is 'CreatingLHMetadata'.", result.FundamentalError);
        }

        /// <summary>
        /// CheckStatusOfPublishResources returns correct result counts of publish operation.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CheckStatusOfPublishResources_ReturnsCorrectResultCountsOfPublishOperation()
        {
            // Arrange
            this.mockMigrationInputRecordRepository.Setup(x => x.GetByMigrationIdAsync(9))
                .Returns(Task.FromResult(new List<MigrationInputRecord>
                {
                    new MigrationInputRecord
                    {
                        Id = 1,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHPublishComplete,
                        ResourceVersionId = 1,
                    },
                    new MigrationInputRecord
                    {
                        Id = 2,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHPublishFailed,
                        ExceptionDetails = "my exception details",
                        ResourceVersionId = 2,
                    },
                    new MigrationInputRecord
                    {
                        Id = 3,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHQueuedForPublish,
                        ResourceVersionId = 3,
                    },
                    new MigrationInputRecord
                    {
                        Id = 4,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHQueuedForPublish,
                        ResourceVersionId = 4,
                    },
                    new MigrationInputRecord
                    {
                        Id = 5,
                        MigrationId = 123,
                        MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHQueuedForPublish,
                        ResourceVersionId = 5,
                    },
                }));

            this.mockMigrationRepository.Setup(x => x.GetByIdAsync(9))
                .Returns(Task.FromResult<Migration>(new Migration()
                {
                    Id = 9,
                    MigrationSourceId = 999,
                    MigrationStatusEnum = MigrationStatusEnum.PublishingLHResources,
                    AzureMigrationContainerName = "migration-container-name",
                }));

            // Act
            var result = await this.migrationService.CheckStatusOfPublishResources(9, 99);

            // Assert
            Assert.Null(result.FundamentalError);
            Assert.Equal(3, result.QueuedForPublishCount);
            Assert.Equal(1, result.PublishedCount);
            Assert.Equal(1, result.PublishFailedCount);
            Assert.Single(result.Errors);
        }

        #endregion

        /// <summary>
        /// The get json data.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        private string GetJsonData()
        {
            // Return a JSON array containing 3 objects.
            return "[{\"Title\":\"My Resource 1\"}, {\"Title\":\"My Resource 2\"}, {\"Title\":\"My Resource 3\"}]";
        }
    }
}
