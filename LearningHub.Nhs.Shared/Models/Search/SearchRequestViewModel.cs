namespace LearningHub.Nhs.Shared.Models.Search
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc;//qqqq

    /// <summary>
    /// Defines the <see cref="SearchRequestViewModel" />.
    /// </summary>
    public class SearchRequestViewModel
    {
        /// <summary>
        /// Gets or sets the search string.
        /// </summary>
        [Required(ErrorMessage = "Search text is required")]
        [FromQuery]
        public string Term { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        [FromQuery]
        public IEnumerable<string> Filters { get; set; }

        /// <summary>
        /// Gets or sets the sort item index.
        /// </summary>
        [FromQuery]
        public int? Sortby { get; set; }

        /// <summary>
        /// Gets or sets the catalogue current page index.
        /// </summary>
        [FromQuery]
        public int? CataloguePageIndex { get; set; }

        /// <summary>
        /// Gets or sets the resource current page index.
        /// </summary>
        [FromQuery]
        public int? ResourcePageIndex { get; set; }

        /// <summary>
        /// Gets or sets the group id.
        /// </summary>
        [FromQuery]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the feedback submitted.
        /// </summary>
        [FromQuery]
        public bool? FeedbackSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the search id.
        /// </summary>
        [FromQuery]
        public int? SearchId { get; set; }

        /// <summary>
        /// Gets or sets the catalogue id, when searching within a particular catalogue.
        /// </summary>
        [FromQuery]
        public int? CatalogueId { get; set; }

        /// <summary>
        /// Gets or sets the resource access level id.
        /// </summary>
        [FromQuery]
        public int? ResourceAccessLevelId { get; set; }

        /// <summary>
        /// Gets or sets the provider ids.
        /// </summary>
        [FromQuery]
        public IEnumerable<string> ProviderFilters { get; set; }
    }
}