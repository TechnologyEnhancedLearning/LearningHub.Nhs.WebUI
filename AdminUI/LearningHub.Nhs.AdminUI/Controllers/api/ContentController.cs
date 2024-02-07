// <copyright file="ContentController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Content;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.WebUI.Models.Contribute;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// DashboardController.
    /// </summary>
    [Route("api/content")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        /// <summary>
        /// Defines the contentService.
        /// </summary>
        private readonly IContentService contentService;
        private readonly IConfiguration configuration;
        private WebSettings settings;
        private IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentController"/> class.
        /// </summary>
        /// <param name="contentService">contentService.</param>
        /// <param name="configuration">configuration.</param>
        /// <param name="settings">settings.</param>
        /// <param name="fileService">fileService.</param>
        public ContentController(IContentService contentService, IConfiguration configuration, IOptions<WebSettings> settings, IFileService fileService)
        {
            this.contentService = contentService;
            this.configuration = configuration;
            this.fileService = fileService;
            this.settings = settings.Value;
        }

        /// <summary>
        /// The GetSettings.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        [Route("GetSettings")]
        public ActionResult GetSettings()
        {
            UploadSettingsModel settings = new UploadSettingsModel();
            settings.FileUploadSettings = this.settings.FileUploadSettings;
            return this.Ok(settings);
        }

        /// <summary>
        /// The GetPages.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("pages")]
        public async Task<IActionResult> GetPages()
        {
            var responseViewModel = await this.contentService.GetPagesAsync();
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// The GetPageById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("page/{id}")]
        public async Task<IActionResult> GetPageByIdAsync(int id)
        {
            var pageViewModel = await this.contentService.GetPageByIdAsync(id);
            pageViewModel.PreviewUrl = $"{this.settings.LearningHubUrl.TrimEnd('/')}{pageViewModel.PreviewUrl}?preview=true";
            return this.Ok(pageViewModel);
        }

        /// <summary>
        /// Gets page.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("page-all/{id}")]
        public async Task<IActionResult> GetPageWithAllSectionsByIdAsync(int id)
        {
            var pageViewModel = await this.contentService.GetPageByIdAsync(id, true);
            pageViewModel.PreviewUrl = $"{this.settings.LearningHubUrl.TrimEnd('/')}{pageViewModel.PreviewUrl}?preview=true";
            return this.Ok(pageViewModel);
        }

        /// <summary>
        /// Gets page.
        /// </summary>
        /// <param name="pageId">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("discard/{pageId}")]
        public async Task<IActionResult> DiscardAsync(int pageId)
        {
            await this.contentService.DiscardAsync(pageId);
            return this.Ok();
        }

        /// <summary>
        /// Gets page.
        /// </summary>
        /// <param name="pageId">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("publish/{pageId}")]
        public async Task<IActionResult> PublishAsync(int pageId)
        {
            await this.contentService.PublishAsync(pageId);
            return this.Ok();
        }

        /// <summary>
        /// Get pageSection detail by id.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("page-section-detail/{id}")]
        public async Task<IActionResult> GetPageSectionDetailByIdAsync(int id)
        {
            var responseViewModel = await this.contentService.GetPageSectionDetailByIdAsync(id);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// Get editable pageSection detail by section id.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("editable-page-section-detail/{pageSectionId}")]
        public async Task<IActionResult> GetEditablePageSectionDetailByIdAsync(int pageSectionId)
        {
            var responseViewModel = await this.contentService.GetEditablePageSectionDetailByIdAsync(pageSectionId);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// Update page image section detail.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="model">The update model<see cref="PageImageSectionUpdateViewModel"/>.</param>
        /// <param name="imageFile">The image file.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost("page-image-section-detail/{pageid}")]
        public async Task<IActionResult> UpdatePageImageSectionDetailAsync(int pageId, [FromForm] PageImageSectionUpdateViewModel model, [FromForm] IFormFile imageFile)
        {
            await this.contentService.UpdatePageImageSectionDetailAsync(pageId, model, imageFile);
            return this.Ok(true);
        }

        /// <summary>
        /// Update PageSectionDetail.
        /// </summary>
        /// <param name="updateViewModel">PageSectionDetailViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("update-page-section-detail")]
        public async Task<IActionResult> UpdatePageSectionDetailAsync([FromBody] PageSectionDetailViewModel updateViewModel)
        {
            await this.contentService.UpdatePageSectionDetailAsync(updateViewModel);
            return this.Ok(true);
        }

        /// <summary>
        /// Clone page section.
        /// </summary>
        /// <param name="requestViewModel">The id<see cref="requestViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("change-order")]
        public async Task<IActionResult> ChangeOrderAsync([FromBody] UpdatePageSectionOrderModel requestViewModel)
        {
            await this.contentService.ChangeOrderAsync(requestViewModel);
            return this.Ok();
        }

        /// <summary>
        /// CloneAsync.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("clone/{pageSectionId}")]
        public async Task<IActionResult> CloneAsync(int pageSectionId)
        {
            await this.contentService.CloneAsync(pageSectionId);
            return this.Ok();
        }

        /// <summary>
        /// Hide page section.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="pageSectionId"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("hide/{pageSectionId}")]
        public async Task<IActionResult> HideAsync(int pageSectionId)
        {
            await this.contentService.HideAsync(pageSectionId);
            return this.Ok();
        }

        /// <summary>
        /// Unhide page section.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="pageSectionId"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("unhide/{pageSectionId}")]
        public async Task<IActionResult> UnHideAsync(int pageSectionId)
        {
            await this.contentService.UnHideAsync(pageSectionId);
            return this.Ok();
        }

        /// <summary>
        /// Unhide page section.
        /// </summary>
        /// <param name="pageSectionId">The page section id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("delete/{pageSectionId}")]
        public async Task<IActionResult> DeleteAsync(int pageSectionId)
        {
            await this.contentService.DeleteAsync(pageSectionId);
            return this.Ok();
        }

        /// <summary>
        /// The CreatePageSectionAsync.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>The page section id.</returns>
        [HttpPost("create-page-section")]
        public async Task<IActionResult> CreatePageSectionAsync(PageSectionViewModel requestViewModel)
        {
            var pageSectionId = await this.contentService.CreatePageSectionAsync(requestViewModel);
            return this.Ok(pageSectionId);
        }

        /// <summary>
        /// The UploadFile.
        /// </summary>
        /// <param name="inputForm">Input form.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("upload-file")]
        public async Task<FileUploadResult> UploadFile(IFormCollection inputForm)
        {
            try
            {
                var file = inputForm.Files[0];
                int pageSectionDetailId = 0;
                int.TryParse(inputForm["pageSectionDetailId"], out pageSectionDetailId);
                var currentUserId = this.User.Identity.GetCurrentUserId();
                return await this.contentService.ProcessUploadFile(pageSectionDetailId, file, currentUserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The UploadAttachedFile.
        /// </summary>
        /// <param name="inputForm">Input form.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("upload-attached-file")]
        public async Task<FileUploadResult> UploadAttachedFile(IFormCollection inputForm)
        {
            try
            {
                var file = inputForm.Files[0];
                int pageSectionDetailId = 0;
                int.TryParse(inputForm["pageSectionDetailId"], out pageSectionDetailId);
                int attachedFileType = 0;
                int.TryParse(inputForm["attachedFileType"], out attachedFileType);
                var currentUserId = this.User.Identity.GetCurrentUserId();
                return await this.contentService.ProcessAttachedFileAsync(pageSectionDetailId, file, attachedFileType, currentUserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The UploadFileChunk.
        /// </summary>
        /// <param name="inputForm">Input form.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("upload-file-chunk")]
        public async Task<FileChunkUploadResult> UploadFileChunk(IFormCollection inputForm)
        {
            try
            {
                int fileChunkDetailId = 0;
                int.TryParse(inputForm["fileChunkDetailId"], out fileChunkDetailId);
                int pageSectionDetailId = 0;
                int.TryParse(inputForm["pageSectionDetailId"], out pageSectionDetailId);
                int chunkCount = 0;
                int.TryParse(inputForm["chunkCount"], out chunkCount);
                int chunkIndex = 0;
                int.TryParse(inputForm["chunkIndex"], out chunkIndex);
                string fileName = inputForm["fileName"];
                int fileSize = 0;
                int.TryParse(inputForm["fileSize"], out fileSize);
                var file = inputForm.Files[0];
                var currentUserId = this.User.Identity.GetCurrentUserId();

                return await this.contentService.UploadFileChunkAsync(fileChunkDetailId, pageSectionDetailId, chunkCount, chunkIndex, fileName, file, fileSize, currentUserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The RegisterChunkedFileAsync.
        /// </summary>
        /// <param name="fileChunkRegisterModel">File chunk register model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("register-chunked-file")]
        public async Task<FileUploadResult> RegisterChunkedFileAsync([FromBody] FileChunkRegisterModel fileChunkRegisterModel)
        {
            var currentUserId = this.User.Identity.GetCurrentUserId();
            return await this.contentService.RegisterChunkedFileAsync(fileChunkRegisterModel, currentUserId);
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
            FileChunkDetailViewModel fileChunkDetail = await this.contentService.GetFileChunkDetail(fileChunkProcessModel.FileChunkDetailId);
            var currentUserId = this.User.Identity.GetCurrentUserId();
            if (fileChunkDetail.CreateUserId != currentUserId)
            {
                return this.Unauthorized();
            }

            await this.contentService.DeleteFileChunkDetailAsync(fileChunkProcessModel.FileChunkDetailId);
            await this.fileService.DeleteChunkDirectory(fileChunkDetail.FilePath, fileChunkDetail.ChunkCount);

            return this.Ok();
        }

        /// <summary>
        /// The UpdateVideoAssetAsync.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("update-video-asset")]
        public async Task<ActionResult> UpdateVideoAssetAsync([FromBody] VideoAssetViewModel model)
        {
            var response = await this.contentService.UpdateVideoAssetAsync(model);
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
        /// Get pageSection detail by id.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("page-section-detail-video/{id}")]
        public async Task<IActionResult> GetPageSectionDetailVideoAssetByIdAsync(int id)
        {
            var responseViewModel = await this.contentService.GetPageSectionDetailVideoAssetByIdAsync(id);
            return this.Ok(responseViewModel);
        }
    }
}
