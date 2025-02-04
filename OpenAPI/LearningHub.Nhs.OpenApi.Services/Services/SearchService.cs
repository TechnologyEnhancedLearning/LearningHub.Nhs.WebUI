namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using static System.Net.Mime.MediaTypeNames;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

    /// <summary>
    /// The search service.
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly ILearningHubService learningHubService;
        private readonly IResourceRepository resourceRepository;
        private readonly IFindwiseClient findwiseClient;
        private readonly ILogger logger;
        private readonly IResourceService resourceService;
        private readonly FindwiseConfig findwiseConfig;

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
        /// <param name="findwiseConfig">
        /// The <see cref="findwiseConfig"/>.
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
            IOptions<FindwiseConfig> findwiseConfig,
            IResourceRepository resourceRepository,
            IResourceService resourceService,
            ILogger<SearchService> logger)
        {
            this.learningHubService = learningHubService;
            this.findwiseClient = findwiseClient;
            this.resourceRepository = resourceRepository;
            this.resourceService = resourceService;
            this.logger = logger;
            this.findwiseConfig = findwiseConfig.Value;
        }

        /// <inheritdoc />
        public async Task<ResourceSearchResultModel> Search(ResourceSearchRequest query, int? currentUserId)
        {
            var findwiseResultModel = await this.findwiseClient.Search(query);

            if (findwiseResultModel.FindwiseRequestStatus != FindwiseRequestStatus.Success)
            {
                return ResourceSearchResultModel.FailedWithStatus(findwiseResultModel.FindwiseRequestStatus);
            }

            var resourceMetadataViewModels = await this.GetResourceMetadataViewModels(findwiseResultModel, currentUserId);

            var totalHits = findwiseResultModel.SearchResults?.Stats.TotalHits;

            return new ResourceSearchResultModel(
                resourceMetadataViewModels,
                findwiseResultModel.FindwiseRequestStatus,
                totalHits ?? 0);
        }

        /// <summary>
        /// The remove resource from search async method.
        /// </summary>
        /// <param name="resourceId">The resource to be removed from search.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RemoveResourceFromSearchAsync(int resourceId)
        {
            try
            {
                if (string.IsNullOrEmpty(this.findwiseConfig.IndexMethod))
                {
                    this.logger.LogWarning("The FindWiseIndexMethod is not configured. Resource not removed from search.");
                }
                else
                {
                    var client = await this.findwiseClient.GetClient(this.findwiseConfig.IndexUrl);

                    var request = string.Format(this.findwiseConfig.IndexMethod, this.findwiseConfig.CollectionIds.Resource) + $"?id={resourceId.ToString()}&token={this.findwiseConfig.Token}";

                    var response = await client.DeleteAsync(request).ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        throw new Exception("AccessDenied");
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Removal of resource to search failed: " + resourceId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Removal of resource from search failed: " + resourceId.ToString() + " : " + ex.Message);
            }
        }

        /// <summary>
        /// Gets AllCatalogue search results from findwise api call.
        /// </summary>
        /// <param name="catalogSearchRequestModel">The allcatalog search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<SearchAllCatalogueResultModel> GetAllCatalogueSearchResultsAsync(AllCatalogueSearchRequestModel catalogSearchRequestModel)
        {
            var viewmodel = new SearchAllCatalogueResultModel();
            try
            {
                var offset = catalogSearchRequestModel.PageIndex * catalogSearchRequestModel.PageSize;
                var client = await this.findwiseClient.GetClient(this.findwiseConfig.SearchBaseUrl);
                var request = string.Format(
                    this.findwiseConfig.SearchEndpointPath + "?offset={1}&hits={2}&q={3}&token={4}",
                    this.findwiseConfig.CollectionIds.Catalogue,
                    offset,
                    catalogSearchRequestModel.PageSize,
                    this.EncodeSearchText(catalogSearchRequestModel.SearchText),
                    this.findwiseConfig.Token);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<SearchAllCatalogueResultModel>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    this.logger.LogError($"Get AllCatalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}");
                    throw new Exception("AccessDenied to FindWise Server");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result.ToString();
                    this.logger.LogError($"Get AllCatalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
                    throw new Exception("Error with FindWise Server");
                }

                return viewmodel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string EncodeSearchText(string searchText)
        {
            string specialSearchCharacters = this.findwiseConfig.SpecialSearchCharacters;

            // Add backslash to the start of the string
            specialSearchCharacters = specialSearchCharacters.Replace(@"\", null);
            specialSearchCharacters = @"\" + specialSearchCharacters;

            for (int i = 0; i < specialSearchCharacters.Length; i++)
            {
                searchText = searchText.Replace(specialSearchCharacters[i].ToString(), @"\" + specialSearchCharacters[i]);
            }

            searchText = HttpUtility.UrlEncode(searchText);
            return searchText;
        }

        private async Task<List<ResourceMetadataViewModel>> GetResourceMetadataViewModels(
            FindwiseResultModel findwiseResultModel, int? currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<ResourceMetadataViewModel> resourceMetadataViewModels = new List<ResourceMetadataViewModel>() { };
            var documentsFound = findwiseResultModel.SearchResults?.DocumentList.Documents?.ToList() ??
                                 new List<Document>();
            var findwiseResourceIds = documentsFound.Select(d => int.Parse(d.Id)).ToList();

            if (!findwiseResourceIds.Any())
            {
                return new List<ResourceMetadataViewModel>();
            }

            var resourcesFound = await this.resourceRepository.GetResourcesFromIds(findwiseResourceIds);

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = resourcesFound.Select(x => x.Id).ToList();
                List<int> userIds = new List<int>() { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            resourceMetadataViewModels = resourcesFound.Select(resource => this.MapToViewModel(resource, resourceActivities.Where(x => x.ResourceId == resource.Id).ToList()))
                .OrderBySequence(findwiseResourceIds)
                .ToList();

            var unmatchedResources = findwiseResourceIds
                .Except(resourceMetadataViewModels.Select(r => r.ResourceId)).ToList();

            if (unmatchedResources.Any())
            {
                var unmatchedResourcesIdsString = string.Join(", ", unmatchedResources);
                this.logger.LogWarning(
                    "Findwise returned documents that were not found in the database with IDs: " +
                    unmatchedResourcesIdsString);
            }

            return resourceMetadataViewModels;
        }

        private ResourceMetadataViewModel MapToViewModel(Resource resource, List<ResourceActivityDTO> resourceActivities)
        {
            var hasCurrentResourceVersion = resource.CurrentResourceVersion != null;
            var hasRating = resource.CurrentResourceVersion?.ResourceVersionRatingSummary != null;

            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            if (resourceActivities != null && resourceActivities.Count != 0)
            {
                majorVersionIdActivityStatusDescription = ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resource, resourceActivities)
                    .ToList();
            }

            if (!hasCurrentResourceVersion)
            {
                this.logger.LogInformation(
                    $"Resource with id {resource.Id} is missing a current resource version");
            }

            if (!hasRating)
            {
                this.logger.LogInformation(
                    $"Resource with id {resource.Id} is missing a ResourceVersionRatingSummary");
            }

            var resourceTypeNameOrEmpty = resource.GetResourceTypeNameOrEmpty();
            if (resourceTypeNameOrEmpty == string.Empty)
            {
                this.logger.LogError($"Resource has unrecognised type: {resource.ResourceTypeEnum}");
            }


            return new ResourceMetadataViewModel(
                resource.Id,
                resource.CurrentResourceVersion?.Title ?? ResourceHelpers.NoResourceVersionText,
                resource.CurrentResourceVersion?.Description ?? string.Empty,
                resource.ResourceReference.Select(this.GetResourceReferenceViewModel).ToList(),
                resourceTypeNameOrEmpty,
                resource.CurrentResourceVersion?.MajorVersion ?? 0,
                resource.CurrentResourceVersion?.ResourceVersionRatingSummary?.AverageRating ?? 0.0m,
                majorVersionIdActivityStatusDescription
                );
        }

        private ResourceReferenceViewModel GetResourceReferenceViewModel(
            ResourceReference resourceReference)
        {
            return new ResourceReferenceViewModel(
                resourceReference.OriginalResourceReferenceId,
                resourceReference.GetCatalogue(),
                this.learningHubService.GetResourceLaunchUrl(resourceReference.OriginalResourceReferenceId));
        }
    }
}
