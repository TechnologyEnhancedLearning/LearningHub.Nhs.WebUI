namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Migration.Interface.Mapping;
    using LearningHub.Nhs.Migration.Interface.Validation;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Staging.Repository;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Migration;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Migrations;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The migration service.
    /// </summary>
    public class MigrationService : IMigrationService
    {
        /// <summary>
        /// The migration repository.
        /// </summary>
        private readonly IMigrationRepository migrationRepository;

        /// <summary>
        /// The migration input record repository.
        /// </summary>
        private readonly IMigrationInputRecordRepository migrationInputRecordRepository;

        /// <summary>
        /// The migration source repository.
        /// </summary>
        private readonly IMigrationSourceRepository migrationSourceRepository;

        /// <summary>
        /// The staging table input model repository.
        /// </summary>
        private readonly IStagingTableInputModelRepository stagingTableInputModelRepository;

        /// <summary>
        /// The node repository.
        /// </summary>
        private readonly INodeRepository nodeRepository;

        /// <summary>
        /// The resource version repository.
        /// </summary>
        private readonly IResourceVersionRepository resourceVersionRepository;

        /// <summary>
        /// The azure blob service.
        /// </summary>
        private readonly IAzureBlobService azureBlobService;

        /// <summary>
        /// The azure media service.
        /// </summary>
        private readonly IAzureMediaService azureMediaService;

        /// <summary>
        /// The azure data factory service.
        /// </summary>
        private readonly IAzureDataFactoryService azureDataFactoryService;

        /// <summary>
        /// The fileTypeService.
        /// </summary>
        private readonly IFileTypeService fileTypeService;

        /// <summary>
        /// The azure queue communicator service.
        /// </summary>
        private readonly IQueueCommunicatorService queueCommunicatorService;

        /// <summary>
        /// The resource service.
        /// </summary>
        private readonly IResourceService resourceService;

        /// <summary>
        /// The IInputRecordValidatorFactory.
        /// </summary>
        private readonly IInputRecordValidatorFactory inputRecordValidatorFactory;

        /// <summary>
        /// The IResourceCreatorFactory.
        /// </summary>
        private readonly IInputRecordMapperFactory inputRecordMapperFactory;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly IOptions<Settings> settings;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<MigrationService> logger;

        /// <summary>
        /// Defines the mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationService"/> class.
        /// </summary>
        /// <param name="migrationRepository">The migration repository.</param>
        /// <param name="migrationInputRecordRepository">The migration input record repository.</param>
        /// <param name="migrationSourceRepository">The migration source repository.</param>
        /// <param name="stagingTableInputModelRepository">The staging table input model repository.</param>
        /// <param name="nodeRepository">The node repository.</param>
        /// <param name="resourceVersionRepository">The resource version repository.</param>
        /// <param name="azureBlobService">The azure blob service.</param>
        /// <param name="azureMediaService">The azure media service.</param>
        /// <param name="azureDataFactoryService">The azure data factory service.</param>
        /// <param name="fileTypeService">The fileTypeService.</param>
        /// <param name="queueCommunicatorService">The queueCommunicatorService.</param>
        /// <param name="resourceService">The resourceService.</param>
        /// <param name="inputRecordValidatorFactory">The inputRecordValidatorFactory.</param>
        /// <param name="inputRecordMapperFactory">The inputRecordMapperFactory.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">mapper.</param>
        public MigrationService(
            IMigrationRepository migrationRepository,
            IMigrationInputRecordRepository migrationInputRecordRepository,
            IMigrationSourceRepository migrationSourceRepository,
            IStagingTableInputModelRepository stagingTableInputModelRepository,
            INodeRepository nodeRepository,
            IResourceVersionRepository resourceVersionRepository,
            IAzureBlobService azureBlobService,
            IAzureMediaService azureMediaService,
            IAzureDataFactoryService azureDataFactoryService,
            IFileTypeService fileTypeService,
            IQueueCommunicatorService queueCommunicatorService,
            IResourceService resourceService,
            IInputRecordValidatorFactory inputRecordValidatorFactory,
            IInputRecordMapperFactory inputRecordMapperFactory,
            IOptions<Settings> settings,
            ILogger<MigrationService> logger,
            IMapper mapper)
        {
            this.migrationRepository = migrationRepository;
            this.migrationInputRecordRepository = migrationInputRecordRepository;
            this.stagingTableInputModelRepository = stagingTableInputModelRepository;
            this.migrationSourceRepository = migrationSourceRepository;
            this.nodeRepository = nodeRepository;
            this.resourceVersionRepository = resourceVersionRepository;
            this.azureBlobService = azureBlobService;
            this.azureMediaService = azureMediaService;
            this.azureDataFactoryService = azureDataFactoryService;
            this.fileTypeService = fileTypeService;
            this.queueCommunicatorService = queueCommunicatorService;
            this.resourceService = resourceService;
            this.inputRecordValidatorFactory = inputRecordValidatorFactory;
            this.inputRecordMapperFactory = inputRecordMapperFactory;
            this.settings = settings;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// The GetMigrationSourcesAsync.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{MigrationSource}"/>.</returns>
        public async Task<IEnumerable<MigrationSourceViewModel>> GetMigrationSourcesAsync()
        {
            var migrationSources = await this.migrationSourceRepository.GetAll().ToListAsync();
            return this.mapper.Map<List<MigrationSourceViewModel>>(migrationSources ?? new List<MigrationSource>()).AsEnumerable();
        }

        /// <summary>
        /// Creates a new migration from an input json string.
        /// </summary>
        /// <param name="jsonData">The json data.</param>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <param name="migrationStagingBlobContainerName">The name of the azure blob container that contains the resource files for the migration.</param>
        /// <param name="destinationNodeId">The destination node id.</param>
        /// <param name="migratorUserId">The user id of the user performing the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateFromJsonString(string jsonData, int migrationSourceId, string migrationStagingBlobContainerName, int destinationNodeId, int migratorUserId)
        {
            this.logger.LogInformation($"Starting creation of new migration. migrationSourceId: {migrationSourceId}, migrationStagingBlobContainerName: {migrationStagingBlobContainerName}, destinationNodeId: {destinationNodeId}, userId: {migratorUserId}");

            var blobConnectionString = this.settings.Value.MigrationTool.AzureStorageAccountConnectionString;
            var metadataArchiveBlobContainerName = this.settings.Value.MigrationTool.AzureBlobContainerNameForMetadataFiles;

            // Check common setup parameters are valid.
            var paramCheckResult = await this.CheckCommonCreationParameters(migrationSourceId, migrationStagingBlobContainerName, metadataArchiveBlobContainerName);
            if (paramCheckResult != null)
            {
                return paramCheckResult;
            }

            // Check destination node id exists.
            string info;
            if (await this.nodeRepository.GetByIdAsync(destinationNodeId) == null)
            {
                info = $"Destination Node ID '{destinationNodeId}' does not exist in the database";
                this.logger.LogInformation(info);
                return new LearningHubValidationResult(false, info);
            }

            // Check input string is a valid JSON string.
            if (!this.IsJsonValid(jsonData, out string jsonValidationError))
            {
                info = $"Input metadata file does not contain correctly formatted JSON data. {jsonValidationError}";
                this.logger.LogInformation(info);
                return new LearningHubValidationResult(false, info);
            }

            // Upload json file to Azure.
            var migrationSource = await this.migrationSourceRepository.GetByIdAsync(migrationSourceId);
            var metadataFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{migrationSource.Id}-{migrationSource.Description}_{migrationStagingBlobContainerName}.json";
            var metadataFilePath = await this.azureBlobService.UploadBlobToContainer(blobConnectionString, metadataArchiveBlobContainerName, metadataFilename, Encoding.UTF8.GetBytes(jsonData));
            this.logger.LogInformation($"Migration input metadata file uploaded to Azure. URL: {metadataFilePath}");

            // Create new Migration Entity and save to DB.
            var migration = new Migration
            {
                MigrationSourceId = migrationSourceId,
                MigrationStatusEnum = Models.Enums.MigrationStatusEnum.Created,
                MetadataFileName = metadataFilename,
                MetadataFilePath = metadataFilePath,
                AzureMigrationContainerName = migrationStagingBlobContainerName,
                DestinationNodeId = destinationNodeId,
                DefaultEsrLinkTypeId = migrationSource.DefaultEsrLinkTypeId,
            };
            int migrationId = await this.migrationRepository.CreateAsync(migratorUserId, migration);
            this.logger.LogInformation($"New Migration created with ID {migrationId}");

            // Create new MigrationInputRecord entities and save to DB.
            JArray jsonObjects = JArray.Parse(jsonData);
            foreach (JToken jsonObject in jsonObjects)
            {
                var recordId = await this.migrationInputRecordRepository.CreateAsync(
                    migratorUserId,
                    new MigrationInputRecord { MigrationId = migrationId, Data = jsonObject.ToString() });

                this.logger.LogInformation($"New MigrationInputRecord created with ID {recordId}");
            }

            info = $"Finished migration creation successfully. {jsonObjects.Count} input records staged. MigrationId: {migrationId}";
            this.logger.LogInformation(info);
            var result = new LearningHubValidationResult(true, info)
            {
                CreatedId = migrationId,
            };

            return result;
        }

        /// <summary>
        /// Creates a new migration from metadata provided in an Excel template file and processed via the staging tables ADF pipeline.
        /// </summary>
        /// <param name="excelFileContent">The Excel metadata template file content.</param>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <param name="migrationAzureContainerName">The migration azure container name.</param>
        /// <param name="migratorUserId">The user id of the user performing the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateFromStagingTables(byte[] excelFileContent, int migrationSourceId, string migrationAzureContainerName, int migratorUserId)
        {
            this.logger.LogInformation($"Starting creation of new migration. migrationSourceId: {migrationSourceId}, migrationAzureContainerName: {migrationAzureContainerName}, userId: {migratorUserId}");

            // The Staging Tables migration has its own appsetting for the destination azure blob container for the input metadata file,
            // but it's likely to be set to the same value as the "MigrationTool.AzureBlobContainerNameForMetadataFiles" in reality. i.e. we'll store all input files in same container.
            var blobConnectionString = this.settings.Value.MigrationTool.AzureStorageAccountConnectionString;
            var stagingTablesAdfPipelineBlobContainerName = this.settings.Value.MigrationTool.StagingTables.AdfPipelineAzureBlobContainerName;

            // Check common setup parameters are valid.
            var paramCheckResult = await this.CheckCommonCreationParameters(migrationSourceId, migrationAzureContainerName, stagingTablesAdfPipelineBlobContainerName);
            if (paramCheckResult != null)
            {
                return paramCheckResult;
            }

            // Upload json file to Azure.
            var migrationSource = await this.migrationSourceRepository.GetByIdAsync(migrationSourceId);
            var metadataFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{migrationSource.Id}-{migrationSource.Description}_{migrationAzureContainerName}.xlsx";
            var metadataArchiveFilePath = await this.azureBlobService.UploadBlobToContainer(blobConnectionString, stagingTablesAdfPipelineBlobContainerName, metadataFilename, excelFileContent);
            this.logger.LogInformation($"Migration input metadata file uploaded to Azure. URL: {metadataArchiveFilePath}");

            // Trigger the ADF pipeline to read in the Excel template.
            try
            {
                this.logger.LogInformation($"Triggering the Staging Tables ADF pipeline.");
                await this.azureDataFactoryService.RunPipeline(
                    this.settings.Value.MigrationTool.StagingTables.AdfResourceGroup,
                    this.settings.Value.MigrationTool.StagingTables.AdfFactoryName,
                    this.settings.Value.MigrationTool.StagingTables.AdfPipelineName,
                    new Dictionary<string, object>
                    {
                        { "ExcelDirectory", stagingTablesAdfPipelineBlobContainerName },
                        { "ExcelFile", metadataFilename },
                    });
                this.logger.LogInformation($"Staging Tables ADF pipeline finished.");
            }
            catch (Exception ex)
            {
                string info = $"The attempt to run the staging tables ADF pipeline resulted in an error: {ex}";
                this.logger.LogInformation(info);
                return new LearningHubValidationResult(false, info);
            }

            // Check the staging tables contain data.
            var stagingTableInputModels = await this.stagingTableInputModelRepository.GetAllStagingTableInputModels();
            if (!stagingTableInputModels.Any())
            {
                string info = $"The staging tables did not contain any resource data.";
                this.logger.LogInformation(info);
                return new LearningHubValidationResult(false, info);
            }

            // Create new Migration Entity and save to DB.
            var migration = new Migration
            {
                MigrationSourceId = migrationSourceId,
                MigrationStatusEnum = Models.Enums.MigrationStatusEnum.Created,
                MetadataFileName = metadataFilename,
                MetadataFilePath = metadataArchiveFilePath,
                AzureMigrationContainerName = migrationAzureContainerName,
                DefaultEsrLinkTypeId = migrationSource.DefaultEsrLinkTypeId,
            };
            int migrationId = await this.migrationRepository.CreateAsync(migratorUserId, migration);
            this.logger.LogInformation($"New Migration created with ID {migrationId}");

            // Create new MigrationInputRecord entities and save to DB.
            foreach (StagingTableInputModel stagingTableInputModel in stagingTableInputModels)
            {
                // For articles, read the article body text from the external file in Azure, and save it into the input model.
                if (stagingTableInputModel.ResourceType.ToLower() == "article" && !string.IsNullOrEmpty(stagingTableInputModel.ArticleContentFilename))
                {
                    // Check if blob exists. Ignore it if it doesn't, validation stage will alert user to that.
                    if (await this.azureBlobService.GetBlobMetadata(this.settings.Value.MigrationTool.AzureStorageAccountConnectionString, migrationAzureContainerName, stagingTableInputModel.ArticleContentFilename) != null)
                    {
                        stagingTableInputModel.ArticleBodyText = await this.azureBlobService.DownloadBlobAsText(this.settings.Value.MigrationTool.AzureStorageAccountConnectionString, migrationAzureContainerName, stagingTableInputModel.ArticleContentFilename);
                    }
                }

                var recordId = await this.migrationInputRecordRepository.CreateAsync(
                    migratorUserId,
                    new MigrationInputRecord { MigrationId = migrationId, Data = JsonConvert.SerializeObject(stagingTableInputModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) });

                this.logger.LogInformation($"New MigrationInputRecord created with ID {recordId}");
            }

            string returnMsg = $"Finished migration creation successfully. {stagingTableInputModels.Count} input records staged. MigrationId: {migrationId}";
            this.logger.LogInformation(returnMsg);
            var result = new LearningHubValidationResult(true, returnMsg)
            {
                CreatedId = migrationId,
            };

            return result;
        }

        /// <summary>
        /// Validates an existing migration.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MigrationValidationResult> Validate(int migrationId, int migratorUserId)
        {
            this.logger.LogInformation($"Starting validation of migration '{migrationId}'");

            var migration = await this.migrationRepository.GetByIdAsync(migrationId);
            if (migration == null)
            {
                string error = $"Migration '{migrationId}' was not found in the database.";
                this.logger.LogError(error);
                return new MigrationValidationResult(false, error);
            }

            if (migration.MigrationStatusEnum != MigrationStatusEnum.Created &&
                migration.MigrationStatusEnum != MigrationStatusEnum.Validating &&
                migration.MigrationStatusEnum != MigrationStatusEnum.Validated)
            {
                string error = $"Migration '{migrationId}' has already been progressed onto the resource creation stage and cannot be validated again.";
                this.logger.LogError(error);
                return new MigrationValidationResult(false, error);
            }

            // Update status to Validating.
            if (migration.MigrationStatusEnum == MigrationStatusEnum.Created)
            {
                migration.MigrationStatusEnum = MigrationStatusEnum.Validating;
                await this.migrationRepository.UpdateAsync(migratorUserId, migration);
            }

            // Get the correct validator type for the migration source, then validate all of the input records not yet validated.
            var validator = this.inputRecordValidatorFactory.GetValidator(migration.MigrationSourceId);
            var migrationValidationResult = new MigrationValidationResult(true);

            var migrationInputRecords = await this.migrationInputRecordRepository.GetByMigrationIdAsync(migrationId);
            int i = 1; // for index
            foreach (MigrationInputRecord inputRecord in migrationInputRecords)
            {
                if (inputRecord.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationComplete)
                {
                    // For records already validated in a previous run, just populate the result object from the saved data.
                    migrationValidationResult.Add(this.GetResultForAlreadyValidatedRecord(inputRecord, i));
                }
                else
                {
                    // Otherwise validate the record and store the results against it in the database.
                    var inputRecordResult = await validator.ValidateAsync(inputRecord.Data, migration.AzureMigrationContainerName);
                    inputRecordResult.RecordIndex = i;
                    migrationValidationResult.Add(inputRecordResult);
                    inputRecord.MigrationInputRecordStatusEnum = inputRecordResult.IsValid ? MigrationInputRecordStatusEnum.ValidationComplete : MigrationInputRecordStatusEnum.ValidationFailed;
                    inputRecord.RecordReference = inputRecordResult.RecordReference ?? $"Record #{i}"; // If migration type doesn't have unique references in the data, use the record index. This is displayed back in the console app afterwards.
                    inputRecord.RecordTitle = inputRecordResult.RecordTitle;
                    inputRecord.ScormEsrLinkUrl = string.IsNullOrEmpty(inputRecordResult.ScormEsrLinkUrl) ? null : inputRecordResult.ScormEsrLinkUrl;

                    string warnings = null;
                    if (inputRecordResult.Warnings.Any())
                    {
                        warnings = string.Join(Environment.NewLine, inputRecordResult.Warnings);
                    }

                    inputRecord.ValidationWarnings = warnings;

                    string errors = null;
                    if (inputRecordResult.Errors.Any())
                    {
                        errors = string.Join(Environment.NewLine, inputRecordResult.Errors);
                    }

                    inputRecord.ValidationErrors = errors;

                    await this.migrationInputRecordRepository.UpdateAsync(migratorUserId, inputRecord);

                    string logText = this.GetValidationResultTextForLog(inputRecord, inputRecordResult, errors, warnings);
                    this.logger.LogInformation(logText);
                }

                i++;
            }

            // Update migration status to Validated.
            migration.MigrationStatusEnum = MigrationStatusEnum.Validated;
            await this.migrationRepository.UpdateAsync(migratorUserId, migration);
            this.logger.LogInformation($"Validation of migration {migrationId} complete. {migrationValidationResult.InputRecordValidationResults.SelectMany(x => x.Errors).Count()} Error(s) and {migrationValidationResult.InputRecordValidationResults.SelectMany(x => x.Warnings).Count()} Warnings found.");

            return migrationValidationResult;
        }

        /// <summary>
        /// Initiates the creation of LH draft resources for a migration. This method places the migration input record Ids onto a queue which is monitored by an Azure
        /// Function that does the actual processing. This is to avoid this method running for a long time and the Azure App Service timing out after 230 seconds. The timeout
        /// cannot be changed in the Azure App Service. The CheckCreateMetadataStatus method is used to poll for completion.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> BeginCreateMetadata(int migrationId, int migratorUserId)
        {
            this.logger.LogInformation($"Queueing migration input records for metadata creation for Migration  '{migrationId}'");

            var migration = await this.migrationRepository.GetByIdAsync(migrationId);
            if (migration == null)
            {
                string error = $"Migration '{migrationId}' was not found in the database.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            if (migration.MigrationStatusEnum != MigrationStatusEnum.Validated &&
                     migration.MigrationStatusEnum != MigrationStatusEnum.CreatingLHMetadata &&
                     migration.MigrationStatusEnum != MigrationStatusEnum.CreatedLHMetadata)
            {
                string error = $"Cannot create resource metadata for Migration '{migrationId}' as its status is '{migration.MigrationStatusEnum}'. This operation can only be called on migrations with a status of Validated, CreatingLHMetadata or CreatedLHMetadata.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            // Update status to CreatingLHMetadata.
            migration.MigrationStatusEnum = MigrationStatusEnum.CreatingLHMetadata;
            await this.migrationRepository.UpdateAsync(migratorUserId, migration);

            this.logger.LogInformation("Adding the migration input record ids to the queue...");

            // Put a message onto the Azure Function queue, one for each migration input record. Azure Function will loop through and call the CreateMetadataForSingleInputRecord method for each.
            var migrationInputRecords = await this.migrationInputRecordRepository.GetByMigrationIdAsync(migrationId);
            foreach (MigrationInputRecord inputRecord in migrationInputRecords
                .Where(x => x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationComplete ||
                            x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHMetadataCreationFailed))
            {
                try
                {
                    var queueMessage = new MigrationInputRecordRequestModel { MigrationInputRecordId = inputRecord.Id, UserId = migratorUserId };
                    await this.queueCommunicatorService.SendAsync(this.settings.Value.MigrationTool.MetadataCreationQueueName, queueMessage);
                    this.logger.LogInformation($"Added message to CreateMetadata queue for migration input record id '{inputRecord.Id}'");
                }
                catch (Exception ex)
                {
                    // Record exception details against input record if possible. The remaining records will continue to be processed.
                    try
                    {
                        inputRecord.ExceptionDetails = ex.ToString();
                        inputRecord.MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationFailed;
                        await this.migrationInputRecordRepository.UpdateAsync(migratorUserId, inputRecord);
                    }
                    catch (Exception)
                    {
                        // If recording the exception fails, throw the original error. No more records will be processed.
                        throw ex;
                    }
                }
            }

            this.logger.LogInformation("All valid migration input record ids have been been queued.");

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Creates the LH draft resource for a single migration input record. This method is called by the Azure Function which loops through all of the migration input
        /// record ids placed onto its queue. The whole process is kicked off by the BeginCreateMetadata method above.
        /// </summary>
        /// <param name="migrationInputRecordId">The migrationInputRecordId.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateMetadataForSingleInputRecord(int migrationInputRecordId, int migratorUserId)
        {
            this.logger.LogInformation($"Creating metadata for migration input record '{migrationInputRecordId}'");

            var migrationInputRecord = await this.migrationInputRecordRepository.GetByIdAsync(migrationInputRecordId);
            if (migrationInputRecord == null)
            {
                string error = $"Migration input record '{migrationInputRecordId}' was not found in the database.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            var migration = await this.migrationRepository.GetByIdAsync(migrationInputRecord.MigrationId);
            if (migration.MigrationStatusEnum != MigrationStatusEnum.CreatingLHMetadata)
            {
                string error = $"Cannot create resource metadata for MigrationInputRecord '{migrationInputRecordId}' as the Migration it belongs to ({migration.Id}) does not have the status CreatingLHMetadata.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            if (migrationInputRecord.MigrationInputRecordStatusEnum != MigrationInputRecordStatusEnum.ValidationComplete &&
                migrationInputRecord.MigrationInputRecordStatusEnum != MigrationInputRecordStatusEnum.LHMetadataCreationFailed)
            {
                string error = $"Migration input record '{migrationInputRecordId}' has incorrect status for metadata creation. Current status is '{migrationInputRecord.MigrationInputRecordStatusEnum}'.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            // Get the correct resource creator type for the migration source, then create the LH resource for the input record.
            var inputRecordMapper = this.inputRecordMapperFactory.GetMapper(migration.MigrationSourceId);

            try
            {
                ResourceParamsModel resourceParams = inputRecordMapper.GetResourceParamsModel(migrationInputRecord.Data);
                resourceParams.MigrationInputRecordId = migrationInputRecord.Id;

                // If destination node is set at migration level, need to set it in the resourceParams now.
                if (migration.DestinationNodeId.HasValue)
                {
                    resourceParams.DestinationNodeId = migration.DestinationNodeId.Value;
                }

                // If the input record does not supply its own ESRLinkTypeId, and the DefaultESRLinkTypeId is set at migration level, we need to set it in the resourceParams now.
                if (resourceParams.EsrLinkTypeId == 0 && migration.DefaultEsrLinkTypeId.HasValue)
                {
                    resourceParams.EsrLinkTypeId = migration.DefaultEsrLinkTypeId.Value;
                }

                var resourceFileParamsList = await this.ProcessResourceFiles(resourceParams, migration.AzureMigrationContainerName);
                int id = await this.migrationRepository.CreateResourceAsync(resourceParams, resourceFileParamsList);

                this.logger.LogInformation($"Resource Version {id} has been successfully created from migration input record {migrationInputRecord.Id}");
            }
            catch (Exception ex)
            {
                // Record exception details against input record if possible. The remaining records will continue to be processed.
                try
                {
                    migrationInputRecord.ExceptionDetails = ex.ToString();
                    migrationInputRecord.MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHMetadataCreationFailed;
                    await this.migrationInputRecordRepository.UpdateAsync(migratorUserId, migrationInputRecord);

                    return new LearningHubValidationResult(false, ex.Message);
                }
                catch (Exception)
                {
                    // If recording the exception fails, throw the original error. No more records will be processed.
                    throw ex;
                }
            }

            // Work out if this was the last input record waiting to be processed in this migration. If so, update overall migration status to suit.
            bool metadataCreationComplete = !this.migrationInputRecordRepository.GetAll().Any(x => x.MigrationId == migration.Id && x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationComplete);
            if (metadataCreationComplete)
            {
                migration.MigrationStatusEnum = MigrationStatusEnum.CreatedLHMetadata;
                await this.migrationRepository.UpdateAsync(migratorUserId, migration);
                this.logger.LogInformation($"Metadata creation for migration {migration.Id} complete.");
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Returns the results of a prior call to the BeginCreateMetadata method. This method is intended to be polled by the migration console app in order to track progress/completion.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MigrationResourceCreationResult> CheckStatusOfCreateMetadata(int migrationId)
        {
            var migration = await this.migrationRepository.GetByIdAsync(migrationId);
            if (migration == null)
            {
                string error = $"Migration '{migrationId}' was not found in the database.";
                this.logger.LogError(error);
                return new MigrationResourceCreationResult(error);
            }

            if (migration.MigrationStatusEnum != MigrationStatusEnum.CreatingLHMetadata || migration.MigrationStatusEnum != MigrationStatusEnum.CreatedLHMetadata)
            {
                return await this.CalculateCreateMetadataResult(migrationId);
            }
            else
            {
                string error = $"Metadata creation is not currently taking place for migration '{migrationId}'. Migration status is '{migration.MigrationStatusEnum}'.";
                return new MigrationResourceCreationResult(error);
            }
        }

        /// <summary>
        /// Begins the process of publishing the resources for an existing migration. This can be called after successful completion of CreateMetadata.
        /// This method updates the status of the migration and returns a list of MigrationInputRecord Ids that need to be published - i.e it doesn't actually publish anything!
        /// The PublishResourceForSingleInputRecord method should then be called for each MigrationInputRecord Id, and that publishes the resource. This used to all happen in a
        /// single web service call but has been split out to avoid timeout errors on large migrations.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MigrationBeginPublishResult> BeginPublishResources(int migrationId, int migratorUserId)
        {
            this.logger.LogInformation($"Starting to add resources to publish queue for migration '{migrationId}'");

            var migration = await this.migrationRepository.GetByIdAsync(migrationId);
            if (migration == null)
            {
                string error = $"Migration '{migrationId}' was not found in the database.";
                this.logger.LogError(error);
                return new MigrationBeginPublishResult(error);
            }

            if (migration.MigrationStatusEnum != MigrationStatusEnum.CreatedLHMetadata &&
                migration.MigrationStatusEnum != MigrationStatusEnum.PublishingLHResources)
            {
                string error = $"Cannot publish resources for Migration '{migrationId}' as its status is '{migration.MigrationStatusEnum}'. This operation can only be called on migrations with a status of CreatedLHMetadata or PublishingLHResources.";
                return new MigrationBeginPublishResult(error);
            }

            // Update status to PublishingLHResources.
            migration.MigrationStatusEnum = MigrationStatusEnum.PublishingLHResources;
            await this.migrationRepository.UpdateAsync(migratorUserId, migration);

            // Get a list of resources that need to be published.
            var migrationPublishResult = new MigrationPublishResult();
            var migrationInputRecords = await this.migrationInputRecordRepository.GetByMigrationIdAsync(migrationId);
            List<int> inputRecordsIdsToPublish = migrationInputRecords.Where(x =>
                            x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHMetadataCreationComplete ||
                            x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHQueuedForPublish ||
                            x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHPublishFailed).Select(x => x.Id).ToList();

            this.logger.LogInformation($"BeginPublishResources for migration '{migrationId}' has returned the IDs to be queued. Nothing has been queued yet. IDs returned: {string.Join(',', inputRecordsIdsToPublish)}");

            return new MigrationBeginPublishResult(inputRecordsIdsToPublish);
        }

        /// <summary>
        /// Publishes the resource for a single input record. This method needs to be called for each input record after the BeginPublishResources method has been called for the migration.
        /// This used to all happen in a single web service call but has been split out to avoid timeout errors on large migrations.
        /// </summary>
        /// <param name="migrationInputRecordId">The migration input record id.</param>
        /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> PublishResourceForSingleInputRecord(int migrationInputRecordId, int migratorUserId)
        {
            this.logger.LogInformation($"About to queue migration input record '{migrationInputRecordId}' for publishing");

            var migrationInputRecord = await this.migrationInputRecordRepository.GetByIdAsync(migrationInputRecordId);
            if (migrationInputRecord == null)
            {
                string error = $"Migration input record '{migrationInputRecordId}' was not found in the database.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            var migration = await this.migrationRepository.GetByIdAsync(migrationInputRecord.MigrationId);
            if (migration.MigrationStatusEnum != MigrationStatusEnum.PublishingLHResources)
            {
                string error = $"Cannot queue MigrationInputRecord '{migrationInputRecordId}' for publishing as the Migration it belongs to ({migration.Id}) does not have the status PublishingLHResources.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            if (migrationInputRecord.MigrationInputRecordStatusEnum != MigrationInputRecordStatusEnum.LHMetadataCreationComplete &&
                migrationInputRecord.MigrationInputRecordStatusEnum != MigrationInputRecordStatusEnum.LHQueuedForPublish &&
                migrationInputRecord.MigrationInputRecordStatusEnum != MigrationInputRecordStatusEnum.LHPublishFailed)
            {
                string error = $"Migration input record '{migrationInputRecordId}' has incorrect status for publishing. Current status is '{migrationInputRecord.MigrationInputRecordStatusEnum}'.";
                this.logger.LogError(error);
                return new LearningHubValidationResult(false, error);
            }

            // Get the correct resource creator type for the migration source, then create the LH resource for the input record.
            var inputRecordMapper = this.inputRecordMapperFactory.GetMapper(migration.MigrationSourceId);

            try
            {
                ResourceParamsModel resourceParams = inputRecordMapper.GetResourceParamsModel(migrationInputRecord.Data);

                // Resource needs to be published.
                var publishViewModel = new PublishViewModel
                {
                    IsMajorRevision = true, // check
                    Notes = "Publish of draft resource via Migration Tool.",
                    ResourceVersionId = migrationInputRecord.ResourceVersionId.Value,
                    PublicationDate = resourceParams.CreateDate,
                    UserId = resourceParams.UserId,
                    PublisherAction = PublisherActionEnum.Publish,
                };

                // Use publish pipeline to publish the resource.
                var result = await this.resourceService.SubmitResourceVersionForPublish(publishViewModel);

                if (result.IsValid)
                {
                    // Success returned from ResourceService.
                    this.logger.LogInformation($"Queued for publish: MigrationInputRecord '{migrationInputRecord.Id}' - ResourceVersion '{migrationInputRecord.ResourceVersionId}'");
                    migrationInputRecord.MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHQueuedForPublish;
                    migrationInputRecord.ExceptionDetails = string.Empty;
                    await this.migrationInputRecordRepository.UpdateAsync(migratorUserId, migrationInputRecord);
                    return new LearningHubValidationResult(true);
                }
                else
                {
                    // Failure returned from ResourceService.
                    var error = $"Failed to queue for publish: MigrationInputRecord '{migrationInputRecord.Id}' - ResourceVersion '{migrationInputRecord.ResourceVersionId}' - {string.Join(Environment.NewLine, result.Details)}";
                    this.logger.LogError(error);
                    migrationInputRecord.MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHPublishFailed;
                    migrationInputRecord.ExceptionDetails = error;
                    await this.migrationInputRecordRepository.UpdateAsync(migratorUserId, migrationInputRecord);
                    return new LearningHubValidationResult(false, error);
                }
            }
            catch (Exception ex)
            {
                // Unhandled exception occurred. Record exception details against input record if possible. The remaining records will continue to be processed.
                try
                {
                    migrationInputRecord.ExceptionDetails = ex.ToString();
                    migrationInputRecord.MigrationInputRecordStatusEnum = MigrationInputRecordStatusEnum.LHPublishFailed;
                    await this.migrationInputRecordRepository.UpdateAsync(migratorUserId, migrationInputRecord);

                    var error = $"Failed to queue for publish: MigrationInputRecord '{migrationInputRecord.Id}' - ResourceVersion '{migrationInputRecord.ResourceVersionId}' - Unhandled exception: {ex}";
                    this.logger.LogError(error);

                    return new LearningHubValidationResult(false, ex.Message);
                }
                catch (Exception)
                {
                    // If recording the exception fails, throw the original error. No more records will be processed.
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Returns the results of a prior call to the PublishResources method. This method is intended to be polled by the migration console app in order to track progress/completion.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// /// <param name="migratorUserId">The user id of the user peforming the migration.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MigrationPublishResult> CheckStatusOfPublishResources(int migrationId, int migratorUserId)
        {
            var migration = await this.migrationRepository.GetByIdAsync(migrationId);
            if (migration == null)
            {
                string error = $"Migration '{migrationId}' was not found in the database.";
                this.logger.LogError(error);
                return new MigrationPublishResult(error);
            }

            if (migration.MigrationStatusEnum == MigrationStatusEnum.PublishingLHResources || migration.MigrationStatusEnum == MigrationStatusEnum.PublishedLHResources)
            {
                var migrationPublishResult = new MigrationPublishResult();

                var migrationInputRecords = await this.migrationInputRecordRepository.GetByMigrationIdAsync(migrationId);
                foreach (MigrationInputRecord inputRecord in migrationInputRecords
                    .Where(x => x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHQueuedForPublish ||
                                x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHPublishComplete ||
                                x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHPublishFailed))
                {
                    switch (inputRecord.MigrationInputRecordStatusEnum)
                    {
                        case MigrationInputRecordStatusEnum.LHQueuedForPublish:
                            migrationPublishResult.QueuedForPublishCount++;
                            break;
                        case MigrationInputRecordStatusEnum.LHPublishComplete:
                            migrationPublishResult.PublishedCount++;
                            break;
                        case MigrationInputRecordStatusEnum.LHPublishFailed:
                            migrationPublishResult.PublishFailedCount++;
                            migrationPublishResult.AddError(inputRecord.Id, inputRecord.ExceptionDetails);
                            break;
                    }
                }

                return migrationPublishResult;
            }
            else
            {
                string error = $"Migration '{migrationId}' is not in the correct status for checking publish progress. Migration status is '{migration.MigrationStatusEnum}'.";
                return new MigrationPublishResult(error);
            }
        }

        /// <summary>
        /// Gets a list of ResourceVersionIds that were published as part of a migration. Intended to be used by the migration tool to unpublish each resource.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MigrationBeginPublishResult> GetPublishedResourceVersionIds(int migrationId)
        {
            this.logger.LogInformation($"Getting list of published resources for migration '{migrationId}'");

            var migration = await this.migrationRepository.GetByIdAsync(migrationId);
            if (migration == null)
            {
                string error = $"Migration '{migrationId}' was not found in the database.";
                this.logger.LogError(error);
                return new MigrationBeginPublishResult(error);
            }

            if (migration.MigrationStatusEnum != MigrationStatusEnum.PublishedLHResources)
            {
                string error = $"Cannot get list of published resources for Migration '{migrationId}' as its status is '{migration.MigrationStatusEnum}'. This operation can only be called on migrations with a status of PublishedLHResources.";
                return new MigrationBeginPublishResult(error);
            }

            // Get a list of ResourceVersionIds that have been published.
            var migrationPublishResult = new MigrationPublishResult();
            var migrationInputRecords = await this.migrationInputRecordRepository.GetByMigrationIdAsync(migrationId);
            List<int> inputRecordsIdsToPublish = migrationInputRecords.Where(x =>
                            x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHPublishComplete).Select(x => x.ResourceVersionId.Value).ToList();

            return new MigrationBeginPublishResult(inputRecordsIdsToPublish);
        }

        private async Task<LearningHubValidationResult> CheckCommonCreationParameters(int migrationSourceId, string migrationStagingBlobContainerName, string metadataBlobContainerName)
        {
            // Check migrationSourceId exists.
            var migrationSource = await this.migrationSourceRepository.GetByIdAsync(migrationSourceId);
            if (migrationSource == null)
            {
                string info = $"Migration Source ID '{migrationSourceId}' does not exist in the database";
                this.logger.LogInformation(info);
                return new LearningHubValidationResult(false, info);
            }

            // Check that the Azure migration staging (resource file) blob container path exists.
            if (!await this.azureBlobService.BlobContainerExists(this.settings.Value.MigrationTool.AzureStorageAccountConnectionString, migrationStagingBlobContainerName))
            {
                string info = $"Blob container '{migrationStagingBlobContainerName}' does not exist in the Migration Azure Storage Account";
                this.logger.LogInformation(info);
                return new LearningHubValidationResult(false, info);
            }

            // Check that the Azure migration metadata file blob container path exists.
            if (!await this.azureBlobService.BlobContainerExists(this.settings.Value.MigrationTool.AzureStorageAccountConnectionString, metadataBlobContainerName))
            {
                string info = $"Blob container '{metadataBlobContainerName}' does not exist in the Migration Azure Storage Account";
                this.logger.LogInformation(info);
                return new LearningHubValidationResult(false, info);
            }

            return null;
        }

        private async Task<List<ResourceFileParamsModel>> ProcessResourceFiles(ResourceParamsModel resourceParams, string azureMigrationContainerName)
        {
            var resourceFileParamsList = new List<ResourceFileParamsModel>();

            for (var i = 0; i < resourceParams.ResourceFileUrls.Count; i++)
            {
                var resourceFileUrl = resourceParams.ResourceFileUrls[i];
                if (resourceFileUrl.StartsWith("public://"))
                {
                    resourceFileUrl = resourceFileUrl.Substring(9);
                }

                var fileShareFileName = Path.GetFileName(resourceFileUrl);
                var fileShareDirectoryName = Guid.NewGuid().ToString();

                int fileSizeKb;
                string filePath;
                if (resourceParams.ResourceTypeId == (int)ResourceTypeEnum.Audio || resourceParams.ResourceTypeId == (int)ResourceTypeEnum.Video)
                {
                    var result = await this.azureMediaService.CreateMediaInputAssetFromBlob(
                        fileShareFileName,
                        this.settings.Value.MigrationTool.AzureStorageAccountConnectionString,
                        azureMigrationContainerName,
                        resourceFileUrl);

                    fileSizeKb = result.FileSizeKb;
                    filePath = result.Name;
                }
                else
                {
                    fileSizeKb = await this.azureBlobService.CopyBlobToFileShare(
                        this.settings.Value.MigrationTool.AzureStorageAccountConnectionString,
                        azureMigrationContainerName,
                        resourceFileUrl,
                        this.settings.Value.AzureFileStorageConnectionString,
                        this.settings.Value.AzureFileStorageResourceShareName,
                        fileShareDirectoryName,
                        fileShareFileName);

                    filePath = fileShareDirectoryName;
                }

                var fileType = await this.fileTypeService.GetByFilename(fileShareFileName);
                resourceFileParamsList.Add(new ResourceFileParamsModel
                {
                    FileTypeId = (fileType != null) ? fileType.Id : 0,
                    FileName = fileShareFileName,
                    FilePath = filePath,
                    FileSizeKb = fileSizeKb,
                });

                // If resource type is not article ignore any further resources. Add validation to prevent/warn about this scenario?
                if (resourceParams.ResourceTypeId != (int)ResourceTypeEnum.Article)
                {
                    break;
                }
            }

            return resourceFileParamsList;
        }

        private async Task<MigrationResourceCreationResult> CalculateCreateMetadataResult(int migrationId)
        {
            var result = new MigrationResourceCreationResult();
            var migrationInputRecords = await this.migrationInputRecordRepository.GetByMigrationIdAsync(migrationId);

            result.NotYetProcessedCount = migrationInputRecords.Where(x => x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationComplete).Count();
            result.SuccessCount = migrationInputRecords.Where(x => x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHMetadataCreationComplete).Count();

            var failedInputRecords = migrationInputRecords.Where(x => x.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.LHMetadataCreationFailed);
            result.ErrorCount = failedInputRecords.Count();
            foreach (var failedInputRecord in failedInputRecords)
            {
                result.AddError(failedInputRecord.Id, failedInputRecord.ExceptionDetails);
            }

            result.TotalCount = result.NotYetProcessedCount + result.ErrorCount + result.SuccessCount;

            return result;
        }

        /// <summary>
        /// The is json valid.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="validationError">The validation error.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool IsJsonValid(string input, out string validationError)
        {
            validationError = null;
            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || // For object
                (input.StartsWith("[") && input.EndsWith("]")) /* For array */)
            {
                try
                {
                    var obj = JToken.Parse(input);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    // JSON parsing exception
                    validationError = jex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    // Some other exception
                    validationError = ex.ToString();
                    return false;
                }
            }
            else
            {
                validationError = "The JSON string is invalid as it does not start and end with either square brackets or curly braces.";
                return false;
            }
        }

        /// <summary>
        /// The GetResultForAlreadyValidatedRecord.
        /// </summary>
        /// <param name="inputRecord">The inputRecord<see cref="MigrationInputRecord"/>.</param>
        /// <param name="recordIndex">The recordIndex<see cref="int"/>.</param>
        /// <returns>The <see cref="MigrationInputRecordValidationResult"/>.</returns>
        private MigrationInputRecordValidationResult GetResultForAlreadyValidatedRecord(MigrationInputRecord inputRecord, int recordIndex)
        {
            var validationResult = new MigrationInputRecordValidationResult()
            {
                IsValid = inputRecord.MigrationInputRecordStatusEnum == MigrationInputRecordStatusEnum.ValidationComplete,
                RecordIndex = recordIndex,
                RecordTitle = inputRecord.RecordTitle,
            };

            if (!string.IsNullOrEmpty(inputRecord.ValidationErrors))
            {
                // Need to ignore the line returns in exception messages when splitting the errors string into individual errors. Replace them with a placeholder and swap back after.
                inputRecord.ValidationErrors = inputRecord.ValidationErrors.Replace(Environment.NewLine + "   ", "[ExceptionLineReturn]");

                string[] errors = inputRecord.ValidationErrors.Split(Environment.NewLine);
                errors = errors.Select((error) => error = error.Replace("[ExceptionLineReturn]", Environment.NewLine + "   ")).ToArray();

                validationResult.Errors = errors.ToList();
            }

            if (!string.IsNullOrEmpty(inputRecord.ValidationWarnings))
            {
                validationResult.Warnings = inputRecord.ValidationWarnings.Split(Environment.NewLine).ToList();
            }

            return validationResult;
        }

        /// <summary>
        /// The GetValidationResultTextForLog.
        /// </summary>
        /// <param name="inputRecord">The inputRecord<see cref="MigrationInputRecord"/>.</param>
        /// <param name="validationResult">The validationResult<see cref="MigrationInputRecordValidationResult"/>.</param>
        /// <param name="errors">The errors<see cref="string"/>.</param>
        /// <param name="warnings">The warnings<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string GetValidationResultTextForLog(MigrationInputRecord inputRecord, MigrationInputRecordValidationResult validationResult, string errors, string warnings)
        {
            string text = $"Validated input record {inputRecord.Id} in migration {inputRecord.MigrationId}. Result: {(validationResult.IsValid ? "Valid" : "Invalid")} - {validationResult.Errors.Count} Error(s) and {validationResult.Warnings.Count} Warning(s).";

            if (validationResult.Errors.Any())
            {
                text += $"{Environment.NewLine}Errors:{Environment.NewLine}{errors}";
            }

            if (validationResult.Warnings.Any())
            {
                text += $"{Environment.NewLine}Warnings:{Environment.NewLine}{warnings}";
            }

            return text;
        }
    }
}
