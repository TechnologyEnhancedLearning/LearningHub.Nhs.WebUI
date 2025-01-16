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
    }
}
