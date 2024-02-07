// <copyright file="PartialFileController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource.Files;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="PartialFileController" />.
    /// </summary>
    [Route("api")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class PartialFileController : ApiControllerBase
    {
        private readonly IPartialFileService partialFileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialFileController"/> class.
        /// </summary>
        /// <param name="partialFileService">
        /// The Partial File Service.
        /// </param>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="logger">The logger.</param>
        public PartialFileController(
            IPartialFileService partialFileService,
            IUserService userService,
            ILogger<PartialFileController> logger)
            : base(userService, logger)
        {
            this.partialFileService = partialFileService;
        }

        /// <summary>
        /// Create a new Partial File.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("partial-file")]
        public async Task<IActionResult> CreatePartialFile(PartialFileViewModel viewModel)
        {
            var response = await this.partialFileService.CreatePartialFile(viewModel, this.CurrentUserId);
            return this.Ok(response);
        }

        /// <summary>
        /// Get Partial File by File ID.
        /// </summary>
        /// <param name="fileId">The File ID.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("partial-file/{fileId}")]
        public async Task<ActionResult> GetPartialFile(int fileId)
        {
            File file = await this.partialFileService.GetFile(fileId);

            if (file.CreateUserId != this.CurrentUserId)
            {
                return this.Forbid();
            }

            var partialFile = await this.partialFileService.GetPartialFile(fileId);
            if (partialFile == null)
            {
                return this.NotFound();
            }

            var partialFileViewModel = new PartialFileViewModel
            {
                FileId = file.Id,
                FileName = file.FileName,
                FilePath = file.FilePath,
                TotalFileSize = partialFile.TotalSize,
            };
            return this.Ok(partialFileViewModel);
        }

        /// <summary>
        /// Sets the Partial File upload to be complete and queues any post-processing.
        /// </summary>
        /// <param name="fileId">The File ID.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("partial-file/{fileId}/complete")]
        public async Task<ActionResult> CompletePartialFile(int fileId)
        {
            File file = await this.partialFileService.GetFile(fileId);

            if (file.CreateUserId != this.CurrentUserId)
            {
                return this.Forbid();
            }

            var partialFile = await this.partialFileService.GetPartialFile(fileId);
            if (partialFile == null)
            {
                return this.NotFound();
            }

            await this.partialFileService.CompletePartialFile(file, partialFile, this.CurrentUserId);
            return this.NoContent();
        }
    }
}
