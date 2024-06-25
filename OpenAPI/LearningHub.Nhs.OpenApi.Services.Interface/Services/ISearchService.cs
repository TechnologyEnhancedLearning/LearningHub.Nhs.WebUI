namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
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

        ResourceMetadataViewModel MapToViewModel(ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO, List<ResourceActivityDTO> resourceActivities);
    }
}
