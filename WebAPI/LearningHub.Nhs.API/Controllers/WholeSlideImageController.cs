// <copyright file="WholeSlideImageController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource.Files;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="WholeSlideImageController" />.
    /// </summary>
    [Route("api")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class WholeSlideImageController : ApiControllerBase
    {
        private readonly IFileRepository fileRepository;
        private readonly IMapper mapper;
        private readonly IWholeSlideImageFileRepository wholeSlideImageFileRepository;
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeSlideImageController"/> class.
        /// </summary>
        /// <param name="fileRepository">
        /// The File Repository.
        /// </param>
        /// <param name="mapper">
        /// The Automapper.
        /// </param>
        /// <param name="wholeSlideImageFileRepository">
        /// The Whole Slide Image File Repository.
        /// </param>
        /// <param name="settings">
        /// The Settings.
        /// </param>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="logger">The logger.</param>
        public WholeSlideImageController(
            IFileRepository fileRepository,
            IMapper mapper,
            IWholeSlideImageFileRepository wholeSlideImageFileRepository,
            IOptions<Settings> settings,
            IUserService userService,
            ILogger<WholeSlideImageController> logger)
            : base(userService, logger)
        {
            this.fileRepository = fileRepository;
            this.mapper = mapper;
            this.wholeSlideImageFileRepository = wholeSlideImageFileRepository;
            this.settings = settings.Value;
        }

        /// <summary>
        /// Saves details about the whole slide image.
        /// </summary>
        /// <param name="fileId">The whole slide image's File ID.</param>
        /// <param name="viewModel">The whole slide image details.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("whole-slide-image/{fileId}/save-details")]
        public async Task<ActionResult> SaveWholeSlideImageDetails(int fileId, WholeSlideImageFileViewModel viewModel)
        {
            WholeSlideImageFile wholeSlideImageFile = await this.wholeSlideImageFileRepository.GetByFileIdAsync(fileId);
            if (wholeSlideImageFile == null)
            {
                return this.BadRequest(new ApiResponse(false));
            }

            this.mapper.Map(viewModel, wholeSlideImageFile);

            await this.wholeSlideImageFileRepository.UpdateAsync(this.CurrentUserId, wholeSlideImageFile);

            return this.Ok(new ApiResponse(true));
        }
    }
}
