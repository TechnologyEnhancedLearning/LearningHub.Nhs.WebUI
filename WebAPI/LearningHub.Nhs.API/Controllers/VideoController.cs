// <copyright file="VideoController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource.Files;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="VideoController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class VideoController : ApiControllerBase
    {
        private readonly IMapper mapper;
        private readonly IVideoFileRepository videoFileRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoController"/> class.
        /// </summary>
        /// <param name="mapper">
        /// The Automapper.
        /// </param>
        /// <param name="videoFileRepository">
        /// The Video File Repository.
        /// </param>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="logger">The logger.</param>
        public VideoController(
            IMapper mapper,
            IVideoFileRepository videoFileRepository,
            IUserService userService,
            ILogger<VideoController> logger)
            : base(userService, logger)
        {
            this.mapper = mapper;
            this.videoFileRepository = videoFileRepository;
        }

        /// <summary>
        /// Get video file details by FileId (NOT by VideoFileId).
        /// This endpoint is used by the AzureFunctions video service to get the VideoFile.
        /// </summary>
        /// <param name="fileId">The video's FileId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{fileId}")]
        public async Task<VideoFileViewModel> GetVideoFileDetailsByFileId(int fileId)
        {
            var videoFile = await this.videoFileRepository.GetByFileIdAsync(fileId);
            return this.mapper.Map<VideoFileViewModel>(videoFile);
        }

        /// <summary>
        /// Saves details about the video.
        /// </summary>
        /// <param name="fileId">The video's File ID.</param>
        /// <param name="viewModel">The video details.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("{fileId}/save-details")]
        public async Task<ActionResult> SaveVideoDetails(int fileId, VideoFileViewModel viewModel)
        {
            VideoFile videoFile = await this.videoFileRepository.GetByFileIdAsync(fileId);
            if (videoFile == null)
            {
                return this.BadRequest(new ApiResponse(false));
            }

            this.mapper.Map(viewModel, videoFile);

            await this.videoFileRepository.UpdateAsync(this.CurrentUserId, videoFile);

            return this.Ok(new ApiResponse(true));
        }
    }
}
