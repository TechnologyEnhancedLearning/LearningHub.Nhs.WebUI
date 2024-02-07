// <copyright file="BookmarkController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The user controller.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        /// <summary>
        /// Defines the bookmarkService.
        /// </summary>
        private readonly IBookmarkService bookmarkService;
        private readonly IOptions<Settings> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkController"/> class.
        /// </summary>
        /// <param name="bookmarkService">bookmarkService.</param>
        /// <param name="settings">settings.</param>
        public BookmarkController(IBookmarkService bookmarkService, IOptions<Settings> settings)
        {
            this.bookmarkService = bookmarkService;
            this.settings = settings;
        }

        /// <summary>
        /// The Toggle.
        /// </summary>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut]
        [Route("toggle")]
        public async Task<IActionResult> Toggle([FromBody] UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkId = await this.bookmarkService.Toggle(bookmarkViewModel);
            return this.Ok(bookmarkId);
        }

        /// <summary>
        /// The DeleteFolder.
        /// </summary>
        /// <param name="bookmarkId">The bookmarkId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpDelete]
        [Route("deletefolder/{bookmarkId}")]
        public async Task<IActionResult> DeleteFolder(int bookmarkId)
        {
            await this.bookmarkService.DeleteFolder(bookmarkId);
            return this.Ok();
        }

        /// <summary>
        /// The GetAllByParent.
        /// </summary>
        /// <param name="parentId">The parentId.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetAllByParent/{parentId?}")]
        public async Task<IActionResult> GetAllByParent(int? parentId = null)
        {
            var bookmarks = await this.bookmarkService.GetAllByParent(parentId);
            return this.Ok(bookmarks);
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkId = await this.bookmarkService.Create(bookmarkViewModel);
            return this.Ok(bookmarkId);
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody] UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkId = await this.bookmarkService.Edit(bookmarkViewModel);
            return this.Ok(bookmarkId);
        }

        /// <summary>
        /// GetHelpUrl.
        /// </summary>
        /// <returns>BookmarksHelpUrl.</returns>
        [HttpGet]
        [Route("GetHelpUrl")]
        public IActionResult GetHelpUrl() => this.Ok(this.settings.Value.SupportUrls.BookmarksHelpUrl);
    }
}
