namespace LearningHub.Nhs.WebUI.Models.Bookmark
{
    using LearningHub.Nhs.Models.Bookmark;

    /// <summary>
    /// View model class for the _BookmarkItem partial view.
    /// </summary>
    public class BookmarkItemViewModel
    {
        /// <summary>
        /// Gets or sets bookmark.
        /// </summary>
        public UserBookmarkViewModel Bookmark { get; set; }

        /// <summary>
        /// Gets or sets bookmark action.
        /// </summary>
        public BookmarkActionViewModel Action { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsChild.
        /// </summary>
        public bool IsChild { get; set; }
    }
}