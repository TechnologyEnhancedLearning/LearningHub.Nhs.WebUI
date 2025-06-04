namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Learning Hub Bookmark controller.
    /// </summary>
    [Route("Bookmark")]
    [ApiController]
    public class BookmarkController : OpenApiControllerBase
    {
        private readonly IBookmarkService bookmarkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkController"/> class.
        /// </summary>
        /// <param name="bookmarkService">The search service.</param>
        public BookmarkController(IBookmarkService bookmarkService)
        {
            this.bookmarkService = bookmarkService;
        }

        /// <summary>
        /// <summary>
        /// Gets all bookmarks by parent.
        /// </summary>
        /// <returns>Bookmarks.</returns>
        [HttpGet]
        [Route("GetAllByParent")]
        public async Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent()
        {
            return await this.bookmarkService.GetAllByParent(this.TokenWithoutBearer);
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
            var bookmarks = await this.bookmarkService.GetAllByParent(this.CurrentUserId.GetValueOrDefault(), parentId, all);
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
            var bookmarkId = await this.bookmarkService.Create(this.CurrentUserId.GetValueOrDefault(), bookmarkViewModel);
            return this.Ok(bookmarkId);
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
            var bookmarkId = await this.bookmarkService.Toggle(this.CurrentUserId.GetValueOrDefault(), bookmarkViewModel);
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
            var bookmarkId = await this.bookmarkService.Edit(this.CurrentUserId.GetValueOrDefault(), bookmarkViewModel);
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
            await this.bookmarkService.DeleteFolder(bookmarkId, this.CurrentUserId.GetValueOrDefault());
            return this.Ok();
        }
    }
}
