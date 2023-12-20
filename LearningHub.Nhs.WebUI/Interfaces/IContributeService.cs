// <copyright file="IContributeService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Models.Contribute;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Defines the <see cref="IContributeService" />.
    /// </summary>
    public interface IContributeService
    {
        /// <summary>
        /// The CreateNewResourceVersionAsync.
        /// </summary>
        /// <param name="resourceId">Resource id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> CreateNewResourceVersionAsync(int resourceId);

        /// <summary>
        /// The CreateResourceAuthorAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> CreateResourceAuthorAsync(ResourceAuthorViewModel model);

        /// <summary>
        /// The CreateResourceKeywordAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> CreateResourceKeywordAsync(ResourceKeywordViewModel model);

        /// <summary>
        /// The DeleteArticleFileAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteArticleFileAsync(FileDeleteRequestModel model);

        /// <summary>
        /// The DeleteFileChunkDetailAsync.
        /// </summary>
        /// <param name="fileChunkDetailId">File chunk detail id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteFileChunkDetailAsync(int fileChunkDetailId);

        /// <summary>
        /// The DeleteResourceAttributeFileAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteResourceAttributeFileAsync(FileDeleteRequestModel model);

        /// <summary>
        /// The DeleteResourceAuthorAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteResourceAuthorAsync(AuthorDeleteRequestModel model);

        /// <summary>
        /// The DeleteResourceKeywordAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteResourceKeywordAsync(KeywordDeleteRequestModel model);

        /// <summary>
        /// The DeleteResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionAsync(int resourceVersionId);

        /// <summary>
        /// The GetFileChunkDetail.
        /// </summary>
        /// <param name="fileChunkDetailId">File chunk detail id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<FileChunkDetailViewModel> GetFileChunkDetail(int fileChunkDetailId);

        /// <summary>
        /// The ProcessArticleFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <param name="file">File.</param>
        /// <param name="changeingFileId">Changing file id.</param>
        /// <param name="currentUserId">Current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<FileUploadResult> ProcessArticleFileAsync(int resourceVersionId, IFormFile file, int changeingFileId, int currentUserId);

        /// <summary>
        /// The ProcessResourceAttachedFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <param name="file">File.</param>
        /// <param name="resourceType">Resource type.</param>
        /// <param name="attachedFileType">Attached file type.</param>
        /// <param name="currentUserId">Current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<FileUploadResult> ProcessResourceAttachedFileAsync(int resourceVersionId, IFormFile file, int resourceType, int attachedFileType, int currentUserId);

        /// <summary>
        /// The ProcessResourceFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <param name="file">File.</param>
        /// <param name="currentUserId">Current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<FileUploadResult> ProcessResourceFileAsync(int resourceVersionId, IFormFile file, int currentUserId);

        /// <summary>
        /// The RegisterChunkedFileAsync.
        /// </summary>
        /// <param name="fileChunkRegisterModel">File chunk register model.</param>
        /// <param name="currentUserId">Current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<FileUploadResult> RegisterChunkedFileAsync(FileChunkRegisterModel fileChunkRegisterModel, int currentUserId);

        /// <summary>
        /// The SaveArticleAttachedFileDetailsAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">File create request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveArticleAttachedFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel);

        /// <summary>
        /// The SaveArticleDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveArticleDetailAsync(ArticleUpdateRequestViewModel model);

        /// <summary>
        /// The SaveFileChunkDetailsAsync.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">File chunk detail create request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveFileChunkDetailsAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel);

        /// <summary>
        /// The SaveFileDetailsAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">File create requrest view model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel);

        /// <summary>
        /// The SaveGenericFileDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveGenericFileDetailAsync(GenericFileUpdateRequestViewModel model);

        /// <summary>
        /// The SaveScormDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveScormDetailAsync(ScormUpdateRequestViewModel model);

        /// <summary>
        /// The SaveImageDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveImageDetailAsync(ImageUpdateRequestViewModel model);

        /// <summary>
        /// The SaveResourceAttributeFileDetailsAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">File create requrest view model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveResourceAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel);

        /// <summary>
        /// The SaveResourceDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveResourceDetailAsync(ResourceDetailViewModel model);

        /// <summary>
        /// The SaveWeblinkDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveWeblinkDetailAsync(WebLinkViewModel model);

        /// <summary>
        /// The SaveCaseDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveCaseDetailAsync(CaseViewModel model);

        /// <summary>
        /// The SaveAsssessmentDetailAsync.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> SaveAssessmentDetailAsync(AssessmentViewModel model);

        /// <summary>
        /// The SubmitResourceVersionForPublishAsync.
        /// </summary>
        /// <param name="publishViewModel">Publish view model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> SubmitResourceVersionForPublishAsync(PublishViewModel publishViewModel);

        /// <summary>
        /// The UploadFileChunkAsync.
        /// </summary>
        /// <param name="fileChunkDetailId">File chunk detail id.</param>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <param name="chunkCount">Chunk count.</param>
        /// <param name="chunkIndex">chunk index.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="file">File.</param>
        /// <param name="fileSize">File size.</param>
        /// <param name="currentUserId">Current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<FileChunkUploadResult> UploadFileChunkAsync(int fileChunkDetailId, int resourceVersionId, int chunkCount, int chunkIndex, string fileName, IFormFile file, int fileSize, int currentUserId);
    }
}
