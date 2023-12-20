// <copyright file="BookmarkFolderViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Bookmark
{
    using System.Collections.Generic;

    /// <summary>
    /// View model class for the _BookmarkFolder partial view.
    /// </summary>
    public class BookmarkFolderViewModel
    {
        /// <summary>
        /// Gets or sets the bookmark for the folder itself.
        /// </summary>
        public BookmarkItemViewModel FolderBookmark { get; set; }

        /// <summary>
        /// Gets or sets the child bookmarks contained in the folder.
        /// </summary>
        public List<BookmarkItemViewModel> ChildBookmarkItems { get; set; }
    }
}
