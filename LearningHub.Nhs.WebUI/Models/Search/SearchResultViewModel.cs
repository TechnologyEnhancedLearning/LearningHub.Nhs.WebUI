namespace LearningHub.Nhs.WebUI.Models.Search
{
    using System;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Search;

    /// <summary>
    /// Defines the <see cref="SearchResultViewModel" />.
    /// </summary>
    public class SearchResultViewModel
    {
        /// <summary>
        /// Gets or sets the resource search result.
        /// </summary>
        public SearchViewModel ResourceSearchResult { get; set; }

        /// <summary>
        /// Gets or sets the catalogue search result.
        /// </summary>
        public SearchCatalogueViewModel CatalogueSearchResult { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the hide restricted badge.
        /// </summary>
        public bool HideRestrictedBadge { get; set; }

        /// <summary>
        /// Gets or sets the search string.
        /// </summary>
        public string SearchString { get; set; }

        /// <summary>
        /// Gets or sets the sort item index.
        /// </summary>
        public int SelectedSortIndex { get; set; }

        /// <summary>
        /// Gets or sets the catalogue id.
        /// </summary>
        public int? CatalogueId { get; set; }

        /// <summary>
        /// Gets or sets the catalogue Url.
        /// </summary>
        public string CatalogueUrl { get; set; }

        /// <summary>
        /// Gets or sets the resource current page index.
        /// </summary>
        public int ResourceCurrentPageIndex { get; set; }

        /// <summary>
        /// Gets or sets the catalogue current page index.
        /// </summary>
        public int CatalogueCurrentPageIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the feedback submitted.
        /// </summary>
        public bool FeedbackSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the feedback.
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Gets or sets the Group Id.
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets the resource result paging.
        /// </summary>
        public PagingViewModel ResourceResultPaging { get; set; }

        /// <summary>
        /// Gets or sets the catalogue result paging.
        /// </summary>
        public PagingViewModel CatalogueResultPaging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Did You Mean Enabled or not.
        /// </summary>
        public bool DidYouMeanEnabled { get; set; }

        /// <summary>
        /// Gets or sets Suggested Catalogue name.
        /// </summary>
        public string SuggestedCatalogue { get; set; }

        /// <summary>
        /// Gets or sets Suggested Resource name.
        /// </summary>
        public string SuggestedResource { get; set; }
    }
}
