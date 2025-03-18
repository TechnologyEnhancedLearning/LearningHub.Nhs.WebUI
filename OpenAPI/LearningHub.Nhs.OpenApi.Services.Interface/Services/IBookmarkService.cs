namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;

    /// <summary>
    /// The Bookmark service interface.
    /// </summary>
    public interface IBookmarkService
    {
        /// <summary>
        /// get all bookmarks.
        /// </summary>
        /// <param name="authHeader">The authentication header value.</param>
        /// <returns>IEnumerable BookmarkViewModel.</returns>
        Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(string authHeader);

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> Create(int currentUserId, UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The Toggle.
        /// </summary>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation..</returns>
        Task<int> Toggle(int currentUserId, UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> Edit(int currentUserId, UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// DeleteFolder.
        /// </summary>
        /// <param name="bookmarkId">bookmarkId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteFolder(int bookmarkId, int userId);
    }
}
