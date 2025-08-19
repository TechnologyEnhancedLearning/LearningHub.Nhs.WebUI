namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Contribute;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualBasic;
    using MK.IO.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ContributeService" />.
    /// </summary>
    public class ContributeService : BaseService<ContributeService>, IContributeService
    {
        private readonly IAzureMediaService azureMediaService;
        private readonly IAzureMediaService mediaService;
        private readonly IFileService fileService;
        private readonly IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContributeService"/> class.
        /// </summary>
        /// <param name="fileService">File service.</param>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="azureMediaService">Azure media service.</param>
        /// <param name="mediaService">MKIO media service.</param>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">Logger.</param>
        public ContributeService(IFileService fileService, IResourceService resourceService, IAzureMediaService azureMediaService, ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<ContributeService> logger, IAzureMediaService mediaService)
        : base(learningHubHttpClient, openApiHttpClient, logger)
        {
            this.fileService = fileService;
            this.resourceService = resourceService;
            this.azureMediaService = azureMediaService;
            this.mediaService = mediaService;
        }

        /// <summary>
        /// The CreateNewResourceVersionAsync.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateNewResourceVersionAsync(int resourceId)
        {
            var model = new KeyValuePair<string, int>("resourceId", resourceId);

            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/CreateNewResourceVersion";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("creating resource version failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The CreateResourceAuthorAsync.
        /// </summary>
        /// <param name="resourceAuthorViewModel">The resourceAuthorViewModel<see cref="ResourceAuthorViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> CreateResourceAuthorAsync(ResourceAuthorViewModel resourceAuthorViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(resourceAuthorViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/AddResourceVersionAuthor";
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
        /// The CreateResourceKeywordAsync.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="ResourceKeywordViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> CreateResourceKeywordAsync(ResourceKeywordViewModel resourceKeywordViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(resourceKeywordViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/AddResourceVersionKeyword";
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
        /// The DeleteArticleFileAsync.
        /// </summary>
        /// <param name="model">The model<see cref="FileDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="T:Task{bool}"/>.</returns>
        public async Task<bool> DeleteArticleFileAsync(FileDeleteRequestModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DeleteArticleFile";
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
        /// The DeleteFileChunkDetailAsync.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteFileChunkDetailAsync(int fileChunkDetailId)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DeleteFileChunkDetail/{fileChunkDetailId}";
            var response = await client.DeleteAsync(request).ConfigureAwait(false);

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
        }

        /// <summary>
        /// The DeleteResourceAttributeFileAsync.
        /// </summary>
        /// <param name="model">The model<see cref="FileDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="T:Task{bool}"/>.</returns>
        public async Task<bool> DeleteResourceAttributeFileAsync(FileDeleteRequestModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DeleteResourceAttributeFile";
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
        /// The DeleteResourceAuthorAsync.
        /// </summary>
        /// <param name="model">The model<see cref="AuthorDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="T:Task{bool}"/>.</returns>
        public async Task<bool> DeleteResourceAuthorAsync(AuthorDeleteRequestModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DeleteResourceVersionAuthor";
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
        /// The DeleteResourceKeywordAsync.
        /// </summary>
        /// <param name="model">The model<see cref="KeywordDeleteRequestModel"/>.</param>
        /// <returns>The <see cref="T:Task{bool}"/>.</returns>
        public async Task<bool> DeleteResourceKeywordAsync(KeywordDeleteRequestModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DeleteResourceVersionKeyword";
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
        /// The DeleteResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteResourceVersionAsync(int resourceVersionId)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/DeleteResourceVersion/{resourceVersionId}";
            var response = await client.DeleteAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (apiResponse.ValidationResult == null)
                {
                    throw new Exception($"DeleteResourceVersion ApiResponse contained empty ValidationResult");
                }

                return apiResponse.ValidationResult;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception($"Exception HttpStatusCode={response.StatusCode}");
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

            var client = await this.OpenApiHttpClient.GetClientAsync();

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
        /// The ProcessArticleFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="fileSize">The fileSize<see cref="int"/>.</param>
        /// <param name="existingFileId">The existingFileId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        public async Task<FileUploadResult> ProcessArticleFileAsync(int resourceVersionId, IFormFile file, int fileSize, int existingFileId, int currentUserId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(file.FileName).Replace(".", string.Empty);

            var fileTypelist = await this.resourceService.GetFileTypeAsync();
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
            int fileId = await this.SaveArticleAttachedFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                ResourceVersionId = resourceVersionId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileName = file.FileName,
                FilePath = filelocation,
                FileSize = (int)(fileSize / 1000),
                ReplacedFileId = existingFileId,
            });

            return new FileUploadResult()
            {
                FileId = fileId,
                ResourceVersionId = resourceVersionId,
                FileName = file.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileSizeKb = (int)(fileSize / 1000),
                Invalid = false,
            };
        }

        /// <summary>
        /// The ProcessResourceAttachedFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="resourceType">The resourceType<see cref="int"/>.</param>
        /// <param name="attachedFileType">The attachedFileType<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        public async Task<FileUploadResult> ProcessResourceAttachedFileAsync(int resourceVersionId, IFormFile file, int resourceType, int attachedFileType, int currentUserId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(file.FileName).Replace(".", string.Empty);

            var fileTypelist = await this.resourceService.GetFileTypeAsync();
            var fileType = fileTypelist.Where(x => x.Extension.ToLower() == extension.ToLower()).FirstOrDefault();

            if ((resourceType == 9 && fileType == null) || (fileType != null && fileType.NotAllowed))
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
            int fileId = await this.SaveResourceAttributeFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                ResourceVersionId = resourceVersionId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileName = file.FileName,
                FilePath = filelocation,
                FileSize = (int)(file.Length / 1000),
                AttachedFileType = (AttachedFileTypeEnum)attachedFileType,
                ResourceType = (ResourceTypeEnum)resourceType,
            });

            return new FileUploadResult()
            {
                FileId = fileId,
                ResourceVersionId = resourceVersionId,
                FileName = file.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileSizeKb = (int)(file.Length / 1000),
                Invalid = false,
                AttachedFileTypeId = attachedFileType,
            };
        }

        /// <summary>
        /// The ProcessResourceFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileUploadResult}"/>.</returns>
        public async Task<FileUploadResult> ProcessResourceFileAsync(int resourceVersionId, IFormFile file, int currentUserId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(file.FileName).Replace(".", string.Empty);
            ResourceTypeEnum resourceType = ResourceTypeEnum.GenericFile;

            //// get resource type from file Type
            var fileTypelist = await this.resourceService.GetFileTypeAsync();
            var fileType = fileTypelist.Where(x => x.Extension.ToLower() == extension.ToLower()).FirstOrDefault();

            if ((fileType == null) || (fileType != null && fileType.NotAllowed) || file.Length <= 0)
            {
                // Define dangerous file extensions
                string[] dangerousExtensions = { ".exe", ".dll", ".bat", ".js", ".vbs", ".sh", ".ps1" };
                if (dangerousExtensions.Any(ext => file.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    var error = $"A potentially harmful file has been detected and blocked: {file.FileName}.";
                    var validationDetail = new ResourceVersionValidationResultViewModel
                    {
                        ResourceVersionId = resourceVersionId,
                        Success = false,
                        Details = string.Empty,
                        AmendUserId = currentUserId,
                        ResourceVersionValidationRuleResultViewModels = new[]
                       {
                        new ResourceVersionValidationRuleResultViewModel
                        {
                            ResourceTypeValidationRuleEnum = ResourceTypeValidationRuleEnum.HtmlResource_RootIndexPresent,
                            Success = false,
                            Details = error,
                        },
                       }.ToList(),
                    };

                    await this.resourceService.CreateResourceVersionValidationResultAsync(validationDetail);
                }

                return new FileUploadResult()
                {
                    FileName = file.FileName,
                    Invalid = true,
                };
            }

            // if file is media (video or audio) then upload at Azure Media Storage
            if (fileType != null && (fileType.DefaultResourceTypeId == (int)ResourceTypeEnum.Video || fileType.DefaultResourceTypeId == (int)ResourceTypeEnum.Audio))
            {
                filelocation = await this.mediaService.CreateMediaInputAsset(file);
            }

            // upload at Azure File Storage
            else
            {
                using (var stream = file.OpenReadStream())
                {
                    filelocation = await this.fileService.ProcessFile(stream, file.FileName);
                }
            }

            if (fileType != null)
            {
                resourceType = (ResourceTypeEnum)fileType.DefaultResourceTypeId;
            }

            if (resourceType == ResourceTypeEnum.GenericFile && resourceVersionId != 0 && extension.ToLower() == "zip")
            {
                var resourceVersion = await this.resourceService.GetResourceVersionViewModelAsync(resourceVersionId);
                if (resourceVersion.ResourceTypeEnum == ResourceTypeEnum.Scorm)
                {
                    resourceType = ResourceTypeEnum.Scorm;
                }
                else if (resourceVersion.ResourceTypeEnum == ResourceTypeEnum.Html)
                {
                    resourceType = ResourceTypeEnum.Html;
                }
            }

            // Save the file details and associate to the ResourceVersion
            resourceVersionId = await this.SaveFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                ResourceVersionId = resourceVersionId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                ResourceType = resourceType,
                FileName = file.FileName,
                FilePath = filelocation,
                FileSize = (int)(file.Length / 1000),
            });

            return new FileUploadResult()
            {
                ResourceVersionId = resourceVersionId,
                FileName = file.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                ResourceType = resourceType,
                FileSizeKb = (int)(file.Length / 1000),
                Invalid = false,
            };
        }

        /// <summary>
        /// The RegisterArticleFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version Id.</param>
        /// <param name="fileChunkDetail">File chunk detail.</param>
        /// <param name="existingFileId">Existing File Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<FileUploadResult> RegisterArticleFileAsync(int resourceVersionId, FileChunkDetailViewModel fileChunkDetail, int existingFileId)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(fileChunkDetail.FileName).Replace(".", string.Empty);

            var fileTypelist = await this.resourceService.GetFileTypeAsync();
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
            int fileId = await this.SaveArticleAttachedFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                ResourceVersionId = resourceVersionId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileName = fileChunkDetail.FileName,
                FilePath = filelocation,
                FileChunkDetailId = fileChunkDetail.Id,
                FileSize = (int)(fileChunkDetail.FileSizeKb / 1000),
                ReplacedFileId = existingFileId,
            });

            return new FileUploadResult()
            {
                FileId = fileId,
                ResourceVersionId = resourceVersionId,
                FileName = fileChunkDetail.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileSizeKb = fileChunkDetail.FileSizeKb,
                Invalid = false,
            };
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

            switch (fileChunkRegisterModel.FileUploadType)
            {
                case FileUploadTypeEnum.Resource:
                    result = await this.RegisterResourceFileAsync(fileChunkDetail.ResourceVersionId ?? 0, fileChunkDetail);
                    break;
                case FileUploadTypeEnum.Article:
                    result = await this.RegisterArticleFileAsync(fileChunkDetail.ResourceVersionId ?? 0, fileChunkDetail, fileChunkRegisterModel.ChangeingFileId);
                    break;
                case FileUploadTypeEnum.ResourceAttached:
                    result = await this.RegisterResourceAttachedFileAsync(fileChunkDetail.ResourceVersionId ?? 0, fileChunkDetail, fileChunkRegisterModel.ResourceType, fileChunkRegisterModel.AttachedFileType);
                    break;
                case FileUploadTypeEnum.Unspecified:
                default:
                    throw new Exception("Invalid FileUploadType!");
            }

            return result;
        }

        /// <summary>
        /// The RegisterResourceAttachedFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version Id.</param>
        /// <param name="fileChunkDetail">File chunk detail.</param>
        /// <param name="resourceType">Resource type.</param>
        /// <param name="attachedFileType">Attached file type.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<FileUploadResult> RegisterResourceAttachedFileAsync(int resourceVersionId, FileChunkDetailViewModel fileChunkDetail, int resourceType, int attachedFileType)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(fileChunkDetail.FileName).Replace(".", string.Empty);

            var fileTypelist = await this.resourceService.GetFileTypeAsync();
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
            int fileId = await this.SaveResourceAttributeFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                ResourceVersionId = resourceVersionId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileName = fileChunkDetail.FileName,
                FilePath = filelocation,
                FileChunkDetailId = fileChunkDetail.Id,
                FileSize = fileChunkDetail.FileSizeKb,
                AttachedFileType = (AttachedFileTypeEnum)attachedFileType,
                ResourceType = (ResourceTypeEnum)resourceType,
            });

            return new FileUploadResult()
            {
                FileId = fileId,
                ResourceVersionId = resourceVersionId,
                FileName = fileChunkDetail.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                FileSizeKb = fileChunkDetail.FileSizeKb,
                Invalid = false,
                AttachedFileTypeId = attachedFileType,
            };
        }

        /// <summary>
        /// The RegisterResourceFileAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource Versin Id.</param>
        /// <param name="fileChunkDetail">File chunk detail.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<FileUploadResult> RegisterResourceFileAsync(int resourceVersionId, FileChunkDetailViewModel fileChunkDetail)
        {
            var filelocation = string.Empty;
            string extension = Path.GetExtension(fileChunkDetail.FileName).Replace(".", string.Empty);
            ResourceTypeEnum resourceType = ResourceTypeEnum.GenericFile;

            //// get resource type from file Type
            var fileTypelist = await this.resourceService.GetFileTypeAsync();
            var fileType = fileTypelist.Where(x => x.Extension.ToLower() == extension.ToLower()).FirstOrDefault();

            if (fileType != null && fileType.NotAllowed)
            {
                return new FileUploadResult()
                {
                    FileName = fileChunkDetail.FileName,
                    Invalid = true,
                };
            }

            if (fileType != null)
            {
                resourceType = (ResourceTypeEnum)fileType.DefaultResourceTypeId;
            }

            if (resourceType == ResourceTypeEnum.GenericFile && resourceVersionId != 0 && extension.ToLower() == "zip")
            {
                var resourceVersion = await this.resourceService.GetResourceVersionViewModelAsync(resourceVersionId);
                if (resourceVersion.ResourceTypeEnum == ResourceTypeEnum.Scorm)
                {
                    resourceType = ResourceTypeEnum.Scorm;
                }
                else if (resourceVersion.ResourceTypeEnum == ResourceTypeEnum.Html)
                {
                    resourceType = ResourceTypeEnum.Html;
                }
            }

            // Save the file details and associate to the ResourceVersion
            resourceVersionId = await this.SaveFileDetailsAsync(
            new FileCreateRequestViewModel()
            {
                ResourceVersionId = resourceVersionId,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                ResourceType = resourceType,
                FileName = fileChunkDetail.FileName,
                FilePath = filelocation,
                FileSize = fileChunkDetail.FileSizeKb,
                FileChunkDetailId = fileChunkDetail.Id,
            });

            return new FileUploadResult()
            {
                ResourceVersionId = resourceVersionId,
                FileName = fileChunkDetail.FileName,
                FileTypeId = fileType == null ? 0 : fileType.Id,
                ResourceType = resourceType,
                FileSizeKb = fileChunkDetail.FileSizeKb,
                Invalid = false,
            };
        }

        /// <summary>
        /// The SaveArticleAttachedFileDetailsAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveArticleAttachedFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(fileCreateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/SaveArticleAttachedFileDetails";
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
        /// The SaveArticleDetailAsync.
        /// </summary>
        /// <param name="articleViewModel">The articleViewModel<see cref="ArticleUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveArticleDetailAsync(ArticleUpdateRequestViewModel articleViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(articleViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateArticleDetail";
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
        /// The SaveFileChunkDetailsAsync.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">The fileChunkDetailCreateRequestViewModel<see cref="FileChunkDetailViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveFileChunkDetailsAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(fileChunkDetailCreateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

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
        /// The SaveFileDetailsAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(fileCreateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/SaveFileDetails";
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
        /// The SaveGenericFileDetailAsync.
        /// </summary>
        /// <param name="genericFileViewModel">The genericFileViewModel<see cref="GenericFileUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveGenericFileDetailAsync(GenericFileUpdateRequestViewModel genericFileViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(genericFileViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateGenericFileDetail";
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
        /// The SaveScormDetailAsync.
        /// </summary>
        /// <param name="scormUpdateRequestViewModel">scormUpdateRequestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<int> SaveScormDetailAsync(ScormUpdateRequestViewModel scormUpdateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(scormUpdateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateScormDetail";
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
        /// The SaveHtmlDetailAsync.
        /// </summary>
        /// <param name="htmlResource">Html resource update view model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<int> SaveHtmlDetailAsync(HtmlResourceUpdateRequestViewModel htmlResource)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(htmlResource);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateHtmlDetail";
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
        /// The SaveImageDetailAsync.
        /// </summary>
        /// <param name="imageViewModel">The imageViewModel<see cref="ImageUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveImageDetailAsync(ImageUpdateRequestViewModel imageViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(imageViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateImageDetail";
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
        /// The SaveResourceAttributeFileDetailsAsync.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveResourceAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(fileCreateRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/SaveResourceAttributeFileDetails";
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
        /// The SaveResourceDetailAsync.
        /// </summary>
        /// <param name="detailViewModel">The detailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveResourceDetailAsync(ResourceDetailViewModel detailViewModel)
        {
            int resourceVersionId;
            if (detailViewModel.ResourceVersionId == 0)
            {
                // Create the Resource & Resource version
                resourceVersionId = await this.CreateResourceAsync(detailViewModel);
            }
            else
            {
                // Update the existing Resource Version
                await this.UpdateResourceVersionAsync(detailViewModel);
                resourceVersionId = detailViewModel.ResourceVersionId;
            }

            return resourceVersionId;
        }

        /// <summary>
        /// The SaveWeblinkDetailAsync.
        /// </summary>
        /// <param name="weblinkViewModel">The weblinkViewModel<see cref="WebLinkViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> SaveWeblinkDetailAsync(WebLinkViewModel weblinkViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(weblinkViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateWeblinkDetail";
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
        /// The SaveCaseDetailAsync.
        /// </summary>
        /// <param name="caseViewModel">The caseViewModel<see cref="CaseViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> SaveCaseDetailAsync(CaseViewModel caseViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(caseViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateCaseDetail";
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
        /// The SaveAssessmentDetailAsync.
        /// </summary>
        /// <param name="assessmentViewModel">The assessmentViewModel<see cref="AssessmentViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> SaveAssessmentDetailAsync(AssessmentViewModel assessmentViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(assessmentViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateAssessmentDetail";
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
        /// The SubmitResourceVersionForPublishAsync.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> SubmitResourceVersionForPublishAsync(PublishViewModel publishViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(publishViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/SubmitResourceVersionForPublish";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("publish failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The UploadFileChunkAsync.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="chunkCount">The chunkCount<see cref="int"/>.</param>
        /// <param name="chunkIndex">The chunkIndex<see cref="int"/>.</param>
        /// <param name="fileName">The fileName.</param>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="fileSize">The fileSize<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileChunkUploadResult}"/>.</returns>
        public async Task<FileChunkUploadResult> UploadFileChunkAsync(int fileChunkDetailId, int resourceVersionId, int chunkCount, int chunkIndex, string fileName, IFormFile file, int fileSize, int currentUserId)
        {
            try
            {
                if (fileChunkDetailId == 0)
                {
                    // removing special characters in file name
                    string fileNameUpdated = Regex.Replace(fileName, "[^a-zA-Z0-9.]", string.Empty);

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
                        ResourceVersionId = (resourceVersionId == 0) ? (int?)null : resourceVersionId,
                        FileName = fileNameUpdated,
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
        /// The CreateResourceAsync.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        private async Task<int> CreateResourceAsync(ResourceDetailViewModel resourceDetailViewModel)
        {
            ApiResponse apiResponse = null;
            var json = JsonConvert.SerializeObject(resourceDetailViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/CreateResource";
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
        /// The UpdateResourceVersionAsync.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task UpdateResourceVersionAsync(ResourceDetailViewModel resourceDetailViewModel)
        {
            var json = JsonConvert.SerializeObject(resourceDetailViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"Resource/UpdateResourceVersion";
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
            else
            {
                throw new Exception("save failed!");
            }
        }
    }
}
