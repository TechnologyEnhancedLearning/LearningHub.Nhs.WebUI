namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
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
    public class ResourceController : OpenApiControllerBase
    {
        private const int MaxNumberOfReferenceIds = 1000;
        private readonly ISearchService searchService;
        private readonly FindwiseConfig findwiseConfig;
        private readonly IResourceService resourceService;
        private readonly IFileTypeService fileTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        /// <param name="findwiseConfig">The findwise config.</param>
        /// <param name="resourceService">The resource service.</param>
        public ResourceController(ISearchService searchService, IResourceService resourceService, IOptions<FindwiseConfig> findwiseConfig, IFileTypeService fileTypeService)
        {
            this.searchService = searchService;
            this.findwiseConfig = findwiseConfig.Value;
            this.resourceService = resourceService;
            this.fileTypeService = fileTypeService;
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
                        resourceTypes), this.CurrentUserId);

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
            return await this.resourceService.GetResourceReferenceByOriginalId(originalResourceReferenceId, this.CurrentUserId);
        }

        /// <summary>
        /// Bulk get by Ids.
        /// </summary>
        /// <param name="resourceReferenceIds">ids.</param>
        /// <returns>ResourceReferenceViewModels for matching resources.</returns>
        [HttpGet("Bulk")]
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds([FromQuery] List<int> resourceReferenceIds)
        {
            if (resourceReferenceIds.Count > MaxNumberOfReferenceIds)
            {
                throw new HttpResponseException($"Too many resources requested. The maximum is {MaxNumberOfReferenceIds}", HttpStatusCode.BadRequest);
            }

            return await this.resourceService.GetResourceReferencesByOriginalIds(resourceReferenceIds.ToList(), this.CurrentUserId);
        }

        /// <summary>
        /// Bulk get by Ids.
        /// </summary>
        /// <param name="resourceReferences">ids.</param>
        /// <returns>ResourceReferenceViewModels for matching resources.</returns>
        [HttpGet("BulkJson")]
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIdsFromJson([FromQuery] string resourceReferences)
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

            return await this.resourceService.GetResourceReferencesByOriginalIds(bulkResourceReferences.ResourceReferenceIds, this.CurrentUserId);
        }

        /// <summary>
        /// Get resourceReferences that have an in progress activity summary
        /// </summary>
        /// <param name="activityStatusId">activityStatusId.</param>
        /// <returns>ResourceReferenceViewModels for matching resources.</returns>
        [HttpGet("User/{activityStatusId}")]
        public async Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferencesByActivityStatus(int activityStatusId)
        {
            // These activity statuses are set with other activity statuses and resource type within the ActivityStatusHelper.GetActivityStatusDescription
            // Note In progress is in complete in the db
            List<int> activityStatusIdsNotInUseInDB = new List<int>() { (int)ActivityStatusEnum.Launched, (int)ActivityStatusEnum.InProgress, (int)ActivityStatusEnum.Viewed, (int)ActivityStatusEnum.Downloaded };
            if (this.CurrentUserId == null)
            {
                throw new UnauthorizedAccessException("User Id required.");
            }

            if (!Enum.IsDefined(typeof(ActivityStatusEnum), activityStatusId))
            {
                throw new ArgumentOutOfRangeException($"activityStatusId : {activityStatusId} does not exist within ActivityStatusEnum");
            }

            if (activityStatusIdsNotInUseInDB.Contains(activityStatusId))
            {
                throw new ArgumentOutOfRangeException($"activityStatusId: {activityStatusId} does not exist within the database definitions");
            }

            return await this.resourceService.GetResourceReferenceByActivityStatus(new List<int>() { activityStatusId }, this.CurrentUserId.Value);
        }

        /// <summary>
        /// Get resourceReferences that have certificates
        /// </summary>
        /// <returns>ResourceReferenceViewModels for matching resources.</returns>
        [HttpGet("User/Certificates")]
        public async Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferencesByCertificates()
        {
            if (this.CurrentUserId == null)
            {
                throw new UnauthorizedAccessException("User Id required.");
            }

            return await this.resourceService.GetResourceReferencesForCertificates(this.CurrentUserId.Value);
        }

        /// <summary>
        /// Get specific Resource by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            return this.Ok(await this.resourceService.GetResourceByIdAsync(id));
        }

        /// <summary>
        /// Get specific GenericFileDetails by ResourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetGenericFileDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetGenericFileDetailsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetGenericFileDetailsByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// Get specific Html resource details by ResourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetHtmlDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetHtmlDetailsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetHtmlDetailsByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// Get specific GetScormFileDetails by ResourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetScormDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetScormDetailsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetScormDetailsByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// The get web link resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetWeblinkDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetWebLinkResourceVersionAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetWebLinkDetailsByIdAsync(resourceVersionId));
        }


        /// <summary>
        /// The get case resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetCaseDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetCaseResourceVersionAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetCaseDetailsByIdAsync(resourceVersionId));
        }
        /// <summary>
        /// The GetFileStatusDetailsAsync.
        /// </summary>
        /// <param name="fileIds">The File Ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetFileStatusDetails")]
        public async Task<ActionResult> GetFileStatusDetailsAsync([FromQuery] int[] fileIds)
        {
            return this.Ok(await this.resourceService.GetFileStatusDetailsAsync(fileIds));
        }

        /// <summary>
        /// Get all file types.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetFileTypes")]
        public async Task<ActionResult> GetFileTypes()
        {
            return this.Ok(await this.fileTypeService.GetAllAsync());
        }

        /// <summary>
        /// The get file async.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetFile/{fileId}")]
        public async Task<IActionResult> GetFileAsync(int fileId)
        {
            var file = await this.resourceService.GetFileAsync(fileId);
            return this.Ok(file);
        }
    }
}
