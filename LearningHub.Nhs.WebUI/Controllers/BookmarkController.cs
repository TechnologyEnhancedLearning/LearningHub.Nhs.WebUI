// <copyright file="BookmarkController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Bookmark;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The BookmarkController.
    /// </summary>
    [Authorize]
    [AutoValidateAntiforgeryToken]
    [ServiceFilter(typeof(LoginWizardFilter))]
    public partial class BookmarkController : BaseController
    {
        private readonly IBookmarkService bookmarkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="httpClientFactory">The httpClientFactory.</param>
        /// <param name="bookmarkService">bookmarkService.</param>
        public BookmarkController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<ResourceController> logger,
            IOptions<Settings> settings,
            IHttpClientFactory httpClientFactory,
            IBookmarkService bookmarkService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.bookmarkService = bookmarkService;
        }

        /// <summary>
        /// Resource bookmark actions.
        /// </summary>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("bookmark")]
        public async Task<IActionResult> Index(string returnUrl = "/")
        {
            this.ViewBag.ReturnUrl = returnUrl;
            this.ViewBag.BookmarksHelpUrl = this.Settings.SupportUrls.BookmarksHelpUrl;

            var bookmarks = await this.bookmarkService.GetAllByParent(null, true);

            return this.View("Index", bookmarks.ToList());
        }

        /// <summary>
        /// Add bookmark folder.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("bookmark/addfolder")]
        public IActionResult BookmarkAddFolder()
        {
            this.ViewBag.ReturnUrl = "/bookmark#bookmark-table";

            var bookmark = new EditBookmarkViewModel { BookmarkTypeId = 1 };

            return this.View("Toggle", bookmark);
        }

        /// <summary>
        /// Resource bookmark.
        /// </summary>
        /// <param name="id">Bookmark id.</param>
        /// <param name="parentId">Bookmark parent id.</param>
        /// <param name="action">Bookmark action.</param>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("bookmark")]
        public async Task<IActionResult> ModifyBookmark(int id, int? parentId, string action, string returnUrl = "/")
        {
            this.ViewBag.ReturnUrl = returnUrl;

            var bookmarks = await this.bookmarkService.GetAllByParent(parentId);
            var bookmark = bookmarks.FirstOrDefault(b => b.Id == id);
            if (bookmark != null)
            {
                var oldPosition = bookmark.Position;
                switch (action)
                {
                    case "rename":
                        var editBookmarkViewModel = new EditBookmarkViewModel()
                        {
                            Id = id,
                            BookmarkTypeId = bookmark.BookmarkTypeId,
                            Title = bookmark.Title,
                            ParentId = bookmark.ParentId,
                        };
                        return this.View("Rename", editBookmarkViewModel);
                    case "moveup":
                        var prevBookmark = bookmarks.Where(b => b.Position < bookmark.Position).OrderByDescending(d => d.Position).FirstOrDefault();
                        bookmark.Position -= 1;
                        await this.bookmarkService.Edit(bookmark);
                        if (prevBookmark != null)
                        {
                            prevBookmark.Position = oldPosition;
                            await this.bookmarkService.Edit(prevBookmark);
                        }

                        break;
                    case "movedown":
                        var nextBookmark = bookmarks.Where(b => b.Position > bookmark.Position).OrderBy(d => d.Position).FirstOrDefault();
                        bookmark.Position += 1;
                        await this.bookmarkService.Edit(bookmark);
                        if (nextBookmark != null)
                        {
                            nextBookmark.Position = oldPosition;
                            await this.bookmarkService.Edit(nextBookmark);
                        }

                        break;
                    case "delete":
                        return this.View("Toggle", new EditBookmarkViewModel()
                            {
                                Id = id,
                                Title = bookmark.Title,
                                BookmarkTypeId = bookmark.BookmarkTypeId,
                                Bookmarked = true,
                            });
                    case "move":
                        var moveBookmarkViewModel = new MoveBookmarkViewModel()
                        {
                            Bookmark = bookmark,
                            Folders = await this.GetFolders(bookmark),
                        };

                        return this.View("Move", moveBookmarkViewModel);
                }
            }

            return this.Redirect($"/bookmark?returnUrl={returnUrl}");
        }

        /// <summary>
        /// Rename bookmark.
        /// </summary>
        /// <param name="editBookmarkViewModel">The EditBookmarkViewModel.</param>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("bookmark/rename")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookmarkRename(EditBookmarkViewModel editBookmarkViewModel, string returnUrl = "/")
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View("Rename", editBookmarkViewModel);
            }

            var bookmarks = await this.bookmarkService.GetAllByParent(editBookmarkViewModel.ParentId);
            var bookmark = bookmarks.FirstOrDefault(b => b.Id == editBookmarkViewModel.Id);
            if (bookmark != null)
            {
                bookmark.Title = editBookmarkViewModel.Title;
                await this.bookmarkService.Edit(bookmark);
            }

            return this.Redirect($"/bookmark?returnUrl={returnUrl}");
        }

        /// <summary>
        /// Move bookmark.
        /// </summary>
        /// <param name="moveViewModel">The destination folder bookmark id.</param>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("bookmark/move")]
        public async Task<IActionResult> MoveBookmark(MoveBookmarkViewModel moveViewModel, string returnUrl = "/")
        {
            if (!this.ModelState.IsValid)
            {
                moveViewModel.Folders = await this.GetFolders(moveViewModel.Bookmark);
                return this.View("Move", moveViewModel);
            }

            if (moveViewModel.SelectedFolderId == 0)
            {
                // Zero means the user wants to create a new folder and move the bookmark into it.
                var newFolderId = await this.bookmarkService.Toggle(new UserBookmarkViewModel
                {
                    BookmarkTypeId = 1,
                    Title = moveViewModel.NewFolderName ?? "New folder",
                });

                moveViewModel.SelectedFolderId = newFolderId;
            }

            if (moveViewModel.SelectedFolderId == -1)
            {
                // -1 means user wants to move bookmark back to the top-level.
                moveViewModel.SelectedFolderId = null;
            }

            // Update bookmark record.
            var bookmarks = await this.bookmarkService.GetAllByParent(moveViewModel.Bookmark.ParentId);
            var bookmark = bookmarks.FirstOrDefault(b => b.Id == moveViewModel.Bookmark.Id);

            if (bookmark != null)
            {
                var folderBookmarks = await this.bookmarkService.GetAllByParent(moveViewModel.SelectedFolderId);

                bookmark.ParentId = moveViewModel.SelectedFolderId;
                bookmark.Position = folderBookmarks.Any() ? folderBookmarks.Max(f => f.Position) + 1 : 1;
                await this.bookmarkService.Edit(bookmark);
            }

            return this.Redirect($"/bookmark?returnUrl={returnUrl}");
        }

        /// <summary>
        /// Resource bookmark.
        /// </summary>
        /// <param name="bookmark">bookmark.</param>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("bookmark/resource")]
        public IActionResult BookmarkResource([FromQuery] EditBookmarkViewModel bookmark, string returnUrl = "/")
        {
            this.ViewBag.ReturnUrl = returnUrl;

            bookmark.BookmarkTypeId = 3;
            bookmark.Link = $"../Resource/{bookmark.Rri}/Item";

            return this.View("Toggle", bookmark);
        }

        /// <summary>
        /// Catalogue bookmark.
        /// </summary>
        /// <param name="bookmark">bookmark.</param>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("bookmark/catalogue")]
        public IActionResult BookmarkCatalogue([FromQuery] EditBookmarkViewModel bookmark, string returnUrl = "/")
        {
            this.ViewBag.ReturnUrl = returnUrl;

            bookmark.BookmarkTypeId = 2;
            bookmark.Link = $"../catalogue/{bookmark.Path}";

            return this.View("Toggle", bookmark);
        }

        /// <summary>
        /// Toggle bookmark.
        /// </summary>
        /// <param name="bookmark">Bookmark view model.</param>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("bookmark/toggle")]
        public async Task<IActionResult> BookmarkToggle(EditBookmarkViewModel bookmark, string returnUrl = "/")
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View("Toggle", bookmark);
            }

            if (bookmark.Bookmarked && bookmark.BookmarkTypeId == 1)
            {
                await this.bookmarkService.DeleteFolder(bookmark.Id.Value);
            }
            else
            {
                await this.bookmarkService.Toggle(new UserBookmarkViewModel
                {
                    Id = bookmark.Id ?? 0,
                    BookmarkTypeId = bookmark.BookmarkTypeId,
                    Title = bookmark.Title,
                    Link = bookmark.Link,
                    NodeId = bookmark.NodeId,
                    ResourceReferenceId = bookmark.Rri,
                });
            }

            return this.Redirect(returnUrl);
        }

        private async Task<Dictionary<int, string>> GetFolders(UserBookmarkViewModel bookmark)
        {
            var rootBookmarks = await this.bookmarkService.GetAllByParent(null);
            var folders = rootBookmarks.Where(m => m.BookmarkTypeId == 1 && m.Id != bookmark.ParentId).ToDictionary(t => t.Id, t => t.Title);
            return folders;
        }
    }
}