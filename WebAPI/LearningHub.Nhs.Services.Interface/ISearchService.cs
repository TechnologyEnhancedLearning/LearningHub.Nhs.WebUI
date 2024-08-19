namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Search.SearchFeedback;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The Search Service interface.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// The get search result async.
        /// </summary>
        /// <param name="searchRequestModel">The catalog search request model.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<SearchResultModel> GetSearchResultAsync(SearchRequestModel searchRequestModel, int userId);

        /// <summary>
        /// The Get Catalogue Search Result Async method.
        /// </summary>
        /// <param name="catalogSearchRequestModel">
        /// The catalog search request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<SearchCatalogueResultModel> GetCatalogueSearchResultAsync(CatalogueSearchRequestModel catalogSearchRequestModel, int userId);

        /// <summary>
        /// The send resource for search Async method.
        /// </summary>
        /// <param name="searchResourceRequestModel">The search resource model.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="iterations">number of iterations.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> SendResourceForSearchAsync(SearchResourceRequestModel searchResourceRequestModel, int userId, int? iterations);

        /// <summary>
        /// The create resource search action async.
        /// </summary>
        /// <param name="searchActionResourceModel">
        /// The search action request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<LearningHubValidationResult> CreateResourceSearchActionAsync(SearchActionResourceModel searchActionResourceModel, int userId);

        /// <summary>
        /// The create catalogue search action async.
        /// </summary>
        /// <param name="searchActionCatalogueModel">
        /// The search action request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<LearningHubValidationResult> CreateCatalogueSearchActionAsync(SearchActionCatalogueModel searchActionCatalogueModel, int userId);

        /// <summary>
        /// The create catalogue resource search action async.
        /// </summary>
        /// <param name="searchActionResourceModel">
        /// The search action request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<LearningHubValidationResult> CreateCatalogueResourceSearchActionAsync(SearchActionResourceModel searchActionResourceModel, int userId);

        /// <summary>
        /// The submit feedback async.
        /// </summary>
        /// <param name="searchFeedbackModel">The search feedback.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> SubmitFeedbackAsync(SearchFeedBackModel searchFeedbackModel, int userId);

        /// <summary>
        /// The remove resource from search method.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task RemoveResourceFromSearchAsync(int resourceId);

        /// <summary>
        /// create search term event.
        /// </summary>
        /// <param name="searchRequestModel">search request.</param>
        /// <param name="userId">user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateSearchTermEvent(SearchRequestModel searchRequestModel, int userId);

        /// <summary>
        /// Create catalogue search term event.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">catalogue search request model.</param>
        /// <param name="userId">user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateCatalogueSearchTermEvent(CatalogueSearchRequestModel catalogueSearchRequestModel, int userId);

        /// <summary>
        /// The create resource search action async.
        /// </summary>
        /// <param name="searchActionResourceModel">
        /// The search action request model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> SendResourceSearchEventClickAsync(SearchActionResourceModel searchActionResourceModel);

        /////// <summary>
        /////// The create catalogue search action async.
        /////// </summary>
        /////// <param name="searchActionCatalogueModel">
        /////// The search action request model.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////Task RemoveResourceFromSearchAsync(int resourceId);

        /// <summary>
        /// Create catalogue search term.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">catalogue search request model.</param>
        /// <param name="resultsPerPage">results per page.</param>
        /// <param name="userId">user id.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<LearningHubValidationResult> CreateCatalogueSearchTerm(CatalogueSearchRequestModel catalogueSearchRequestModel, int resultsPerPage, int userId);

        /////// <summary>
        /////// The create resource search action async.
        /////// </summary>
        /////// <param name="searchActionResourceModel">
        /////// The search action request model.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////Task<bool> SendResourceSearchEventClickAsync(SearchActionResourceModel searchActionResourceModel);

        /// <summary>
        /// The create catalogue search action async.
        /// </summary>
        /// <param name="searchActionCatalogueModel">
        /// The search action request model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> SendCatalogueSearchEventAsync(SearchActionCatalogueModel searchActionCatalogueModel);

        /// <summary>
        /// The Auto suggetion result method.
        /// </summary>
        /// <param name="term">the term.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AutoSuggestionModel> GetAutoSuggestionResultsAsync(string term);
    }
}
