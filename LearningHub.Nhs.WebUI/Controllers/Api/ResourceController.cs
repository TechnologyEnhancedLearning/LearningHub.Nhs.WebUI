namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Defines the <see cref="ResourceController" />.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceService resourceService;
        private readonly IFileService fileService;
        private readonly IActivityService activityService;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="fileService">File service.</param>
        /// <param name="activityService">Activity service.</param>
        /// <param name="configuration">Configuration.</param>
        public ResourceController(IResourceService resourceService, IFileService fileService, IActivityService activityService, IConfiguration configuration)
        {
            this.resourceService = resourceService;
            this.fileService = fileService;
            this.activityService = activityService;
            this.configuration = configuration;
        }

        /// <summary>
        /// The AcceptSensitiveContentAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("AcceptSensitiveContent/{resourceVersionId}")]
        public async Task<ActionResult> AcceptSensitiveContentAsync(int resourceVersionId)
        {
            var validationResult = await this.resourceService.AcceptSensitiveContentAsync(resourceVersionId);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The DownloadResource.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("DownloadResource")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadResource(string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return this.Content("Filename doesn't exist in request");
            }

            var file = await this.fileService.DownloadFileAsync(filePath, fileName);
            if (file != null)
            {
                return this.File(file.Content, file.ContentType, fileName);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }

        /// <summary>
        /// Download a resource and also record an activity. Required for non-JS activity recording on image/generic file resource types.
        /// </summary>
        /// <param name="resourceVersionId">the resourceVersionId.</param>
        /// <param name="nodePathId">The nodePathId.</param>
        /// <param name="filePath">The filePath.</param>
        /// <param name="fileName">The fileName.</param>
        /// <returns>The resource file.</returns>
        [HttpGet("DownloadResourceAndRecordActivity")]
        public async Task<IActionResult> DownloadResourceAndRecordActivity(int resourceVersionId, int nodePathId, string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return this.Content("Filename doesn't exist in request");
            }

            var file = await this.fileService.DownloadFileAsync(filePath, fileName);
            if (file != null)
            {
                var activity = new CreateResourceActivityViewModel()
                {
                    ResourceVersionId = resourceVersionId,
                    NodePathId = nodePathId,
                    ActivityStart = DateTime.UtcNow, // TODO: What about user's timezone offset when Javascript is disabled? Needs JavaScript.
                    ActivityStatus = ActivityStatusEnum.Completed,
                };
                await this.activityService.CreateResourceActivityAsync(activity);
                return this.File(file.Content, file.ContentType, fileName);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }

        /// <summary>
        /// Navigate to a weblink URL and also record an activity. Required for non-JS activity recording on image/generic file resource types.
        /// </summary>
        /// <param name="resourceVersionId">the resourceVersionId.</param>
        /// <param name="nodePathId">The nodePathId.</param>
        /// <param name="url">The weblink URL.</param>
        /// <returns>Redirect to the weblink URL.</returns>
        [HttpGet("NavigateToWeblinkAndRecordActivity")]
        public async Task<IActionResult> NavigateToWeblinkAndRecordActivity(int resourceVersionId, int nodePathId, string url)
        {
            var activity = new CreateResourceActivityViewModel()
            {
                ResourceVersionId = resourceVersionId,
                NodePathId = nodePathId,
                ActivityStart = DateTime.UtcNow, // TODO: What about user's timezone offset when Javascript is disabled? Needs JavaScript.
                ActivityStatus = ActivityStatusEnum.Completed,
            };
            await this.activityService.CreateResourceActivityAsync(activity);

            return this.Redirect(url);
        }

        /// <summary>
        /// Download Covid Testing Resource.
        /// This is a temporary solution until we have permission setup around resource files.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [AllowAnonymous]
        [HttpGet("DownloadCovidTestingResource")]
        public async Task<IActionResult> DownloadCovidTestingResource()
        {
            var filePath = this.configuration["AssetDetails:FilePath1"];
            var fileName = this.configuration["AssetDetails:FileName1"];

            var file = await this.fileService.DownloadFileAsync(filePath, fileName);
            if (file != null)
            {
                return this.File(file.Content, file.ContentType, fileName);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }

        /// <summary>
        /// The GetArticleDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetArticleDetailsById/{id}")]
        public async Task<ActionResult> GetArticleDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetArticleDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetAudioDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAudioDetailsById/{id}")]
        public async Task<ActionResult> GetAudioDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetAudioDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetGenericFileDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetGenericFileDetailsById/{id}")]
        public async Task<ActionResult> GetGenericFileDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetGenericFileDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetHtmlDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetHtmlDetailsById/{id}")]
        public async Task<ActionResult> GetHtmlDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetHtmlDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetScormDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetScormDetailsById/{id}")]
        public async Task<ActionResult> GetScormDetailsByIdAsync(int id)
        {
            var sfd = await this.resourceService.GetScormDetailsByIdAsync(id);
            return this.Ok(sfd);
        }

        /// <summary>
        /// The GetScormContentDetailsByIdAsync.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetExternalContentDetails/{id}")]
        public async Task<ActionResult> GetExternalContentDetailsAsync(int id)
        {
            return this.Ok(await this.resourceService.GetExternalContentDetailsAsync(id));
        }

        /// <summary>
        /// The RecordExternalReferenceUserAgreementAsync.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("RecordExternalReferenceUserAgreement")]
        public async Task<ActionResult> RecordExternalReferenceUserAgreementAsync([FromBody] ExternalReferenceUserAgreementViewModel model)
        {
            var response = await this.resourceService.RecordExternalReferenceUserAgreementAsync(model);
            if (response)
            {
                return this.Ok();
            }
            else
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// The GetHeaderById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetHeaderById/{id}")]
        public async Task<ActionResult> GetHeaderById(int id)
        {
            var resource = await this.resourceService.GetByIdAsync(id);
            return this.Ok(resource);
        }

        /// <summary>
        /// The GetImageDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetImageDetailsById/{id}")]
        public async Task<ActionResult> GetImageDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetImageDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetInformationById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetInformationById/{id}")]
        public async Task<ActionResult> GetInformationById(int id)
        {
            var resource = await this.resourceService.GetInformationByIdAsync(id);
            return this.Ok(resource);
        }

        /// <summary>
        /// The GetItemById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetItemById/{id}")]
        public async Task<ActionResult> GetItemById(int id)
        {
            var resource = await this.resourceService.GetItemByIdAsync(id);

            return this.Ok(resource);
        }

        /// <summary>
        /// The GetLocationsById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetLocationsById/{id}")]
        public async Task<ActionResult> GetLocationsById(int id)
        {
            var resource = await this.resourceService.GetLocationsByIdAsync(id);
            return this.Ok(resource);
        }

        /// <summary>
        /// The GetVersionHistoryById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetVersionHistoryById/{id}")]
        public async Task<ActionResult> GetVersionHistoryById(int id)
        {
            var resource = await this.resourceService.GetVersionHistoryByIdAsync(id);
            return this.Ok(resource);
        }

        /// <summary>
        /// The GetVideoDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetVideoDetailsById/{id}")]
        public async Task<ActionResult> GetVideoDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetVideoDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetVideoFileContentAuthenticationToken.
        /// </summary>
        /// <param name="assetFilePath">assetFilePath.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetVideoFileContentAuthenticationToken/{assetFilePath}")]
        public async Task<ActionResult> GetVideoFileContentAuthenticationTokenByIdAsync(string assetFilePath)
        {
            return this.Ok(await this.resourceService.GetVideoFileContentAuthenticationTokenAsync(assetFilePath));
        }

        /// <summary>
        /// The GetWeblinkDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetWeblinkDetailsById/{id}")]
        public async Task<ActionResult> GetWeblinkDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetWeblinkDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetCaseDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetCaseDetailsById/{id}")]
        public async Task<ActionResult> GetCaseDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetCaseDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetAssessmentDetailsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAssessmentDetailsById/{id}")]
        public async Task<ActionResult> GetAssessmentDetailsByIdAsync(int id)
        {
            return this.Ok(await this.resourceService.GetAssessmentDetailsByIdAsync(id));
        }

        /// <summary>
        /// The GetAssessmentContent.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAssessmentContent/{resourceVersionId}")]
        public async Task<ActionResult> GetAssessmentContent(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetAssessmentContent(resourceVersionId));
        }

        /// <summary>
        /// Retrieves the latest assessment progress by the resource version id.
        /// </summary>
        /// <param name="resourceVersionId"> The resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAssessmentProgress/resource/{resourceVersionId}")]
        public async Task<ActionResult> GetAssessmentProgressByResourceVersion(int resourceVersionId)
        {
            return this.Ok(await this.resourceService.GetAssessmentProgressByResourceVersion(resourceVersionId));
        }

        /// <summary>
        /// Retrieves the assessment progress for a given assessment resource activity id.
        /// </summary>
        /// <param name="assessmentResourceActivityId"> The assessment resource activity id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAssessmentProgress/activity/{assessmentResourceActivityId}")]
        public async Task<ActionResult> GetAssessmentProgressByActivity(int assessmentResourceActivityId)
        {
            return this.Ok(await this.resourceService.GetAssessmentProgressByActivity(assessmentResourceActivityId));
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
        /// Gets a Whole Slide Image tile.
        /// </summary>
        /// <param name="pathPrefix">File Path of the whole-slide image.</param>
        /// <param name="layer">The layer of the whole-slide image.</param>
        /// <param name="zoomLevel">Zoom level of the whole-slide image.</param>
        /// <param name="x">X coordinate of the whole-slide image.</param>
        /// <param name="y">Y coordinate of the whole-slide image.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("SlideImageTile/{pathPrefix}/{layer}/{zoomLevel}/{x}_{y}.jpg")]
        public async Task<IActionResult> GetWholeSlideImageTile(string pathPrefix, int layer, int zoomLevel, int x, int y)
        {
            string filePath = $"{pathPrefix}/{layer}_files/{zoomLevel}";
            string fileName = $"{x}_{y}.jpg";

            var file = await this.fileService.DownloadFileAsync(filePath, fileName);
            if (file == null)
            {
                string oldFilePath = $"{pathPrefix}/{zoomLevel}";
                file = await this.fileService.DownloadFileAsync(oldFilePath, fileName);
            }

            if (file != null)
            {
                return this.File(file.Content, file.ContentType ?? "application/octet-stream");
            }

            return this.Ok(this.Content("No file found"));
        }

        /// <summary>
        /// The UnpublishResourceVersion.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("UnpublishResourceVersion")]
        public async Task<ActionResult> UnpublishResourceVersion(int id)
        {
            var validationResult = await this.resourceService.UnpublishResourceVersionAsync(id);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The DuplicateResourceAsync.
        /// </summary>
        /// <param name="model">The DuplicateResourceRequestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DuplicateResource/")]
        public async Task<ActionResult> DuplicateResourceAsync(DuplicateResourceRequestModel model)
        {
            var validationResult = await this.resourceService.DuplicateResourceAsync(model);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The DuplicateBlocksAsync.
        /// </summary>
        /// <param name="model">The DuplicateBlocksRequestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DuplicateBlocks/")]
        public async Task<ActionResult> DuplicateBlocksAsync(DuplicateBlocksRequestModel model)
        {
            var validationResult = await this.resourceService.DuplicateBlocksAsync(model);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The GetMyContributionsAsync.
        /// </summary>
        /// <param name="model">The DuplicateBlocksRequestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("MyContributions/")]
        public async Task<ActionResult> GetMyMyContributionsAsync(MyContributionsRequestViewModel model)
        {
            var validationResult = await this.resourceService.GetMyContributionsAsync(model);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The AddResourceProvider.
        /// </summary>
        /// <param name="model">The ResourceVersionProviderViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("AddResourceProvider/")]
        public async Task<ActionResult> AddResourceProviderAsync(ResourceVersionProviderViewModel model)
        {
            var validationResult = await this.resourceService.CreateResourceVersionProviderAsync(model);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The DeleteResourceProvider.
        /// </summary>
        /// <param name="model">The ResourceVersionProviderViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DeleteResourceProvider/")]
        public async Task<ActionResult> DeleteResourceProviderAsync(ResourceVersionProviderViewModel model)
        {
            var validationResult = await this.resourceService.DeleteResourceVersionProviderAsync(model);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The DeleteAllResourceProvider.
        /// </summary>
        /// <param name="resourceVersionId">The reesource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DeleteAllResourceProvider/{resourceVersionId}")]
        public async Task<ActionResult> DeleteResourceProviderAsync(int resourceVersionId)
        {
            var validationResult = await this.resourceService.DeleteAllResourceVersionProviderAsync(resourceVersionId);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The ArchiveResourceFile.
        /// </summary>
        /// <param name="filePaths">filePaths.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost]
        [Route("ArchiveResourceFile")]
        public ActionResult ArchiveResourceFile(List<string> filePaths)
        {
            _ = Task.Run(async () => { await this.fileService.PurgeResourceFile(null, filePaths); });
            return this.Ok();
        }

        /// <summary>
        /// The GetObsoleteResourceFile.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="deletedResource">.</param>
        /// <returns>The <see cref="T:Task{List{FileTypeViewModel}}"/>.</returns>
        [HttpGet]
        [Route("GetObsoleteResourceFile/{resourceVersionId}/{deletedResource}")]
        public async Task<List<string>> GetObsoleteResourceFile(int resourceVersionId, bool deletedResource)
        {
            var result = await this.resourceService.GetObsoleteResourceFile(resourceVersionId, deletedResource);
            return result;
        }
    }
}
