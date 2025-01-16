namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Specialized;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The resource service.
    /// </summary>
    public class ResourceService : IResourceService
    {
        /// <summary>
        /// The learning hub service.
        /// </summary>
        private readonly ILearningHubService learningHubService;
        private readonly IResourceRepository resourceRepository;
        private readonly ILogger<ResourceService> logger;
        private readonly IMapper mapper;
        private readonly IFileTypeService fileTypeService;
        private readonly IBlockCollectionRepository blockCollectionRepository;
        private readonly IWebLinkResourceVersionRepository webLinkResourceVersionRepository;
        private readonly ICaseResourceVersionRepository caseResourceVersionRepository;
        private readonly IScormResourceVersionRepository scormResourceVersionRepository;
        private readonly IGenericFileResourceVersionRepository genericFileResourceVersionRepository;
        private readonly IResourceVersionRepository resourceVersionRepository;
        private readonly IHtmlResourceVersionRepository htmlResourceVersionRepository;
        private readonly IFileRepository fileRepository;
        private readonly AzureConfig azureConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceService"/> class.
        /// The search service.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="learningHubService">
        /// The <see cref="ILearningHubService"/>.
        /// </param>
        /// <param name="resourceRepository">
        /// The <see cref="IResourceRepository"/>.
        /// </param>
        public ResourceService(ILearningHubService learningHubService, IResourceRepository resourceRepository, ILogger<ResourceService> logger, IWebLinkResourceVersionRepository webLinkResourceVersionRepository, ICaseResourceVersionRepository caseResourceVersionRepository, IScormResourceVersionRepository scormResourceVersionRepository, IGenericFileResourceVersionRepository genericFileResourceVersionRepository, IResourceVersionRepository resourceVersionRepository, IHtmlResourceVersionRepository htmlResourceVersionRepository,IMapper mapper, IFileRepository fileRepository, IOptions<AzureConfig> azureConfig)
        {
            this.learningHubService = learningHubService;
            this.resourceRepository = resourceRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.azureConfig = azureConfig.Value;
            this.webLinkResourceVersionRepository = webLinkResourceVersionRepository;
            this.caseResourceVersionRepository = caseResourceVersionRepository;
            this.scormResourceVersionRepository = scormResourceVersionRepository;
            this.genericFileResourceVersionRepository = genericFileResourceVersionRepository;
            this.resourceVersionRepository = resourceVersionRepository;
            this.htmlResourceVersionRepository = htmlResourceVersionRepository;
            this.fileRepository = fileRepository;
        }

        /// <summary>
        /// the get by id async.
        /// </summary>
        /// <param name="originalResourceReferenceId">the id.</param>
        /// <param name="currentUserId">.</param>
        /// <returns>the resource.</returns>
        public async Task<ResourceReferenceWithResourceDetailsViewModel> GetResourceReferenceByOriginalId(int originalResourceReferenceId, int? currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            var list = new List<int>() { originalResourceReferenceId };

            var resourceReferences = await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(list);
            var resourceReferencesList = resourceReferences.ToList();

            try
            {
                var resourceReference = resourceReferencesList.SingleOrDefault();

                if (resourceReference == null)
                {
                    throw new HttpResponseException("No matching resource reference", HttpStatusCode.NotFound);
                }

                if (currentUserId.HasValue)
                {
                    List<int> resourceIds = new List<int>() { resourceReference.ResourceId };
                    List<int> userIds = new List<int>() { currentUserId.Value };

                    resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
                }

                return this.GetResourceReferenceWithResourceDetailsViewModel(resourceReference, resourceActivities);
            }
            catch (InvalidOperationException exception)
            {
                this.logger.LogError(exception, $"Multiple resource references found with OriginalResourceReferenceId {originalResourceReferenceId}");
                throw;
            }
        }

        /// <summary>
        /// bulk get by ids async.
        /// </summary>
        /// <param name="originalResourceReferenceIds">the resource reference ids.</param>
        /// <returns>the resource.</returns>
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds(List<int> originalResourceReferenceIds, int? currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            var resourceReferences = await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(originalResourceReferenceIds);
            var resourceReferencesList = resourceReferences.ToList();
            var matchedIds = resourceReferencesList.Select(r => r.OriginalResourceReferenceId).ToList();
            var unmatchedIds = originalResourceReferenceIds.Except(matchedIds).ToList();

            if (unmatchedIds.Any())
            {
                this.logger.LogInformation("Some resource ids not matched");
            }

            foreach (var duplicateIds in matchedIds.GroupBy(id => id).Where(groupOfIds => groupOfIds.Count() > 1))
            {
                this.logger.LogWarning($"Multiple resource references found with OriginalResourceReferenceId {duplicateIds.First()}");
            }

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = resourceReferencesList.Select(rrl => rrl.ResourceId).ToList();
                List<int> userIds = new List<int>() { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            List<ResourceReferenceWithResourceDetailsViewModel> matchedResources = resourceReferencesList
            .Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities.Where(ra => ra.ResourceId == rr.ResourceId).ToList()))
            .ToList<ResourceReferenceWithResourceDetailsViewModel>();

            return new BulkResourceReferenceViewModel(matchedResources, unmatchedIds);
        }


        /// <summary>
        /// the get by id async.
        /// </summary>
        /// <param name="activityStatusIds">.</param>
        /// <param name="currentUserId">c.</param>
        /// <returns>list resource ViewModel.</returns>
        public async Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferenceByActivityStatus(List<int> activityStatusIds, int currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<ResourceReferenceWithResourceDetailsViewModel> resourceReferenceWithResourceDetailsViewModelLS = new List<ResourceReferenceWithResourceDetailsViewModel>() { };

            resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(new List<int>(){ }, new List<int>(){ currentUserId }))?.ToList() ?? new List<ResourceActivityDTO>() { };

            // Removing resources that have no major versions with the required activitystatus
            List<int> resourceIds = resourceActivities
                .GroupBy(ra => ra.ResourceId)
                .Where(group => group.Any(g => activityStatusIds.Contains(g.ActivityStatusId)))
                .Select(group => group.Key)
                .Distinct()
                .ToList();

            var resourceReferencesList = (await this.resourceRepository.GetResourcesFromIds(resourceIds)).SelectMany(r => r.ResourceReference).ToList();

            resourceReferenceWithResourceDetailsViewModelLS = resourceReferencesList.Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities)).ToList();

            return resourceReferenceWithResourceDetailsViewModelLS;
        }

        /// <summary>
        /// Gets ResourceReferences ForCertificates using the ResourceReferenceWithResourceDetailsViewModel .
        /// </summary>
        /// <param name="currentUserId">user Id.</param>
        /// <returns>list resource reference ViewModel.</returns>
        public async Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferencesForCertificates(int currentUserId)
        {

            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<ResourceReferenceWithResourceDetailsViewModel> resourceReferenceWithResourceDetailsViewModelLS = new List<ResourceReferenceWithResourceDetailsViewModel>() { };
            List<int> achievedCertificatedResourceIds = (await this.resourceRepository.GetAchievedCertificatedResourceIds(currentUserId)).ToList();

            resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(achievedCertificatedResourceIds, new List<int>() { currentUserId }))?.ToList() ?? new List<ResourceActivityDTO>() { };

            var resourceList = (await this.resourceRepository.GetResourcesFromIds(achievedCertificatedResourceIds)).ToList();

            resourceReferenceWithResourceDetailsViewModelLS = resourceList.SelectMany(r => r.ResourceReference)
                .Distinct()
                .Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities)).ToList();

            return resourceReferenceWithResourceDetailsViewModelLS;
        }

        private ResourceReferenceWithResourceDetailsViewModel GetResourceReferenceWithResourceDetailsViewModel(ResourceReference resourceReference, List<ResourceActivityDTO> resourceActivities)
        {
            var hasCurrentResourceVersion = resourceReference.Resource.CurrentResourceVersion != null;
            var hasRating = resourceReference.Resource.CurrentResourceVersion?.ResourceVersionRatingSummary != null;

            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            if (resourceActivities != null && resourceActivities.Count != 0)
            {
                majorVersionIdActivityStatusDescription = ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resourceReference.Resource, resourceActivities).ToList();
            }

            if (resourceReference.Resource == null)
            {
                throw new Exception("No matching resource");
            }

            if (!hasCurrentResourceVersion)
            {
                this.logger.LogInformation($"Resource with OriginalResourceReferenceId {resourceReference.Id} is missing a current resource version");
            }

            if (!hasRating)
            {
                this.logger.LogInformation($"Resource with Id: {resourceReference.ResourceId} is missing a ResourceVersionRatingSummary");
            }

            var resourceTypeNameOrEmpty = resourceReference.Resource.GetResourceTypeNameOrEmpty();
            if (resourceTypeNameOrEmpty == string.Empty)
            {
                this.logger.LogError($"Resource has unrecognised type: {resourceReference.Resource.ResourceTypeEnum}");
            }

            return new ResourceReferenceWithResourceDetailsViewModel(
                resourceReference.ResourceId,
                resourceReference.OriginalResourceReferenceId,
                resourceReference.Resource.CurrentResourceVersion?.Title ?? ResourceHelpers.NoResourceVersionText,
                resourceReference.Resource.CurrentResourceVersion?.Description ?? string.Empty,
                resourceReference.GetCatalogue(),
                resourceTypeNameOrEmpty,
                resourceReference.Resource?.CurrentResourceVersion?.MajorVersion ?? 0,
                resourceReference.Resource?.CurrentResourceVersion?.ResourceVersionRatingSummary?.AverageRating ?? 0,
                this.learningHubService.GetResourceLaunchUrl(resourceReference.OriginalResourceReferenceId),
                majorVersionIdActivityStatusDescription);
        }

        /// <summary>
        /// The get resource by id async.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Resource}"/>.</returns>
        public async Task<Resource> GetResourceByIdAsync(int id)
        {
            var resource = await this.resourceRepository.GetByIdAsync(id);

            return resource;
        }

        /// <summary>
        /// The get generic file details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{GenericFileViewModel}"/>.</returns>
        public async Task<GenericFileViewModel> GetGenericFileDetailsByIdAsync(int resourceVersionId)
        {
            var genericFile = await this.genericFileResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            var vm = this.mapper.Map<GenericFileViewModel>(genericFile);

            // User id is used to populate a field we aren't going to use, so we can just pass in the system user id.
            var externalContentDetails = await this.resourceVersionRepository.GetExternalContentDetails(resourceVersionId, 4);
            if (!string.IsNullOrEmpty(externalContentDetails?.ExternalReference))
            {
                vm.HostedContentUrl = this.learningHubService.GetExternalResourceLaunchUrl(externalContentDetails.ExternalReference).ToLower();
                vm.FullHistoricUrl = externalContentDetails.FullHistoricUrl;
            }

            return vm;
        }


        public async Task<HtmlResourceViewModel> GetHtmlDetailsByIdAsync(int resourceVersionId)
        {
            var htmlFile = await this.htmlResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            var vm = this.mapper.Map<HtmlResourceViewModel>(htmlFile);

            // User id is used to populate a field we aren't going to use, so we can just pass in the system user id.
            var externalContentDetails = await this.resourceVersionRepository.GetExternalContentDetails(resourceVersionId, 4);
            if (!string.IsNullOrEmpty(externalContentDetails?.ExternalReference))
            {
                vm.HostedContentUrl = this.learningHubService.GetExternalResourceLaunchUrl(externalContentDetails.ExternalReference).ToLower();
                vm.FullHistoricUrl = externalContentDetails.FullHistoricUrl;
            }

            return vm;
        }


        /// <summary>
        /// The get scorm details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ScormViewModel}"/>.</returns>
        public async Task<ScormViewModel> GetScormDetailsByIdAsync(int resourceVersionId)
        {
            var scorm = await this.scormResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            var vm = this.mapper.Map<ScormViewModel>(scorm);

            // User id is used to populate a field we aren't going to use, so we can just pass in the system user id.
            var externalContentDetails = await this.resourceVersionRepository.GetExternalContentDetails(resourceVersionId, 4);
            if (!string.IsNullOrEmpty(externalContentDetails?.ExternalReference))
            {
                vm.HostedContentUrl = this.learningHubService.GetExternalResourceLaunchUrl(externalContentDetails.ExternalReference).ToLower();
                vm.FullHistoricUrl = externalContentDetails.FullHistoricUrl;
            }

            return vm;
        }


        /// <summary>
        /// The get web link resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{WebLinkViewModel}"/>.</returns>
        public async Task<WebLinkViewModel> GetWebLinkDetailsByIdAsync(int resourceVersionId)
        {
            var e = await this.webLinkResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);

            var vm = this.mapper.Map<WebLinkViewModel>(e);

            return vm;
        }

        /// <summary>
        /// The get case resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<CaseViewModel> GetCaseDetailsByIdAsync(int resourceVersionId)
        {
            CaseResourceVersion caseResourceVersion = await this.caseResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            BlockCollection blockCollection = await this.blockCollectionRepository.GetBlockCollection(caseResourceVersion?.BlockCollectionId);

            BlockCollectionViewModel blockCollectionViewModel = null;
            if (blockCollection != null)
            {
                blockCollectionViewModel = this.mapper.Map<BlockCollectionViewModel>(blockCollection);
            }

            await this.AddPartialFileDetails(blockCollectionViewModel);

            CaseViewModel caseViewModel = new CaseViewModel
            {
                ResourceVersionId = resourceVersionId,
                BlockCollection = blockCollectionViewModel,
            };

            return caseViewModel;
        }


        /// <summary>
        /// The GetFileStatusDetailsAsync.
        /// </summary>
        /// <param name="fileIds">The File Ids.</param>
        /// <returns>The files.</returns>
        public async Task<List<FileViewModel>> GetFileStatusDetailsAsync(int[] fileIds)
        {
            List<File> files = await this.fileRepository
                .GetAll()
                .Where(file => fileIds.Contains(file.Id))
                .Include(file => file.PartialFile)
                .Include(file => file.VideoFile)
                .Include(file => file.WholeSlideImageFile)
                .ToListAsync();

            List<FileViewModel> fileViewModels = this.mapper.Map<List<FileViewModel>>(files);

            return fileViewModels;
        }

        /// <summary>
        /// The get resource licences async.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{File}"/>.</returns>
        public async Task<File> GetFileAsync(int fileId)
        {
            File file = await this.fileRepository.GetByIdAsync(fileId);
            return file;
        }

        private async Task AddPartialFileDetails(BlockCollectionViewModel blockCollectionViewModel)
        {
            if (blockCollectionViewModel?.Blocks != null)
            {
                foreach (var blockViewModel in blockCollectionViewModel.Blocks)
                {
                    switch (blockViewModel.BlockType)
                    {
                        case BlockType.None:
                            break;
                        case BlockType.Text:
                            break;
                        case BlockType.PageBreak:
                            break;
                        case BlockType.WholeSlideImage:
                            await this.AddPartialFileDetailsToWholeSlideImages(blockViewModel);
                            break;
                        case BlockType.Media:
                            await this.AddPartialFileDetailsToMediaBlock(blockViewModel);
                            break;
                        case BlockType.Question:
                            await this.AddPartialFileDetailsToQuestionBlock(blockViewModel);
                            break;
                        case BlockType.ImageCarousel:
                            await this.AddPartialFileDetailsToImageCarouselBlock(blockViewModel);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(blockViewModel.BlockType), blockViewModel.BlockType, null);
                    }
                }
            }
        }

        private async Task AddPartialFileDetailsToWholeSlideImages(BlockViewModel blockViewModel)
        {
            foreach (var wholeSlideImageBlockItem in blockViewModel?.WholeSlideImageBlock?.WholeSlideImageBlockItems
                                                     ?? Enumerable.Empty<WholeSlideImageBlockItemViewModel>())
            {
                await this.AddUploadedFileSize(wholeSlideImageBlockItem.WholeSlideImage.File);
            }
        }

        private async Task AddPartialFileDetailsToMediaBlock(BlockViewModel blockViewModel)
        {
            switch (blockViewModel.MediaBlock.MediaType)
            {
                case MediaType.Attachment:
                    var attachment = blockViewModel.MediaBlock?.Attachment;
                    await this.AddUploadedFileSize(attachment?.File);
                    break;
                case MediaType.Image:
                    var image = blockViewModel.MediaBlock?.Image;
                    await this.AddUploadedFileSize(image?.File);
                    break;
                case MediaType.Video:
                    var video = blockViewModel.MediaBlock?.Video;
                    await this.AddUploadedFileSize(video?.File);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(blockViewModel.MediaBlock.MediaType), blockViewModel.MediaBlock.MediaType, null);
            }
        }

        private async Task AddPartialFileDetailsToQuestionBlock(BlockViewModel blockViewModel)
        {
            if (blockViewModel.QuestionBlock != null)
            {
                var questionTask = this.AddPartialFileDetails(blockViewModel.QuestionBlock.QuestionBlockCollection);
                var feedbackTask = this.AddPartialFileDetails(blockViewModel.QuestionBlock.FeedbackBlockCollection);
                var answersTask = Task.WhenAll(blockViewModel.QuestionBlock.Answers.Select(a => this.AddPartialFileDetails(a.BlockCollection)));
                await Task.WhenAll(questionTask, feedbackTask, answersTask);
            }
        }

        private async Task AddPartialFileDetailsToImageCarouselBlock(BlockViewModel blockViewModel)
        {
            if (blockViewModel.ImageCarouselBlock != null)
            {
                var imageTask = this.AddPartialFileDetails(blockViewModel.ImageCarouselBlock.ImageBlockCollection);

                await Task.WhenAll(imageTask);
            }
        }

        private async Task AddUploadedFileSize(FileViewModel file)
        {
            if (file?.PartialFile != null)
            {
                var blobServiceClient = new BlobServiceClient(this.azureConfig.AzureBlobSettings.ConnectionString);

                var container = blobServiceClient.GetBlobContainerClient(this.azureConfig.AzureBlobSettings.UploadContainer);

                var appendBlobClient = container.GetAppendBlobClient($"partial-files/{file.FileId}");
                if (appendBlobClient.Exists())
                {
                    var props = await appendBlobClient.GetPropertiesAsync();
                    file.PartialFile.UploadedFileSize = props.Value.ContentLength;
                }
            }
        }
    }
}
