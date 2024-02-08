namespace LearningHub.Nhs.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Files;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The Partial FileService.
    /// </summary>
    public class PartialFileService : IPartialFileService
    {
        private readonly IFileRepository fileRepository;
        private readonly IPartialFileRepository partialFileRepository;
        private readonly IWholeSlideImageFileRepository wholeSlideImageFileRepository;
        private readonly IVideoFileRepository videoFileRepository;
        private readonly IQueueCommunicatorService queueCommunicatorService;
        private readonly IResourceService resourceService;
        private readonly IInternalSystemService internalSystemService;
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialFileService"/> class.
        /// </summary>
        /// <param name="fileRepository">
        /// The File Repository.
        /// </param>
        /// <param name="partialFileRepository">
        /// The Partial File Repository.
        /// </param>
        /// <param name="wholeSlideImageFileRepository">
        /// The Whole Slide Image File Repository.
        /// </param>
        /// <param name="videoFileRepository">
        /// The Video File Repository.
        /// </param>
        /// <param name="queueCommunicatorService">
        /// The Queue Communicator Service.
        /// </param>
        /// <param name="resourceService">
        /// The Resource Service.
        /// </param>
        /// <param name="internalSystemService">
        /// The internalSystemService.
        /// </param>
        /// <param name="settings">
        /// The Settings.
        /// </param>
        public PartialFileService(
            IFileRepository fileRepository,
            IPartialFileRepository partialFileRepository,
            IWholeSlideImageFileRepository wholeSlideImageFileRepository,
            IVideoFileRepository videoFileRepository,
            IQueueCommunicatorService queueCommunicatorService,
            IResourceService resourceService,
            IInternalSystemService internalSystemService,
            IOptions<Settings> settings)
        {
            this.fileRepository = fileRepository;
            this.partialFileRepository = partialFileRepository;
            this.wholeSlideImageFileRepository = wholeSlideImageFileRepository;
            this.videoFileRepository = videoFileRepository;
            this.queueCommunicatorService = queueCommunicatorService;
            this.resourceService = resourceService;
            this.internalSystemService = internalSystemService;
            this.settings = settings.Value;
        }

        /// <inheritdoc/>
        public async Task<PartialFileViewModel> CreatePartialFile(PartialFileViewModel viewModel, int userId)
        {
            string filePath = Guid.NewGuid().ToString();

            var file = new File
            {
                FileTypeId = 0, // TODO work out how to set the File Type ID
                FileChunkDetailId = null, // We are not using chunks here
                FileName = viewModel.FileName,
                FilePath = filePath,
                FileSizeKb = 0,
                Deleted = false,
            };
            int fileId = await this.fileRepository.CreateAsync(userId, file);

            var partialFile = new PartialFile
            {
                FileId = fileId,
                TotalSize = viewModel.TotalFileSize,
            };
            await this.partialFileRepository.CreateAsync(userId, partialFile);

            switch (viewModel.PostProcessingOptions)
            {
                case PartialFilePostProcessingOptions.WholeSlideImage:
                    var wholeSlideImageFile = new WholeSlideImageFile
                    {
                        FileId = fileId,
                        Status = WholeSlideImageFileStatus.Uploading,
                    };
                    await this.wholeSlideImageFileRepository.CreateAsync(userId, wholeSlideImageFile);

                    break;
                case PartialFilePostProcessingOptions.Video:
                    var videoFile = new VideoFile
                    {
                        FileId = fileId,
                        Status = VideoFileStatus.Uploading,
                    };
                    await this.videoFileRepository.CreateAsync(userId, videoFile);

                    break;
                case PartialFilePostProcessingOptions.None:
                default:
                    break;
            }

            viewModel.FileId = fileId;
            viewModel.FilePath = filePath;

            return viewModel;
        }

        /// <inheritdoc/>
        public async Task<File> GetFile(int fileId)
        {
            return await this.fileRepository.GetByIdAsync(fileId);
        }

        /// <inheritdoc/>
        public async Task<PartialFile> GetPartialFile(int fileId)
        {
            return await this.partialFileRepository.GetByFileIdAsync(fileId);
        }

        /// <inheritdoc/>
        public async Task CompletePartialFile(File file, PartialFile partialFile, int userId)
        {
            file.FileSizeKb = (int)(partialFile.TotalSize / 1000);
            this.fileRepository.Update(userId, file);

            WholeSlideImageFile wholeSlideImageFile = await this.wholeSlideImageFileRepository.GetByFileIdAsync(file.Id);
            if (wholeSlideImageFile != null)
            {
                if (file.FileName.ToLowerInvariant().EndsWith(".zip"))
                {
                    var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.WholeSlideImageProcessingQueue);
                    var wholeSlideImageProcessingQueue = internalSystem.IsOffline ? $"{this.settings.WholeSlideImageProcessingQueueRouteName}-temp" : this.settings.WholeSlideImageProcessingQueueRouteName;
                    await this.queueCommunicatorService.SendAsync(wholeSlideImageProcessingQueue, file.Id);
                }
                else
                {
                    var resourceVersion = await this.resourceService.GetResourceVersionForWholeSlideImageAsync(file.Id);
                    dynamic wholeSlideQueueItem = new JObject();
                    wholeSlideQueueItem.InputFileName = file.FileName;
                    wholeSlideQueueItem.InputFilePath = file.FilePath;
                    wholeSlideQueueItem.DeepZoomOverlap = 2;
                    wholeSlideQueueItem.DeepZoomTileSize = 256;
                    wholeSlideQueueItem.FileId = file.Id;
                    wholeSlideQueueItem.ResourceVersionId = resourceVersion?.ResourceVersionId;
                    wholeSlideQueueItem.UserId = userId;

                    var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.WholeSlideImageProcessingJavaQueue);
                    var wholeSlideImageProcessingQueue = internalSystem.IsOffline ? $"{this.settings.JavaWholeSlideImageProcessingQueueRouteName}-temp" : this.settings.JavaWholeSlideImageProcessingQueueRouteName;
                    await this.queueCommunicatorService.SendAsync(wholeSlideImageProcessingQueue, wholeSlideQueueItem);
                }

                wholeSlideImageFile.Status = WholeSlideImageFileStatus.QueuedForProcessing;
                await this.wholeSlideImageFileRepository.UpdateAsync(userId, wholeSlideImageFile);
            }

            VideoFile videoFile = await this.videoFileRepository.GetByFileIdAsync(file.Id);
            if (videoFile != null)
            {
                var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.VideoProcessingQueue);
                var videoProcessingQueue = internalSystem.IsOffline ? $"{this.settings.VideoProcessingQueueRouteName}-temp" : this.settings.VideoProcessingQueueRouteName;
                await this.queueCommunicatorService.SendAsync(videoProcessingQueue, file.Id);
                videoFile.Status = VideoFileStatus.QueuedForProcessing;
                await this.videoFileRepository.UpdateAsync(userId, videoFile);
            }

            partialFile.Deleted = true;
            this.partialFileRepository.Update(userId, partialFile);
        }
    }
}
