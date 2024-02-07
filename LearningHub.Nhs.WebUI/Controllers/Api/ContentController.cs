// <copyright file="ContentController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The user controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IContentService contentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentController"/> class.
        /// </summary>
        /// <param name="contentService">contentService.</param>
        public ContentController(IContentService contentService)
        {
            this.contentService = contentService;
        }

        /// <summary>
        /// The GetPageById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="preview">The preview<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("page/{id}")]
        public async Task<IActionResult> GetPageByIdAsync(int id, [FromQuery] bool preview = false)
        {
            var pageViewModel = await this.contentService.GetPageByIdAsync(id, preview);
            return this.Ok(pageViewModel);
        }

        /// <summary>
        /// Get pageSection detail by id.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("page-section-detail-video/{id}")]
        public async Task<IActionResult> GetPageSectionDetailVideoAssetByIdAsync(int id)
        {
            var responseViewModel = await this.contentService.GetPageSectionDetailVideoAssetByIdAsync(id);
            return this.Ok(responseViewModel);
        }
    }
}