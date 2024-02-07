// <copyright file="PageService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Content;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Enums.Content;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Content;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The PageService.
    /// </summary>
    public class PageService : IPageService
    {
        /// <summary>
        /// Defines the mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Defines the pageRepository.
        /// </summary>
        private readonly IPageRepository pageRepository;

        /// <summary>
        /// Defines the pageSectionRepository.
        /// </summary>
        private readonly IPageSectionRepository pageSectionRepository;

        /// <summary>
        /// Defines the pageSectionDetailRepository.
        /// </summary>
        private readonly IPageSectionDetailRepository pageSectionDetailRepository;

        /// <summary>
        /// Defines the fileTypeService.
        /// </summary>
        private readonly IFileTypeService fileTypeService;

        /// <summary>
        /// Defines the fileRepository.
        /// </summary>
        private readonly IFileRepository fileRepository;

        /// <summary>
        /// Defines the videoAssetRepository.
        /// </summary>
        private readonly IVideoAssetRepository videoAssetRepository;

        /// <summary>
        /// Defines the queueCommunicatorService.
        /// </summary>
        private readonly IQueueCommunicatorService queueCommunicatorService;
        private readonly IInternalSystemService internalSystemService;

        /// <summary>
        /// Defines the settings.
        /// </summary>
        private readonly Settings settings;
        private readonly ILogger<PageService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageService"/> class.
        /// </summary>
        /// <param name="mapper">mapper.</param>
        /// <param name="pageRepository">pageRepository.</param>
        /// <param name="pageSectionRepository">pageSectionRepository.</param>
        /// <param name="pageSectionDetailRepository">pageSectionDetailRepository.</param>
        /// <param name="fileTypeService">fileTypeService.</param>
        /// <param name="fileRepository">fileRepository.</param>
        /// <param name="videoAssetRepository">videoAssetRepository.</param>
        /// <param name="queueCommunicatorService">.</param>
        /// <param name="internalSystemService">The internalSystemService.</param>
        /// <param name="settings">settings.</param>
        /// <param name="logger">The logger.</param>
        public PageService(
            IMapper mapper,
            IPageRepository pageRepository,
            IPageSectionRepository pageSectionRepository,
            IPageSectionDetailRepository pageSectionDetailRepository,
            IFileTypeService fileTypeService,
            IFileRepository fileRepository,
            IVideoAssetRepository videoAssetRepository,
            IQueueCommunicatorService queueCommunicatorService,
            IInternalSystemService internalSystemService,
            IOptions<Settings> settings,
            ILogger<PageService> logger)
        {
            this.mapper = mapper;
            this.pageRepository = pageRepository;
            this.pageSectionRepository = pageSectionRepository;
            this.pageSectionDetailRepository = pageSectionDetailRepository;
            this.fileTypeService = fileTypeService;
            this.fileRepository = fileRepository;
            this.videoAssetRepository = videoAssetRepository;
            this.queueCommunicatorService = queueCommunicatorService;
            this.internalSystemService = internalSystemService;
            this.settings = settings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// The DiscardAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DiscardAsync(int pageId, int currentUserId)
        {
            await this.pageRepository.DiscardAsync(pageId, currentUserId);
        }

        /// <summary>
        /// The GetPageById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="includeHidden">includeHidden.</param>
        /// <param name="publishedOnly">The published only<see cref="bool"/>.</param>
        /// <param name="preview">Preview mode.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        public async Task<PageViewModel> GetPageByIdAsync(int id, bool includeHidden = false, bool publishedOnly = false, bool preview = false)
        {
            var page = await this.pageRepository.GetPageByIdAsync(id, publishedOnly, preview);
            var pageViewModel = new PageViewModel
            {
                Id = page.Id,
                Name = page.Name,
                PreviewUrl = page.Url,
                CanDiscard = page.CanDiscard(),
                CanPreview = page.CanPreview(),
                CanPublish = page.CanPublish(),
                HasHiddenSections = page.HasHiddenSections(),
                PageStatus = page.GetStatus(),
                PageSections = page.PageSections.Select(ps => new PageSectionViewModel
                {
                    Id = ps.Id,
                    IsHidden = ps.IsHidden,
                    Position = ps.Position,
                    AmendUserId = ps.AmendUserId,
                    SectionTemplateType = (SectionTemplateType)ps.SectionTemplateTypeId,
                    PageSectionDetail = this.mapper.Map<PageSectionDetailViewModel>(ps.PageSectionDetails.First()),
                })
                .Where(ps => !publishedOnly || (ps.PageSectionDetail.DeletePending.HasValue ? !ps.PageSectionDetail.DeletePending.Value : true))
                .Where(ps =>
                    (!includeHidden
                    && (ps.PageSectionDetail.DraftHidden != null
                        ? !ps.PageSectionDetail.DraftHidden.Value
                        : !ps.IsHidden))
                    || includeHidden)
                    .OrderBy(ps => ps.PageSectionDetail.DraftPosition ?? ps.Position)
                    .ToList(),
            };
            return pageViewModel;
        }

        /// <summary>
        /// GetPagesAsync.
        /// </summary>
        /// <returns>PageResultViewModel.</returns>
        public async Task<PageResultViewModel> GetPagesAsync()
        {
            try
            {
                var pages = await this.pageRepository.GetPagesAsync();

                var response = new PageResultViewModel
                {
                    TotalCount = pages.Count(),
                    Pages = pages.Select(p =>
                            new PageViewModel
                            {
                                Id = p.Id,
                                Name = p.Name,
                                PreviewUrl = p.Url,
                                PageStatus = p.PageSections.Any(
                                    ps => (PageSectionStatus)ps.PageSectionDetails.First().PageSectionStatusId != PageSectionStatus.Live) ? PageStatus.EditsPending : PageStatus.Live,
                            }).ToList(),
                };
                return response;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetPageSectionById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionViewModel}"/>.</returns>
        public async Task<PageSectionViewModel> GetPageSectionByIdAsync(int id)
        {
            var pageSection = await this.pageSectionRepository.GetByIdAsync(id);
            return this.mapper.Map<PageSectionViewModel>(pageSection);
        }

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetailViewModel}"/>.</returns>
        public async Task<PageSectionDetailViewModel> GetPageSectionDetailImageAssetByIdAsync(int id)
        {
            var detail = await this.pageSectionDetailRepository.GetPageSectionDetailImageAssetByIdAsync(id);

            return this.mapper.Map<PageSectionDetailViewModel>(detail);
        }

        /// <inheritdoc/>
        public async Task<PageSectionDetailViewModel> GetEditablePageSectionDetailByPageSectionIdAsync(int pageSectionId, int currentUserId)
        {
            PageSectionDetail detail = null;

            var pageSection = await this.pageSectionRepository.GetAll().Where(ps => ps.Id == pageSectionId)
                                .Include(ps => ps.PageSectionDetails.Where(psd => psd.PageSectionStatusId != (int)PageSectionStatus.Live).OrderByDescending(psd => psd.Id))
                                .FirstOrDefaultAsync();

            if (pageSection == null)
            {
                throw new System.Exception($"GetEditablePageSectionDetailByPageSectionIdAsync: PageSectionId {pageSectionId} not valid!");
            }

            if (!pageSection.PageSectionDetails.Any())
            {
                var liveDetail = await this.pageSectionDetailRepository.GetAll()
                                .Where(psd => psd.PageSectionId == pageSectionId && psd.PageSectionStatusId == (int)PageSectionStatus.Live)
                                .FirstOrDefaultAsync();

                detail = await this.pageSectionDetailRepository.CloneSectionDetailAsync(liveDetail.Id, (SectionTemplateType)pageSection.SectionTemplateTypeId, currentUserId);
            }
            else
            {
                if (pageSection.SectionTemplateTypeId == (int)SectionTemplateType.Video)
                {
                    detail = await this.pageSectionDetailRepository.GetPageSectionDetailVideoAssetByIdAsync(pageSection.PageSectionDetails.First().Id);
                }
                else
                {
                    detail = await this.pageSectionDetailRepository.GetPageSectionDetailImageAssetByIdAsync(pageSection.PageSectionDetails.First().Id);
                }
            }

            return this.mapper.Map<PageSectionDetailViewModel>(detail);
        }

        /// <summary>
        /// The PublishAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task PublishAsync(int pageId, int currentUserId)
        {
            await this.pageRepository.PublishAsync(pageId, currentUserId);
        }

        /// <summary>
        /// Update page image section detail.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="model">The update model<see cref="PageImageSectionUpdateViewModel"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task UpdatePageImageSectionDetailAsync(int pageId, PageImageSectionUpdateViewModel model, int currentUserId)
        {
            int? fileId = null;

            if (!string.IsNullOrWhiteSpace(model.ImageFilePath))
            {
                var fileType = await this.fileTypeService.GetByFilename(model.ImageFileName);

                fileId = await this.fileRepository.CreateAsync(currentUserId, new File
                {
                    FileTypeId = fileType == null ? 0 : fileType.Id,
                    FileName = model.ImageFileName,
                    FilePath = model.ImageFilePath,
                    FileSizeKb = model.ImageFileSize,
                });
            }

            var detail = await this.pageSectionDetailRepository.GetPageSectionDetailImageAssetByIdAsync(model.PageSectionDetailId.Value);

            if (detail.ImageAsset == null)
            {
                detail.ImageAsset = new ImageAsset();
            }

            detail.PageSectionStatusId = (int)PageSectionStatus.Processed;
            detail.BackgroundColour = model.BackgroundColour;
            detail.TextColour = model.TextColour;
            detail.HyperLinkColour = model.HyperLinkColour;
            detail.Description = model.Description;
            detail.TextBackgroundColour = model.TextBackgroundColour;
            detail.SectionLayoutTypeId = (int)model.SectionLayoutType;
            detail.ImageAsset.AltTag = model.ImageAlt;
            detail.SectionTitle = model.SectionTitle;
            detail.SectionTitleElement = model.SectionTitleElement;
            detail.TopMargin = model.TopMargin;
            detail.BottomMargin = model.BottomMargin;
            detail.HasBorder = model.HasBorder;

            if (fileId.HasValue)
            {
                detail.ImageAsset.ImageFileId = fileId;
            }

            await this.pageSectionDetailRepository.UpdateAsync(currentUserId, detail);
        }

        /// <summary>
        /// The CloneImageSectionAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CloneAsync(int pageSectionId, int currentUserId)
        {
            var pageSection = await this.pageSectionRepository.GetByIdAsync(pageSectionId);
            if ((SectionTemplateType)pageSection.SectionTemplateTypeId == SectionTemplateType.Image)
            {
                await this.pageSectionRepository.CloneImageSectionAsync(pageSectionId, currentUserId);
            }
            else if ((SectionTemplateType)pageSection.SectionTemplateTypeId == SectionTemplateType.Video)
            {
                await this.pageSectionRepository.CloneVideoSectionAsync(pageSectionId, currentUserId);
            }
        }

        /// <summary>
        /// The ChangeOrderAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="UpdatePageSectionOrderModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ChangeOrderAsync(UpdatePageSectionOrderModel requestViewModel, int currentUserId)
        {
            if (requestViewModel.DirectionType == DirectionType.Up)
            {
                await this.pageSectionRepository.ChangeOrderDownAsync(requestViewModel.PageId, requestViewModel.PageSectionId, currentUserId);
            }
            else
            {
                await this.pageSectionRepository.ChangeOrderUpAsync(requestViewModel.PageId, requestViewModel.PageSectionId, currentUserId);
            }
        }

        /// <summary>
        /// The HideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task HideAsync(int pageSectionId, int currentUserId)
        {
            await this.pageSectionRepository.HideAsync(pageSectionId, currentUserId);
        }

        /// <summary>
        /// The UnHideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UnHideAsync(int pageSectionId, int currentUserId)
        {
            await this.pageSectionRepository.UnHideAsync(pageSectionId, currentUserId);
        }

        /// <summary>
        /// The DeleteAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int pageSectionId, int currentUserId)
        {
            await this.pageSectionRepository.DeleteAsync(pageSectionId, currentUserId);
        }

        /// <summary>
        /// The CreatePageSectionAsync.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="PageSectionViewModel"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The page section id.</returns>
        public async Task<int> CreatePageSectionAsync(PageSectionViewModel requestViewModel, int currentUserId)
        {
            try
            {
                var pageSectionModel = this.mapper.Map<PageSection>(requestViewModel);
                var pageSectionDetail = this.mapper.Map<PageSectionDetail>(requestViewModel.PageSectionDetail);
                var pageSectionId = await this.pageSectionRepository.CreateWithPositionAsync(currentUserId, pageSectionModel);
                pageSectionDetail.PageSectionId = pageSectionId;

                if (pageSectionModel.SectionTemplateTypeId == (int)SectionTemplateType.Video)
                {
                    pageSectionDetail.VideoAsset = new VideoAsset { CreateUserId = currentUserId, AmendUserId = currentUserId };
                }

                pageSectionDetail.Id = await this.pageSectionDetailRepository.CreateAsync(currentUserId, pageSectionDetail);
                return pageSectionId;
            }
            catch (System.Exception ex)
            {
                var errorMessage = "Error creating page section.";
                this.logger.LogError(ex, errorMessage);
                throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// The save video asset async.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> SaveVideoAssetAsync(FileCreateRequestViewModel requestViewModel, int userId)
        {
            File newFile = new File()
            {
                FileTypeId = requestViewModel.FileTypeId,
                FileName = requestViewModel.FileName,
                FilePath = requestViewModel.FilePath,
                FileSizeKb = requestViewModel.FileSize,
                FileChunkDetailId = requestViewModel.FileChunkDetailId,
            };

            int fileId = await this.fileRepository.CreateAsync(userId, newFile);
            var videoAsset = await this.videoAssetRepository.GetByPageSectionDetailId(requestViewModel.PageSectionDetailId);

            var updateVideoAssetStateViewModel = new UpdateVideoAssetStateViewModel
            {
                PageSectionDetailId = requestViewModel.PageSectionDetailId,
                UserId = userId,
                PageSectionStatus = PageSectionStatus.Processing,
            };

            await this.UpdateVideoAssetStateAsync(updateVideoAssetStateViewModel);

            if (videoAsset == null)
            {
                videoAsset = new VideoAsset()
                {
                    PageSectionDetailId = requestViewModel.PageSectionDetailId,
                    VideoFileId = fileId,
                };
                await this.videoAssetRepository.CreateAsync(userId, videoAsset);
            }
            else
            {
                videoAsset.VideoFileId = fileId;
                videoAsset.VideoFile = null;
                videoAsset.AzureMediaAssetId = null;
                videoAsset.AzureMediaAsset = null;
                videoAsset.DurationInMilliseconds = null;
                videoAsset.Deleted = false;
                await this.videoAssetRepository.UpdateAsync(userId, videoAsset);
            }

            var processVideoAssetQueueMessage = new ProcessVideoAssetQueueMessage
            {
                PageSectionDetailId = videoAsset.PageSectionDetailId,
                VideoAssetFileId = videoAsset.VideoFileId.Value,
                UserId = userId,
            };
            var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.ContentManagementQueue);
            var contentManagementQueue = internalSystem.IsOffline ? $"{this.settings.ContentManagementQueueName}-temp" : this.settings.ContentManagementQueueName;
            await this.queueCommunicatorService.SendAsync(contentManagementQueue, processVideoAssetQueueMessage);
            var retVal = new LearningHubValidationResult(true)
            {
                CreatedId = fileId,
            };
            return retVal;
        }

        /// <summary>
        /// The SaveAttributeFileDetails.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> SaveAttributeFileDetails(FileCreateRequestViewModel requestViewModel, int currentUserId)
        {
            var videoAsset = await this.videoAssetRepository.GetByPageSectionDetailId(requestViewModel.PageSectionDetailId);
            int fileId = await this.fileRepository.CreateAsync(
                currentUserId,
                new File()
                {
                    FileTypeId = requestViewModel.FileTypeId,
                    FileName = requestViewModel.FileName,
                    FilePath = requestViewModel.FilePath,
                    FileChunkDetailId = requestViewModel.FileChunkDetailId,
                    FileSizeKb = requestViewModel.FileSize,
                });

            switch (requestViewModel.AttachedFileType)
            {
                case AttachedFileTypeEnum.Transcript:
                    videoAsset.TranscriptFile = null;
                    videoAsset.TranscriptFileId = fileId;
                    break;
                case AttachedFileTypeEnum.ClosedCaptions:
                    videoAsset.ClosedCaptionsFile = null;
                    videoAsset.ClosedCaptionsFileId = fileId;
                    break;
                case AttachedFileTypeEnum.ThumbnailImage:
                    videoAsset.ThumbnailImageFile = null;
                    videoAsset.ThumbnailImageFileId = fileId;
                    break;
            }

            await this.videoAssetRepository.UpdateAsync(currentUserId, videoAsset);

            var retVal = new LearningHubValidationResult(true)
            {
                CreatedId = fileId,
            };
            return retVal;
        }

        /// <summary>
        /// The UpdatePageSectionDetailsAsync.
        /// </summary>
        /// <param name="model">The model<see cref="PageSectionDetailViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdatePageSectionDetailsAsync(PageSectionDetailViewModel model, int currentUserId)
        {
            var pageSectionDetail = await this.pageSectionDetailRepository.GetPageSectionDetailImageAssetByIdAsync(model.Id);
            if (pageSectionDetail.PageSectionStatusId == (int)PageSectionStatus.Draft)
            {
                pageSectionDetail.PageSectionStatusId = (int)PageSectionStatus.Processed;
            }

            pageSectionDetail.SectionTitle = model.SectionTitle;
            pageSectionDetail.SectionTitleElement = model.SectionTitleElement;
            pageSectionDetail.TopMargin = model.TopMargin;
            pageSectionDetail.BottomMargin = model.BottomMargin;
            pageSectionDetail.HasBorder = model.HasBorder;
            pageSectionDetail.BackgroundColour = model.BackgroundColour;
            pageSectionDetail.TextColour = model.TextColour;
            pageSectionDetail.HyperLinkColour = model.HyperLinkColour;
            pageSectionDetail.Description = model.Description;
            pageSectionDetail.TextBackgroundColour = model.TextBackgroundColour;
            pageSectionDetail.SectionLayoutTypeId = (int)model.SectionLayoutType;
            await this.pageSectionDetailRepository.UpdateAsync(currentUserId, pageSectionDetail);
        }

        /// <summary>
        /// The UpdateVideoAssetAsync.
        /// </summary>
        /// <param name="videoAssetStateViewModel">videoAssetStateViewModel.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateVideoAssetStateAsync(UpdateVideoAssetStateViewModel videoAssetStateViewModel)
        {
            var pageSectionDetail = await this.pageSectionDetailRepository.GetPageSectionDetailImageAssetByIdAsync(videoAssetStateViewModel.PageSectionDetailId);
            pageSectionDetail.PageSectionStatusId = (int)videoAssetStateViewModel.PageSectionStatus;
            await this.pageSectionDetailRepository.UpdateAsync(videoAssetStateViewModel.UserId, pageSectionDetail);
        }

        /// <summary>
        /// The GetPageSectionDetailVideoAssetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetailViewModel}"/>.</returns>
        public async Task<PageSectionDetailViewModel> GetPageSectionDetailVideoAssetByIdAsync(int id)
        {
            var detail = await this.pageSectionDetailRepository.GetPageSectionDetailVideoAssetByIdAsync(id);

            return this.mapper.Map<PageSectionDetailViewModel>(detail);
        }

        /// <summary>
        /// The UpdateVideoAssetManifestDetailsAsync.
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="UpdateVideoAssetManifestRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateVideoAssetManifestDetailsAsync(UpdateVideoAssetManifestRequestViewModel viewModel)
        {
            var videoAsset = await this.videoAssetRepository.GetByPageSectionDetailId(viewModel.PageSectionDetailId);
            videoAsset.AzureMediaAssetId = viewModel.AzureMediaAssetId;
            videoAsset.DurationInMilliseconds = viewModel.DurationInMilliseconds;
            await this.videoAssetRepository.UpdateAsync(viewModel.UserId, videoAsset);
        }

        /// <summary>
        /// The UpdateVideoAssetAsync.
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="VideoAssetViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateVideoAssetAsync(VideoAssetViewModel viewModel)
        {
            var videoAsset = await this.videoAssetRepository.GetById(viewModel.Id);
            videoAsset.TranscriptFileId = viewModel.TranscriptFileId;
            videoAsset.ClosedCaptionsFileId = viewModel.ClosedCaptionsFileId;
            videoAsset.ThumbnailImageFileId = viewModel.ThumbnailImageFileId;
            await this.videoAssetRepository.UpdateAsync(viewModel.AmendUserId, videoAsset);
        }
    }
}
