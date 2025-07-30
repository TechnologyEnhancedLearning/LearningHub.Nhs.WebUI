namespace LearningHub.Nhs.Shared.Interfaces.Services
{
    using LearningHub.Nhs.Shared.Models.Search;
    using LearningHub.Nhs.Models.Search.SearchClick;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Search;

    /// <summary>
    /// Defines the <see cref="ISearchService" />.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Performs a search - either a combined resource and catalogue search, or just a resource search if
        /// searching within a catalogue.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="searchRequest">The SearchRequestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<SearchResultViewModel> PerformSearch(IPrincipal user, SearchRequestViewModel searchRequest);

        /// <summary>
        /// Records the analytics events associated with a search.
        /// </summary>
        /// <param name="search">The SearchRequestViewModel.</param>
        /// <param name="action">The SearchFormActionTypeEnum.</param>
        /// <param name="resourceCount">The resourceCount.</param>
        /// <param name="catalogueCount">The catalogueCount.</param>
        /// <returns>The event ID.</returns>
        Task<int> RegisterSearchEventsAsync(SearchRequestViewModel search, SearchFormActionTypeEnum action, int resourceCount = 0, int catalogueCount = 0);

        /// <summary>
        /// The CreateSearchActionAsync.
        /// </summary>
        /// <param name="searchActionResourceModel">Search action resource  model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> CreateResourceSearchActionAsync(SearchActionResourceModel searchActionResourceModel);

        /// <summary>
        /// The Create Catalogue Search Action.
        /// </summary>
        /// <param name="searchActionCatalogueModel">The search action catalogue model.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<int> CreateCatalogueSearchActionAsync(SearchActionCatalogueModel searchActionCatalogueModel);

        /// <summary>
        /// The GetSearchResultAsync.
        /// </summary>
        /// <param name="searchRequestModel">Search request model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<SearchViewModel> GetSearchResultAsync(SearchRequestModel searchRequestModel);

        /// <summary>
        /// The SubmitFeedbackAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SubmitFeedbackAsync(SearchFeedBackModel model);

        /// <summary>
        /// The Get Catalogue Search Result Async.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">The catalog search request model.</param>
        /// <returns>The <see cref="Task{SearchCatalogueViewModel}"/>.</returns>
        Task<SearchCatalogueViewModel> GetCatalogueSearchResultAsync(CatalogueSearchRequestModel catalogueSearchRequestModel);

        /// <summary>
        /// Create search term event.
        /// </summary>
        /// <param name="searchRequestModel">The searchRequestModel<see cref="SearchRequestModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        Task<int> CreateSearchTermEventAsync(SearchRequestModel searchRequestModel);

        /// <summary>
        /// Create catalogue search term event.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">catalogue search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> CreateCatalogueSearchTermEventAsync(CatalogueSearchRequestModel catalogueSearchRequestModel);

        /// <summary>
        /// Get AllCatalogue Search Result Async.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">The catalogue Search Request Model.</param>
        /// <returns>The <see cref="Task{SearchAllCatalogueViewModel}"/>.</returns>
        Task<SearchAllCatalogueViewModel> GetAllCatalogueSearchResultAsync(AllCatalogueSearchRequestModel catalogueSearchRequestModel);

        /// <summary>
        /// The Get AutoSuggestion List.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AutoSuggestionModel> GetAutoSuggestionList(string term);

        /// <summary>
        /// The Send AutoSuggestion Click Action Async.
        /// </summary>
        /// <param name="clickPayloadModel">The click Payload Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task SendAutoSuggestionClickActionAsync(AutoSuggestionClickPayloadModel clickPayloadModel);
    }
}
