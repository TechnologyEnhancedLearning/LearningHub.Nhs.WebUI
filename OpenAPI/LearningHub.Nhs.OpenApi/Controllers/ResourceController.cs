namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.Validation;
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
        private readonly IActivityService activityService;
        private readonly IHierarchyService hierarchyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        /// <param name="findwiseConfig">The findwise config.</param>
        /// <param name="activityService">The activity service.</param>
        /// <param name="fileTypeService">The fileType service.</param>
        /// <param name="resourceService">The resource service.</param>
        /// <param name="hierarchyService">The hierachy service.</param>
        public ResourceController(ISearchService searchService, IResourceService resourceService, IHierarchyService hierarchyService, IOptions<FindwiseConfig> findwiseConfig, IActivityService activityService, IFileTypeService fileTypeService)
        {
            this.searchService = searchService;
            this.findwiseConfig = findwiseConfig.Value;
            this.resourceService = resourceService;
            this.fileTypeService = fileTypeService;
            this.activityService = activityService;
            this.hierarchyService = hierarchyService;
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
            return await this.resourceService.GetResourceReferenceByOriginalId(originalResourceReferenceId, this.CurrentUserId.GetValueOrDefault());
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

            return await this.resourceService.GetResourceReferencesByOriginalIds(resourceReferenceIds.ToList(), this.CurrentUserId.GetValueOrDefault());
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

            return await this.resourceService.GetResourceReferencesByOriginalIds(bulkResourceReferences.ResourceReferenceIds, this.CurrentUserId.GetValueOrDefault());
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
        [HttpGet("GetResource/{id}")]
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

        /// <summary>
        /// Get all resource licences.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceLicences")]
        public async Task<ActionResult> GetResourceLicences()
        {
            return this.Ok(await this.resourceService.GetResourceLicencesAsync());
        }

        /// <summary>
        /// Get specific ResourceVersionViewModel by Id.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetResourceVersionViewModel/{resourceVersionId}")]
        public async Task<ActionResult> GetResourceVersionViewModelAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionViewModelAsync(resourceVersionId));
        }

        /// <summary>
        /// Get specific resource version view model that is linked to a Video file from the file Id.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetResourceVersionForVideo/{fileId}")]
        public async Task<ActionResult> GetResourceVersionForVideo(int fileId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionForVideoAsync(fileId));
        }


        /// <summary>
        /// Get specific resource version view model that is linked to a whole slide image file from the file Id.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetResourceVersionForWholeSlideImage/{fileId}")]
        public async Task<ActionResult> GetResourceVersionForWholeSlideImage(int fileId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionForWholeSlideImageAsync(fileId));
        }

        /// <summary>
        /// Get specific ResourceVersionViewModel by Resource Reference Id.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("GetResourceVersionByResourceReference/{resourceReferenceId}")]
        public async Task<ActionResult> GetResourceVersionByResourceReferenceAsync(int resourceReferenceId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionByResourceReferenceAsync(resourceReferenceId));
        }

        /// <summary>
        /// Unpublish resource version.
        /// </summary>
        /// <param name="unpublishViewModel">The unpublishViewModel<see cref="UnpublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "ReadWrite")]
        [Route("UnpublishResourceVersion")]
        public async Task<IActionResult> UnpublishResourceVersionAsync(UnpublishViewModel unpublishViewModel)
        {
            var vr = await this.resourceService.UnpublishResourceVersion(unpublishViewModel, this.CurrentUserId.GetValueOrDefault());

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// Unpublish resource version.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "ReadWrite")]
        [Route("RevertToDraft")]
        public async Task<IActionResult> RevertToDraft([FromBody] int resourceVersionId)
        {
            var vr = await this.resourceService.RevertToDraft(resourceVersionId, this.CurrentUserId.GetValueOrDefault());

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The mark sensitive conatet async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("AcceptSensitiveContent")]
        public async Task<IActionResult> AcceptSensitiveContent([FromBody] int resourceVersionId)
        {
            var vr = await this.resourceService.AcceptSensitiveContentAsync(resourceVersionId, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// The get resource versions.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceVersions/{resourceId}")]
        public async Task<ActionResult> GetResourceVersionsAsync(int resourceId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionsAsync(resourceId));
        }

        /// <summary>
        /// Get specific VideoDetails by ResourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetVideoDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetVideoDetailsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetVideoDetailsByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// Get specific AudioDetails by ResourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetAudioDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetAudioDetailsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetAudioDetailsByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// Get specific Article Details by ResourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetArticleDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetArticleDetailsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetArticleDetailsByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// Add a new Resource Version Keyword
        ///   TODO - requires complete validation - same Keyword added > once.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="ResourceKeywordViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("AddResourceVersionKeyword")]
        public async Task<IActionResult> AddResourceVersionKeywordAsync(ResourceKeywordViewModel resourceKeywordViewModel)
        {
            var vr = await this.resourceService.AddResourceVersionKeywordAsync(resourceKeywordViewModel, this.CurrentUserId.GetValueOrDefault());
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }
        /// <summary>
        /// The get resource version validation result.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceVersionValidationResult/{resourceVersionId}")]
        public async Task<ActionResult> GetResourceVersionValidationResultAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionValidationResultAsync(resourceVersionId));
        }
        /// <summary>
        /// The get resource version flags.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceVersionFlags/{resourceVersionId}")]
        public async Task<ActionResult> GetResourceVersionFlagsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionFlagsAsync(resourceVersionId));
        }


        /// <summary>
        /// Retrieves the entire assessment details given a resource version ID.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetAssessmentDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetAssessmentResourceVersionAsync(int resourceVersionId)
        {
            var response = await this.resourceService.GetAssessmentDetailsByIdAsync(resourceVersionId, this.CurrentUserId.GetValueOrDefault());

            if (response == null)
            {
                var result = new LearningHubValidationResult();
                result.IsValid = false;
                result.Details = new List<string>(new string[] { "The resource does not exist or you do not have access privilege" });
                return this.BadRequest(new ApiResponse(false, result));
            }

            return this.Ok(response);
        }

        /// <summary>
        /// Retrieves the assessment details up to the first question, leaving out the feedback and answer types nullified.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetAssessmentContent/{resourceVersionId}")]
        public async Task<ActionResult> GetInitialAssessmentContent(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetInitialAssessmentContent(resourceVersionId));
        }

        /// <summary>
        /// Retrieves the assessment progress.
        /// </summary>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetAssessmentProgress/activity/{assessmentResourceActivityId}")]
        public async Task<ActionResult> GetAssessmentProgressByAssessmentResourceActivityId(int assessmentResourceActivityId)
        {
            AssessmentProgressViewModel response;
            var createUserId = this.activityService.GetUserIdForActivity(assessmentResourceActivityId);

            if (createUserId != this.CurrentUserId)
            {
                var result = new LearningHubValidationResult();
                result.IsValid = false;
                result.Details = new List<string>(new string[] { "Invalid request. The summary for the submitted activity does not belong to you." });
                return this.BadRequest(new ApiResponse(false, result));
            }

            int resourceVersionId =
                await this.activityService.GetAssessmentResourceIdByActivity(assessmentResourceActivityId);
            int attempts = this.activityService.GetAttempts(this.CurrentUserId.GetValueOrDefault(), resourceVersionId);
            var assessmentResourceActivitiesWithAnswers =
                await this.activityService.GetAnswersForAllTheAssessmentResourceActivities(this.CurrentUserId.GetValueOrDefault(), resourceVersionId);
            try
            {
                response = await this.resourceService.GetAssessmentProgress(resourceVersionId, attempts, assessmentResourceActivityId, assessmentResourceActivitiesWithAnswers);
            }
            catch
            {
                var result = new LearningHubValidationResult();
                result.IsValid = false;
                result.Details = new List<string>(new string[] { "Invalid request. Submitted answers are invalid." });
                return this.BadRequest(new ApiResponse(false, result));
            }

            return this.Ok(response);
        }

        /// <summary>
        /// Retrieves the latest assessment progress of a user for the given resource version id, or an empty response if no such attempt exists.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetAssessmentProgress/resource/{resourceVersionId}")]
        public async Task<ActionResult> GetAssessmentProgressByResourceVersionId(int resourceVersionId)
        {
            var assessmentResourceActivity = await this.activityService.GetLatestAssessmentResourceActivityByResourceVersionAndUserId(resourceVersionId, this.CurrentUserId.GetValueOrDefault());
            if (assessmentResourceActivity != null)
            {
                return await this.GetAssessmentProgressByAssessmentResourceActivityId(assessmentResourceActivity.Id);
            }

            return this.Ok(null);
        }
        /// <summary>
        /// The get resource information view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceInformationViewModelAsync/{resourceReferenceId}")]
        public async Task<ActionResult> GetResourceInformationViewModelAsync(int resourceReferenceId)
        {
            return this.Ok(await this.resourceService.GetResourceInformationViewModelAsync(resourceReferenceId, this.CurrentUserId.GetValueOrDefault()));
        }

        /// <summary>
        /// The get resource item view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceItemViewModelAsync/{resourceReferenceId}")]
        public async Task<ActionResult> GetResourceItemViewModelAsync(int resourceReferenceId)
        {
            var resourceItem = await this.resourceService.GetResourceItemViewModelAsync(resourceReferenceId, this.CurrentUserId.GetValueOrDefault(), this.HttpContext.User.IsInRole("ReadOnly"));
            return this.Ok(resourceItem);
        }

        /// <summary>
        /// The get catalogue locations.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetCatalogueLocations/{resourceReferenceId}")]
        public async Task<ActionResult> GetCatalogueLocations(int resourceReferenceId)
        {
            return this.Ok(await this.hierarchyService.GetCatalogueLocationsForResourceReference(resourceReferenceId));
        }

        /// <summary>
        /// Returns totals for "My Contributions".
        /// </summary>
        /// <param name="catalogueId">The catalogueId<see cref="int"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        [Route("GetMyContributionsTotals/{catalogueId}")]
        public ActionResult GetMyContributionsTotalsAsync(int catalogueId)
        {
            return this.Ok(this.resourceService.GetMyContributionTotals(catalogueId, this.CurrentUserId.GetValueOrDefault()));
        }


        /// <summary>
        /// Returns if the user has published resources.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("HasPublishedResources")]
        public async Task<IActionResult> HasPublishedResourcesAsync()
        {
            return this.Ok(await this.resourceService.HasPublishedResourcesAsync(this.CurrentUserId.GetValueOrDefault()));
        }

        /// <summary>
        /// The save file details for an article async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveArticleAttachedFileDetails")]
        public async Task<IActionResult> SaveArticleAttachedFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            var vr = await this.resourceService.CreateArticleAttachedFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// The save file details for a resource attribute async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveResourceAttributeFileDetails")]
        public async Task<IActionResult> SaveResourceAttributeFileDetails(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            LearningHubValidationResult vr = new LearningHubValidationResult();

            switch (fileCreateRequestViewModel.ResourceType)
            {
                case Nhs.Models.Enums.ResourceTypeEnum.Audio:
                    vr = await this.resourceService.CreateAudioAttributeFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId.GetValueOrDefault());
                    break;
                case Nhs.Models.Enums.ResourceTypeEnum.Video:
                    vr = await this.resourceService.CreateVideoAttributeFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId.GetValueOrDefault());
                    break;
                default:
                    vr.IsValid = false;
                    break;
            }

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// DuplicateResourceAsync.
        /// </summary>
        /// <param name="duplicateResourceRequestModel">The duplicateResourceRequestModel<see cref="DuplicateResourceRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DuplicateBlocks")]
        public async Task<IActionResult> DuplicateBlocks(DuplicateBlocksRequestModel duplicateResourceRequestModel)
        {
            LearningHubValidationResult vr = await this.resourceService.DuplicateBlocks(duplicateResourceRequestModel, this.CurrentUserId.GetValueOrDefault());
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }


        /// <summary>
        /// Create a new Resource and an initial ResourceVersion with a status of "Draft".
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateResource")]
        public async Task<IActionResult> CreateResourceAsync(ResourceDetailViewModel viewModel)
        {
            var vr = await this.resourceService.CreateResourceAsync(viewModel, this.CurrentUserId.GetValueOrDefault());
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// DuplicateResourceAsync.
        /// </summary>
        /// <param name="duplicateResourceRequestModel">The duplicateResourceRequestModel<see cref="DuplicateResourceRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DuplicateResource")]
        public async Task<IActionResult> DuplicateResourceAsync(DuplicateResourceRequestModel duplicateResourceRequestModel)
        {
            LearningHubValidationResult vr =
                await this.resourceService.DuplicateResourceAsync(duplicateResourceRequestModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// CreateNewResourceVersion.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "ReadWrite")]
        [Route("CreateNewResourceVersion")]
        public async Task<IActionResult> CreateNewResourceVersion(KeyValuePair<string, int> model)
        {
            LearningHubValidationResult vr = await this.resourceService.CreateNewResourceVersionAsync(model.Value, this.CurrentUserId.GetValueOrDefault());

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// SetResourceType
        /// Applies a ResourceType to a ResourceVersion "Draft"
        /// Cannot change the ResourceType of a Published ResourceVersion(?)
        /// TODO - requires validation.
        /// </summary>
        /// <param name="resourceViewModel">The resourceViewModel<see cref="ResourceViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("SetResourceType")]
        public IActionResult SetResourceType(ResourceViewModel resourceViewModel)
        {
            this.resourceService.SetResourceType(resourceViewModel, this.CurrentUserId.GetValueOrDefault());
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// Submit ResourceVersion For Publish.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SubmitResourceVersionForPublish")]
        public async Task<IActionResult> SubmitResourceVersionForPublishAsync(PublishViewModel publishViewModel)
        {
            publishViewModel.PublisherAction = PublisherActionEnum.Publish;
            publishViewModel.UserId = this.CurrentUserId.GetValueOrDefault();

            var vr = await this.resourceService.SubmitResourceVersionForPublish(publishViewModel);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The update resource version async.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "ReadWrite")]
        [Route("UpdateResourceVersion")]
        public async Task<IActionResult> UpdateResourceVersionAsync(ResourceDetailViewModel resourceDetailViewModel)
        {
            var vr = await this.resourceService.UpdateResourceVersionAsync(resourceDetailViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Delete a resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpDelete]
        [Authorize(Policy = "ReadWrite")]
        [Route("DeleteResourceVersion/{resourceVersionId}")]
        public async Task<IActionResult> DeleteResourceVersionAsync(int resourceVersionId)
        {
            try
            {
                var vr = await this.resourceService.DeleteResourceVersionAsync(resourceVersionId, this.CurrentUserId.GetValueOrDefault());

                if (vr.IsValid)
                {
                    return this.Ok(new ApiResponse(true, vr));
                }
                else
                {
                    return this.BadRequest(new ApiResponse(false, vr));
                }
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }


        /// <summary>
        /// Update Generic File Details.
        /// </summary>
        /// <param name="genericFileViewModel">The genericFileViewModel<see cref="GenericFileUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateGenericFileDetail")]
        public async Task<IActionResult> UpdateGenericFileDetailAsync(GenericFileUpdateRequestViewModel genericFileViewModel)
        {
            var vr = await this.resourceService.UpdateGenericFileDetailAsync(genericFileViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Update Scorm Detail.
        /// </summary>
        /// <param name="scormUpdateRequestViewModel">The scormUpdateRequestViewModel<see cref="ScormUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateScormDetail")]
        public async Task<IActionResult> UpdateScormDetailAsync(ScormUpdateRequestViewModel scormUpdateRequestViewModel)
        {
            var vr = await this.resourceService.UpdateScormDetailAsync(scormUpdateRequestViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }



        /// <summary>
        /// Update HTML Detail.
        /// </summary>
        /// <param name="htmlResourceViewModel">Html resource update view model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateHtmlDetail")]
        public async Task<IActionResult> UpdateHtmlDetailAsync(HtmlResourceUpdateRequestViewModel htmlResourceViewModel)
        {
            var vr = await this.resourceService.UpdateHtmlDetailAsync(htmlResourceViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Get specific ImageDetails by ResourceVersionId.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetImageDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetImageDetailsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetImageDetailsByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// The update image detail async.
        /// </summary>
        /// <param name="imageViewModel">The imageViewModel<see cref="ImageUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateImageDetail")]
        public async Task<IActionResult> UpdateImageDetailAsync(ImageUpdateRequestViewModel imageViewModel)
        {
            var vr = await this.resourceService.UpdateImageDetailAsync(imageViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// The update video detail async.
        /// </summary>
        /// <param name="videoViewModel">The videoViewModel<see cref="VideoUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateVideoDetail")]
        public async Task<IActionResult> UpdateVideoDetailAsync(VideoUpdateRequestViewModel videoViewModel)
        {
            var vr = await this.resourceService.UpdateVideoDetailAsync(videoViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Delete resource attribute File async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteResourceAttributeFile")]
        public async Task<IActionResult> DeleteResourceAttributeFileAsync(FileDeleteRequestModel fileDeleteRequestModel)
        {
            LearningHubValidationResult vr = new LearningHubValidationResult();
            switch (fileDeleteRequestModel.ResourceType)
            {
                case ResourceTypeEnum.Audio:
                    vr = await this.resourceService.DeleteAudioAttributeFileAsync(fileDeleteRequestModel, this.CurrentUserId.GetValueOrDefault());
                    break;
                case ResourceTypeEnum.Video:
                    vr = await this.resourceService.DeleteVideoAttributeFileAsync(fileDeleteRequestModel, this.CurrentUserId.GetValueOrDefault());
                    break;
                default:
                    vr.IsValid = false;
                    break;
            }

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// The update audio detail async.
        /// </summary>
        /// <param name="audioViewModel">The audioViewModel<see cref="AudioUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateAudioDetail")]
        public async Task<IActionResult> UpdateAudioDetailAsync(AudioUpdateRequestViewModel audioViewModel)
        {
            var vr = await this.resourceService.UpdateAudioDetailAsync(audioViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Update article detail async.
        /// </summary>
        /// <param name="articleViewModel">The articleViewModel<see cref="ArticleUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateArticleDetail")]
        public async Task<IActionResult> UpdateArticleDetailAsync(ArticleUpdateRequestViewModel articleViewModel)
        {
            var vr = await this.resourceService.UpdateArticleDetailAsync(articleViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Delete article detail async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteArticleFile")]
        public async Task<IActionResult> DeleteArticleFileAsync(FileDeleteRequestModel fileDeleteRequestModel)
        {
            var vr = await this.resourceService.DeleteArticleFileAsync(fileDeleteRequestModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Add a new Resource Version Author.
        /// </summary>
        /// <param name="resourceAuthorViewModel">The resourceAuthorViewModel<see cref="ResourceAuthorViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("AddResourceVersionAuthor")]
        public async Task<IActionResult> AddResourceVersionAuthorAsync(ResourceAuthorViewModel resourceAuthorViewModel)
        {
            int i = await this.resourceService.AddResourceVersionAuthorAsync(resourceAuthorViewModel, this.CurrentUserId.GetValueOrDefault());
            var vr = new LearningHubValidationResult(true);
            vr.CreatedId = i;

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The delete resource version author async.
        /// </summary>
        /// <param name="resourceAuthorViewModel">The resourceAuthorViewModel<see cref="AuthorDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteResourceVersionAuthor")]
        public async Task<IActionResult> DeleteResourceVersionAuthorAsync(AuthorDeleteRequestModel resourceAuthorViewModel)
        {
            await this.resourceService.DeleteResourceVersionAuthorAsync(resourceAuthorViewModel, this.CurrentUserId.GetValueOrDefault());
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The delete resource version keyword async.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="KeywordDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteResourceVersionKeyword")]
        public async Task<IActionResult> DeleteResourceVersionKeywordAsync(KeywordDeleteRequestModel resourceKeywordViewModel)
        {
            await this.resourceService.DeleteResourceVersionKeywordAsync(resourceKeywordViewModel, this.CurrentUserId.GetValueOrDefault());
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
        }


        /// <summary>
        /// Save the Resource Version Flag.
        /// </summary>
        /// <param name="resourceVersionFlagViewModel">The resourceVersionFlagViewModel<see cref="ResourceVersionFlagViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveResourceVersionFlag")]
        public async Task<IActionResult> SaveResourceVersionFlagAsync(ResourceVersionFlagViewModel resourceVersionFlagViewModel)
        {
            var vr = await this.resourceService.SaveResourceVersionFlagAsync(resourceVersionFlagViewModel, this.CurrentUserId.GetValueOrDefault());
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// The delete resource version flag async.
        /// </summary>
        /// <param name="resourceVersionFlagId">The resourceVersionFlagId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteResourceVersionFlag")]
        public async Task<IActionResult> DeleteResourceVersionFlagAsync(int resourceVersionFlagId)
        {
            await this.resourceService.DeleteResourceVersionFlagAsync(resourceVersionFlagId, this.CurrentUserId.GetValueOrDefault());

            return this.Ok(new ApiResponse(true, new LearningHubValidationResult(true)));
        }

        /// <summary>
        /// The update web link resource version async.
        /// </summary>
        /// <param name="webLinkViewModel">The webLinkViewModel<see cref="WebLinkViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateWeblinkDetail")]
        public async Task<IActionResult> UpdateWebLinkResourceVersionAsync(WebLinkViewModel webLinkViewModel)
        {
            var vr = await this.resourceService.UpdateWebLinkResourceVersionAsync(webLinkViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }


        /// <summary>
        /// The update case resource version async.
        /// </summary>
        /// <param name="caseViewModel">The web link view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateCaseDetail")]
        public async Task<IActionResult> UpdateCaseResourceVersionAsync(CaseViewModel caseViewModel)
        {
            var vr = await this.resourceService.UpdateCaseResourceVersionAsync(caseViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// This method updates the database entry with the assessment details, passed down as a parameter.
        /// </summary>
        /// <param name="assessmentViewModel">The web link view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateAssessmentDetail")]
        public async Task<IActionResult> UpdateAssessmentResourceVersionAsync(AssessmentViewModel assessmentViewModel)
        {
            var vr = await this.resourceService.UpdateAssessmentResourceVersionAsync(assessmentViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }


        /// <summary>
        /// The save file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">The fileChunkDetailCreateRequestViewModel<see cref="FileChunkDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveFileChunkDetail")]
        public async Task<IActionResult> SaveFileChunkDetailAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel)
        {
            var vr = await this.resourceService.CreateFileChunkDetailAsync(fileChunkDetailCreateRequestViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// The get file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetFileChunkDetail/{fileChunkDetailId}")]
        public async Task<IActionResult> GetFileChunkDetailAsync(int fileChunkDetailId)
        {
            var fileChunkDetail = await this.resourceService.GetFileChunkDetailAsync(fileChunkDetailId);
            return this.Ok(fileChunkDetail);
        }

        /// <summary>
        /// Delete a file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailDeleteRequestModel">The fileChunkDetailDeleteRequestModel<see cref="FileChunkDetailDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteFileChunkDetail")]
        public async Task<IActionResult> DeleteFileChunkDetailAsync(FileChunkDetailDeleteRequestModel fileChunkDetailDeleteRequestModel)
        {
            int amendUserId = fileChunkDetailDeleteRequestModel.AmendUserId.HasValue ? fileChunkDetailDeleteRequestModel.AmendUserId.Value : this.CurrentUserId.GetValueOrDefault();
            var vr = await this.resourceService.DeleteFileChunkDetailAsync(fileChunkDetailDeleteRequestModel.FileChunkDetailId, amendUserId);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// The save file details async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveFileDetails")]
        public async Task<IActionResult> SaveFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            var vr = await this.resourceService.CreateFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }


    }
}
