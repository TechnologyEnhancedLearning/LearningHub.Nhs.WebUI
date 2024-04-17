namespace LearningHub.Nhs.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Admin;
    using LearningHub.Nhs.Models.Resource.AzureMediaAsset;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Resource operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class ResourceController : ApiControllerBase
    {
        /// <summary>
        /// The resource service...
        /// </summary>
        private readonly IResourceService resourceService;

        /// <summary>
        /// The activity service...
        /// </summary>
        private readonly IActivityService activityService;

        /// <summary>
        /// The hierarchy service...
        /// </summary>
        private readonly IHierarchyService hierarchyService;

        /// <summary>
        /// The file type service...
        /// </summary>
        private readonly IFileTypeService fileTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="userService">The UserService<see cref="IUserService"/>.</param>
        /// <param name="resourceService">The resourceService<see cref="IResourceService"/>.</param>
        /// <param name="activityService">The activityService<see cref="IActivityService"/>.</param>
        /// <param name="hierarchyService">The hierarchyService<see cref="IHierarchyService"/>.</param>
        /// <param name="fileTypeService">The fileTypeService<see cref="IFileTypeService"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{ResourceController}"/>.</param>
        public ResourceController(
            IUserService userService,
            IResourceService resourceService,
            IActivityService activityService,
            IHierarchyService hierarchyService,
            IFileTypeService fileTypeService,
            ILogger<ResourceController> logger)
            : base(userService, logger)
        {
            this.resourceService = resourceService;
            this.activityService = activityService;
            this.hierarchyService = hierarchyService;
            this.fileTypeService = fileTypeService;
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
        /// Create a new Resource and an initial ResourceVersion with a status of "Draft".
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateResource")]
        public async Task<IActionResult> CreateResourceAsync(ResourceDetailViewModel viewModel)
        {
            var vr = await this.resourceService.CreateResourceAsync(viewModel, this.CurrentUserId);
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
                await this.resourceService.DuplicateResourceAsync(duplicateResourceRequestModel, this.CurrentUserId);

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
            LearningHubValidationResult vr = await this.resourceService.CreateNewResourceVersionAsync(model.Value, this.CurrentUserId);

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
            this.resourceService.SetResourceType(resourceViewModel, this.CurrentUserId);
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// Get specific ResourceVersion by Id.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetResourceVersion/{resourceVersionId}")]
        public async Task<ActionResult> GetResourceVersionAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionByIdAsync(resourceVersionId));
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
        /// Get specific ResourceVersionViewModel by Id.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetResourceVersionExtendedViewModel/{resourceVersionId}")]
        public async Task<ActionResult> GetResourceVersionExtendedViewModelAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionExtendedViewModelAsync(resourceVersionId, this.CurrentUserId));
        }

        /// <summary>
        /// Publish ResourceVersion
        ///   TODO - requires validation.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("PublishResourceVersion")]
        public async Task<IActionResult> PublishResourceVersion(PublishViewModel publishViewModel)
        {
            try
            {
                int publicationId = await this.resourceService.PublishResourceVersionAsync(publishViewModel);

                if (publicationId > 0)
                {
                    this.resourceService.ConfirmSubmissionToSearch(publicationId, publishViewModel.UserId);
                }
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }

            return this.Ok(new ApiResponse(true, new LearningHubValidationResult(true)));
        }

        /// <summary>
        /// Submit ResourceVersion For Prepare.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SubmitResourceVersionForPrepare")]
        public async Task<IActionResult> SubmitResourceVersionForPrepareAsync(PublishViewModel publishViewModel)
        {
            publishViewModel.PublisherAction = PublisherActionEnum.Prepare;

            var vr = await this.resourceService.SubmitResourceVersionForPrepare(publishViewModel);

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
            publishViewModel.UserId = this.CurrentUserId;

            var vr = await this.resourceService.SubmitResourceVersionForPublish(publishViewModel);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// Set ResourceVersion to Publishing.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("ResourceVersionPublishing")]
        public IActionResult ResourceVersionPublishing(PublishViewModel publishViewModel)
        {
            var vr = this.resourceService.SetResourceVersionPublishing(publishViewModel);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// Submit ResourceVersion to Publish Failed.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("ResourceVersionFailedToPublish")]
        public async Task<IActionResult> ResourceVersionFailedToPublish(PublishViewModel publishViewModel)
        {
            var vr = await this.resourceService.SetResourceVersionFailedToPublish(publishViewModel);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionEventViewModel">resourceVersionEventViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateResourceVersionEvent")]
        public IActionResult CreateResourceVersionEvent(ResourceVersionEventViewModel resourceVersionEventViewModel)
        {
            if (resourceVersionEventViewModel.CreateUserId == 0)
            {
                resourceVersionEventViewModel.CreateUserId = this.CurrentUserId;
            }

            this.resourceService.CreateResourceVersionEvent(resourceVersionEventViewModel);
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
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
            var vr = await this.resourceService.UnpublishResourceVersion(unpublishViewModel, this.CurrentUserId);

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
            var vr = await this.resourceService.RevertToDraft(resourceVersionId, this.CurrentUserId);

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
            var vr = await this.resourceService.UpdateResourceVersionAsync(resourceDetailViewModel, this.CurrentUserId);

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
                var vr = await this.resourceService.DeleteResourceVersionAsync(resourceVersionId, this.CurrentUserId);

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
        /// The mark sensitive conatet async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("AcceptSensitiveContent")]
        public async Task<IActionResult> AcceptSensitiveContent([FromBody] int resourceVersionId)
        {
            var vr = await this.resourceService.AcceptSensitiveContentAsync(resourceVersionId, this.CurrentUserId);

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
        /// Get file directory for unpublished or deleted versions.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="deletedResource">.</param>
        /// <returns>The <see cref="Task{String}"/>.</returns>
        [HttpGet]
        [Route("GetObsoleteResourceFile/{resourceVersionId}")]
        [Route("GetObsoleteResourceFile/{resourceVersionId}/{deletedResource}")]
        public async Task<List<string>> GetObsoleteResourceFile(int resourceVersionId, bool deletedResource = false)
        {
            return await this.resourceService.GetObsoleteResourceFile(resourceVersionId, deletedResource);
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
        /// The GetExternalContentDetailsById.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetExternalContentDetailsById/{resourceVersionId}")]
        public ActionResult GetScormContentDetailsById(int resourceVersionId)
        {
            return this.Ok(this.resourceService.GetExternalContentDetails(resourceVersionId, this.CurrentUserId));
        }

        /// <summary>
        /// The RecordExternalReferenceUserAgreement.
        /// </summary>
        /// <param name="viewModel">viewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("RecordExternalReferenceUserAgreement")]
        public async Task<IActionResult> RecordExternalReferenceUserAgreementAsync(ExternalReferenceUserAgreementViewModel viewModel)
        {
            viewModel.UserId = this.CurrentUserId;
            var vr = await this.resourceService.RecordExternalReferenceUserAgreementAsync(viewModel, this.CurrentUserId);
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
        /// Update Generic File Details.
        /// </summary>
        /// <param name="genericFileViewModel">The genericFileViewModel<see cref="GenericFileUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateGenericFileDetail")]
        public async Task<IActionResult> UpdateGenericFileDetailAsync(GenericFileUpdateRequestViewModel genericFileViewModel)
        {
            var vr = await this.resourceService.UpdateGenericFileDetailAsync(genericFileViewModel, this.CurrentUserId);

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
            var vr = await this.resourceService.UpdateScormDetailAsync(scormUpdateRequestViewModel, this.CurrentUserId);

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
            var vr = await this.resourceService.UpdateHtmlDetailAsync(htmlResourceViewModel, this.CurrentUserId);

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
            var vr = await this.resourceService.UpdateImageDetailAsync(imageViewModel, this.CurrentUserId);

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
        /// The update video detail async.
        /// </summary>
        /// <param name="videoViewModel">The videoViewModel<see cref="VideoUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateVideoDetail")]
        public async Task<IActionResult> UpdateVideoDetailAsync(VideoUpdateRequestViewModel videoViewModel)
        {
            var vr = await this.resourceService.UpdateVideoDetailAsync(videoViewModel, this.CurrentUserId);

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
                    vr = await this.resourceService.DeleteAudioAttributeFileAsync(fileDeleteRequestModel, this.CurrentUserId);
                    break;
                case ResourceTypeEnum.Video:
                    vr = await this.resourceService.DeleteVideoAttributeFileAsync(fileDeleteRequestModel, this.CurrentUserId);
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
        /// The update audio detail async.
        /// </summary>
        /// <param name="audioViewModel">The audioViewModel<see cref="AudioUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateAudioDetail")]
        public async Task<IActionResult> UpdateAudioDetailAsync(AudioUpdateRequestViewModel audioViewModel)
        {
            var vr = await this.resourceService.UpdateAudioDetailAsync(audioViewModel, this.CurrentUserId);

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
        /// Update article detail async.
        /// </summary>
        /// <param name="articleViewModel">The articleViewModel<see cref="ArticleUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateArticleDetail")]
        public async Task<IActionResult> UpdateArticleDetailAsync(ArticleUpdateRequestViewModel articleViewModel)
        {
            var vr = await this.resourceService.UpdateArticleDetailAsync(articleViewModel, this.CurrentUserId);

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
            var vr = await this.resourceService.DeleteArticleFileAsync(fileDeleteRequestModel, this.CurrentUserId);

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
            int i = await this.resourceService.AddResourceVersionAuthorAsync(resourceAuthorViewModel, this.CurrentUserId);
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
            await this.resourceService.DeleteResourceVersionAuthorAsync(resourceAuthorViewModel, this.CurrentUserId);
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
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
            var vr = await this.resourceService.AddResourceVersionKeywordAsync(resourceKeywordViewModel, this.CurrentUserId);
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
        /// The delete resource version keyword async.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="KeywordDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteResourceVersionKeyword")]
        public async Task<IActionResult> DeleteResourceVersionKeywordAsync(KeywordDeleteRequestModel resourceKeywordViewModel)
        {
            await this.resourceService.DeleteResourceVersionKeywordAsync(resourceKeywordViewModel, this.CurrentUserId);
            var vr = new LearningHubValidationResult(true);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The get resource version events.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceVersionEvents/{resourceVersionId}")]
        public async Task<ActionResult> GetResourceVersionEventsAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetResourceVersionEventsAsync(resourceVersionId));
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
        /// Save the Resource Version Flag.
        /// </summary>
        /// <param name="resourceVersionFlagViewModel">The resourceVersionFlagViewModel<see cref="ResourceVersionFlagViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveResourceVersionFlag")]
        public async Task<IActionResult> SaveResourceVersionFlagAsync(ResourceVersionFlagViewModel resourceVersionFlagViewModel)
        {
            var vr = await this.resourceService.SaveResourceVersionFlagAsync(resourceVersionFlagViewModel, this.CurrentUserId);
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
            await this.resourceService.DeleteResourceVersionFlagAsync(resourceVersionFlagId, this.CurrentUserId);

            return this.Ok(new ApiResponse(true, new LearningHubValidationResult(true)));
        }

        /// <summary>
        /// The get embedded resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetEmbeddedResourceVersionAsync/{resourceVersionId}")]
        public async Task<ActionResult> GetEmbeddedResourceVersionAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetEmbeddedResourceVersionByIdAsync(resourceVersionId));
        }

        /// <summary>
        /// The update embedded resource version async.
        /// </summary>
        /// <param name="embedCodeViewModel">The embedCodeViewModel<see cref="EmbedCodeViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateEmbeddedResourceVersion")]
        public async Task<IActionResult> UpdateEmbeddedResourceVersionAsync(EmbedCodeViewModel embedCodeViewModel)
        {
            var vr = await this.resourceService.UpdateEmbeddedResourceVersionAsync(embedCodeViewModel, this.CurrentUserId);

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
        /// The get equipment resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetEquipmentResourceVersionAsync/{resourceVersionId}")]
        public async Task<ActionResult> GetEquipmentResourceVersionAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetEquipmentDetailsByIdAsync(resourceVersionId));
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
        /// The update web link resource version async.
        /// </summary>
        /// <param name="webLinkViewModel">The webLinkViewModel<see cref="WebLinkViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateWeblinkDetail")]
        public async Task<IActionResult> UpdateWebLinkResourceVersionAsync(WebLinkViewModel webLinkViewModel)
        {
            var vr = await this.resourceService.UpdateWebLinkResourceVersionAsync(webLinkViewModel, this.CurrentUserId);

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
        /// The scorm publish update async.
        /// </summary>
        /// <param name="scormPublishUpdateViewModel">The scormPublishUpdateViewModel<see cref="ScormPublishUpdateViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateScormPublishDetails")]
        public async Task<IActionResult> UpdateScormPublishDetailsAsync(ScormPublishUpdateViewModel scormPublishUpdateViewModel)
        {
            var vr = await this.resourceService.UpdateScormPublishDetailAsync(scormPublishUpdateViewModel);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The html resource publish update async.
        /// </summary>
        /// <param name="viewModel">The HtmlResourcePublishUpdateViewModel<see cref="HtmlResourcePublishUpdateViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateHtmlResourcePublishDetails")]
        public async Task<IActionResult> UpdateHtmlResourcePublishDetailsAsync(HtmlResourcePublishUpdateViewModel viewModel)
        {
            var vr = await this.resourceService.UpdateHtmlResourcePublishDetailsAsync(viewModel);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The create scorm manifest async.
        /// </summary>
        /// <param name="scormManifestViewModel">The scormManifestViewModel<see cref="ScormManifestUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SetScormManifestDetails")]
        public async Task<IActionResult> SetScormManifestDetailAsync(ScormManifestUpdateRequestViewModel scormManifestViewModel)
        {
            var vr = await this.resourceService.SetScormManifestDetailAsync(scormManifestViewModel);

            return this.Ok(new ApiResponse(true, vr));
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
        /// The update case resource version async.
        /// </summary>
        /// <param name="caseViewModel">The web link view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateCaseDetail")]
        public async Task<IActionResult> UpdateCaseResourceVersionAsync(CaseViewModel caseViewModel)
        {
            var vr = await this.resourceService.UpdateCaseResourceVersionAsync(caseViewModel, this.CurrentUserId);

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
        /// Retrieves the entire assessment details given a resource version ID.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetAssessmentDetails/{resourceVersionId}")]
        public async Task<ActionResult> GetAssessmentResourceVersionAsync(int resourceVersionId)
        {
            var response = await this.resourceService.GetAssessmentDetailsByIdAsync(resourceVersionId, this.CurrentUserId);

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
        /// This method updates the database entry with the assessment details, passed down as a parameter.
        /// </summary>
        /// <param name="assessmentViewModel">The web link view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateAssessmentDetail")]
        public async Task<IActionResult> UpdateAssessmentResourceVersionAsync(AssessmentViewModel assessmentViewModel)
        {
            var vr = await this.resourceService.UpdateAssessmentResourceVersionAsync(assessmentViewModel, this.CurrentUserId);

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
            int attempts = this.activityService.GetAttempts(this.CurrentUserId, resourceVersionId);
            var assessmentResourceActivitiesWithAnswers =
                await this.activityService.GetAnswersForAllTheAssessmentResourceActivities(this.CurrentUserId, resourceVersionId);
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
            var assessmentResourceActivity = await this.activityService.GetLatestAssessmentResourceActivityByResourceVersionAndUserId(resourceVersionId, this.CurrentUserId);
            if (assessmentResourceActivity != null)
            {
                return await this.GetAssessmentProgressByAssessmentResourceActivityId(assessmentResourceActivity.Id);
            }

            return this.Ok(null);
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
        /// The create resource version validation result async.
        /// </summary>
        /// <param name="validationResultViewModel">The validationResultViewModel<see cref="ResourceVersionValidationResultViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateResourceVersionValidationResult")]
        public async Task<IActionResult> CreateResourceVersionValidationResultAsync(ResourceVersionValidationResultViewModel validationResultViewModel)
        {
            var vr = await this.resourceService.CreateResourceVersionValidationResultAsync(validationResultViewModel);

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// The get resource header view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetResourceHeaderViewModelAsync/{resourceReferenceId}")]
        public async Task<ActionResult> GetResourceHeaderViewModelAsync(int resourceReferenceId)
        {
            return this.Ok(await this.resourceService.GetResourceHeaderViewModelAsync(resourceReferenceId));
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
            return this.Ok(await this.resourceService.GetResourceInformationViewModelAsync(resourceReferenceId, this.CurrentUserId));
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
            var resourceItem = await this.resourceService.GetResourceItemViewModelAsync(resourceReferenceId, this.CurrentUserId, this.HttpContext.User.IsInRole("ReadOnly"));
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
            return this.Ok(this.resourceService.GetMyContributionTotals(catalogueId, this.CurrentUserId));
        }

        /// <summary>
        /// Returns Resource Cards for "My Contributions".
        /// </summary>
        /// <param name="resourceContributionsRequestViewModel">The resourceContributionsRequestViewModel<see cref="ResourceContributionsRequestViewModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [Route("GetContributions")]
        public ActionResult GetMyContributionsAsync(ResourceContributionsRequestViewModel resourceContributionsRequestViewModel)
        {
            return this.Ok(this.resourceService.GetContributions(this.CurrentUserId, resourceContributionsRequestViewModel, this.HttpContext.User.IsInRole("ReadOnly")));
        }

        /// <summary>
        /// Returns the requested contributions.
        /// </summary>
        /// <param name="myContributionsRequestViewModel">The myContributionsRequestViewModel<see cref="MyContributionsRequestViewModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [Route("GetMyContributions")]
        public ActionResult GetMyContributions(MyContributionsRequestViewModel myContributionsRequestViewModel)
        {
            return this.Ok(this.resourceService.GetMyContributions(this.CurrentUserId, myContributionsRequestViewModel));
        }

        /// <summary>
        /// Returns Resource Cards.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetMyResourceViewModel")]
        public async Task<ActionResult> GetMyResourceViewModelAsync()
        {
            return this.Ok(await this.resourceService.GetMyResourceViewModelAsync(this.CurrentUserId));
        }

        /// <summary>
        /// Returns Extended Card details for the supplied Id (resourceVersionId).
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("ResourceCardExtendedViewModel/{resourceVersionId}")]
        public async Task<ActionResult> GetResourceCardExtendedViewModelAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetResourceCardExtendedViewModelAsync(resourceVersionId, this.CurrentUserId));
        }

        /// <summary>
        /// Returns if the user has published resources.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("HasPublishedResources")]
        public async Task<IActionResult> HasPublishedResourcesAsync()
        {
            return this.Ok(await this.resourceService.HasPublishedResourcesAsync(this.CurrentUserId));
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
        /// The save file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">The fileChunkDetailCreateRequestViewModel<see cref="FileChunkDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveFileChunkDetail")]
        public async Task<IActionResult> SaveFileChunkDetailAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel)
        {
            var vr = await this.resourceService.CreateFileChunkDetailAsync(fileChunkDetailCreateRequestViewModel, this.CurrentUserId);

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
        /// Delete a file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailDeleteRequestModel">The fileChunkDetailDeleteRequestModel<see cref="FileChunkDetailDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteFileChunkDetail")]
        public async Task<IActionResult> DeleteFileChunkDetailAsync(FileChunkDetailDeleteRequestModel fileChunkDetailDeleteRequestModel)
        {
            int amendUserId = fileChunkDetailDeleteRequestModel.AmendUserId.HasValue ? fileChunkDetailDeleteRequestModel.AmendUserId.Value : this.CurrentUserId;
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
            var vr = await this.resourceService.CreateFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId);

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
        /// The update file details async.
        /// </summary>
        /// <param name="fileUpdateRequestViewModel">The fileUpdateRequestViewModel<see cref="FileUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateFileDetails")]
        public async Task<IActionResult> UpdateFileDetailsAsync(FileUpdateRequestViewModel fileUpdateRequestViewModel)
        {
            int amendUserId = fileUpdateRequestViewModel.AmendUserId.HasValue ? fileUpdateRequestViewModel.AmendUserId.Value : this.CurrentUserId;
            var vr = await this.resourceService.UpdateFileDetailsAsync(fileUpdateRequestViewModel, amendUserId);

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
        /// The save file details for an article async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SaveArticleAttachedFileDetails")]
        public async Task<IActionResult> SaveArticleAttachedFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            var vr = await this.resourceService.CreateArticleAttachedFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId);

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
                case Models.Enums.ResourceTypeEnum.Audio:
                    vr = await this.resourceService.CreateAudioAttributeFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId);
                    break;
                case Models.Enums.ResourceTypeEnum.Video:
                    vr = await this.resourceService.CreateVideoAttributeFileDetailsAsync(fileCreateRequestViewModel, this.CurrentUserId);
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
        /// Add a new resource azure media asset.
        /// </summary>
        /// <param name="mediaAssetOutputViewModel">The mediaAssetOutputViewModel<see cref="MediaAssetOutputViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("AddResourceAzureMediaAsset")]
        public async Task<IActionResult> AddResourceAzureMediaAssetAsync(MediaAssetOutputViewModel mediaAssetOutputViewModel)
        {
            int i = await this.resourceService.CreateResourceAzureMediaAssetAsync(mediaAssetOutputViewModel);
            var vr = new LearningHubValidationResult(true);
            vr.CreatedId = i;

            return this.Ok(new ApiResponse(true, vr));
        }

        /// <summary>
        /// Get media asset input resourceVersion by Id.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet("GetMediaAssetInput/{resourceVersionId}")]
        public async Task<ActionResult> GetMediaAssetInputAsync(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetMediaAssetInputViewModelAsync(resourceVersionId));
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
        /// Get a filtered page of User records.
        /// </summary>
        /// <param name="pagingRequestModel">The filter<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        ////[HttpGet]
        ////[Route("GetResourceAdminSearchFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{filter}")]
        ////public async Task<IActionResult> GetResourceAdminSearchFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string filter)
        [HttpPost]
        [Route("GetResourceAdminSearchFilteredPage")]
        public async Task<IActionResult> GetResourceAdminSearchFilteredPage([FromBody] PagingRequestModel pagingRequestModel)
        {
            var pagedResultSet = await this.resourceService.GetResourceAdminSearchFilteredPageAsync(this.CurrentUserId, pagingRequestModel);
            return this.Ok(pagedResultSet);
        }

        /// <summary>
        /// Transfer Resource Ownership.
        /// </summary>
        /// <param name="transferResourceOwnershipViewModel">The transferResourceOwnershipViewModel<see cref="TransferResourceOwnershipViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("TransferResourceOwnership")]
        public async Task<IActionResult> TransferResourceOwnershipAsync(TransferResourceOwnershipViewModel transferResourceOwnershipViewModel)
        {
            var vr = await this.resourceService.TransferResourceOwnership(transferResourceOwnershipViewModel, this.CurrentUserId);

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
        /// Get All published resources id.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet("GetAllPublishedResource")]
        public async Task<IActionResult> GetAllPublishedResourceAsync()
        {
            return this.Ok(await this.resourceService.GetAllPublishedResourceAsync());
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
            LearningHubValidationResult vr = await this.resourceService.DuplicateBlocks(duplicateResourceRequestModel, this.CurrentUserId);
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
        /// DeleteResourceProvider.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("DeleteAllResourceProvider/{resourceVersionId}")]
        public async Task DeleteAllResourceProvider(int resourceVersionId)
        {
            await this.resourceService.DeleteAllResourceVersionProviderAsync(resourceVersionId, this.CurrentUserId);
        }
    }
}
