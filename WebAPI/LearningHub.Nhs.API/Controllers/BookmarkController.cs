// <copyright file="BookmarkController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="BookmarkController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookmarkController : ApiControllerBase // ApiElfhControllerBase
    {
        /// <summary>
        /// Defines the bookmarkService.
        /// </summary>
        private readonly IBookmarkService bookmarkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkController"/> class.
        /// </summary>
        /// <param name="userService">userService.</param>
        /// <param name="bookmarkService">The bookmarkService<see cref="IBookmarkService"/>.</param>
        /// <param name="logger">The logger.</param>
        public BookmarkController(IUserService userService, IBookmarkService bookmarkService, ILogger<BookmarkController> logger)
            : base(userService, logger)
        {
            this.bookmarkService = bookmarkService;
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
            var bookmarkId = await this.bookmarkService.Toggle(this.CurrentUserId, bookmarkViewModel);
            return this.Ok(bookmarkId);
        }

        /// <summary>
        /// The GetAllByParent.
        /// </summary>
        /// <param name="parentId">The parentId.</param>
        /// <param name="all">The all.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetAllByParent/{parentId?}")]
        public async Task<IActionResult> GetAllByParent(int? parentId, bool? all = false)
        {
            var bookmarks = await this.bookmarkService.GetAllByParent(this.CurrentUserId, parentId, all);
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
            var bookmarkId = await this.bookmarkService.Create(this.CurrentUserId, bookmarkViewModel);
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
            var bookmarkId = await this.bookmarkService.Edit(this.CurrentUserId, bookmarkViewModel);
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
            await this.bookmarkService.DeleteFolder(bookmarkId, this.CurrentUserId);
            return this.Ok();
        }
    }
}
