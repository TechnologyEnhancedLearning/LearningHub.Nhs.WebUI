// <copyright file="IBookmarkService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
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
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int> Toggle(UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The Create         .
        /// </summary>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The created record id.</returns>
        Task<int> Create(UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="bookmarkViewModel">The bookmarkViewModel<see cref="UserBookmarkViewModel"/>.</param>
        /// <returns>The edited record id.</returns>
        Task<int> Edit(UserBookmarkViewModel bookmarkViewModel);

        /// <summary>
        /// The GetAllByParent.
        /// </summary>
        /// <param name="parentId">The parentId.</param>
        /// <param name="all">The all.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(int? parentId, bool? all = false);

        /// <summary>
        /// DeleteFolder.
        /// </summary>
        /// <param name="bookmarkId">bookmarkId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteFolder(int bookmarkId);
    }
}
