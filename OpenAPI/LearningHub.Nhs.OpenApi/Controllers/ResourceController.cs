namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Resource controller.
    /// </summary>
    [Route("Resource")]
    [Authorize]
    public class ResourceController : Controller
    {
        private const int MaxNumberOfReferenceIds = 1000;
        private readonly ISearchService searchService;
        private readonly FindwiseConfig findwiseConfig;
        private readonly IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        /// <param name="findwiseConfig">The findwise config.</param>
        /// <param name="resourceService">The resource service.</param>
        public ResourceController(ISearchService searchService, IResourceService resourceService, IOptions<FindwiseConfig> findwiseConfig)
        {
            this.searchService = searchService;
            this.findwiseConfig = findwiseConfig.Value;
            this.resourceService = resourceService;
        }

        /// <summary>
        /// GET Search.
        /// </summary>
        /// <param name="text">text.</param>
        /// <param name="offset">offset.</param>
        /// <param name="limit">limit.</param>
        /// <param name="catalogueId">catalogueId.</param>
        /// <param name="resourceTypes">resourceTypeIds.</param>
        /// <returns>None.</returns>
        [HttpGet("Search")]
        public async Task<ResourceSearchResultViewModel> Search(
            string text,
            int offset = 0,
            int? limit = null,
            int? catalogueId = null,
            [FromQuery] IEnumerable<string>? resourceTypes = null)
        {
            if (offset < 0)
            {
                throw new HttpResponseException("Offset must not be negative.", HttpStatusCode.BadRequest);
            }

            if (limit < 0)
            {
                throw new HttpResponseException("Limit must not be negative.", HttpStatusCode.BadRequest);
            }

            var resourceSearchResult =
                await this.searchService.Search(
                    new ResourceSearchRequest(
                        text,
                        offset,
                        limit ?? this.findwiseConfig.DefaultItemLimitForSearch,
                        catalogueId,
                        resourceTypes));

            switch (resourceSearchResult.FindwiseRequestStatus)
            {
                case FindwiseRequestStatus.Success:
                    return new ResourceSearchResultViewModel(resourceSearchResult, offset);

                case FindwiseRequestStatus.AccessDenied:
                case FindwiseRequestStatus.BadRequest:
                    throw new HttpResponseException(
                        "Search agent refused connection.",
                        HttpStatusCode.InternalServerError);

                case FindwiseRequestStatus.Timeout:
                    throw new HttpResponseException(
                        "Request to search agent timed out.",
                        HttpStatusCode.GatewayTimeout);

                case FindwiseRequestStatus.RequestException:
                    throw new HttpResponseException("Could not connect to search agent.", HttpStatusCode.BadGateway);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// GET by Id.
        /// </summary>
        /// <param name="originalResourceReferenceId">id.</param>
        /// <returns>Resource data.</returns>
        [HttpGet("{originalResourceReferenceId}")]
        public async Task<ResourceReferenceWithResourceDetailsViewModel> GetResourceReferenceByOriginalId(int originalResourceReferenceId)
        {
            return await this.resourceService.GetResourceReferenceByOriginalId(originalResourceReferenceId);
        }

        /// <summary>
        /// Bulk get by Ids.
        /// </summary>
        /// <param name="resourceReferenceIds">ids.</param>
        /// <returns>ResourceReferenceViewModels for matching resources.</returns>
        [HttpGet("Bulk")]
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds([FromQuery]List<int> resourceReferenceIds)
        {
            if (resourceReferenceIds.Count > MaxNumberOfReferenceIds)
            {
                throw new HttpResponseException($"Too many resources requested. The maximum is {MaxNumberOfReferenceIds}", HttpStatusCode.BadRequest);
            }

            return await this.resourceService.GetResourceReferencesByOriginalIds(resourceReferenceIds.ToList());
        }

        /// <summary>
        /// Bulk get by Ids.
        /// </summary>
        /// <param name="resourceReferences">ids.</param>
        /// <returns>ResourceReferenceViewModels for matching resources.</returns>
        [HttpGet("BulkJson")]
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIdsFromJson([FromQuery]string resourceReferences)
        {
            var bulkResourceReferences = JsonConvert.DeserializeObject<BulkResourceReferencesFromJsonRequestModel>(resourceReferences);

            if (bulkResourceReferences?.ResourceReferenceIds == null)
            {
                throw new HttpResponseException("Could not parse JSON to list of resource reference ids.", HttpStatusCode.BadRequest);
            }

            if (bulkResourceReferences.ResourceReferenceIds.Count > MaxNumberOfReferenceIds)
            {
                throw new HttpResponseException($"Too many resources requested. The maximum is {MaxNumberOfReferenceIds}", HttpStatusCode.BadRequest);
            }

            return await this.resourceService.GetResourceReferencesByOriginalIds(bulkResourceReferences.ResourceReferenceIds);
        }
    }
}
