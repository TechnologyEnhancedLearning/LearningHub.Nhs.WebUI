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
    }
}
