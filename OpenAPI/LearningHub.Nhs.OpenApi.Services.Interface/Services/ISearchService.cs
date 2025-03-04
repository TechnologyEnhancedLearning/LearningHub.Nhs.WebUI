namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// The Search Service interface.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// No info.
        /// </summary>
        /// <param name="query"><see cref="LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource.ResourceSearchRequest"/>.</param>
        /// <returns><see cref="ResourceSearchResultViewModel"/>.</returns>
        Task<ResourceSearchResultModel> Search(ResourceSearchRequest query, int? currentUserId);

        /// <summary>
        /// Gets AllCatalogue search results async.
        /// </summary>
        /// <param name="catalogSearchRequestModel">The allcatalog search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<SearchAllCatalogueResultModel> GetAllCatalogueSearchResultsAsync(AllCatalogueSearchRequestModel catalogSearchRequestModel);

        /// <summary>
        /// The Get Auto suggestion Results Async method.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AutoSuggestionModel> GetAutoSuggestionResultsAsync(string term);

        /// <summary>
        /// The remove resource from search method.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task RemoveResourceFromSearchAsync(int resourceId);
    }
}
