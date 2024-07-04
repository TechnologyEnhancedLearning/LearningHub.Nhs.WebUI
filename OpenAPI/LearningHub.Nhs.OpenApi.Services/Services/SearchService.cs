namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The search service.
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly ILearningHubService learningHubService;
        private readonly IResourceRepository resourceRepository;
        private readonly IFindwiseClient findwiseClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// The search service.
        /// </summary>
        /// <param name="learningHubService">
        /// The <see cref="ILearningHubService"/>.
        /// </param>
        /// <param name="findwiseClient">
        /// The <see cref="IFindwiseClient"/>.
        /// </param>
        /// <param name="resourceRepository">
        /// The <see cref="IResourceRepository"/>.
        /// </param>
        /// <param name="resourceService">
        /// The <see cref="IResourceService"/>.
        /// </param>
        /// <param name="logger">Logger.</param>
        public SearchService(
            ILearningHubService learningHubService,
            IFindwiseClient findwiseClient,
            IResourceRepository resourceRepository,
            IResourceService resourceService,
            ILogger<SearchService> logger)
        {
            this.learningHubService = learningHubService;
            this.findwiseClient = findwiseClient;
            this.resourceRepository = resourceRepository;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<ResourceSearchResultModel> Search(ResourceSearchRequest query, int? currentUserId)
        {
            
            var findwiseResultModel = await this.findwiseClient.Search(query);

            if (findwiseResultModel.FindwiseRequestStatus != FindwiseRequestStatus.Success)
            {
                return ResourceSearchResultModel.FailedWithStatus(findwiseResultModel.FindwiseRequestStatus);
            }

            List<ResourceMetadataViewModel> resourceMetadataViewModels = await this.GetResourceMetadataViewModels(findwiseResultModel, currentUserId);

            var totalHits = findwiseResultModel.SearchResults?.Stats.TotalHits;

            return new ResourceSearchResultModel(
                resourceMetadataViewModels,
                findwiseResultModel.FindwiseRequestStatus,
                totalHits ?? 0);
        }

        private async Task<List<ResourceMetadataViewModel>> GetResourceMetadataViewModels(
            FindwiseResultModel findwiseResultModel, int? currentUserId)
        {
            bool includeExternalResources = true;
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };

            var documentsFound = findwiseResultModel.SearchResults?.DocumentList.Documents?.ToList() ??
                                 new List<Document>();
            var findwiseResourceIds = documentsFound.Select(d => int.Parse(d.Id)).ToList();

            if (!findwiseResourceIds.Any())
            {
                return new List<ResourceMetadataViewModel>();
            }

            List<ResourceReferenceAndCatalogueDTO> resourcesReferenceAndCatalogueDTOsFound = (await this.resourceRepository.GetResourceReferenceAndCatalogues(findwiseResourceIds, new List<int>() { }, includeExternalResources)).ToList();


            if (currentUserId.HasValue)
            {
                List<int> resourceIds = resourcesReferenceAndCatalogueDTOsFound.Select(x => x.ResourceId).ToList();
                List<int> userIds = new List<int>() { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            List<ResourceMetadataViewModel> resourceMetadataViewModels = resourcesReferenceAndCatalogueDTOsFound.Select(resourcesReferenceAndCatalogueDTO => MapToViewModel(resourcesReferenceAndCatalogueDTO, resourceActivities.Where(x => x.ResourceId == resourcesReferenceAndCatalogueDTO.ResourceId).ToList()))
                .OrderBySequence(findwiseResourceIds)
                .ToList();

            List<int> unmatchedResources = findwiseResourceIds
                .Except(resourceMetadataViewModels.Select(r => r.ResourceId)).ToList();

            if (unmatchedResources.Any())
            {
                string? unmatchedResourcesIdsString = string.Join(", ", unmatchedResources);
                this.logger.LogWarning(
                    "Findwise returned documents that were not found in the database with IDs: " +
                    unmatchedResourcesIdsString);
            }

            return resourceMetadataViewModels;
        }

        /// <inheritdoc/>
        public ResourceMetadataViewModel MapToViewModel(ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO, List<ResourceActivityDTO> resourceActivities)
        {
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            if (resourceActivities != null && resourceActivities.Count != 0)
            {
                majorVersionIdActivityStatusDescription = ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resourceReferenceAndCatalogueDTO, resourceActivities)
                    .ToList();
            }

            bool hasRating = resourceReferenceAndCatalogueDTO.Rating != null;

            if (!hasRating)
            {
                this.logger.LogInformation(
                    $"Resource with id {resourceReferenceAndCatalogueDTO.ResourceId} is missing a ResourceVersionRatingSummary");
            }

            string resourceTypeNameOrEmpty = resourceReferenceAndCatalogueDTO.GetResourceTypeNameOrEmpty();
            if (resourceTypeNameOrEmpty == string.Empty)
            {
                this.logger.LogError($"Resource has unrecognised type: {resourceReferenceAndCatalogueDTO.ResourceTypeEnum}");
            }

            return new ResourceMetadataViewModel(
                resourceReferenceAndCatalogueDTO.ResourceId,
                resourceReferenceAndCatalogueDTO.Title,
                resourceReferenceAndCatalogueDTO.Description,
                resourceReferenceAndCatalogueDTO.CatalogueDTOs.Select(this.GetResourceReferenceViewModel).ToList(),
                resourceTypeNameOrEmpty,
                resourceReferenceAndCatalogueDTO.MajorVersion,
                resourceReferenceAndCatalogueDTO.Rating,
                majorVersionIdActivityStatusDescription);
        }

        private ResourceReferenceViewModel GetResourceReferenceViewModel(
        CatalogueDTO catalogueDTOs)
        {
            return new ResourceReferenceViewModel(
                catalogueDTOs.OriginalResourceReferenceId,
                catalogueDTOs.GetCatalogue(),
                this.learningHubService.GetResourceLaunchUrl(catalogueDTOs.OriginalResourceReferenceId));
        }
    }
}
