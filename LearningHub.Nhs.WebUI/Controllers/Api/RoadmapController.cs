// <copyright file="RoadmapController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="RoadmapController" />.
    /// </summary>
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class RoadmapController : ControllerBase
    {
        private const string RoadmapImageDirectory = "RoadmapImage";
        private readonly IFileService fileService;
        private readonly IRoadMapService updatesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoadmapController"/> class.
        /// </summary>
        /// <param name="updatesService">Updates service.</param>
        /// <param name="fileService">File service.</param>
        public RoadmapController(
            IRoadMapService updatesService,
            IFileService fileService)
        {
            this.updatesService = updatesService;
            this.fileService = fileService;
        }

        /// <summary>
        /// The Image.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("roadmap/download-image/{fileName}")]
        public async Task<IActionResult> Image(string fileName)
        {
            var file = await this.fileService.DownloadFileAsync(RoadmapImageDirectory, fileName);
            if (file != null)
            {
                return this.File(file.Content, file.ContentType);
            }
            else
            {
                return this.Ok(this.Content("No image found"));
            }
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="numberOfResults">Number of results.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("roadmap/updates/{numberOfResults}")]
        public async Task<IActionResult> Updates(int numberOfResults)
        {
            var responseViewModel = await this.updatesService.GetUpdatesAsync(numberOfResults);
            return this.Ok(responseViewModel);
        }
    }
}
