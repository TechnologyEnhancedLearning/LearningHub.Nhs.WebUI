// <copyright file="EditBookmarkViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Bookmark
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// EditBookmarkViewModel.
    /// </summary>
    public class EditBookmarkViewModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets BookmarkTypeId.
        /// </summary>
        public int BookmarkTypeId { get; set; }

        /// <summary>
        /// Gets or sets parent id.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets Link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "You must edit bookmark name")]
        [MinLength(2, ErrorMessage = "The bookmark name must be at least 2 characters")]
        [MaxLength(60, ErrorMessage = "The bookmark name must be no more than 60 characters")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Resource reference id.
        /// </summary>
        public int? Rri { get; set; }

        /// <summary>
        /// Gets or sets Node id.
        /// </summary>
        public int? NodeId { get; set; }

        /// <summary>
        /// Gets or sets Catalogue path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Bookmarked.
        /// </summary>
        public bool Bookmarked { get; set; }
    }
}