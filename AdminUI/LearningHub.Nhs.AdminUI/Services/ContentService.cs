namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Content;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ContentService" />.
    /// </summary>
    public class ContentService : BaseService, IContentService
    {
        /// <summary>
        /// Defines the fileService.
        /// </summary>
        private readonly IFileService fileService;
        private readonly IAzureMediaService azureMediaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="fileService">The fileService<see cref="IFileService"/>.</param>
        /// <param name="azureMediaService">azureMediaService.</param>
        public ContentService(ILearningHubHttpClient learningHubHttpClient, IFileService fileService, IAzureMediaService azureMediaService)
        : base(learningHubHttpClient)
        {
            this.fileService = fileService;
            this.azureMediaService = azureMediaService;
        }

        /// <summary>
        /// The DiscardAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DiscardAsync(int pageId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = $"content/discard/{pageId}";
            var response = await client.PutAsync(request, null).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The GetPageByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="includeHidden">includeHidden.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        public async Task<PageViewModel> GetPageByIdAsync(int id, bool includeHidden = false)
        {
            PageViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = !includeHidden ? $"content/page/{id}" : $"content/page-all/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PageViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetailViewModel}"/>.</returns>
        public async Task<PageSectionDetailViewModel> GetPageSectionDetailByIdAsync(int id)
        {
            PageSectionDetailViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/page-section-detail/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PageSectionDetailViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="pageSectionId">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetailViewModel}"/>.</returns>
        public async Task<PageSectionDetailViewModel> GetEditablePageSectionDetailByIdAsync(int pageSectionId)
        {
            PageSectionDetailViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/editable-page-section-detail/{pageSectionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PageSectionDetailViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetPagesAsync.
        /// </summary>
        /// <returns>The <see cref="Task{PageResultViewModel}"/>.</returns>
        public async Task<PageResultViewModel> GetPagesAsync()
        {
            PageResultViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/pages";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PageResultViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The PublishAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task PublishAsync(int pageId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = $"content/publish/{pageId}";
            var response = await client.PutAsync(request, null).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// Update page image section detail.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="model">The update model<see cref="PageImageSectionUpdateViewModel"/>.</param>
        /// <param name="imageFile">The image file.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task UpdatePageImageSectionDetailAsync(int pageId, PageImageSectionUpdateViewModel model, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();

                var filelocation = await this.fileService.ProcessFile(stream, imageFile.FileName);

                model.ImageFileName = imageFile.FileName;
                model.ImageFilePath = filelocation;
                model.ImageFileSize = (int)(imageFile.Length / 1000);
            }

            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/page-image-section-detail/{pageId}";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The ChangeOrderAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ChangeOrderAsync(UpdatePageSectionOrderModel requestViewModel)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = "content/change-order/";
            var content = new StringContent(
                JsonConvert.SerializeObject(requestViewModel),
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The CloneAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CloneAsync(int pageSectionId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = $"content/clone/{pageSectionId}";
            var response = await client.PutAsync(request, null).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The HideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task HideAsync(int pageSectionId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = $"content/hide/{pageSectionId}";
            var response = await client.PutAsync(request, null).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The UnHideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UnHideAsync(int pageSectionId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = $"content/unhide/{pageSectionId}";
            var response = await client.PutAsync(request, null).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The DeleteAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int pageSectionId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = $"content/delete/{pageSectionId}";
            var response = await client.PutAsync(request, null).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The ProcessUploadFile.
        /// </summary>
        /// <param name="pageSectionDetailId">The pageSectionDetailId<see cref="int"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        public async Task<FileUploadResult> ProcessUploadFile(int pageSectionDetailId, IFormFile file, int currentUserId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(file.FileName).Replace(".", string.Empty);
            ResourceTypeEnum resourceType = ResourceTypeEnum.Video;

            var fileTypelist = await this.GetFileTypeAsync();
            var fileType = fileTypelist.Where(x => x.Extension.ToLower() == extension.ToLower()).FirstOrDefault();

            if (fileType != null && fileType.NotAllowed)
            {
                return new FileUploadResult()
                {
                    FileName = file.FileName,
                    Invalid = true,
                };
            }

            filelocation = await this.azureMediaService.CreateMediaInputAsset(file);

            // Save the file details and associate to the PageSectionDetail
            var fileId = await this.SaveVideoAssetAsync(
            new FileCreateRequestViewModel()
            {
                PageSectionDetailId = pageSectionDetailId,
                FileName = file.FileName,
                FilePath = filelocation,
                FileSize = (int)(file.Length / 1000),
            });

            return new FileUploadResult()
            {
                PageSectionDetailId = pageSectionDetailId,
                FileId = fileId,
                FileName = file.FileName,
                ResourceType = resourceType,
                FileSizeKb = (int)(file.Length / 1000),
                Invalid = false,
            };
        }

        /// <summary>
        /// The GetFileTypeAsync.
        /// </summary>
        /// <returns>The <see cref="List{FileTypeViewModel}"/>.</returns>
        public async Task<List<FileTypeViewModel>> GetFileTypeAsync()
        {
            List<FileTypeViewModel> fileTypeList = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetFileTypes";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                fileTypeList = JsonConvert.DeserializeObject<List<FileTypeViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return fileTypeList;
        }

        /// <summary>
        /// The ProcessResourceAttachedFileAsync.
        /// </summary>
        /// <param name="pageSectionDetailId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="attachedFileType">The attachedFileType<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        public async Task<FileUploadResult> ProcessAttachedFileAsync(int pageSectionDetailId, IFormFile file, int attachedFileType, int currentUserId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(file.FileName).Replace(".", string.Empty);

            var fileTypelist = await this.GetFileTypeAsync();
            var fileType = fileTypelist.Where(x => x.Extension.ToLower() == extension.ToLower()).FirstOrDefault();

            if (fileType != null && fileType.NotAllowed)
            {
                return new FileUploadResult()
                {
                    FileName = file.FileName,
                    Invalid = true,
                };
            }

            using (var stream = file.OpenReadStream())
            {
                filelocation = await this.fileService.ProcessFile(stream, file.FileName);
            }

            // Save the file details
            int fileId = await this.SaveAttributeFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                PageSectionDetailId = pageSectionDetailId,
                FileName = file.FileName,
                FilePath = filelocation,
                FileSize = (int)(file.Length / 1000),
                AttachedFileType = (AttachedFileTypeEnum)attachedFileType,
            });

            return new FileUploadResult()
            {
                FileId = fileId,
                PageSectionDetailId = pageSectionDetailId,
                FileName = file.FileName,
                FileSizeKb = (int)(file.Length / 1000),
                Invalid = false,
                AttachedFileTypeId = attachedFileType,
            };
        }

        /// <summary>
        /// The SaveAttributeFileDetailsAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> SaveAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(fileCreateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/save-attribute-file";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return (int)apiResponse.ValidationResult.CreatedId;
        }

        /// <summary>
        /// The SaveVideoAssetAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> SaveVideoAssetAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(fileCreateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/save-video-asset";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return (int)apiResponse.ValidationResult.CreatedId;
        }

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
        public async Task<FileChunkUploadResult> UploadFileChunkAsync(int fileChunkDetailId, int pageSectionDetailId, int chunkCount, int chunkIndex, string fileName, IFormFile file, int fileSize, int currentUserId)
        {
            try
            {
                if (fileChunkDetailId == 0)
                {
                    // Store the chunk
                    var filelocation = string.Empty;
                    var directoryRef = "chunks_" + Guid.NewGuid().ToString();

                    using (var stream = file.OpenReadStream())
                    {
                        filelocation = await this.fileService.ProcessFile(stream, "Chunk_" + chunkIndex.ToString(), directoryRef);
                    }

                    // Create the chunk detail record
                    fileChunkDetailId = await this.SaveFileChunkDetailsAsync(
                    new FileChunkDetailViewModel()
                    {
                        FilePath = filelocation,
                        ChunkCount = chunkCount,
                        PageSectionDetailId = pageSectionDetailId,
                        FileName = fileName,
                        FileSizeKb = (int)(fileSize / 1000),
                    });
                }
                else
                {
                    var fileChunkDetail = await this.GetFileChunkDetail(fileChunkDetailId);

                    // Store the chunk
                    var filelocation = string.Empty;
                    using (var stream = file.OpenReadStream())
                    {
                        filelocation = await this.fileService.ProcessFile(stream, "Chunk_" + chunkIndex.ToString(), fileChunkDetail.FilePath);
                    }
                }

                return new FileChunkUploadResult()
                {
                    FileChunkDetailId = fileChunkDetailId,
                    FileChunkId = chunkIndex,
                    Success = true,
                };
            }
            catch (Exception)
            {
                return new FileChunkUploadResult()
                {
                    FileChunkDetailId = fileChunkDetailId,
                    FileChunkId = chunkIndex,
                    Success = false,
                };
            }
        }

        /// <summary>
        /// The GetFileChunkDetail.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileChunkDetailViewModel}"/>.</returns>
        public async Task<FileChunkDetailViewModel> GetFileChunkDetail(int fileChunkDetailId)
        {
            FileChunkDetailViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetFileChunkDetail/{fileChunkDetailId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<FileChunkDetailViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The SaveFileChunkDetailsAsync.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">The fileChunkDetailCreateRequestViewModel<see cref="FileChunkDetailViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> SaveFileChunkDetailsAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(fileChunkDetailCreateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/SaveFileChunkDetail";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return (int)apiResponse.ValidationResult.CreatedId;
        }

        /// <summary>
        /// The RegisterChunkedFileAsync.
        /// </summary>
        /// <param name="fileChunkRegisterModel">The fileChunkRegisterModel<see cref="FileChunkRegisterModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        public async Task<FileUploadResult> RegisterChunkedFileAsync(FileChunkRegisterModel fileChunkRegisterModel, int currentUserId)
        {
            var fileChunkDetail = await this.GetFileChunkDetail(fileChunkRegisterModel.FileChunkDetailId);
            var result = new FileUploadResult();
            fileChunkDetail.PageSectionDetailId = fileChunkRegisterModel.PageSectionDetailId;

            switch (fileChunkRegisterModel.FileUploadType)
            {
                case FileUploadTypeEnum.Video:
                    result = await this.RegisterVideoFileAsync(fileChunkDetail.PageSectionDetailId, fileChunkDetail, currentUserId);
                    break;
                case FileUploadTypeEnum.VideoAttached:
                    result = await this.RegisterVideoAttachedFileAsync(fileChunkDetail.PageSectionDetailId, fileChunkDetail, fileChunkRegisterModel.ResourceType, fileChunkRegisterModel.AttachedFileType, currentUserId);
                    break;
                default:
                    throw new Exception("Invalid FileUploadType!");
            }

            return result;
        }

        /// <summary>
        /// The DeleteFileChunkDetailAsync.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteFileChunkDetailAsync(int fileChunkDetailId)
        {
            ApiResponse apiResponse = null;
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/DeleteFileChunkDetail/{fileChunkDetailId}";
            var response = await client.DeleteAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The RegisterVideoFileAsync.
        /// </summary>
        /// <param name="pageSectionDetailId">Resource Versin Id.</param>
        /// <param name="fileChunkDetail">File chunk detail.</param>
        /// <param name="currentUserId">Current user Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<FileUploadResult> RegisterVideoFileAsync(int pageSectionDetailId, FileChunkDetailViewModel fileChunkDetail, int currentUserId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(fileChunkDetail.FileName).Replace(".", string.Empty);
            ResourceTypeEnum resourceType = ResourceTypeEnum.Video;

            //// get resource type from file Type
            var fileTypelist = await this.GetFileTypeAsync();
            var fileType = fileTypelist.Where(x => x.Extension.ToLower() == extension.ToLower()).FirstOrDefault();

            if (fileType != null && fileType.NotAllowed)
            {
                return new FileUploadResult()
                {
                    FileName = fileChunkDetail.FileName,
                    Invalid = true,
                };
            }

            // Save the file details and associate to the PageSectionDetail
            var fileId = await this.SaveVideoAssetAsync(
            new FileCreateRequestViewModel()
            {
                PageSectionDetailId = pageSectionDetailId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileName = fileChunkDetail.FileName,
                FilePath = filelocation,
                FileSize = fileChunkDetail.FileSizeKb,
                FileChunkDetailId = fileChunkDetail.Id,
            });

            return new FileUploadResult()
            {
                PageSectionDetailId = pageSectionDetailId,
                FileId = fileId,
                FileName = fileChunkDetail.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                ResourceType = resourceType,
                FileSizeKb = fileChunkDetail.FileSizeKb,
                Invalid = false,
            };
        }

        /// <summary>
        /// The RegisterResourceAttachedFileAsync.
        /// </summary>
        /// <param name="pageSectionDetailId">Page Section Detail Id.</param>
        /// <param name="fileChunkDetail">File chunk detail.</param>
        /// <param name="resourceType">Resource type.</param>
        /// <param name="attachedFileType">Attached file type.</param>
        /// <param name="currentUserId">Current user Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<FileUploadResult> RegisterVideoAttachedFileAsync(int pageSectionDetailId, FileChunkDetailViewModel fileChunkDetail, int resourceType, int attachedFileType, int currentUserId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(fileChunkDetail.FileName).Replace(".", string.Empty);

            var fileTypelist = await this.GetFileTypeAsync();
            var fileType = fileTypelist.Where(x => x.Extension.ToLower() == extension.ToLower()).FirstOrDefault();

            if (fileType != null && fileType.NotAllowed)
            {
                return new FileUploadResult()
                {
                    FileName = fileChunkDetail.FileName,
                    Invalid = true,
                };
            }

            // Save the file details
            int fileId = await this.SaveAttributeFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                PageSectionDetailId = pageSectionDetailId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileName = fileChunkDetail.FileName,
                FilePath = filelocation,
                FileChunkDetailId = fileChunkDetail.Id,
                FileSize = fileChunkDetail.FileSizeKb,
                AttachedFileType = (AttachedFileTypeEnum)attachedFileType,
            });

            return new FileUploadResult()
            {
                FileId = fileId,
                PageSectionDetailId = pageSectionDetailId,
                FileName = fileChunkDetail.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileSizeKb = fileChunkDetail.FileSizeKb,
                Invalid = false,
                AttachedFileTypeId = attachedFileType,
            };
        }

        /// <summary>
        /// The DeleteResourceAttributeFileAsync.
        /// </summary>
        /// <param name="model">The model<see cref="VideoAssetViewModel"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public async Task<bool> UpdateVideoAssetAsync(VideoAssetViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/update-video-asset";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return true;
        }

        /// <summary>
        /// The CreatePageSectionAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="PageSectionViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreatePageSectionAsync(PageSectionViewModel requestViewModel)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = "content/create-page-section/";
            var content = new StringContent(
                JsonConvert.SerializeObject(requestViewModel),
                Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);
            int pageSectionId = 0;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                pageSectionId = JsonConvert.DeserializeObject<int>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return pageSectionId;
        }

        /// <summary>
        /// The UpdatePageSectionDetailAsync.
        /// </summary>
        /// <param name="updateViewModel">The updateViewModel<see cref="PageSectionDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdatePageSectionDetailAsync(PageSectionDetailViewModel updateViewModel)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();
            var request = "content/update-page-section-detail/";
            var content = new StringContent(
                JsonConvert.SerializeObject(updateViewModel),
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync(request, content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The GetPageSectionDetailVideoAssetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PageSectionDetailViewModel> GetPageSectionDetailVideoAssetByIdAsync(int id)
        {
            PageSectionDetailViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"content/page-section-detail-video/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PageSectionDetailViewModel>(result);
                if (viewmodel?.VideoAsset?.AzureMediaAssetId > 0)
                {
                    viewmodel.VideoAsset.AzureMediaAsset.AuthenticationToken = await this.azureMediaService.GetContentAuthenticationTokenAsync(viewmodel.VideoAsset.AzureMediaAsset.FilePath);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
