// <copyright file="BookmarkActionViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Bookmark
{
    using System.Collections.Generic;

    /// <summary>
    /// BookmarkActionViewModel.
    /// </summary>
    public class BookmarkActionViewModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets ParentId.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsFirst.
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsLast.
        /// </summary>
        public bool IsLast { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsLast.
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// Gets or sets Link.
        /// </summary>
        public Dictionary<int, string> Folders { get; set; }
    }
}