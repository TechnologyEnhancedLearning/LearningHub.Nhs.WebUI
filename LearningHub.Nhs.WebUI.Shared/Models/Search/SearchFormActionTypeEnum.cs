namespace LearningHub.Nhs.WebUI.Shared.Models.Search
{
    /// <summary>
    /// Defines the SearchFormActionTypeEnum.
    /// </summary>
    public enum SearchFormActionTypeEnum
    {
        /// <summary>
        /// Defines the basic search.
        /// </summary>
        BasicSearch = 0,

        /// <summary>
        /// Defines the resource search previoous page change.
        /// </summary>
        ResourcePreviousPageChange = 1,

        /// <summary>
        /// Defines the resource search next page change.
        /// </summary>
        ResourceNextPageChange = 2,

        /// <summary>
        /// Defines the catalogue search previous page change
        /// </summary>
        CataloguePreviousPageChange = 3,

        /// <summary>
        /// Defines the catalogue search next page change
        /// </summary>
        CatalogueNextPageChange = 4,

        /// <summary>
        /// Defines the resource search apply filter.
        /// </summary>
        ApplyFilter = 5,

        /// <summary>
        /// Defines the submit feedback.
        /// </summary>
        SubmitFeedback = 6,

        /// <summary>
        /// Defines the search within catalogue search.
        /// </summary>
        SearchWithinCatalogue = 7,
    }
}
