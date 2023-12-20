// <copyright file="ContentController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Content;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The ContentController.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/content")]
    [ApiController]
    public class ContentController : ApiControllerBase
    {
        /// <summary>
        /// Defines the pageService.
        /// </summary>
        private readonly IPageService pageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentController"/> class.
        /// </summary>
        /// <param name="userService">userService.</param>
        /// <param name="pageService">pageService.</param>
        /// <param name="logger">The logger.</param>
        public ContentController(IUserService userService, IPageService pageService, ILogger<ContentController> logger)
            : base(userService, logger)
        {
            this.pageService = pageService;
        }

        /// <summary>
        /// Gets pages.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("pages")]
        public async Task<IActionResult> GetPagesAsync()
        {
            var response = await this.pageService.GetPagesAsync();
            return this.Ok(response);
        }

        /// <summary>
        /// Gets page.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="publishedOnly">The published only<see cref="bool"/>.</param>
        /// <param name="preview">Return page sections seen in preview mode.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("page/{id}")]
        public async Task<IActionResult> GetPageByIdAsync(int id, bool publishedOnly = false, bool preview = false)
        {
            var response = await this.pageService.GetPageByIdAsync(id, publishedOnly: publishedOnly, preview: preview);
            return this.Ok(response);
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
            var response = await this.pageService.GetPageByIdAsync(id, true);
            return this.Ok(response);
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
            await this.pageService.DiscardAsync(pageId, this.CurrentUserId);
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
            await this.pageService.PublishAsync(pageId, this.CurrentUserId);
            return this.Ok();
        }

        /// <summary>
        /// Clone page section.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("clone/{pageSectionId}")]
        public async Task<IActionResult> CloneAsync(int pageSectionId)
        {
            await this.pageService.CloneAsync(pageSectionId, this.CurrentUserId);
            return this.Ok();
        }

        /// <summary>
        /// Clone page section.
        /// </summary>
        /// <param name="requestViewModel">The id<see cref="UpdatePageSectionOrderModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("change-order")]
        public async Task<IActionResult> ChangeOrderAsync([FromBody] UpdatePageSectionOrderModel requestViewModel)
        {
            await this.pageService.ChangeOrderAsync(requestViewModel, this.CurrentUserId);
            return this.Ok();
        }

        /// <summary>
        /// Hide page section.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("hide/{pageSectionId}")]
        public async Task<IActionResult> HideAsync(int pageSectionId)
        {
            await this.pageService.HideAsync(pageSectionId, this.CurrentUserId);
            return this.Ok();
        }

        /// <summary>
        /// Unhide page section.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("unhide/{pageSectionId}")]
        public async Task<IActionResult> UnHideAsync(int pageSectionId)
        {
            await this.pageService.UnHideAsync(pageSectionId, this.CurrentUserId);
            return this.Ok();
        }

        /// <summary>
        /// Unhide page section.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("delete/{pageSectionId}")]
        public async Task<IActionResult> DeleteAsync(int pageSectionId)
        {
            await this.pageService.DeleteAsync(pageSectionId, this.CurrentUserId);
            return this.Ok();
        }

        /// <summary>
        /// The GetPageSectionById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("page-section/{id}")]
        public async Task<IActionResult> GetPageSectionByDetailIdAsync(int id)
        {
            var responseViewModel = await this.pageService.GetPageSectionByIdAsync(id);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("page-section-detail/{id}")]
        public async Task<IActionResult> GetPageSectionDetailByIdAsync(int id)
        {
            var responseViewModel = await this.pageService.GetPageSectionDetailImageAssetByIdAsync(id);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("editable-page-section-detail/{id}")]
        public async Task<IActionResult> GetEditablePageSectionDetailByPageSectionIdAsync(int id)
        {
            var responseViewModel = await this.pageService.GetEditablePageSectionDetailByPageSectionIdAsync(id, this.CurrentUserId);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// The GetPageSectionDetailVideoById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("page-section-detail-video/{id}")]
        public async Task<IActionResult> GetPageSectionDetailVideoAssetByIdAsync(int id)
        {
            var responseViewModel = await this.pageService.GetPageSectionDetailVideoAssetByIdAsync(id);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// The CreatePageSectionAsync.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost("create-page-section")]
        public async Task<IActionResult> CreatePageSectionAsync(PageSectionViewModel requestViewModel)
        {
            var pageSectionId = await this.pageService.CreatePageSectionAsync(requestViewModel, this.CurrentUserId);
            return this.Ok(pageSectionId);
        }

        /// <summary>
        /// Update page image section detail.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="model">The update model<see cref="PageImageSectionUpdateViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost("page-image-section-detail/{pageid}")]
        public async Task<IActionResult> UpdatePageImageSectionDetailAsync(int pageId, PageImageSectionUpdateViewModel model)
        {
            await this.pageService.UpdatePageImageSectionDetailAsync(pageId, model, this.CurrentUserId);
            return this.Ok();
        }

        /// <summary>
        /// The save video asset async.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("save-video-asset")]
        public async Task<IActionResult> SaveVideoAssetAsync(FileCreateRequestViewModel requestViewModel)
        {
            var vr = await this.pageService.SaveVideoAssetAsync(requestViewModel, this.CurrentUserId);
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
        /// The save file details for a video asset attribute async.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("save-attribute-file")]
        public async Task<IActionResult> SaveAttributeFileDetails(FileCreateRequestViewModel requestViewModel)
        {
            var vr = await this.pageService.SaveAttributeFileDetails(requestViewModel, this.CurrentUserId);
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
        /// The UpdateVideoAssetAsync.
        /// </summary>
        /// <param name="videoAssetStateViewModel">videoAssetStateViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("update-video-asset-state")]
        public async Task<IActionResult> UpdateVideoAssetStateAsync(UpdateVideoAssetStateViewModel videoAssetStateViewModel)
        {
            await this.pageService.UpdateVideoAssetStateAsync(videoAssetStateViewModel);
            return this.Ok(new ApiResponse(true, new LearningHubValidationResult { IsValid = true }));
        }

        /// <summary>
        /// The UpdateVideoAssetManifestDetailsAsync.
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="UpdateVideoAssetManifestRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("update-video-asset-manifest")]
        public async Task<IActionResult> UpdateVideoAssetManifestDetailsAsync(UpdateVideoAssetManifestRequestViewModel viewModel)
        {
            await this.pageService.UpdateVideoAssetManifestDetailsAsync(viewModel);
            return this.Ok(new ApiResponse(true, new LearningHubValidationResult { IsValid = true }));
        }

        /// <summary>
        /// The UpdateVideoAssetAsync.
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="VideoAssetViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("update-video-asset")]
        public async Task<IActionResult> UpdateVideoAssetAsync(VideoAssetViewModel viewModel)
        {
            viewModel.AmendUserId = this.CurrentUserId;
            await this.pageService.UpdateVideoAssetAsync(viewModel);
            return this.Ok(new ApiResponse(true, new LearningHubValidationResult { IsValid = true }));
        }

        /// <summary>
        /// save page section details.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="PageSectionDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut]
        [Route("update-page-section-detail")]
        public async Task<IActionResult> UpdatePageSectionDetailsAsync(PageSectionDetailViewModel requestViewModel)
        {
            await this.pageService.UpdatePageSectionDetailsAsync(requestViewModel, this.CurrentUserId);
            return this.Ok();
        }
    }
}
