// <copyright file="MigrationController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Migration;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The migration controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MigrationController : ApiControllerBase
    {
        /// <summary>
        /// The migration service..
        /// </summary>
        private readonly IMigrationService migrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationController"/> class.
        /// </summary>
        /// <param name="userService">The UserService<see cref="IUserService"/>.</param>
        /// <param name="migrationService">The migrationService<see cref="IMigrationService"/>.</param>
        /// <param name="logger">The logger.</param>
        public MigrationController(
            IUserService userService,
            IMigrationService migrationService,
            ILogger<MigrationController> logger)
            : base(userService, logger)
        {
            this.migrationService = migrationService;
        }

        /// <summary>
        /// The create from json file.
        /// </summary>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="migrationSourceId">The migrationSourceId<see cref="int"/>.</param>
        /// <param name="azureMigrationContainerName">The azureMigrationContainerName<see cref="string"/>.</param>
        /// <param name="destinationNodeId">The destinationNodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateFromJsonFile/{migrationSourceId}/{azureMigrationContainerName}/{destinationNodeId}")]
        [Authorize]
        public async Task<IActionResult> CreateFromJsonFile(IFormFile file, int migrationSourceId, string azureMigrationContainerName, int destinationNodeId)
        {
            if (file == null || file.Length == 0)
            {
                return this.BadRequest("Invalid file");
            }

            string jsonContent;
            using (MemoryStream ms = new MemoryStream())
            {
                await file.CopyToAsync(ms).ConfigureAwait(false);
                jsonContent = Encoding.UTF8.GetString(ms.ToArray());
            }

            var vr = await this.migrationService.CreateFromJsonString(jsonContent, migrationSourceId, azureMigrationContainerName, destinationNodeId, this.CurrentUserId);
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Creates a new migration from metadata provided in an Excel template file and processed via the staging tables ADF pipeline.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="migrationSourceId">The migration source id.</param>
        /// <param name="azureMigrationContainerName">The azure migration container name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("CreateFromStagingTables/{migrationSourceId}/{azureMigrationContainerName}")]
        [Authorize]
        public async Task<IActionResult> CreateFromStagingTables(IFormFile file, int migrationSourceId, string azureMigrationContainerName)
        {
            if (file == null || file.Length == 0)
            {
                return this.BadRequest("Invalid file");
            }

            byte[] fileContent;
            using (MemoryStream ms = new MemoryStream())
            {
                await file.CopyToAsync(ms).ConfigureAwait(false);
                fileContent = ms.ToArray();
            }

            var vr = await this.migrationService.CreateFromStagingTables(fileContent, migrationSourceId, azureMigrationContainerName, this.CurrentUserId);
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Validates an existing migration. This can be called after CreateFromJsonFile.
        /// </summary>
        /// <param name="migrationId">The migrationId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("Validate/{migrationId}")]
        [Authorize]
        public async Task<IActionResult> Validate(int migrationId)
        {
            var vr = await this.migrationService.Validate(migrationId, this.CurrentUserId);
            return this.Ok(vr);
        }

        /// <summary>
        /// Initiates the creation of the Learning Hub resources for an existing migration. This can be called after the Validate method.
        /// Puts messages onto a queue, which an Azure Function then picks up and performs the actual record creation.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("BeginCreateMetadata/{migrationId}")]
        [Authorize]
        public async Task<IActionResult> BeginCreateMetadata(int migrationId)
        {
            var vr = await this.migrationService.BeginCreateMetadata(migrationId, this.CurrentUserId);
            return this.Ok(new ApiResponse(vr.IsValid, vr));
        }

        /// <summary>
        /// Returns the current progress of a previous call to CreateMetadata.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("CheckStatusOfCreateMetadata/{migrationId}")]
        [Authorize]
        public async Task<IActionResult> CheckStatusOfCreateMetadata(int migrationId)
        {
            var vr = await this.migrationService.CheckStatusOfCreateMetadata(migrationId);
            return this.Ok(vr);
        }

        /// <summary>
        /// Initiates the creation of the Learning Hub resources for an existing migration. Puts messages onto a queue, which
        /// an Azure Function picks up and performs the actual record creation.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("CreateMetadataForSingleInputRecord")]
        [Authorize(Policy = "AuthorizeOrCallFromLH")]
        public async Task<IActionResult> CreateMetadataForSingleInputRecord([FromBody] MigrationInputRecordRequestModel requestModel)
        {
            var vr = await this.migrationService.CreateMetadataForSingleInputRecord(requestModel.MigrationInputRecordId, requestModel.UserId);
            return this.Ok(new ApiResponse(vr.IsValid, vr));
        }

        /// <summary>
        /// Publish the draft resources of a migration. This can be called after CreateMetadata has completed.
        /// This method results in messages being placed on an Azure queue for publishing afterwards. The UpdateStatusOfPublishResources method is called to
        /// update the status of the input records as resources are published by the LH publishing Azure function.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("BeginPublishResources/{migrationId}")]
        [Authorize]
        public async Task<IActionResult> BeginPublishResources(int migrationId)
        {
            var vr = await this.migrationService.BeginPublishResources(migrationId, this.CurrentUserId);
            return this.Ok(vr);
        }

        /// <summary>
        /// Publishes the resource for a single input record. This method needs to be called for each input record after the BeginPublishResources method has been called for the migration.
        /// This used to all happen in a single web service call but has been split out to avoid timeout errors on large migrations.
        /// </summary>
        /// <param name="migrationInputRecordId">The migration input record id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("PublishResourceForSingleInputRecord/{migrationInputRecordId}")]
        [Authorize]
        public async Task<IActionResult> PublishResourceForSingleInputRecord(int migrationInputRecordId)
        {
            var vr = await this.migrationService.PublishResourceForSingleInputRecord(migrationInputRecordId, this.CurrentUserId);
            return this.Ok(new ApiResponse(vr.IsValid, vr));
        }

        /// <summary>
        /// Checks the status of MigrationInputRecords following a previous call to PublishResources. The LH publishing Azure function picks up messages placed on a
        /// queue by the BeginPublishResources method and carries out the actual publish operations. This method checks the status of the ResourceVersions to see if they have been published yet.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("CheckStatusOfPublishResources/{migrationId}")]
        [Authorize]
        public async Task<IActionResult> CheckStatusOfPublishResources(int migrationId)
        {
            var vr = await this.migrationService.CheckStatusOfPublishResources(migrationId, this.CurrentUserId);
            return this.Ok(vr);
        }

        /// <summary>
        /// Gets a list of ResourceVersionIds that were published as part of a migration. Intended to be used by the migration tool to unpublish each resource.
        /// </summary>
        /// <param name="migrationId">The migration id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetPublishedResourceVersionIds/{migrationId}")]
        [Authorize]
        public async Task<IActionResult> GetPublishedResourceVersionIds(int migrationId)
        {
            var vr = await this.migrationService.GetPublishedResourceVersionIds(migrationId);
            return this.Ok(vr);
        }

        /// <summary>
        /// The GetMigrationSourcesAsync.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("get-migration-sources")]
        public async Task<IActionResult> GetMigrationSourcesAsync()
        {
            var result = await this.migrationService.GetMigrationSourcesAsync();

            return this.Ok(result);
        }
    }
}
