namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Content;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Defines the <see cref="IContentService" />.
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        /// The GetPagesAsync.
        /// </summary>
        /// <returns>The <see cref="Task{PageResultViewModel}"/>.</returns>
        Task<PageResultViewModel> GetPagesAsync();

        /// <summary>
        /// The GetPageByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="includeHidden">includeHidden.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        Task<PageViewModel> GetPageByIdAsync(int id, bool includeHidden = false);

        /// <summary>
        /// The DiscardAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DiscardAsync(int pageId);

        /// <summary>
        /// The PublishAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task PublishAsync(int pageId);

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        Task<PageSectionDetailViewModel> GetPageSectionDetailByIdAsync(int id);

        /// <summary>
        /// The GetEditablePageSectionDetailByIdAsync.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        Task<PageSectionDetailViewModel> GetEditablePageSectionDetailByIdAsync(int pageSectionId);

        /// <summary>
        /// Update page image section detail.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="model">The update model<see cref="PageImageSectionUpdateViewModel"/>.</param>
        /// <param name="imageFile">The image file.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task UpdatePageImageSectionDetailAsync(int pageId, PageImageSectionUpdateViewModel model, IFormFile imageFile);

        /// <summary>
        /// The ChangeOrderAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ChangeOrderAsync(UpdatePageSectionOrderModel requestViewModel);

        /// <summary>
        /// The CloneAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task CloneAsync(int pageSectionId);

        /// <summary>
        /// The HideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HideAsync(int pageSectionId);

        /// <summary>
        /// The UnHideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UnHideAsync(int pageSectionId);

        /// <summary>
        /// The DeleteAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int pageSectionId);

        /// <summary>
        /// The ProcessUploadFile.
        /// </summary>
        /// <param name="pageSectionDetailId">The pageSectionDetailId<see cref="int"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        Task<FileUploadResult> ProcessUploadFile(int pageSectionDetailId, IFormFile file, int currentUserId);

        /// <summary>
        /// The UploadFileChunkAsync.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <param name="pageSectionDetailId">The pageSectionDetailId<see cref="int"/>.</param>
        /// <param name="chunkCount">The chunkCount<see cref="int"/>.</param>
        /// <param name="chunkIndex">The chunkIndex<see cref="int"/>.</param>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="fileSize">The fileSize<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileChunkUploadResult}"/>.</returns>
        Task<FileChunkUploadResult> UploadFileChunkAsync(int fileChunkDetailId, int pageSectionDetailId, int chunkCount, int chunkIndex, string fileName, IFormFile file, int fileSize, int currentUserId);

        /// <summary>
        /// The ProcessAttachedFileAsync.
        /// </summary>
        /// <param name="pageSectionDetailId">The pageSectionDetailId<see cref="int"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="attachedFileType">The attachedFileType<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        Task<FileUploadResult> ProcessAttachedFileAsync(int pageSectionDetailId, IFormFile file, int attachedFileType, int currentUserId);

        /// <summary>
        /// UpdatePageSectionDetail.
        /// </summary>
        /// <param name="updateViewModel">updateViewModel.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdatePageSectionDetailAsync(PageSectionDetailViewModel updateViewModel);

        /// <summary>
        /// The RegisterChunkedFileAsync.
        /// </summary>
        /// <param name="fileChunkRegisterModel">The fileChunkRegisterModel<see cref="FileChunkRegisterModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        Task<FileUploadResult> RegisterChunkedFileAsync(FileChunkRegisterModel fileChunkRegisterModel, int currentUserId);

        /// <summary>
        /// The GetFileChunkDetail.
        /// </summary>
        /// <param name="fileChunkDetailId">File chunk detail id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<FileChunkDetailViewModel> GetFileChunkDetail(int fileChunkDetailId);

        /// <summary>
        /// The DeleteFileChunkDetailAsync.
        /// </summary>
        /// <param name="fileChunkDetailId">File chunk detail id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteFileChunkDetailAsync(int fileChunkDetailId);

        /// <summary>
        /// The CreatePageSectionAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="PageSectionViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> CreatePageSectionAsync(PageSectionViewModel requestViewModel);

        /// <summary>
        /// GetPageSectionDetailVideoAssetById.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<PageSectionDetailViewModel> GetPageSectionDetailVideoAssetByIdAsync(int id);

        /// <summary>
        /// The UpdateVideoAssetAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdateVideoAssetAsync(VideoAssetViewModel model);
    }
}
