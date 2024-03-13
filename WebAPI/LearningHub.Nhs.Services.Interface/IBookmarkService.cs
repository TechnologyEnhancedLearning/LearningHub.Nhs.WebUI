namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;

    /// <summary>
    /// Defines the <see cref="IBookmarkService" />.
    /// </summary>
    public interface IBookmarkService
    {
        /// <summary>
        /// The Toggle.
        /// </summary>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation..</returns>
        Task<int> Toggle(int currentUserId, UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> Create(int currentUserId, UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> Edit(int currentUserId, UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The GetAllByParent.
        /// </summary>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <param name="parentId">The parentId.</param>
        /// <param name="all">The all.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(int currentUserId, int? parentId, bool? all = false);

        /// <summary>
        /// DeleteFolder.
        /// </summary>
        /// <param name="bookmarkId">bookmarkId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteFolder(int bookmarkId, int userId);
    }
}
