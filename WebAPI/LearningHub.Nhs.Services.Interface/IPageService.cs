// <copyright file="IPageService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Content;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// IDashboardService.
    /// </summary>
    public interface IPageService
    {
        /// <summary>
        /// GetPagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PageResultViewModel> GetPagesAsync();

        /// <summary>
        /// The GetPageById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="includeHidden">includeHidden.</param>
        /// <param name="publishedOnly">The published only<see cref="bool"/>.</param>
        /// <param name="preview">Preview mode.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        Task<PageViewModel> GetPageByIdAsync(int id, bool includeHidden = false, bool publishedOnly = false, bool preview = false);

        /// <summary>
        /// The GetPageSectionById.
        /// </summary>
        /// <param name="id">The page section id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionViewModel}"/>.</returns>
        Task<PageSectionViewModel> GetPageSectionByIdAsync(int id);

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetailViewModel}"/>.</returns>
        Task<PageSectionDetailViewModel> GetPageSectionDetailImageAssetByIdAsync(int id);

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="pageSectionDetailId">The id<see cref="int"/>.</param>
        /// <param name="currentUserId">The userid<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetailViewModel}"/>.</returns>
        Task<PageSectionDetailViewModel> GetEditablePageSectionDetailByPageSectionIdAsync(int pageSectionDetailId, int currentUserId);

        /// <summary>
        /// The GetPageSectionDetailVideoAsset.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetailViewModel}"/>.</returns>
        Task<PageSectionDetailViewModel> GetPageSectionDetailVideoAssetByIdAsync(int id);

        /// <summary>
        /// The DiscardAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DiscardAsync(int pageId, int currentUserId);

        /// <summary>
        /// The PublishAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task PublishAsync(int pageId, int currentUserId);

        /// <summary>
        /// Update page image section detail.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="model">The update model<see cref="PageImageSectionUpdateViewModel"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task UpdatePageImageSectionDetailAsync(int pageId, PageImageSectionUpdateViewModel model, int currentUserId);

        /// <summary>
        /// The CloneAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task CloneAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The HideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HideAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The UnHideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UnHideAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The DeleteAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The ChangeOrderAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ChangeOrderAsync(UpdatePageSectionOrderModel requestViewModel, int currentUserId);

        /// <summary>
        /// The CreatePageSectionAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="PageSectionViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The page section id.</returns>
        Task<int> CreatePageSectionAsync(PageSectionViewModel requestViewModel, int currentUserId);

        /// <summary>
        /// The SaveVideoAssetAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> SaveVideoAssetAsync(FileCreateRequestViewModel requestViewModel, int currentUserId);

        /// <summary>
        /// The UpdateVideoAssetManifestDetailsAsync.
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="UpdateVideoAssetManifestRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateVideoAssetManifestDetailsAsync(UpdateVideoAssetManifestRequestViewModel viewModel);

        /// <summary>
        /// The UpdateVideoAssetAsync.
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="VideoAssetViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateVideoAssetAsync(VideoAssetViewModel viewModel);

        /// <summary>
        /// The SaveAttributeFileDetails.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> SaveAttributeFileDetails(FileCreateRequestViewModel requestViewModel, int currentUserId);

        /// <summary>
        /// The UpdateVideoAssetStateAsync.
        /// </summary>
        /// <param name="videoAssetStateViewModel">videoAssetStateViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task UpdateVideoAssetStateAsync(UpdateVideoAssetStateViewModel videoAssetStateViewModel);

        /// <summary>
        /// The UpdatePageSectionDetailsAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="PageSectionDetailViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdatePageSectionDetailsAsync(PageSectionDetailViewModel requestViewModel, int currentUserId);
    }
}
