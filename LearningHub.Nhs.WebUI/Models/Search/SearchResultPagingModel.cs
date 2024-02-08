namespace LearningHub.Nhs.WebUI.Models.Search
{
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="SearchResultPagingModel" />.
    /// </summary>
    public class SearchResultPagingModel : PagingViewModel
    {
        /// <summary>
        /// Gets or sets the page previous action value.
        /// </summary>
        public int PreviousActionValue { get; set; }

        /// <summary>
        /// Gets or sets the page next action value.
        /// </summary>
        public int NextActionValue { get; set; }

        /// <summary>
        /// Gets or sets the page action name.
        /// </summary>
        public string ActionParamName { get; set; }
    }
}
