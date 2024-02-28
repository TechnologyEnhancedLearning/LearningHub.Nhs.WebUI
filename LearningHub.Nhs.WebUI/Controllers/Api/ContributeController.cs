// <copyright file="ContributeController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.Files;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Contribute;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="ContributeController" />.
    /// </summary>
    [Authorize(Roles = "Administrator,BlueUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContributeController : BaseApiController
    {
        private readonly ICatalogueService catalogueService;
        private readonly IContributeService contributeService;
        private readonly IFileService fileService;
        private readonly IPartialFileUploadService partialFileUploadService;
        private readonly IResourceService resourceService;
        private readonly Configuration.Settings settings;
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContributeController"/> class.
        /// </summary>
        /// <param name="userService">User service.</param>
        /// <param name="contributeService">Contribute service.</param>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="fileService">File service.</param>
        /// <param name="partialFileUploadService">Partial file upload service.</param>
        /// <param name="catalogueService">Catalogue service.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="logger">Logger.</param>
        public ContributeController(
            IUserService userService,
            IContributeService contributeService,
            IResourceService resourceService,
            IFileService fileService,
            IPartialFileUploadService partialFileUploadService,
            ICatalogueService catalogueService,
            IOptions<Configuration.Settings> settings,
            ILogger<ContributeController> logger)
            : base(logger)
        {
            this.userService = userService;
            this.contributeService = contributeService;
            this.resourceService = resourceService;
            this.fileService = fileService;
            this.partialFileUploadService = partialFileUploadService;
            this.catalogueService = catalogueService;
            this.settings = settings.Value;
        }

        /// <summary>
        /// The AddResourceAuthorAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("AddResourceAuthor")]
        public async Task<ActionResult> AddResourceAuthorAsync([FromBody] ResourceAuthorViewModel model)
        {
            if (model.ResourceVersionId == 0)
            {
                model.ResourceVersionId = await this.contributeService.SaveResourceDetailAsync(
                    new ResourceDetailViewModel()
                    {
                        ResourceType = ResourceTypeEnum.Undefined,
                        Title = string.Empty,
                    });
            }

            model.Id = await this.contributeService.CreateResourceAuthorAsync(model);
            return this.Ok(model);
        }

        /// <summary>
        /// The AddResourceKeywordAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("AddResourceKeyword")]
        public async Task<ActionResult> AddResourceKeywordAsync([FromBody] ResourceKeywordViewModel model)
        {
            if (model.ResourceVersionId == 0)
            {
                model.ResourceVersionId = await this.contributeService.SaveResourceDetailAsync(
                    new ResourceDetailViewModel()
                    {
                        ResourceType = ResourceTypeEnum.Undefined,
                        Title = string.Empty,
                    });
            }

            model.Id = await this.contributeService.CreateResourceKeywordAsync(model);
            return this.Ok(model);
        }

        /// <summary>
        /// The CancelChunkedFileAsync.
        /// </summary>
        /// <param name="fileChunkProcessModel">File chunk process model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("CancelChunkedFile")]
        public async Task<ActionResult> CancelChunkedFileAsync([FromBody] FileChunkProcessModel fileChunkProcessModel)
        {
            FileChunkDetailViewModel fileChunkDetail = await this.contributeService.GetFileChunkDetail(fileChunkProcessModel.FileChunkDetailId);

            if (fileChunkDetail.CreateUserId != this.CurrentUserId)
            {
                return this.Unauthorized();
            }

            await this.contributeService.DeleteFileChunkDetailAsync(fileChunkProcessModel.FileChunkDetailId);
            await this.fileService.DeleteChunkDirectory(fileChunkDetail.FilePath, fileChunkDetail.ChunkCount);

            return this.Ok();
        }

        /// <summary>
        /// The DeleteArticleFileAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DeleteArticleFile")]
        public async Task<ActionResult> DeleteArticleFileAsync([FromBody] FileDeleteRequestModel model)
        {
            var response = await this.contributeService.DeleteArticleFileAsync(model);
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
        /// The DeleteResourceAttributeFileAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DeleteResourceAttributeFile")]
        public async Task<ActionResult> DeleteResourceAttributeFileAsync([FromBody] FileDeleteRequestModel model)
        {
            var response = await this.contributeService.DeleteResourceAttributeFileAsync(model);
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
        /// The DeleteResourceAuthorAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DeleteResourceAuthor")]
        public async Task<ActionResult> DeleteResourceAuthorAsync([FromBody] AuthorDeleteRequestModel model)
        {
            var response = await this.contributeService.DeleteResourceAuthorAsync(model);

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
        /// The DeleteResourceKeywordAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("DeleteResourceKeyword")]
        public async Task<ActionResult> DeleteResourceKeywordAsync([FromBody] KeywordDeleteRequestModel model)
        {
            var response = await this.contributeService.DeleteResourceKeywordAsync(model);

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
        /// The DeleteResourceVersion.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete]
        [Route("DeleteResourceVersion/{resourceversionId}")]
        public async Task<ActionResult> DeleteResourceVersion(int resourceVersionId)
        {
            var validationResult = await this.contributeService.DeleteResourceVersionAsync(resourceVersionId);
            return this.Ok(validationResult);
        }

        /// <summary>
        /// The GetConfigurationAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetConfiguration")]
        public async Task<ActionResult> GetConfigurationAsync()
        {
            var user = await this.userService.GetCurrentUserAsync();
            var configuration = new ConfigurationModel
            {
                CurrentUserName = $"{user.FirstName} {user.LastName}",
            };
            return this.Ok(configuration);
        }

        /// <summary>
        /// The GetCataloguesForUserAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetCataloguesForUser")]
        public async Task<ActionResult> GetCataloguesForUserAsync()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("AccessDenied", "Home");
            }

            return this.Ok(await this.catalogueService.GetCataloguesForUserAsync());
        }

        /// <summary>
        /// The GetFileTypeAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetFileType")]
        public async Task<ActionResult> GetFileTypeAsync()
        {
            return this.Ok(await this.resourceService.GetFileTypeAsync());
        }

        /// <summary>
        /// The GetLicencesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetLicences")]
        public async Task<ActionResult> GetLicencesAsync()
        {
            return this.Ok(await this.resourceService.GetLicencesAsync());
        }

        /// <summary>
        /// The GetResourceVersionByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetResourceVersionById/{id}")]
        public async Task<ActionResult> GetResourceVersionByIdAsync(int id)
        {
            var resourceVersion = await this.resourceService.GetResourceVersionAsync(id);
            if ((resourceVersion != null && resourceVersion.CreateUserId == this.CurrentUserId) || await this.UserCanEditCatalogue((int)resourceVersion.ResourceCatalogueId))
            {
                if (resourceVersion.VersionStatusEnum == VersionStatusEnum.Draft)
                {
                    return this.Ok(resourceVersion);
                }
                else
                {
                    return this.NoContent();
                }
            }
            else
            {
                return this.Forbid();
            }
        }

        /// <summary>
        /// The GetSettings.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        [Route("GetSettings")]
        public ActionResult GetSettings()
        {
            var settings = new ContributeSettingsModel { FileUploadSettings = this.settings.FileUploadSettings };
            return this.Ok(settings);
        }

        /// <summary>
        /// The PublishResourceVersionAsync.
        /// </summary>
        /// <param name="publishViewModel">Publish view model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("PublishResourceVersion")]
        public async Task<ActionResult> PublishResourceVersionAsync([FromBody] PublishViewModel publishViewModel)
        {
            var validationResult = await this.contributeService.SubmitResourceVersionForPublishAsync(publishViewModel);

            return this.Ok(validationResult);
        }

        /// <summary>
        /// The RegisterChunkedFileAsync.
        /// </summary>
        /// <param name="fileChunkRegisterModel">File chunk register model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("RegisterChunkedFile")]
        public async Task<FileUploadResult> RegisterChunkedFileAsync([FromBody] FileChunkRegisterModel fileChunkRegisterModel)
        {
            return await this.contributeService.RegisterChunkedFileAsync(fileChunkRegisterModel, this.CurrentUserId);
        }

        /// <summary>
        /// The SaveArticleDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveArticleDetail")]
        public async Task<ActionResult> SaveArticleDetailAsync([FromBody] ArticleUpdateRequestViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveArticleDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveGenericFileDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveGenericFileDetail")]
        public async Task<ActionResult> SaveGenericFileDetailAsync([FromBody] GenericFileUpdateRequestViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveGenericFileDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveScormDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveScormDetail")]
        public async Task<ActionResult> SaveScormDetailAsync([FromBody] ScormUpdateRequestViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveScormDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveHtmlDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveHtmlDetail")]
        public async Task<ActionResult> SaveHtmlDetailAsync([FromBody] HtmlResourceUpdateRequestViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveHtmlDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveImageDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveImageDetail")]
        public async Task<ActionResult> SaveImageDetailAsync([FromBody] ImageUpdateRequestViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveImageDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveResourceDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveResourceDetail")]
        public async Task<ActionResult> SaveResourceDetailAsync([FromBody] ResourceDetailViewModel request)
        {
            if (request == null)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            request.Title = request.Title.Trim();
            request.AdditionalInformation = request.AdditionalInformation.Trim();

            request.Description = Regex.Replace(request.Description, "&nbsp;", string.Empty);
            request.Description = Regex.Replace(request.Description, @" {2}", string.Empty);
            request.Description = Regex.Replace(request.Description, " </p>", "</p>");
            request.Description = Regex.Replace(request.Description, "<p> ", "<p>");
            request.Description = Regex.Replace(request.Description, "<p></p>", string.Empty);
            request.Description = Regex.Replace(request.Description, "\\n", string.Empty);

            int resourceVersionId = await this.contributeService.SaveResourceDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveWeblinkDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveWeblinkDetail")]
        public async Task<ActionResult> SaveWeblinkDetailAsync([FromBody] WebLinkViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveWeblinkDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveCaseDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveCaseDetail")]
        public async Task<ActionResult> SaveCaseDetailAsync([FromBody] CaseViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveCaseDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The SaveAssessmentDetailAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("SaveAssessmentDetail")]
        public async Task<ActionResult> SaveAssessmentDetailAsync([FromBody] AssessmentViewModel request)
        {
            int resourceVersionId = await this.contributeService.SaveAssessmentDetailAsync(request);
            return this.Ok(resourceVersionId);
        }

        /// <summary>
        /// The CreatePartialFileAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("CreatePartialFile")]
        public async Task<ActionResult> CreatePartialFileAsync([FromBody] PartialFileViewModel request)
        {
            // Create new PartialFileViewModel, copying across only the properties that are allowed
            var newPartialFile = new PartialFileViewModel
            {
                FileName = request.FileName,
                TotalFileSize = request.TotalFileSize,
                PostProcessingOptions = request.PostProcessingOptions,
            };

            PartialFileViewModel savedPartialFile = await this.partialFileUploadService.CreatePartialFile(newPartialFile);
            return this.Ok(savedPartialFile);
        }

        /// <summary>
        /// The UrlIsAccessibleAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("UrlIsAccessible")]
        public async Task<bool> UrlIsAccessibleAsync([FromBody] UrlValidationModel model)
        {
            if (model.Url == null)
            {
                return false;
            }

            model.Url = HttpUtility.UrlDecode(model.Url);

            if (!Uri.IsWellFormedUriString(model.Url, UriKind.Absolute))
            {
                this.Logger.LogInformation("UrlIsAccessible check. NotWellFormatted: {url}. UserId: {userId}", model.Url, this.CurrentUserId.ToString());
                return false;
            }

            // Try a HEAD request. Some web servers don't support it.
            if (await this.CheckUrl(model.Url, "HEAD"))
            {
                return true;
            }

            // Fall back to a GET request. Request is aborted to prevent downloading the whole resource.
            return await this.CheckUrl(model.Url, "GET");
        }

        /// <summary>
        /// The CheckUrl.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="httpMethod">The httpMethod.</param>
        /// <returns>The <see cref="T:Task{bool}"/>.</returns>
        private async Task<bool> CheckUrl(string url, string httpMethod)
        {
            // Remove any trailing bookmark section from Url
            int idx = url.LastIndexOf('#');
            if (idx > -1)
            {
                url = url.Substring(0, idx);
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = httpMethod;

            // Some web servers return error code if useragent is null. We're using a realistic looking one, but anything non-null seems to fool them.
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36";

            try
            {
                using (var response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    var statusCode = (response != null) ? (int)response.StatusCode : 0;
                    request.Abort();  // call ASAP to kill connection
                    response.Close();

                    if (response != null && statusCode >= 200 && statusCode <= 399)
                    {
                        return true;
                    }
                    else
                    {
                        if (response == null)
                        {
                            this.Logger.LogInformation($"UrlIsAccessible check. URL: {url}. {httpMethod} response is null! UserId: {this.CurrentUserId}");
                        }
                        else
                        {
                            this.Logger.LogInformation($"UrlIsAccessible check.  URL: {url}. {httpMethod} response.StatusCode: {statusCode}. UserId: {this.CurrentUserId}");
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                this.Logger.LogInformation($"UrlIsAccessible check. URL: {url}. {httpMethod} response ERROR: {ex.Message}, UserId: {this.CurrentUserId}.");

                if (ex.Message.Contains("(403) Forbidden"))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> UserCanEditCatalogue(int catalogueId)
        {
            return await this.catalogueService.CanCurrentUserEditCatalogue(catalogueId);
        }
    }
}
