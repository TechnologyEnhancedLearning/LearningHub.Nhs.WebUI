// <copyright file="ToggleBookmarkViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    /// <summary>
    /// Defines the <see cref="ToggleBookmarkViewModel" />.
    /// </summary>
    public class ToggleBookmarkViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleBookmarkViewModel"/> class.
        /// </summary>
        /// <param name="isBookmarked">Is bookmarked.</param>
        /// <param name="bookmarkId">The bookmarkId.</param>
        /// <param name="title">The title.</param>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="showLabel">Show label.</param>
        public ToggleBookmarkViewModel(bool showLabel, bool isBookmarked, int bookmarkId, string title, int resourceReferenceId)
        {
            this.ShowLabel = showLabel;
            this.IsBookmarked = isBookmarked;
            this.BookmarkId = bookmarkId;
            this.Title = title;
            this.ResourceReferenceId = resourceReferenceId;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the "Add bookmark" and "Remove bookmark" labels.
        /// </summary>
        public bool ShowLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has bookmarked the resource.
        /// </summary>
        public bool IsBookmarked { get; set; }

        /// <summary>
        /// Gets or sets the bookmark Id.
        /// </summary>
        public int BookmarkId { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the ResourceReferenceId.
        /// </summary>
        public int ResourceReferenceId { get; set; }
    }
}
