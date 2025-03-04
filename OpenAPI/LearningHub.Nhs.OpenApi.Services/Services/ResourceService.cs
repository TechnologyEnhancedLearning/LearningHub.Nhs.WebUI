namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Specialized;
    using LearningHub.Nhs.Models.Constants;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.Files;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Migrations;
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
        private readonly IUserProfileService userProfileService;
        private readonly IBlockCollectionRepository blockCollectionRepository;
        private readonly IWebLinkResourceVersionRepository webLinkResourceVersionRepository;
        private readonly ICaseResourceVersionRepository caseResourceVersionRepository;
        private readonly IScormResourceVersionRepository scormResourceVersionRepository;
        private readonly IGenericFileResourceVersionRepository genericFileResourceVersionRepository;
        private readonly IResourceVersionRepository resourceVersionRepository;
        private readonly IAssessmentResourceActivityMatchQuestionRepository assessmentResourceActivityMatchQuestionRepository;
        private readonly IHtmlResourceVersionRepository htmlResourceVersionRepository;
        private readonly IArticleResourceVersionRepository articleResourceVersionRepository;
        private readonly IArticleResourceVersionFileRepository articleResourceVersionFileRepository;
        private readonly IAudioResourceVersionRepository audioResourceVersionRepository;
        private readonly IBookmarkRepository bookmarkRepository;
        private readonly IImageResourceVersionRepository imageResourceVersionRepository;
        private readonly IVideoResourceVersionRepository videoResourceVersionRepository;
        private readonly IAssessmentResourceVersionRepository assessmentResourceVersionRepository;
        private readonly IResourceLicenceRepository resourceLicenceRepository;
        private readonly IResourceReferenceRepository resourceReferenceRepository;
        private readonly IResourceVersionUserAcceptanceRepository resourceVersionUserAcceptanceRepository;
        private readonly IResourceVersionFlagRepository resourceVersionFlagRepository;
        private readonly IResourceVersionValidationResultRepository resourceVersionValidationResultRepository;
        private readonly IResourceVersionKeywordRepository resourceVersionKeywordRepository;
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;
        private readonly IEmbeddedResourceVersionRepository embeddedResourceVersionRepository;
        private readonly IVideoRepository videoRepository;
        private readonly IWholeSlideImageRepository wholeSlideImageRepository;
        private readonly INodePathRepository nodePathRepository;
        private readonly INodeResourceRepository nodeResourceRepository;
        private readonly IEquipmentResourceVersionRepository equipmentResourceVersionRepository;
        private readonly IPublicationRepository publicationRepository;
        private readonly IQuestionBlockRepository questionBlockRepository;
        private readonly IMigrationSourceRepository migrationSourceRepository;
        private readonly LearningHubDbContext dbContext;
        private readonly INodeRepository nodeRepository;
        private readonly IFileRepository fileRepository;
        private readonly AzureConfig azureConfig;
        private readonly LearningHubConfig learningHubConfig;
        private readonly ICachingService cachingService;
        private readonly ISearchService searchService;
        private readonly ICatalogueService catalogueService;
        private readonly IUserService userService;
        private readonly IProviderService providerService;
        private readonly IInternalSystemService internalSystemService;
        private readonly IQueueCommunicatorService queueCommunicatorService;
        private readonly IResourceVersionProviderRepository resourceVersionProviderRepository;
        private readonly IResourceVersionAuthorRepository resourceVersionAuthorRepository;
        private readonly IFileChunkDetailRepository fileChunkDetailRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceService"/> class.
        /// The search service.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="webLinkResourceVersionRepository"></param>
        /// <param name="caseResourceVersionRepository"></param>
        /// <param name="scormResourceVersionRepository"></param>
        /// <param name="genericFileResourceVersionRepository"></param>
        /// <param name="resourceVersionRepository"></param>
        /// <param name="htmlResourceVersionRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="fileRepository"></param>
        /// <param name="azureConfig"></param>
        /// <param name="learningHubConfig"></param>
        /// <param name="userProfileService"></param>
        /// <param name="resourceVersionFlagRepository"></param>
        /// <param name="articleResourceVersionRepository"></param>
        /// <param name="audioResourceVersionRepository"></param>
        /// <param name="videoResourceVersionRepository"></param>
        /// <param name="assessmentResourceVersionRepository"></param>
        /// <param name="resourceLicenceRepository"></param>
        /// <param name="resourceReferenceRepository"></param>
        /// <param name="resourceVersionUserAcceptanceRepository"></param>
        /// <param name="catalogueNodeVersionRepository"></param>
        /// <param name="cachingService"></param>
        /// <param name="searchService"></param>
        /// <param name="catalogueService"></param>
        /// <param name="nodeResourceRepository"></param>
        /// <param name="nodePathRepository"></param>
        /// <param name="userService"></param>
        /// <param name="nodeRepository"></param>
        /// <param name="dbContext"></param>
        /// <param name=""></param>
        /// <param name="learningHubService">
        /// The <see cref="ILearningHubService"/>.
        /// </param>
        /// <param name="internalSystemService"></param>
        /// <param name="resourceVersionAuthorRepository"></param>
        /// <param name="fileChunkDetailRepository"></param>
        /// <param name="queueCommunicatorService"></param>
        /// <param name="resourceRepository">
        /// The <see cref="IResourceRepository"/>.
        /// </param>
        /// <param name="resourceVersionProviderRepository"></param>
        /// <param name="providerService"></param>
        /// <param name="articleResourceVersionFileRepository"></param>
        /// <param name="publicationRepository"></param>
        /// <param name="migrationSourceRepository"></param>
        /// <param name="questionBlockRepository"></param>
        /// <param name="videoRepository"></param>
        /// <param name="wholeSlideImageRepository"></param>
        /// <param name="embeddedResourceVersionRepository"></param>
        /// <param name="equipmentResourceVersionRepository"></param>
        /// <param name="imageResourceVersionRepository"></param>
        /// <param name="bookmarkRepository"></param>
        /// <param name="assessmentResourceActivityMatchQuestionRepository"></param>
        /// <param name="resourceVersionKeywordRepository"></param>
        /// <param name="resourceVersionValidationResultRepository"></param>
        public ResourceService(ILearningHubService learningHubService, IInternalSystemService internalSystemService, IResourceVersionAuthorRepository resourceVersionAuthorRepository, IFileChunkDetailRepository fileChunkDetailRepository, IQueueCommunicatorService queueCommunicatorService, IResourceRepository resourceRepository, IResourceVersionProviderRepository resourceVersionProviderRepository, IProviderService providerService, IArticleResourceVersionFileRepository articleResourceVersionFileRepository, IPublicationRepository publicationRepository, IMigrationSourceRepository migrationSourceRepository, IQuestionBlockRepository questionBlockRepository, IVideoRepository videoRepository, IWholeSlideImageRepository wholeSlideImageRepository, IEmbeddedResourceVersionRepository embeddedResourceVersionRepository, IEquipmentResourceVersionRepository equipmentResourceVersionRepository, IImageResourceVersionRepository imageResourceVersionRepository, IBookmarkRepository bookmarkRepository, IAssessmentResourceActivityMatchQuestionRepository assessmentResourceActivityMatchQuestionRepository, IResourceVersionKeywordRepository resourceVersionKeywordRepository, IResourceVersionValidationResultRepository resourceVersionValidationResultRepository, ILogger<ResourceService> logger, IWebLinkResourceVersionRepository webLinkResourceVersionRepository, ICaseResourceVersionRepository caseResourceVersionRepository, IScormResourceVersionRepository scormResourceVersionRepository, IGenericFileResourceVersionRepository genericFileResourceVersionRepository, IResourceVersionRepository resourceVersionRepository, IHtmlResourceVersionRepository htmlResourceVersionRepository, IMapper mapper, IFileRepository fileRepository, IOptions<AzureConfig> azureConfig, IOptions<LearningHubConfig> learningHubConfig, IUserProfileService userProfileService, IResourceVersionFlagRepository resourceVersionFlagRepository, IArticleResourceVersionRepository articleResourceVersionRepository, IAudioResourceVersionRepository audioResourceVersionRepository, IVideoResourceVersionRepository videoResourceVersionRepository, IAssessmentResourceVersionRepository assessmentResourceVersionRepository, IResourceLicenceRepository resourceLicenceRepository, IResourceReferenceRepository resourceReferenceRepository, IResourceVersionUserAcceptanceRepository resourceVersionUserAcceptanceRepository, ICatalogueNodeVersionRepository catalogueNodeVersionRepository, ICachingService cachingService, ISearchService searchService, ICatalogueService catalogueService, INodeResourceRepository nodeResourceRepository, INodePathRepository nodePathRepository, IUserService userService, INodeRepository nodeRepository, LearningHubDbContext dbContext)
        {
            this.learningHubService = learningHubService;
            this.resourceRepository = resourceRepository;
            this.resourceVersionAuthorRepository = resourceVersionAuthorRepository;
            this.fileChunkDetailRepository = fileChunkDetailRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.catalogueService = catalogueService;
            this.azureConfig = azureConfig.Value;
            this.learningHubConfig = learningHubConfig.Value;
            this.webLinkResourceVersionRepository = webLinkResourceVersionRepository;
            this.caseResourceVersionRepository = caseResourceVersionRepository;
            this.scormResourceVersionRepository = scormResourceVersionRepository;
            this.genericFileResourceVersionRepository = genericFileResourceVersionRepository;
            this.resourceVersionRepository = resourceVersionRepository;
            this.equipmentResourceVersionRepository = equipmentResourceVersionRepository;
            this.htmlResourceVersionRepository = htmlResourceVersionRepository;
            this.imageResourceVersionRepository = imageResourceVersionRepository;
            this.embeddedResourceVersionRepository = embeddedResourceVersionRepository;
            this.bookmarkRepository = bookmarkRepository;
            this.fileRepository = fileRepository;
            this.userProfileService = userProfileService;
            this.assessmentResourceActivityMatchQuestionRepository = assessmentResourceActivityMatchQuestionRepository;
            this.articleResourceVersionRepository = articleResourceVersionRepository;
            this.articleResourceVersionFileRepository = articleResourceVersionFileRepository;
            this.audioResourceVersionRepository = audioResourceVersionRepository;
            this.videoResourceVersionRepository = videoResourceVersionRepository;
            this.resourceReferenceRepository = resourceReferenceRepository;
            this.resourceLicenceRepository = resourceLicenceRepository;
            this.resourceVersionFlagRepository = resourceVersionFlagRepository;
            this.resourceVersionUserAcceptanceRepository = resourceVersionUserAcceptanceRepository;
            this.resourceVersionValidationResultRepository = resourceVersionValidationResultRepository;
            this.resourceVersionKeywordRepository= resourceVersionKeywordRepository;
            this.resourceVersionProviderRepository= resourceVersionProviderRepository;
            this.providerService = providerService;
            this.nodePathRepository = nodePathRepository;
            this.nodeResourceRepository = nodeResourceRepository;
            this.nodeRepository = nodeRepository;
            this.searchService = searchService;
            this.cachingService = cachingService;
            this.userService = userService;
            this.publicationRepository = publicationRepository;
            this.questionBlockRepository = questionBlockRepository;
            this.migrationSourceRepository = migrationSourceRepository;
            this.queueCommunicatorService = queueCommunicatorService;
            this.internalSystemService = internalSystemService;
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

        /// <summary>
        /// The get resource licences async.
        /// </summary>
        /// <returns>The <see cref="List{ResourceLicenceViewModel}"/>.</returns>
        public async Task<List<ResourceLicenceViewModel>> GetResourceLicencesAsync()
        {
            var licences = this.resourceLicenceRepository.GetAll().OrderBy(l => l.DisplayOrder);
            var licenceList = await this.mapper.ProjectTo<ResourceLicenceViewModel>(licences).ToListAsync();
            return licenceList;
        }

        /// <summary>
        /// The get resource version view model async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersionViewModel> GetResourceVersionViewModelAsync(int resourceVersionId)
        {
            var rvvm = new ResourceVersionViewModel();

            await this.GetResourceVersionViewModel(resourceVersionId, rvvm);

            return rvvm;
        }

        /// <summary>
        /// The get resource version view model from video file id async.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionViewModel}"/>.</returns>
        public async Task<ResourceVersionViewModel> GetResourceVersionForVideoAsync(int fileId)
        {
            int resourceVersionId = await this.GetResourceVersionIdForVideo(fileId);
            if (resourceVersionId == -1)
            {
                return null;
            }

            return await this.GetResourceVersionViewModelAsync(resourceVersionId);
        }

        /// <summary>
        /// The get resource version view model from whole slide image file id async.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionViewModel}"/>.</returns>
        public async Task<ResourceVersionViewModel> GetResourceVersionForWholeSlideImageAsync(int fileId)
        {
            int resourceVersionId = await this.GetResourceVersionIdForWholeSlideImage(fileId);
            if (resourceVersionId == -1)
            {
                return null;
            }

            return await this.GetResourceVersionViewModelAsync(resourceVersionId);
        }

        /// <summary>
        /// The get resource version view model by resource reference async.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersionViewModel> GetResourceVersionByResourceReferenceAsync(int resourceReferenceId)
        {
            var rv = await this.resourceVersionRepository.GetCurrentForResourceReferenceIdAsync(resourceReferenceId);

            return await this.GetResourceVersionViewModelAsync(rv.Id);
        }

        /// <summary>
        /// The unpublish resource version.
        /// </summary>
        /// <param name="unpublishViewModel">The unpublishViewModel<see cref="UnpublishViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UnpublishResourceVersion(UnpublishViewModel unpublishViewModel, int userId)
        {
            // Note the active NodePath before unpublishing - needed for cache update.
            NodePath activeNodePath = null;
            var currentResourceVersion = await this.resourceVersionRepository.GetBasicByIdAsync(unpublishViewModel.ResourceVersionId);

            if (currentResourceVersion == null)
            {
                return new LearningHubValidationResult(false, $"Attempt to unpublish resource version failed. Resource version not found. resourceVersionId: {unpublishViewModel.ResourceVersionId}, userId: {userId}");
            }
            else if (!await this.UserCanEditResourceVersion(currentResourceVersion.CreateUserId, currentResourceVersion.Id, userId) && !this.userService.IsAdminUser(userId))
            {
                return new LearningHubValidationResult(false, $"Attempt to unpublish resource version by invalid user. resourceVersionId: {unpublishViewModel.ResourceVersionId}, userId: {userId}");
            }
            else if (currentResourceVersion.VersionStatusEnum == VersionStatusEnum.Unpublished)
            {
                return new LearningHubValidationResult(false, $"Resource version has already been unpublished. resourceVersionId: {unpublishViewModel.ResourceVersionId}, userId: {userId}");
            }

            var nodeResource = (await this.nodeResourceRepository.GetByResourceIdAsync(currentResourceVersion.ResourceId))
                                    .Where(nr => nr.VersionStatusEnum == VersionStatusEnum.Published).FirstOrDefault();

            if (nodeResource != null)
            {
                activeNodePath = (await this.nodePathRepository.GetNodePathsForNodeId(nodeResource.NodeId))
                                        .Where(np => np.IsActive).FirstOrDefault();
            }

            // Unpublish the ResourceVersion
            var validator = new UnpublishViewModelValidator();
            var vr = await validator.ValidateAsync(unpublishViewModel);
            var retVal = new LearningHubValidationResult(vr);

            if (retVal.IsValid)
            {
                if (unpublishViewModel.SetFlag)
                {
                    var f = new ResourceVersionFlagViewModel()
                    {
                        ResourceVersionId = unpublishViewModel.ResourceVersionId,
                        Details = unpublishViewModel.Details,
                        IsActive = true,
                    };

                    await this.SaveResourceVersionFlagAsync(f, userId);
                }

                this.resourceVersionRepository.Unpublish(unpublishViewModel.ResourceVersionId, unpublishViewModel.Details, userId);

                // Remove from Findwise Search Index
                var r = await this.resourceVersionRepository.GetBasicByIdAsync(unpublishViewModel.ResourceVersionId);
                await this.searchService.RemoveResourceFromSearchAsync(r.ResourceId);

                // Remove from ScormContent server cache
                if (currentResourceVersion.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm)
                {
                    await this.RemoveExternalReferenceFromCache(currentResourceVersion.Resource);
                }
            }

            // Cache updates
            // IT1: note that GetNodeContents includes "HasPublishedResourcesInBranch" indication.
            // This requires that the node contents of the Nodes on the path to the unpublished Resource are refreshed.
            // ContentStructure not available on Community contributions catalogue, so no need to refresh cache if nodeId=1.
            if (activeNodePath != null && activeNodePath.NodeId > 1)
            {
                string[] nodePath = activeNodePath.NodePathString.Split("\\");
                int[] nodeIds = Array.ConvertAll(nodePath, int.Parse);
                foreach (int nodeId in nodeIds)
                {
                    // Ignore Community Catalogue
                    if (nodeId > 1)
                    {
                        string nodeContentsCacheKey = $"{CacheKeys.PublishedNodeContents}:{nodeId}";
                        var publishedNodeContents = await this.nodeRepository.GetNodeContentsForCatalogueBrowse(nodeId, false);
                        await this.cachingService.SetAsync(nodeContentsCacheKey, publishedNodeContents);
                    }
                }
            }

            // Remove unpublished resource version from cache.
            string cacheKey = $"{CacheKeys.ResourceVersionExtended}:{unpublishViewModel.ResourceVersionId}";
            await this.cachingService.RemoveAsync(cacheKey);

            return retVal;
        }


        /// <summary>
        /// The revert publishing resource version to draft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> RevertToDraft(int resourceVersionId, int userId)
        {
            var retVal = new LearningHubValidationResult(true);

            var rv = await this.resourceVersionRepository.GetByIdAsync(resourceVersionId, true);
            if (rv.VersionStatusEnum == VersionStatusEnum.FailedToPublish || rv.VersionStatusEnum == VersionStatusEnum.Publishing)
            {
                this.resourceVersionRepository.RevertToDraft(resourceVersionId, userId);
            }
            else
            {
                retVal.Add(new LearningHubValidationResult(false, "Resource Version must have a status of 'Failed to Publish' or 'Publishing' in order to revert to a status of 'Draft'"));
            }

            return retVal;
        }

        /// <summary>
        /// Creation of sensitive content acceptance.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> AcceptSensitiveContentAsync(int resourceVersionId, int currentUserId)
        {
            var rvua = new ResourceVersionUserAcceptance();
            rvua.ResourceVersionId = resourceVersionId;
            rvua.UserId = currentUserId;

            var validator = new ResourceVersionUserAcceptanceValidator();
            var vr = await validator.ValidateAsync(rvua);
            var retVal = new LearningHubValidationResult(vr);

            if (vr.IsValid)
            {
                retVal.CreatedId = await this.resourceVersionUserAcceptanceRepository.CreateAsync(currentUserId, rvua);
            }

            return retVal;
        }

        /// <summary>
        /// The get resource versions.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The ResourceVersionViewModel list"/>.</returns>
        public async Task<List<ResourceVersionViewModel>> GetResourceVersionsAsync(int resourceId)
        {
            var retVal = new List<ResourceVersionViewModel>();
            var rvs = await this.resourceVersionRepository.GetResourceVersionsAsync(resourceId);
            foreach (var rv in rvs)
            {
                var vm = new ResourceVersionViewModel();
                await this.GetResourceVersionViewModel(rv.Id, vm);
                retVal.Add(vm);
            }

            return retVal;
        }

        /// <summary>
        /// The get video details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VideoViewModel}"/>.</returns>
        public async Task<VideoViewModel> GetVideoDetailsByIdAsync(int resourceVersionId)
        {
            var videoDetails = await this.videoResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            return this.mapper.Map<VideoViewModel>(videoDetails);
        }

        /// <summary>
        /// The get video details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{AudioViewModel}"/>.</returns>
        public async Task<AudioViewModel> GetAudioDetailsByIdAsync(int resourceVersionId)
        {
            var audioDetails = await this.audioResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            return this.mapper.Map<AudioViewModel>(audioDetails);
        }

        /// <summary>
        /// The get article details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ArticleViewModel}"/>.</returns>
        public async Task<ArticleViewModel> GetArticleDetailsByIdAsync(int resourceVersionId)
        {
            var articleDetails = await this.articleResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            return this.mapper.Map<ArticleViewModel>(articleDetails);
        }

        /// <summary>
        /// The add resource version keyword async.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="ResourceKeywordViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> AddResourceVersionKeywordAsync(ResourceKeywordViewModel resourceKeywordViewModel, int userId)
        {
            var rvk = this.mapper.Map<ResourceVersionKeyword>(resourceKeywordViewModel);
            var validator = new ResourceVersionKeywordValidator();
            var vr = await validator.ValidateAsync(rvk);
            var retVal = new LearningHubValidationResult(vr);

            if (!vr.IsValid)
            {
                return retVal;
            }

            bool doesKeywordAlreadyExist = await this.resourceVersionKeywordRepository.DoesResourceVersionKeywordAlreadyExistAsync(rvk.ResourceVersionId, rvk.Keyword);
            if (doesKeywordAlreadyExist)
            {
                retVal.CreatedId = 0;
                return retVal;
            }

            retVal.CreatedId = await this.resourceVersionKeywordRepository.CreateAsync(userId, rvk);
            return retVal;
        }


        /// <summary>
        /// The get resource version events async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionEventViewModel}"/>.</returns>
        public async Task<ResourceVersionValidationResultViewModel> GetResourceVersionValidationResultAsync(int resourceVersionId)
        {
            var res = await this.resourceVersionValidationResultRepository.GetByResourceVersionIdAsync(resourceVersionId);

            var retVal = this.mapper.Map<ResourceVersionValidationResultViewModel>(res);
            return retVal;
        }

        /// <summary>
        /// The get resource version flags async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionFlagViewModel}"/>.</returns>
        public async Task<List<ResourceVersionFlagViewModel>> GetResourceVersionFlagsAsync(int resourceVersionId)
        {
            var rfs = this.resourceVersionFlagRepository.GetByResourceVersionIdAsync(resourceVersionId);
            var retVal = await this.mapper.ProjectTo<ResourceVersionFlagViewModel>(rfs).ToListAsync();
            return retVal;
        }

        /// <summary>
        /// Retrieves the entire assessment details given a resource version ID.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AssessmentViewModel> GetAssessmentDetailsByIdAsync(int resourceVersionId, int currentUserId)
        {
            var assessmentResourceVersion = await this.assessmentResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            AssessmentViewModel assessmentViewModel = await this.GetAssessmentViewModel(resourceVersionId);

            if (assessmentResourceVersion != null &&
                !(this.userService.IsAdminUser(currentUserId) ||
                 await this.UserCanEditResourceVersion(assessmentResourceVersion.CreateUserId, resourceVersionId, currentUserId)))
            {
                return await this.GetInitialAssessmentContent(resourceVersionId);
            }

            return assessmentViewModel;
        }

        /// <summary>
        /// Retrieves the assessment details up to the first question, leaving out the feedback and answer types nullified.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AssessmentViewModel> GetInitialAssessmentContent(int resourceVersionId)
        {
            var assessmentViewModel = await this.GetAssessmentViewModel(resourceVersionId);
            var assessmentContentViewModel = assessmentViewModel.AssessmentContent;

            assessmentViewModel.AssessmentContent =
                HideAssessmentDetailsHelper.NullifyItemsAfterQuestion(assessmentContentViewModel, 0, assessmentViewModel.AnswerInOrder);
            return assessmentViewModel;
        }

        /// <summary>
        /// Retrieves the assessment progress.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="numberOfAttempts">The number of attempts made.</param>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <param name="assessmentResourceActivitiesWithAnswers">The assessment resource activities.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AssessmentProgressViewModel> GetAssessmentProgress(
            int resourceVersionId,
            int numberOfAttempts,
            int assessmentResourceActivityId,
            Dictionary<int, Dictionary<int, IEnumerable<int>>> assessmentResourceActivitiesWithAnswers)
        {
            var assessmentViewModel = await this.GetAssessmentViewModel(resourceVersionId);
            var assessmentContentViewModel = assessmentViewModel.AssessmentContent;
            var questionBlocks = assessmentContentViewModel.Blocks.Where(b => b.BlockType == BlockType.Question);
            var lastQuestionNumber = questionBlocks.Count() - 1;

            var matchQuestions = this.assessmentResourceActivityMatchQuestionRepository
                .GetByAssessmentResourceActivityIdAsync(assessmentResourceActivityId).ToList();
            var assessmentProgress = this.GetAssessmentProgress(
                assessmentResourceActivityId,
                assessmentResourceActivitiesWithAnswers,
                assessmentContentViewModel,
                assessmentViewModel.PassMark,
                matchQuestions);
            var questionAnswersMapped = this.MapBlockOrderToQuestionNumber(assessmentContentViewModel, assessmentResourceActivitiesWithAnswers[assessmentResourceActivityId]);

            if (assessmentViewModel.AssessmentType == AssessmentTypeEnum.Formal)
            {
                assessmentContentViewModel = HideAssessmentDetailsHelper.NullifyItemsFromBlock(assessmentContentViewModel, assessmentContentViewModel.Blocks.Count, assessmentViewModel.AnswerInOrder);
            }

            assessmentViewModel.AssessmentContent = assessmentContentViewModel;
            return new AssessmentProgressViewModel
            {
                MaxScore = assessmentProgress.Score.MaxScore,
                UserScore = assessmentProgress.Score.UserScore,
                AssessmentViewModel = assessmentViewModel,
                NumberOfAttempts = numberOfAttempts,
                AssessmentResourceActivityId = assessmentResourceActivityId,
                Answers = this.GenerateQuestionAnswersList(questionAnswersMapped, lastQuestionNumber),
                Passed = assessmentProgress.Passed,
                MatchQuestions = this.mapper.Map<List<AssessmentResourceActivityMatchQuestionViewModel>>(matchQuestions),
            };
        }

        /// <summary>
        /// The get resource information view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceInformationViewModel}"/>.</returns>
        public async Task<ResourceInformationViewModel> GetResourceInformationViewModelAsync(int resourceReferenceId, int userId)
        {
            var retVal = new ResourceInformationViewModel();

            var rv = await this.resourceVersionRepository.GetCurrentForResourceReferenceIdAsync(resourceReferenceId);

            if ((rv.VersionStatusEnum == VersionStatusEnum.Draft || rv.VersionStatusEnum == VersionStatusEnum.Publishing) && !await this.UserCanEditResourceVersion(rv.CreateUserId, rv.Id, userId))
            {
                rv = await this.resourceVersionRepository.GetCurrentPublicationForResourceReferenceIdAsync(resourceReferenceId);
            }

            if (rv.VersionStatusEnum == VersionStatusEnum.Unpublished && !await this.UserCanEditResourceVersion(rv.CreateUserId, rv.Id, userId))
            {
                rv = null;
            }

            if (rv == null)
            {
                return retVal;
            }

            await this.GetResourceVersionViewModel(rv.Id, retVal);

            retVal.Id = resourceReferenceId;
            var bookmark = this.bookmarkRepository.GetAll().Where(b => b.ResourceReferenceId == resourceReferenceId && b.UserId == userId).FirstOrDefault();
            retVal.BookmarkId = bookmark?.Id;
            retVal.IsBookmarked = !bookmark?.Deleted ?? false;
            retVal.LicenseName = "CLA Licence Plus for NHS England";
            retVal.LicenseUrl = "#";

            if (rv.ResourceVersionFlag.Count > 0)
            {
                retVal.Flags = await this.GetResourceVersionFlagViewModels(rv.ResourceVersionFlag);
            }

            return retVal;
        }


        /// <summary>
        /// The get resource item view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="readOnly">The readOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{ResourceItemViewModel}"/>.</returns>
        public async Task<ResourceItemViewModel> GetResourceItemViewModelAsync(int resourceReferenceId, int userId, bool readOnly)
        {
            // Retrieve the latest ResourceVersion for the supplied reference.
            // If the latest has a 'Draft' or 'Publishing' status then retrieve the most recent version that has been Published
            // - note that this most recent Published version may currently have a status of 'Published' or 'Unpublished'
            // - set field ResourceVersionIdInEdit / ResourceVersionIdPublishing for this scenario, required for UI rendering
            var rv = await this.resourceVersionRepository.GetCurrentForResourceReferenceIdAsync(resourceReferenceId);

            if (rv != null)
            {
                int resourceVersionIdInEdit = 0;
                int resourceVersionIdPublishing = 0;

                if (rv.VersionStatusEnum == VersionStatusEnum.Draft || rv.VersionStatusEnum == VersionStatusEnum.Publishing)
                {
                    var rvp = await this.resourceVersionRepository.GetCurrentPublicationForResourceReferenceIdAsync(resourceReferenceId);

                    if (rvp != null && rv.VersionStatusEnum == VersionStatusEnum.Draft)
                    {
                        resourceVersionIdInEdit = rv.Id;
                    }

                    if (rvp != null && rv.VersionStatusEnum == VersionStatusEnum.Publishing)
                    {
                        resourceVersionIdPublishing = rv.Id;
                    }

                    rv = rvp;
                }

                var erv = await this.GetResourceVersionExtendedViewModelAsync(rv.Id, userId);

                if (erv == null)
                {
                    return null;
                }

                var retVal = new ResourceItemViewModel(erv);
                retVal.Id = resourceReferenceId;
                var bookmark = this.bookmarkRepository.GetAll().Where(b => b.ResourceReferenceId == resourceReferenceId && b.UserId == userId).FirstOrDefault();
                retVal.BookmarkId = bookmark?.Id;
                retVal.IsBookmarked = !bookmark?.Deleted ?? false;
                retVal.ReadOnly = readOnly;
                retVal.DisplayForContributor = await this.UserCanEditResourceVersion(rv.CreateUserId, rv.Id, userId);
                retVal.ResourceVersionIdInEdit = resourceVersionIdInEdit;
                retVal.ResourceVersionIdPublishing = resourceVersionIdPublishing;
                retVal.ResourceAccessibilityEnum = rv.ResourceAccessibilityEnum;

                // Obtain catalogue associated with the supplied Resource Reference.
                var rr = await this.resourceReferenceRepository.GetByOriginalResourceReferenceIdAsync(resourceReferenceId, true);
                var catalogueNodeVersion = this.catalogueNodeVersionRepository.GetBasicCatalogue(rr.NodePath.CatalogueNode.Id);
                retVal.Catalogue = this.mapper.Map<Nhs.Models.Catalogue.CatalogueViewModel>(catalogueNodeVersion);
                retVal.NodePathId = rr.NodePathId.Value;

                switch (rv.Resource.ResourceTypeEnum)
                {
                    case ResourceTypeEnum.Case:
                        retVal.CaseDetails = erv.CaseDetails;
                        break;

                    case ResourceTypeEnum.Assessment:
                        retVal.AssessmentDetails = erv.AssessmentDetails;
                        break;

                    case ResourceTypeEnum.Html:
                        retVal.HtmlDetails = erv.HtmlDetails;
                        break;

                    default:
                        break;
                }

                return retVal;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The get my contributions totals view model async.
        /// </summary>
        /// <param name="catalogueId">The catalogueId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="MyContributionsTotalsViewModel"/>.</returns>
        public MyContributionsTotalsViewModel GetMyContributionTotals(int catalogueId, int userId)
        {
            return this.resourceVersionRepository.GetMyContributionTotals(catalogueId, userId);
        }

        /// <summary>
        /// The has published resources method.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>If the user has published resources.</returns>
        public async Task<bool> HasPublishedResourcesAsync(int userId)
        {
            return await this.resourceRepository.UserHasPublishedResourcesAsync(userId);
        }

        /// <summary>
        /// The get resource version by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceDetailViewModel}"/>.</returns>
        public async Task<ResourceDetailViewModel> GetResourceVersionByIdAsync(int resourceVersionId)
        {
            var resourceVersion = await this.resourceVersionRepository.GetBasicByIdAsync(resourceVersionId);

            var vm = this.mapper.Map<ResourceDetailViewModel>(resourceVersion);

            var nodeResources = await this.nodeResourceRepository.GetByResourceIdAsync(vm.ResourceId);
            var draftNodeResources = nodeResources.Where(x => x.VersionStatusEnum == VersionStatusEnum.Draft).ToList();
            var publishedNodeResources = nodeResources.Where(x => x.VersionStatusEnum == VersionStatusEnum.Published || x.VersionStatusEnum == VersionStatusEnum.Unpublished).ToList();

            if (draftNodeResources.Count() == 0)
            {
                if (publishedNodeResources != null && publishedNodeResources.Count == 1)
                {
                    vm.NodeId = publishedNodeResources[0].NodeId;
                    vm.ResourceCatalogueId = await this.nodePathRepository.GetCatalogueRootNodeId(publishedNodeResources[0].NodeId);
                }
                else
                {
                    vm.NodeId = 0;
                    vm.ResourceCatalogueId = 0;
                }
            }
            else if (draftNodeResources.Count() > 1)
            {
                throw new Exception("Resource must belong to a single catalogue. ResourceVersionId: " + vm.ResourceVersionId.ToString() + ". ResourceId: " + vm.ResourceId.ToString() + ", VersionStatusEnum:" + vm.VersionStatusEnum.ToString());
            }
            else
            {
                vm.NodeId = draftNodeResources[0].NodeId;
                vm.ResourceCatalogueId = await this.nodePathRepository.GetCatalogueRootNodeId(draftNodeResources[0].NodeId);
            }

            if (publishedNodeResources.Count == 1)
            {
                vm.PublishedResourceCatalogueId = await this.nodePathRepository.GetCatalogueRootNodeId(publishedNodeResources[0].NodeId);
            }

            vm.Flags = await this.GetResourceVersionFlagViewModels(resourceVersion.ResourceVersionFlag);

            return vm;
        }

        /// <summary>
        /// The save resource version flag async.
        /// </summary>
        /// <param name="resourceVersionFlagViewModel">The resourceVersionFlagViewModel<see cref="ResourceVersionFlagViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> SaveResourceVersionFlagAsync(ResourceVersionFlagViewModel resourceVersionFlagViewModel, int userId)
        {
            var rvf = this.mapper.Map<ResourceVersionFlag>(resourceVersionFlagViewModel);
            var validator = new ResourceVersionFlagValidator();
            var vr = await validator.ValidateAsync(rvf);
            var retVal = new LearningHubValidationResult(vr);

            if (vr.IsValid)
            {
                if (rvf.IsNew())
                {
                    retVal.CreatedId = await this.resourceVersionFlagRepository.CreateAsync(userId, rvf);
                }
                else
                {
                    retVal.Add(await this.resourceVersionFlagRepository.UpdateResourceVersionFlagAsync(userId, rvf));
                }
            }

            return retVal;
        }

        /// <summary>
        /// The get extended resource version view model async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionExtendedViewModel}"/>.</returns>
        public async Task<ResourceVersionExtendedViewModel> GetResourceVersionExtendedViewModelAsync(int resourceVersionId, int userId = -1)
        {
            // Attempt to retrieve from cache.
            string cacheKey = $"{CacheKeys.ResourceVersionExtended}:{resourceVersionId}";
            var retVal = await this.cachingService.GetAsync<ResourceVersionExtendedViewModel>(cacheKey);
            if (retVal.ResponseEnum == CacheReadResponseEnum.Found)
            {
                return retVal.Item;
            }

            // Not acquired from cache - load from db.
            var rvvm = new ResourceVersionExtendedViewModel();

            await this.GetResourceVersionViewModel(resourceVersionId, rvvm);

            if (rvvm.ResourceVersionId > 0)
            {
                switch (rvvm.ResourceTypeEnum)
                {
                    case ResourceTypeEnum.Article:

                        var articleDetails = await this.GetArticleDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.ArticleDetails = articleDetails;
                        break;

                    case ResourceTypeEnum.Equipment:

                        var equipmentDetails = await this.GetEquipmentDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.EquipmentDetails = equipmentDetails;
                        break;

                    case ResourceTypeEnum.WebLink:

                        var weblinkDetails = await this.GetWebLinkDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.WebLinkDetails = weblinkDetails;
                        break;

                    case ResourceTypeEnum.GenericFile:

                        var genericFileDetails = await this.GetGenericFileDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.GenericFileDetails = genericFileDetails;
                        break;

                    case ResourceTypeEnum.Html:

                        var htmlDetails = await this.GetHtmlDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.HtmlDetails = htmlDetails;
                        break;

                    case ResourceTypeEnum.Image:

                        var imageDetails = await this.GetImageDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.ImageDetails = imageDetails;
                        break;

                    case ResourceTypeEnum.Embedded:

                        var embedDetails = await this.GetEmbeddedResourceVersionByIdAsync(rvvm.ResourceVersionId);
                        rvvm.EmbedCodeDetails = embedDetails;
                        break;

                    case ResourceTypeEnum.Video:

                        var videoDetails = await this.GetVideoDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.VideoDetails = videoDetails;
                        break;

                    case ResourceTypeEnum.Audio:

                        var audioDetails = await this.GetAudioDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.AudioDetails = audioDetails;
                        break;

                    case ResourceTypeEnum.Scorm:

                        var scormDetails = await this.GetScormDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.ScormDetails = scormDetails;
                        break;

                    case ResourceTypeEnum.Case:

                        var caseDetails = await this.GetCaseDetailsByIdAsync(rvvm.ResourceVersionId);
                        rvvm.CaseDetails = caseDetails;
                        break;
                    case ResourceTypeEnum.Assessment:
                        var assessmentDetails = await this.GetAssessmentDetailsByIdAsync(rvvm.ResourceVersionId, userId);
                        rvvm.AssessmentDetails = assessmentDetails;
                        break;

                    default:
                        break;
                }
            }

            // Published resource versions served from cache (non-expiring).
            if (rvvm.VersionStatusEnum == VersionStatusEnum.Published)
            {
                await this.cachingService.SetAsync($"ResourceVersionExtended:{resourceVersionId}", rvvm);
            }

            // IT1 - always retrieve resource reference from the database to avoid complex cache refresh
            // scenarios that may arise from content structure changes / node resource publishing.
            // NodeResourceLookup, NodePath related updates to be cached separately in future iterations.
            var rr = await this.resourceReferenceRepository.GetDefaultByResourceIdAsync(rvvm.ResourceId);
            if (rr != null)
            {
                rvvm.DefaultResourceReferenceId = rr.OriginalResourceReferenceId;
            }

            return rvvm;
        }

        /// <summary>
        /// The get image details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ImageViewModel}"/>.</returns>
        public async Task<ImageViewModel> GetImageDetailsByIdAsync(int resourceVersionId)
        {
            var imageDetails = await this.imageResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            return this.mapper.Map<ImageViewModel>(imageDetails);
        }

        /// <summary>
        /// The get embedded resource version by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{EmbedCodeViewModel}"/>.</returns>
        public async Task<EmbedCodeViewModel> GetEmbeddedResourceVersionByIdAsync(int resourceVersionId)
        {
            var e = await this.embeddedResourceVersionRepository.GetByIdAsync(resourceVersionId);

            var vm = this.mapper.Map<EmbedCodeViewModel>(e);

            return vm;
        }

        /// <summary>
        /// The get equipment resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{EquipmentViewModel}"/>.</returns>
        public async Task<EquipmentViewModel> GetEquipmentDetailsByIdAsync(int resourceVersionId)
        {
            var e = await this.equipmentResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);

            var vm = this.mapper.Map<EquipmentViewModel>(e);

            return vm;
        }


        private Dictionary<int, IEnumerable<int>> MapBlockOrderToQuestionNumber(BlockCollectionViewModel blockCollection, Dictionary<int, IEnumerable<int>> questionAnswers)
        {
            var result = new Dictionary<int, IEnumerable<int>>();
            var blocks = blockCollection.Blocks.OrderBy(b => b.Order).ToList();
            var questions = blocks.Where(b => b.BlockType == BlockType.Question).ToList();

            foreach (var (blockOrder, answers) in questionAnswers)
            {
                var blockViewModel = blocks[blockOrder];
                var questionNumber = questions.IndexOf(blockViewModel);
                result.Add(questionNumber, answers);
            }

            return result;
        }

        private List<List<int>> GenerateQuestionAnswersList(Dictionary<int, IEnumerable<int>> questionAnswers, int lastQuestionNumber)
        {
            var output = new List<List<int>>();
            for (int i = 0; i <= lastQuestionNumber; i++)
            {
                var value = questionAnswers.ContainsKey(i) ? questionAnswers[i] : new List<int>() { -1 };
                output.Add(value.ToList());
            }

            return output;
        }

        private async Task<AssessmentViewModel> GetAssessmentViewModel(int resourceVersionId)
        {
            var assessmentResourceVersion = await this.assessmentResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);

            var assessmentContent = await this.blockCollectionRepository.GetBlockCollection(assessmentResourceVersion?.AssessmentContentId);
            var endGuidance = await this.blockCollectionRepository.GetBlockCollection(assessmentResourceVersion?.EndGuidanceId);

            var assessmentContentViewModel = assessmentContent == null ? null : this.mapper.Map<BlockCollectionViewModel>(assessmentContent);
            var endGuidanceViewModel = endGuidance == null ? null : this.mapper.Map<BlockCollectionViewModel>(endGuidance);

            await this.AddPartialFileDetails(assessmentContentViewModel);
            await this.AddPartialFileDetails(endGuidanceViewModel);

            AssessmentViewModel assessmentViewModel = new AssessmentViewModel
            {
                ResourceVersionId = resourceVersionId,
                AssessmentContent = assessmentContentViewModel,
                EndGuidance = endGuidanceViewModel,
                PassMark = assessmentResourceVersion?.PassMark,
                AnswerInOrder = assessmentResourceVersion?.AnswerInOrder ?? default(bool),
                MaximumAttempts = assessmentResourceVersion?.MaximumAttempts,
                AssessmentType = assessmentResourceVersion?.AssessmentType ?? 0,
            };

            return assessmentViewModel;
        }


        /// <summary>
        /// The GetResourceVersionFlagViewModels.
        /// </summary>
        /// <param name="resourceVersionFlags">The resourceVersionFlags<see cref="ICollection{ResourceVersionFlag}"/>.</param>
        /// <returns>The <see cref="ICollection{ResourceVersionFlagViewModel}"/>.</returns>
        private async Task<ICollection<ResourceVersionFlagViewModel>> GetResourceVersionFlagViewModels(
            ICollection<ResourceVersionFlag> resourceVersionFlags)
        {
            var flags = this.mapper
                .ProjectTo<ResourceVersionFlagViewModel>(resourceVersionFlags.AsQueryable()).ToList();

            foreach (var f in flags)
            {
                var u = await this.userProfileService.GetByIdAsync(f.FlaggedByUserId);
                f.FlaggedByName = this.userService.IsAdminUser(u.Id) ? "Administrator" : u.UserName;
            }

            return flags;
        }

        /// <summary>
        /// The populate resource version view model.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="resourceVersionViewModel">The resource version view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task GetResourceVersionViewModel(int resourceVersionId, ResourceVersionViewModel resourceVersionViewModel)
        {
            var resourceVersion = await this.resourceVersionRepository.GetByIdAsync(resourceVersionId, true);

            if (resourceVersion != null)
            {
                resourceVersionViewModel.ResourceId = resourceVersion.ResourceId;
                resourceVersionViewModel.ResourceVersionId = resourceVersion.Id;
                resourceVersionViewModel.VersionStatusEnum = resourceVersion.VersionStatusEnum;
                resourceVersionViewModel.Title = resourceVersion.Title;
                resourceVersionViewModel.Description = resourceVersion.Description;
                resourceVersionViewModel.SensitiveContent = resourceVersion.SensitiveContent;
                resourceVersionViewModel.CertificateEnabled = resourceVersion.CertificateEnabled;
                resourceVersionViewModel.ResourceTypeEnum = resourceVersion.Resource.ResourceTypeEnum;
                resourceVersionViewModel.MajorVersion = resourceVersion.MajorVersion;
                resourceVersionViewModel.MinorVersion = resourceVersion.MinorVersion;

                if (resourceVersion.ReviewDate.HasValue)
                {
                    resourceVersionViewModel.NextReviewDate = resourceVersion.ReviewDate.Value.DateTime;
                }

                resourceVersionViewModel.AdditionalInformation = resourceVersion.AdditionalInformation;
                resourceVersionViewModel.ResourceFree = !resourceVersion.HasCost;
                resourceVersionViewModel.Cost = resourceVersion.HasCost ? (double)resourceVersion.Cost : 0.00d;

                resourceVersionViewModel.Keywords = new List<string>();
                foreach (var k in resourceVersion.ResourceVersionKeyword)
                {
                    resourceVersionViewModel.Keywords.Add(k.Keyword);
                }

                resourceVersionViewModel.Authors = new List<string>();
                foreach (var a in resourceVersion.ResourceVersionAuthor)
                {
                    string displayText = string.Empty;

                    if (!string.IsNullOrEmpty(a.AuthorName) && !string.IsNullOrEmpty(a.Organisation))
                    {
                        displayText = string.Join(", ", a.AuthorName, a.Organisation);
                    }

                    if (!string.IsNullOrEmpty(a.AuthorName) && string.IsNullOrEmpty(a.Organisation))
                    {
                        displayText = a.AuthorName;
                    }

                    if (string.IsNullOrEmpty(a.AuthorName) && !string.IsNullOrEmpty(a.Organisation))
                    {
                        displayText = a.Organisation;
                    }

                    if (!string.IsNullOrEmpty(a.Role))
                    {
                        displayText += ", " + a.Role;
                    }

                    resourceVersionViewModel.Authors.Add(displayText);
                }

                resourceVersionViewModel.AuthoredDate = resourceVersion.CreateDate.DateTime;

                if (resourceVersion.VersionStatusEnum == VersionStatusEnum.Published
                    || resourceVersion.VersionStatusEnum == VersionStatusEnum.Unpublished)
                {
                    if (resourceVersion.Publication != null)
                    {
                        var u = await this.userProfileService.GetByIdAsync(resourceVersion.Publication.CreateUserId);
                        resourceVersionViewModel.PublishedBy = $"{u?.FirstName} {u?.LastName}";
                        resourceVersionViewModel.PublishedDate = resourceVersion.Publication.CreateDate.DateTime;
                        resourceVersionViewModel.PublishedNotes = resourceVersion.Publication.Notes;
                    }
                }

                if (resourceVersion.VersionStatusEnum == VersionStatusEnum.Unpublished)
                {
                    var unpublishEvent = resourceVersion.ResourceVersionEvent.Where(e => e.ResourceVersionEventType == ResourceVersionEventTypeEnum.UnpublishedByAdmin)
                                                                 .OrderByDescending(e => e.Id)
                                                                 .FirstOrDefault();
                    resourceVersionViewModel.UnpublishedByAdmin = unpublishEvent != null;
                }

                if (resourceVersion.ResourceLicence != null)
                {
                    resourceVersionViewModel.LicenseName = resourceVersion.ResourceLicence.Title;
                }

                if (resourceVersion.ResourceVersionFlag.Count > 0)
                {
                    resourceVersionViewModel.Flags = await this.GetResourceVersionFlagViewModels(resourceVersion.ResourceVersionFlag);
                }

                var rr = await this.resourceReferenceRepository.GetDefaultByResourceIdAsync(resourceVersionViewModel.ResourceId);
                if (rr != null)
                {
                    resourceVersionViewModel.DefaultResourceReferenceId = rr.OriginalResourceReferenceId;
                }

                var firstPublishedEvent = await this.publicationRepository.GetResourceFirstPublication(resourceVersionViewModel.ResourceId);

                if (firstPublishedEvent != null)
                {
                    resourceVersionViewModel.FirstPublishedDate = firstPublishedEvent.CreateDate.DateTime;
                }

                resourceVersionViewModel.CreateUserId = resourceVersion.CreateUserId;
                resourceVersionViewModel.CreateUser = resourceVersion.CreateUser.UserName;
                resourceVersionViewModel.CreateDate = resourceVersion.CreateDate;
                resourceVersionViewModel.HasValidationErrors = resourceVersion.ResourceVersionValidationResult != null && resourceVersion.VersionStatusEnum == VersionStatusEnum.FailedToPublish;
                resourceVersionViewModel.ResourceAccessibilityEnum = resourceVersion.ResourceAccessibilityEnum;

                if (resourceVersion.ResourceVersionProvider.Count > 0)
                {
                    resourceVersionViewModel.Providers = this.mapper.Map<List<ProviderViewModel>>(resourceVersion.ResourceVersionProvider.Select(n => n.Provider).ToList());
                }
            }
        }

        private (bool Passed, (int? UserScore, int MaxScore) Score) GetAssessmentProgress(
     int assessmentResourceActivityId,
     Dictionary<int, Dictionary<int, IEnumerable<int>>> assessmentResourceActivitiesWithAnswers,
     BlockCollectionViewModel assessmentContent,
     int? passMark,
     IList<AssessmentResourceActivityMatchQuestion> matchQuestions)
        {
            var passed = false;
            (int? UserScore, int MaxScore) currentAssessmentScore = (null, 0);
            var currentScoreCalculated = false;
            foreach (var item in assessmentResourceActivitiesWithAnswers)
            {
                if (passed && currentScoreCalculated)
                {
                    break;
                }

                if (passed && item.Key != assessmentResourceActivityId)
                {
                    continue;
                }

                var answers = item.Value;

                var score = ScoreCalculationHelper.CalculateScore(
                    assessmentContent,
                    answers);
                if (item.Key == assessmentResourceActivityId)
                {
                    currentAssessmentScore = score;
                    currentScoreCalculated = true;
                }

                var percentageScore = (decimal?)score.UserScore / score.MaxScore * 100;

                if (passMark == null || (percentageScore >= passMark || score.UserScore >= score.MaxScore))
                {
                    passed = true;
                }
            }

            return (
                Passed: passed,
                Score: currentAssessmentScore);
        }


        /// <summary>
        /// Gets the ResourceVersionId from a fileId related to a video.
        /// </summary>
        /// <param name="fileId">The file Id of the video.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<int> GetResourceVersionIdForVideo(int fileId)
        {
            int resourceId = -1;
            Video video = await this.videoRepository.GetByFileIdAsync(fileId);
            MediaBlock mediaBlock = video.MediaBlocks?.FirstOrDefault();

            if (mediaBlock != null)
            {
                BlockCollection blockCollection = mediaBlock.Block.BlockCollection;
                resourceId = await this.GetResourceVersionIdForBlockCollection(blockCollection);
            }

            return resourceId;
        }

        /// <summary>
        /// Gets the ResourceVersionId from a blockCollection.
        /// </summary>
        /// <param name="blockCollection">The block collection.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<int> GetResourceVersionIdForBlockCollection(BlockCollection blockCollection)
        {
            int resourceId = -1;
            CaseResourceVersion caseResourceVersion = blockCollection.CaseResourceVersion;

            if (caseResourceVersion != null)
            {
                resourceId = caseResourceVersion.ResourceVersionId;
            }
            else
            {
                resourceId = await this.GetAssessmentVersionIdForBlockCollection(blockCollection);
            }

            if (resourceId == -1)
            {
                resourceId = await this.GetCaseOrAssessmentVersionIdForQuestionBlock(blockCollection.Id);
            }

            return resourceId;
        }

        /// <summary>
        /// Gets the Case or Assessment version Id from the blockCollectionId of a question block.
        /// </summary>
        /// <param name="blockCollectionId">The block collection Id of the question block.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<int> GetCaseOrAssessmentVersionIdForQuestionBlock(int blockCollectionId)
        {
            int resourceId = -1;
            QuestionBlock qb =
                await this.questionBlockRepository.GetByQuestionBlockCollectionIdAsync(blockCollectionId);
            if (qb?.Block != null)
            {
                resourceId = await this.GetResourceVersionIdForBlockCollection(qb.Block.BlockCollection);
            }

            return resourceId;
        }

        /// <summary>
        /// Gets the Assessment version Id from a blockCollection.
        /// </summary>
        /// <param name="blockCollection">The block collection.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<int> GetAssessmentVersionIdForBlockCollection(BlockCollection blockCollection)
        {
            int resourceId = -1;
            AssessmentResourceVersion assessmentResourceVersion =
                await this.assessmentResourceVersionRepository.GetByAssessmentContentBlockCollectionIdAsync(
                    blockCollection.Id);
            if (assessmentResourceVersion != null)
            {
                resourceId = assessmentResourceVersion.ResourceVersionId;
            }

            return resourceId;
        }

        /// <summary>
        /// Gets the ResourceVersionId from a fileId related to a whole slide image.
        /// </summary>
        /// <param name="fileId">The file Id of the whole slide image.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<int> GetResourceVersionIdForWholeSlideImage(int fileId)
        {
            int resourceId = -1;
            WholeSlideImage wholeSlideImage = await this.wholeSlideImageRepository.GetByFileIdAsync(fileId);
            WholeSlideImageBlockItem wholeSlideImageBlockItem =
                wholeSlideImage?.WholeSlideImageBlockItems?.FirstOrDefault();

            if (wholeSlideImageBlockItem != null)
            {
                BlockCollection blockCollection = wholeSlideImageBlockItem.WholeSlideImageBlock.Block
                    .BlockCollection;
                resourceId = await this.GetResourceVersionIdForBlockCollection(blockCollection);
            }

            return resourceId;
        }

        private async Task RemoveExternalReferenceFromCache(Resource resource)
        {
            var extReferences = await this.scormResourceVersionRepository.GetExternalReferenceByResourceId(resource.Id);

            var migrationSourcesList = await this.migrationSourceRepository.GetAll().ToListAsync();

            var migrationSources = migrationSourcesList.Select(m => m.Description).ToArray().Concat(new[] { "LearningHub" });

            using var client = new HttpClient();

            var contentServerUrl = new Uri(this.learningHubConfig.ContentServerUrl).GetLeftPart(UriPartial.Authority);

            foreach (var reference in extReferences)
            {
                foreach (var migrationSource in migrationSources)
                {
                    var url = $"{contentServerUrl}/remove-cache/{migrationSource}_{reference.ToLower()}";
                    await client.PostAsync(url, null);
                }
            }
        }

        /// <summary>
        /// The UserCanEditResourceVersion.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private async Task<bool> UserCanEditResourceVersion(int userId, int resourceVersionId, int currentUserId)
        {
            if (userId == currentUserId)
            {
                return true;
            }

            var resourceVersion = await this.GetResourceVersionByIdAsync(resourceVersionId);
            if (resourceVersion == null)
            {
                return false;
            }

            return await this.catalogueService.CanUserEditCatalogueAsync(currentUserId, (int)resourceVersion.ResourceCatalogueId);
        }

        /// <summary>
        /// The create file details for article attachment async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateArticleAttachedFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId)
        {
            var articleDetail = await this.articleResourceVersionRepository.GetByResourceVersionIdAsync(fileCreateRequestViewModel.ResourceVersionId);

            if (!await this.UserCanEditResourceVersion(articleDetail.CreateUserId, articleDetail.ResourceVersionId, userId))
            {
                return new LearningHubValidationResult(false);
            }

            int fileId = await this.fileRepository.CreateAsync(
                userId,
                new File()
                {
                    FileTypeId = fileCreateRequestViewModel.FileTypeId,
                    FileName = fileCreateRequestViewModel.FileName,
                    FilePath = fileCreateRequestViewModel.FilePath,
                    FileChunkDetailId = fileCreateRequestViewModel.FileChunkDetailId,
                    FileSizeKb = fileCreateRequestViewModel.FileSize,
                });

            ArticleResourceVersionFile articleResourceVersionFile = new ArticleResourceVersionFile()
            {
                ArticleResourceVersionId = articleDetail.Id,
                FileId = fileId,
            };
            int articleResourceVersionFileId = await this.articleResourceVersionFileRepository.CreateAsync(userId, articleResourceVersionFile);

            if (fileCreateRequestViewModel.ReplacedFileId != 0)
            {
                var delRetVal = await this.DeleteArticleFileAsync(new FileDeleteRequestModel() { ResourceVersionId = fileCreateRequestViewModel.ResourceVersionId, FileId = fileCreateRequestViewModel.ReplacedFileId }, userId);
                if (!delRetVal.IsValid)
                {
                    return delRetVal;
                }
            }

            var retVal = new LearningHubValidationResult(true);
            retVal.CreatedId = fileId;
            return retVal;
        }

        /// <summary>
        /// The create file details for audio attribute file async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAudioAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId)
        {
            var audioDetail = await this.audioResourceVersionRepository.GetByResourceVersionIdAsync(fileCreateRequestViewModel.ResourceVersionId);

            if (!await this.UserCanEditResourceVersion(audioDetail.CreateUserId, audioDetail.ResourceVersionId, userId))
            {
                return new LearningHubValidationResult(false);
            }

            int fileId = await this.fileRepository.CreateAsync(
                userId,
                new File()
                {
                    FileTypeId = fileCreateRequestViewModel.FileTypeId,
                    FileName = fileCreateRequestViewModel.FileName,
                    FilePath = fileCreateRequestViewModel.FilePath,
                    FileChunkDetailId = fileCreateRequestViewModel.FileChunkDetailId,
                    FileSizeKb = fileCreateRequestViewModel.FileSize,
                });

            switch (fileCreateRequestViewModel.AttachedFileType)
            {
                case AttachedFileTypeEnum.Transcript:
                    audioDetail.TranscriptFile = null;
                    audioDetail.TranscriptFileId = fileId;
                    break;
            }

            await this.audioResourceVersionRepository.UpdateAsync(userId, audioDetail);

            var retVal = new LearningHubValidationResult(true);
            retVal.CreatedId = fileId;
            return retVal;
        }
        /// <summary>
        /// The create file details for video attribute file async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateVideoAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId)
        {
            var videoDetail = await this.videoResourceVersionRepository.GetByResourceVersionIdAsync(fileCreateRequestViewModel.ResourceVersionId);

            if (!await this.UserCanEditResourceVersion(videoDetail.CreateUserId, videoDetail.ResourceVersionId, userId))
            {
                return new LearningHubValidationResult(false);
            }

            int fileId = await this.fileRepository.CreateAsync(
                userId,
                new File()
                {
                    FileTypeId = fileCreateRequestViewModel.FileTypeId,
                    FileName = fileCreateRequestViewModel.FileName,
                    FilePath = fileCreateRequestViewModel.FilePath,
                    FileChunkDetailId = fileCreateRequestViewModel.FileChunkDetailId,
                    FileSizeKb = fileCreateRequestViewModel.FileSize,
                });

            switch (fileCreateRequestViewModel.AttachedFileType)
            {
                case AttachedFileTypeEnum.Transcript:
                    videoDetail.TranscriptFile = null;
                    videoDetail.TranscriptFileId = fileId;
                    break;
                case AttachedFileTypeEnum.ClosedCaptions:
                    videoDetail.ClosedCaptionsFile = null;
                    videoDetail.ClosedCaptionsFileId = fileId;
                    break;
            }

            await this.videoResourceVersionRepository.UpdateAsync(userId, videoDetail);

            var retVal = new LearningHubValidationResult(true);
            retVal.CreatedId = fileId;
            return retVal;
        }

        /// <summary>
        /// Duplicate blocks.
        /// </summary>
        /// <param name="requestModel">The DuplicateBlocksRequestModel.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DuplicateBlocks(DuplicateBlocksRequestModel requestModel, int userId)
        {
            var sourceResourceVersion = await this.resourceVersionRepository.GetCurrentForResourceAsync(requestModel.SourceResourceId);
            int? sourceBlockCollectionId;
            int destResourceVersionId;
            var result = this.ValidateRequestWithSourceResource(sourceResourceVersion, requestModel);
            if (!result.IsValid)
            {
                return result;
            }

            var sourceResource = sourceResourceVersion.Resource;
            if (sourceResource.ResourceTypeEnum == ResourceTypeEnum.Case)
            {
                var caseResourceVersion = await this.caseResourceVersionRepository
                    .GetByResourceVersionIdAsync(sourceResourceVersion.Id);
                sourceBlockCollectionId = caseResourceVersion.BlockCollectionId;
            }
            else
            {
                var assessmentResourceVersion = await this.assessmentResourceVersionRepository
                    .GetByResourceVersionIdAsync(sourceResourceVersion.Id);
                sourceBlockCollectionId = assessmentResourceVersion.AssessmentContentId;
            }

            var sourceBlockCollection =
                    await this.blockCollectionRepository.GetBlockCollection(sourceBlockCollectionId);
            result = this.ValidateRequestWithSourceBlockCollection(sourceBlockCollection, requestModel);
            if (!result.IsValid)
            {
                return result;
            }

            var strategy = this.dbContext.Database.CreateExecutionStrategy();

            await strategy.Execute(
              async () =>
              {
                  using (var transaction = this.dbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          int destinationBlockCollectionId;

                          if (requestModel.DestinationResourceId.HasValue)
                          {
                              result = await this.ValidateDestinationAndGetResourceVersionId(requestModel.DestinationResourceId.Value, userId, sourceResource.ResourceTypeEnum);
                              if (!result.IsValid)
                              {
                                  transaction.Rollback();
                                  return result;
                              }

                              destResourceVersionId = result.CreatedId.Value;
                          }
                          else
                          {
                              destResourceVersionId = await this.CreateNewResource(sourceResource.ResourceTypeEnum, "Copy of " + sourceResourceVersion.Title, userId, requestModel.AssessmentType);
                          }

                          if (sourceResourceVersion.Resource.ResourceTypeEnum == ResourceTypeEnum.Case)
                          {
                              var destResourceVersion =
                                  await this.caseResourceVersionRepository.GetByResourceVersionIdAsync(destResourceVersionId);
                              destinationBlockCollectionId = destResourceVersion.BlockCollectionId.Value;
                          }
                          else
                          {
                              var destResourceVersion = await this.assessmentResourceVersionRepository.GetByResourceVersionIdAsync(destResourceVersionId);
                              destinationBlockCollectionId = destResourceVersion.AssessmentContentId.Value;
                          }

                          await this.resourceVersionRepository.FractionalDuplication(userId, sourceBlockCollection.Id, destinationBlockCollectionId, requestModel.BlockIds.ToList());
                          transaction.Commit();
                      }
                      catch (Exception e)
                      {
                          transaction.Rollback();
                          result.Details.Add("Failed to duplicate the blocks. Changes have not been made to the database. Please try again.");
                          result.Details.Add(e.Message);
                          result.Details.Add(e.StackTrace);
                          return result;
                      }
                  }

                  result.IsValid = true;
                  result.CreatedId = destResourceVersionId;
                  return result;
              });

            return result;
        }

        /// <summary>
        /// The create resource async.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateResourceAsync(ResourceDetailViewModel resourceDetailViewModel, int userId)
        {
            var retVal = new LearningHubValidationResult(true);

            retVal.CreatedId = await this.resourceRepository.CreateResourceAsync(resourceDetailViewModel.ResourceType, resourceDetailViewModel.Title, resourceDetailViewModel.Description, userId);

            if (resourceDetailViewModel.ResourceLicenceId != 0
                || (int)resourceDetailViewModel.ResourceCatalogueId != 0
                || resourceDetailViewModel.AdditionalInformation != string.Empty
                || resourceDetailViewModel.SensitiveContent)
            {
                resourceDetailViewModel.ResourceVersionId = (int)retVal.CreatedId;
                await this.UpdateResourceVersionAsync(resourceDetailViewModel, userId);
            }

            if (resourceDetailViewModel.ResourceType == ResourceTypeEnum.WebLink
                || resourceDetailViewModel.ResourceType == ResourceTypeEnum.Article
                || resourceDetailViewModel.ResourceType == ResourceTypeEnum.Scorm)
            {
                resourceDetailViewModel.ResourceVersionId = retVal.CreatedId.Value;
                this.resourceVersionRepository.SetResourceType(resourceDetailViewModel.ResourceVersionId, resourceDetailViewModel.ResourceType, userId);
            }
            else if (resourceDetailViewModel.ResourceType == ResourceTypeEnum.Case)
            {
                var caseResourceVersion = new CaseResourceVersion
                {
                    ResourceVersionId = retVal.CreatedId.Value,
                };
                await this.caseResourceVersionRepository.CreateAsync(userId, caseResourceVersion);
            }

            // If the User has only "Editor" rights to one Catalogue then create a draft NodeResource record.
            var userCatalogues = this.catalogueService.GetCataloguesForUser(userId);
            if (userCatalogues.Count == 1)
            {
                var resourceVersion = await this.GetResourceVersionByIdAsync(retVal.CreatedId.Value);
                await this.nodeResourceRepository.CreateOrUpdateAsync(userCatalogues.First().NodeId, resourceVersion.ResourceId, userId);
            }

            return retVal;
        }

        /// <summary>
        /// The duplicate resource async.
        /// </summary>
        /// <param name="duplicateResourceRequestModel">The duplicateResourceRequestModel<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DuplicateResourceAsync(DuplicateResourceRequestModel duplicateResourceRequestModel, int userId)
        {
            try
            {
                var resourceVersionId = duplicateResourceRequestModel.ResourceVersionId;

                // The resourceCatalogueId should be the `nodeId` value from the front-end's `selectedCatalogue` object
                var resourceCatalogueId = duplicateResourceRequestModel.ResourceCatalogueId;

                var result = new LearningHubValidationResult(false, "Duplicate Resource Async: ");

                var originalResource = await this.resourceRepository.GetByResourceVersionIdAsync(resourceVersionId);
                var originalResourceVersion = await this.resourceVersionRepository.GetByIdAsync(resourceVersionId, true);

                // For now we are only allowing CASES and ASSESSMENTS to be duplicated, but in future this will be extended to other resource types
                if (originalResource.ResourceTypeEnum != ResourceTypeEnum.Case && originalResource.ResourceTypeEnum != ResourceTypeEnum.Assessment)
                {
                    result.IsValid = false;
                    result.Details.Add("Cannot duplicate a resource of type " + originalResource.ResourceTypeEnum);

                    return result;
                }

                if (originalResourceVersion.VersionStatusEnum != VersionStatusEnum.Draft
                    && originalResourceVersion.VersionStatusEnum != VersionStatusEnum.Published
                    && originalResourceVersion.VersionStatusEnum != VersionStatusEnum.Unpublished)
                {
                    result.IsValid = false;
                    result.Details.Add("Only Draft, Published or Unpublished resources can be duplicated. This resource is: " + originalResourceVersion.VersionStatusEnum);

                    return result;
                }

                var duplicateResourceVersionId = await this.resourceVersionRepository.CreateDuplicateVersionAsync(originalResource.Id, userId);

                if (resourceCatalogueId != 0)
                {
                    var duplicateResourceVersion = await this.resourceVersionRepository.GetByIdAsync(duplicateResourceVersionId, true);
                    await this.SetNodeResource(resourceCatalogueId, userId, duplicateResourceVersion);
                }

                if (duplicateResourceVersionId == 0)
                {
                    result.IsValid = false;
                    result.Details.Add("Failed to duplicate the resource with ResourceId " + originalResource.Id + " and CurrentResourceVersionId " + originalResource.CurrentResourceVersionId);

                    return result;
                }

                result.IsValid = true;
                result.CreatedId = duplicateResourceVersionId;

                return result;
            }
            catch (Exception err)
            {
                var result = new LearningHubValidationResult(false, "Duplicate Resource Async: ");
                result.Details.Add("Failed to duplicate the resource. Changes have not been made to the database. Please try again.");
                result.Details.Add(err.Message);
                result.Details.Add(err.StackTrace);
                return result;
            }
        }

        /// <summary>
        /// Creation of a new resource version.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateNewResourceVersionAsync(int resourceId, int currentUserId)
        {
            var resourceVersion = await this.resourceVersionRepository.GetCurrentForResourceAsync(resourceId);

            var response = new LearningHubValidationResult(true);

            if (resourceVersion == null)
            {
                response.IsValid = false;
                response.Details.Add("Attempted of resource version creation failed. No current version. resourceId: " + resourceId.ToString() + ", currentUserId: " + currentUserId.ToString());
            }
            else if (!await this.UserCanEditResourceVersion(resourceVersion.CreateUserId, resourceVersion.Id, currentUserId))
            {
                response.IsValid = false;
                response.Details.Add("Attempted of resource version creation by invalid user. resourceId: " + resourceId.ToString() + ", currentUserId: " + currentUserId.ToString());
            }
            else if (await this.resourceVersionRepository.DoesVersionExist(resourceId, VersionStatusEnum.Draft))
            {
                response.IsValid = false;
                response.Details.Add("Attempted of resource version creation when draft version already exists. resourceId: " + resourceId.ToString() + ", currentUserId: " + currentUserId.ToString());
            }

            if (response.IsValid)
            {
                response.IsValid = true;
                response.CreatedId = await this.resourceVersionRepository.CreateNextVersionAsync(resourceId, currentUserId);

                // This code (below) would normally live inside the ResourceVersionCreateNext stored procedure
                // But Cases use a BlockCollection structure, which has lots of sub-tables, so this code would get really messy
                // Since Entity Framework deals with this so well, it's simpler to have this logic here
                if (resourceVersion.Resource.ResourceTypeEnum == ResourceTypeEnum.Case)
                {
                    var caseViewModel = await this.GetCaseDetailsByIdAsync(resourceVersion.Id);
                    caseViewModel.ResourceVersionId = response.CreatedId.Value;
                    await this.UpdateCaseResourceVersionAsync(caseViewModel, currentUserId);
                }

                if (resourceVersion.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment)
                {
                    var assessmentViewModel = await this.GetAssessmentDetailsByIdAsync(resourceVersion.Id, currentUserId);
                    assessmentViewModel.ResourceVersionId = response.CreatedId.Value;
                    await this.UpdateAssessmentResourceVersionAsync(assessmentViewModel, currentUserId);
                }
            }

            return response;
        }

        /// <summary>
        /// The set resource type.
        /// </summary>
        /// <param name="resourceViewModel">The resourceViewModel<see cref="ResourceViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        public void SetResourceType(ResourceViewModel resourceViewModel, int userId)
        {
            this.resourceVersionRepository.SetResourceType(resourceViewModel.ResourceVersionId, resourceViewModel.ResourceType, userId);
        }

        /// <summary>
        /// Submit resource version for publishing.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> SubmitResourceVersionForPublish(PublishViewModel publishViewModel)
        {
            if (publishViewModel.UserId == 0)
            {
                return new LearningHubValidationResult(false, "Operation to submit a resource version for publishing requires a UserId");
            }

            var validation = await this.ValidateResourceVersion(publishViewModel.ResourceVersionId);
            if (!validation.IsValid)
            {
                return validation;
            }

            try
            {
                await this.CheckAndRemoveResourceVersionProviderAsync(publishViewModel.ResourceVersionId, publishViewModel.UserId);

                this.resourceVersionRepository.SubmitForPublishing(publishViewModel.ResourceVersionId, publishViewModel.UserId);

                // send to azure storage queue
                var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.ResourcePublishQueue);
                var resourcePublishQueue = internalSystem.IsOffline ? $"{this.learningHubConfig.ResourcePublishQueueRouteName}-temp" : this.learningHubConfig.ResourcePublishQueueRouteName;
                await this.queueCommunicatorService.SendAsync(resourcePublishQueue, publishViewModel);
            }
            catch (Exception ex)
            {
                string message = "Failed to submit 'publish' message to 'ResourcePublisher' queue.";
                var resourceVersionEvent = new ResourceVersionEventViewModel()
                {
                    ResourceVersionId = publishViewModel.ResourceVersionId,
                    ResourceVersionEventType = ResourceVersionEventTypeEnum.Publishing,
                    Details = $"{message} : Error - {ex.Message}",
                    CreateUserId = publishViewModel.UserId,
                };

                this.CreateResourceVersionEvent(resourceVersionEvent);
                return new LearningHubValidationResult(false, message);
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The update resource version async.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateResourceVersionAsync(ResourceDetailViewModel resourceDetailViewModel, int userId)
        {
            ResourceVersion resourceVersion = await this.resourceVersionRepository.GetBasicByIdAsync(resourceDetailViewModel.ResourceVersionId);

            if (!await this.UserCanEditResourceVersion(resourceVersion.CreateUserId, resourceDetailViewModel.ResourceVersionId, userId))
            {
                return new LearningHubValidationResult(false);
            }

            Resource resource = await this.resourceRepository.GetByResourceVersionIdAsync(resourceDetailViewModel.ResourceVersionId);
            if (resourceDetailViewModel.ResourceType != resource.ResourceTypeEnum)
            {
                this.resourceVersionRepository.SetResourceType(resourceDetailViewModel.ResourceVersionId, resourceDetailViewModel.ResourceType, userId);
            }

            // set resource provider
            var currentproviderId = 0;

            if (resourceVersion.ResourceVersionProvider != null && resourceVersion.ResourceVersionProvider.Count > 0)
            {
                currentproviderId = (int)resourceVersion.ResourceVersionProvider.FirstOrDefault().ProviderId;
            }

            if (currentproviderId != resourceDetailViewModel.ResourceProviderId)
            {
                await this.DeleteAllResourceVersionProviderAsync(resourceVersion.Id, userId);

                if (resourceDetailViewModel.ResourceProviderId > 0)
                {
                    await this.CreateResourceVersionProviderAsync((int)resourceDetailViewModel.ResourceProviderId, resourceVersion.Id, userId);
                }
            }

            var rv = this.mapper.Map<ResourceVersion>(resourceDetailViewModel);
            var resourceVersionValidator = new ResourceVersionValidator();

            var vr = await resourceVersionValidator.ValidateAsync(rv);
            if (vr.IsValid)
            {
                if ((int)resourceDetailViewModel.ResourceCatalogueId != 0)
                {
                    await this.SetNodeResource((int)resourceDetailViewModel.NodeId, userId, resourceVersion);
                }

                await this.resourceVersionRepository.UpdateAsync(userId, rv);
            }

            return new LearningHubValidationResult(vr);
        }

        /// <summary>
        /// Delete all resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteAllResourceVersionProviderAsync(int resourceVersionId, int userId)
        {
            try
            {
                await this.resourceVersionProviderRepository.DeleteAllAsync(resourceVersionId, userId);
                return new LearningHubValidationResult(true);
            }
            catch (Exception ex)
            {
                return new LearningHubValidationResult(false, $"Error: {ex.Message}");
            }
        }


        /// <summary>
        /// Delete a resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteResourceVersionAsync(int resourceVersionId, int userId)
        {
            ResourceVersion resourceVersion = await this.resourceVersionRepository.GetBasicByIdAsync(resourceVersionId);

            if (!await this.UserCanEditResourceVersion(resourceVersion.CreateUserId, resourceVersionId, userId))
            {
                return new LearningHubValidationResult(false);
            }

            this.resourceVersionRepository.Delete(resourceVersionId, userId);
            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The update generic file detail async.
        /// </summary>
        /// <param name="genericFileViewModel">The genericFileViewModel<see cref="GenericFileUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateGenericFileDetailAsync(GenericFileUpdateRequestViewModel genericFileViewModel, int currentUserId)
        {
            var genericFile = await this.genericFileResourceVersionRepository.GetByResourceVersionIdAsync(genericFileViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (genericFile == null || !await this.UserCanEditResourceVersion(genericFile.CreateUserId, genericFileViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
            }
            else
            {
                GenericFileResourceVersion updatedGenericFile = this.mapper.Map<GenericFileResourceVersion>(genericFileViewModel);
                updatedGenericFile.Id = genericFile.Id;
                updatedGenericFile.FileId = genericFile.FileId;
                await this.genericFileResourceVersionRepository.UpdateAsync(currentUserId, updatedGenericFile);
                result.IsValid = true;
                result.CreatedId = genericFileViewModel.ResourceVersionId;
            }

            return result;
        }

        /// <summary>
        /// The update scorm detail async.
        /// </summary>
        /// <param name="scormViewModel">The scorm update request view model<see cref="ScormUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateScormDetailAsync(ScormUpdateRequestViewModel scormViewModel, int currentUserId)
        {
            var srv = await this.scormResourceVersionRepository.GetByResourceVersionIdAsync(scormViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (srv == null)
            {
                srv = this.mapper.Map<ScormResourceVersion>(scormViewModel);
                await this.scormResourceVersionRepository.CreateAsync(currentUserId, srv);
                result.IsValid = true;
                result.CreatedId = scormViewModel.ResourceVersionId;
                return result;
            }

            if (srv == null || !await this.UserCanEditResourceVersion(srv.CreateUserId, scormViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
            }
            else
            {
                ScormResourceVersion updatedScormResourceVersion = this.mapper.Map<ScormResourceVersion>(scormViewModel);
                updatedScormResourceVersion.Id = srv.Id;
                updatedScormResourceVersion.FileId = srv.FileId;
                await this.scormResourceVersionRepository.UpdateAsync(currentUserId, updatedScormResourceVersion);
                result.IsValid = true;
                result.CreatedId = scormViewModel.ResourceVersionId;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> UpdateHtmlDetailAsync(HtmlResourceUpdateRequestViewModel htmlResourceViewModel, int currentUserId)
        {
            var hrv = await this.htmlResourceVersionRepository.GetByResourceVersionIdAsync(htmlResourceViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (hrv == null)
            {
                hrv = this.mapper.Map<HtmlResourceVersion>(htmlResourceViewModel);
                await this.htmlResourceVersionRepository.CreateAsync(currentUserId, hrv);
                result.IsValid = true;
                result.CreatedId = htmlResourceViewModel.ResourceVersionId;
                return result;
            }

            if (hrv == null || !await this.UserCanEditResourceVersion(hrv.CreateUserId, htmlResourceViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
            }
            else
            {
                var updatedHtmlResourceVersion = this.mapper.Map<HtmlResourceVersion>(htmlResourceViewModel);
                updatedHtmlResourceVersion.Id = hrv.Id;
                updatedHtmlResourceVersion.FileId = hrv.FileId;
                await this.htmlResourceVersionRepository.UpdateAsync(currentUserId, updatedHtmlResourceVersion);
                result.IsValid = true;
                result.CreatedId = htmlResourceViewModel.ResourceVersionId;
            }

            return result;
        }

        /// <summary>
        /// The update image detail async.
        /// </summary>
        /// <param name="imageViewModel">The imageViewModel<see cref="ImageUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateImageDetailAsync(ImageUpdateRequestViewModel imageViewModel, int currentUserId)
        {
            var image = await this.imageResourceVersionRepository.GetByResourceVersionIdAsync(imageViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (image == null || !await this.UserCanEditResourceVersion(image.CreateUserId, imageViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
            }
            else
            {
                ImageResourceVersion updatedImage = this.mapper.Map<ImageResourceVersion>(imageViewModel);
                updatedImage.Id = image.Id;
                updatedImage.ImageFileId = image.ImageFileId;
                await this.imageResourceVersionRepository.UpdateAsync(currentUserId, updatedImage);
                result.IsValid = true;
                result.CreatedId = imageViewModel.ResourceVersionId;
            }

            return result;
        }

        /// <summary>
        /// The update video detail async.
        /// </summary>
        /// <param name="videoViewModel">The videoViewModel<see cref="VideoUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateVideoDetailAsync(VideoUpdateRequestViewModel videoViewModel, int currentUserId)
        {
            var video = await this.videoResourceVersionRepository.GetByResourceVersionIdAsync(videoViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (video == null || !await this.UserCanEditResourceVersion(video.CreateUserId, videoViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
            }
            else
            {
                VideoResourceVersion updatedVideo = this.mapper.Map<VideoResourceVersion>(videoViewModel);
                updatedVideo.Id = video.Id;
                updatedVideo.VideoFileId = video.VideoFileId;
                await this.videoResourceVersionRepository.UpdateAsync(currentUserId, updatedVideo);
                result.IsValid = true;
                result.CreatedId = videoViewModel.ResourceVersionId;
            }

            return result;
        }

        /// <summary>
        /// The delete audio attribute file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteAudioAttributeFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId)
        {
            var audioFile = await this.audioResourceVersionRepository.GetByResourceVersionIdAsync(fileDeleteRequestModel.ResourceVersionId);
            var retVal = new LearningHubValidationResult();

            if (audioFile == null || !await this.UserCanEditResourceVersion(audioFile.CreateUserId, fileDeleteRequestModel.ResourceVersionId, currentUserId))
            {
                retVal.IsValid = false;
            }
            else
            {
                switch (fileDeleteRequestModel.AttachedFileType)
                {
                    case AttachedFileTypeEnum.Transcript:
                        audioFile.TranscriptFile = null;
                        audioFile.TranscriptFileId = null;
                        break;
                }

                await this.audioResourceVersionRepository.UpdateAsync(currentUserId, audioFile);
                retVal.IsValid = true;
                retVal.CreatedId = audioFile.Id;
            }

            return retVal;
        }

        /// <summary>
        /// The delete video attribute file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteVideoAttributeFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId)
        {
            var videoFile = await this.videoResourceVersionRepository.GetByResourceVersionIdAsync(fileDeleteRequestModel.ResourceVersionId);
            var retVal = new LearningHubValidationResult();

            if (videoFile == null || !await this.UserCanEditResourceVersion(videoFile.CreateUserId, fileDeleteRequestModel.ResourceVersionId, currentUserId))
            {
                retVal.IsValid = false;
            }
            else
            {
                switch (fileDeleteRequestModel.AttachedFileType)
                {
                    case AttachedFileTypeEnum.Transcript:
                        videoFile.TranscriptFile = null;
                        videoFile.TranscriptFileId = null;
                        break;
                    case AttachedFileTypeEnum.ClosedCaptions:
                        videoFile.ClosedCaptionsFile = null;
                        videoFile.ClosedCaptionsFileId = null;
                        break;
                }

                await this.videoResourceVersionRepository.UpdateAsync(currentUserId, videoFile);
                retVal.IsValid = true;
                retVal.CreatedId = videoFile.Id;
            }

            return retVal;
        }

        /// <summary>
        /// The update audio detail async.
        /// </summary>
        /// <param name="audioViewModel">The audioViewModel<see cref="AudioUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public Task<LearningHubValidationResult> UpdateAudioDetailAsync(AudioUpdateRequestViewModel audioViewModel, int currentUserId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update article detail async.
        /// </summary>
        /// <param name="articleViewModel">The articleViewModel<see cref="ArticleUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateArticleDetailAsync(ArticleUpdateRequestViewModel articleViewModel, int currentUserId)
        {
            var article = await this.articleResourceVersionRepository.GetByResourceVersionIdAsync(articleViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (article == null || !await this.UserCanEditResourceVersion(article.CreateUserId, articleViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
            }
            else
            {
                ArticleResourceVersion updatedArticle = this.mapper.Map<ArticleResourceVersion>(articleViewModel);
                updatedArticle.Id = article.Id;
                await this.articleResourceVersionRepository.UpdateAsync(currentUserId, updatedArticle);
                result.IsValid = true;
                result.CreatedId = articleViewModel.ResourceVersionId;
            }

            return result;
        }

        /// <summary>
        /// The delete article file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteArticleFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId)
        {
            var articleFile = await this.articleResourceVersionFileRepository.GetByResourceVersionAndFileAsync(fileDeleteRequestModel.ResourceVersionId, fileDeleteRequestModel.FileId);
            var retVal = new LearningHubValidationResult();

            if (articleFile == null || !await this.UserCanEditResourceVersion(articleFile.CreateUserId, fileDeleteRequestModel.ResourceVersionId, currentUserId))
            {
                retVal.IsValid = false;
            }
            else
            {
                articleFile.Deleted = true;
                await this.articleResourceVersionFileRepository.UpdateAsync(currentUserId, articleFile);
                retVal.IsValid = true;
                retVal.CreatedId = articleFile.Id;
            }

            return retVal;
        }

        /// <summary>
        /// The add resource version author async.
        /// </summary>
        /// <param name="resourceAuthorViewModel">The resourceAuthorViewModel<see cref="ResourceAuthorViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> AddResourceVersionAuthorAsync(ResourceAuthorViewModel resourceAuthorViewModel, int userId)
        {
            var rva = this.mapper.Map<ResourceVersionAuthor>(resourceAuthorViewModel);
            if (rva.IsContributor)
            {
                rva.AuthorUserId = userId;
            }

            return await this.resourceVersionAuthorRepository.CreateAsync(userId, rva);
        }

        /// <summary>
        /// The create audio details async.
        /// </summary>
        /// <param name="authorDeleteRequestModel">The authorDeleteRequestModel<see cref="AuthorDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteResourceVersionAuthorAsync(AuthorDeleteRequestModel authorDeleteRequestModel, int currentUserId)
        {
            var author = await this.resourceVersionAuthorRepository.GetByResourceVersionAndAuthorAsync(authorDeleteRequestModel.ResourceVersionId, authorDeleteRequestModel.AuthorId);
            var retVal = new LearningHubValidationResult();

            if (author == null || (!await this.UserCanEditResourceVersion(author.CreateUserId, authorDeleteRequestModel.ResourceVersionId, currentUserId)))
            {
                retVal.IsValid = false;
            }
            else
            {
                await this.resourceVersionAuthorRepository.DeleteAsync(currentUserId, author.Id);
                retVal.IsValid = true;
                retVal.CreatedId = author.Id;
            }

            return retVal;
        }

        /// <summary>
        /// The delete resource version keyword async.
        /// </summary>
        /// <param name="keywordDeleteRequestModel">The keywordDeleteRequestModel<see cref="KeywordDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteResourceVersionKeywordAsync(KeywordDeleteRequestModel keywordDeleteRequestModel, int currentUserId)
        {
            var keyword = await this.resourceVersionKeywordRepository.GetByResourceVersionAndKeywordAsync(keywordDeleteRequestModel.ResourceVersionId, keywordDeleteRequestModel.KeywordId);
            var retVal = new LearningHubValidationResult();

            if (keyword == null || (!await this.UserCanEditResourceVersion(keyword.CreateUserId, keywordDeleteRequestModel.ResourceVersionId, currentUserId)))
            {
                retVal.IsValid = false;
            }
            else
            {
                await this.resourceVersionKeywordRepository.DeleteAsync(currentUserId, keyword.Id);
                retVal.IsValid = true;
                retVal.CreatedId = keyword.Id;
            }

            return retVal;
        }

        /// <summary>
        /// The delete resource version flag async.
        /// </summary>
        /// <param name="resourceVersionFlagId">The resourceVersionFlagId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteResourceVersionFlagAsync(int resourceVersionFlagId, int userId)
        {
            await this.resourceVersionFlagRepository.DeleteAsync(userId, resourceVersionFlagId);
        }


        /// <summary>
        /// The update web link resource version async.
        /// </summary>
        /// <param name="weblinkViewModel">The weblinkViewModel<see cref="WebLinkViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateWebLinkResourceVersionAsync(WebLinkViewModel weblinkViewModel, int currentUserId)
        {
            var weblink = await this.webLinkResourceVersionRepository.GetByResourceVersionIdAsync(weblinkViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (weblink == null || (!await this.UserCanEditResourceVersion(weblink.CreateUserId, weblinkViewModel.ResourceVersionId, currentUserId)))
            {
                result.IsValid = false;
            }
            else
            {
                WebLinkResourceVersion updatedWeblink = this.mapper.Map<WebLinkResourceVersion>(weblinkViewModel);
                updatedWeblink.Id = weblink.Id;
                await this.webLinkResourceVersionRepository.UpdateAsync(currentUserId, updatedWeblink);
                result.IsValid = true;
                result.CreatedId = weblinkViewModel.ResourceVersionId;
            }

            return result;
        }

        /// <summary>
        /// The update case resource version async.
        /// </summary>
        /// <param name="caseViewModel">The case view model.</param>
        /// <param name="currentUserId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateCaseResourceVersionAsync(CaseViewModel caseViewModel, int currentUserId)
        {
            CaseResourceVersion caseResourceVersion = await this.caseResourceVersionRepository
                .GetByResourceVersionIdAsync(caseViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (caseResourceVersion != null && !await this.UserCanEditResourceVersion(caseResourceVersion.CreateUserId, caseViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
                return result;
            }

            if (caseResourceVersion == null)
            {
                caseResourceVersion = new CaseResourceVersion
                {
                    ResourceVersionId = caseViewModel.ResourceVersionId,
                };
                await this.caseResourceVersionRepository.CreateAsync(currentUserId, caseResourceVersion);
            }

            int? oldBlockCollectionId = caseResourceVersion.BlockCollectionId;

            // Save new BlockCollection
            BlockCollection blockCollection = this.mapper.Map<BlockCollection>(caseViewModel.BlockCollection);
            int newBlockCollectionId = await this.blockCollectionRepository.CreateAsync(currentUserId, blockCollection);

            // Update Case with BlockCollection Id
            caseResourceVersion.BlockCollectionId = newBlockCollectionId;
            await this.caseResourceVersionRepository.UpdateAsync(currentUserId, caseResourceVersion);

            // Delete old BlockCollection
            if (oldBlockCollectionId.HasValue)
            {
                await this.blockCollectionRepository.DeleteBlockCollection(currentUserId, oldBlockCollectionId.Value);
            }

            result.IsValid = true;
            result.CreatedId = caseViewModel.ResourceVersionId;

            return result;
        }

        /// <summary>
        /// The update assessment resource version async.
        /// </summary>
        /// <param name="assessmentViewModel">The assessment view model.</param>
        /// <param name="currentUserId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateAssessmentResourceVersionAsync(AssessmentViewModel assessmentViewModel, int currentUserId)
        {
            var assessmentResourceVersion = await this.assessmentResourceVersionRepository
                .GetByResourceVersionIdAsync(assessmentViewModel.ResourceVersionId);
            var result = new LearningHubValidationResult();

            if (assessmentResourceVersion != null && !await this.UserCanEditResourceVersion(assessmentResourceVersion.CreateUserId, assessmentViewModel.ResourceVersionId, currentUserId))
            {
                result.IsValid = false;
                result.Details = new List<string>(new string[] { "The resource does not exist or you do not have access privilege" });
                return result;
            }

            if (assessmentResourceVersion == null)
            {
                assessmentResourceVersion = new AssessmentResourceVersion
                {
                    ResourceVersionId = assessmentViewModel.ResourceVersionId,
                };
                await this.assessmentResourceVersionRepository.CreateAsync(currentUserId, assessmentResourceVersion);
            }

            var oldAssessmentContentId = assessmentResourceVersion.AssessmentContentId;
            var oldEndGuidanceId = assessmentResourceVersion.EndGuidanceId;

            // Save new BlockCollections
            var assessmentContent = this.mapper.Map<BlockCollection>(assessmentViewModel.AssessmentContent);
            var newAssessmentContentId = await this.blockCollectionRepository.CreateAsync(currentUserId, assessmentContent);
            var endGuidance = this.mapper.Map<BlockCollection>(assessmentViewModel.EndGuidance);
            var newEndGuidanceId = await this.blockCollectionRepository.CreateAsync(currentUserId, endGuidance);

            // Update Assessment with BlockCollection Ids
            assessmentResourceVersion.AssessmentContentId = newAssessmentContentId;
            assessmentResourceVersion.EndGuidanceId = newEndGuidanceId;

            // Update Assessment Fields
            assessmentResourceVersion.AssessmentType = assessmentViewModel.AssessmentType;
            assessmentResourceVersion.MaximumAttempts = assessmentViewModel.MaximumAttempts;
            assessmentResourceVersion.PassMark = assessmentViewModel.PassMark;
            assessmentResourceVersion.AnswerInOrder = assessmentViewModel.AnswerInOrder;
            await this.assessmentResourceVersionRepository.UpdateAsync(currentUserId, assessmentResourceVersion);

            // Delete old BlockCollections
            if (oldAssessmentContentId.HasValue)
            {
                await this.blockCollectionRepository.DeleteBlockCollection(currentUserId, oldAssessmentContentId.Value);
            }

            if (oldEndGuidanceId.HasValue)
            {
                await this.blockCollectionRepository.DeleteBlockCollection(currentUserId, oldEndGuidanceId.Value);
            }

            result.IsValid = true;
            result.CreatedId = assessmentViewModel.ResourceVersionId;

            return result;
        }

        /// <summary>
        /// The create file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">The fileChunkDetailCreateRequestViewModel<see cref="FileChunkDetailViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateFileChunkDetailAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel, int currentUserId)
        {
            try
            {
                FileChunkDetail fileChunkDetail = this.mapper.Map<FileChunkDetail>(fileChunkDetailCreateRequestViewModel);

                var fileChunkDetailId = await this.fileChunkDetailRepository.CreateAsync(currentUserId, fileChunkDetail);

                var retVal = new LearningHubValidationResult(true);
                retVal.CreatedId = fileChunkDetailId;
                return retVal;
            }
            catch (Exception ex)
            {
                var retVal = new LearningHubValidationResult(false);
                retVal.Details = new List<string>() { ex.Message };
                return retVal;
            }
        }

        /// <summary>
        /// The get resource licences async.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileChunkDetailViewModel}"/>.</returns>
        public async Task<FileChunkDetailViewModel> GetFileChunkDetailAsync(int fileChunkDetailId)
        {
            FileChunkDetail fileChunkDetail = await this.fileChunkDetailRepository.GetByIdAsync(fileChunkDetailId);
            return this.mapper.Map<FileChunkDetailViewModel>(fileChunkDetail);
        }

        /// <summary>
        /// Delete a file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteFileChunkDetailAsync(int fileChunkDetailId, int userId)
        {
            FileChunkDetail fileChunkDetail = await this.fileChunkDetailRepository.GetByIdAsync(fileChunkDetailId);

            if (userId == 0)
            {
                return new LearningHubValidationResult(false, "The operation to delete file chunk detail requires an AmendUserId.");
            }

            await this.fileChunkDetailRepository.Delete(fileChunkDetailId, userId);
            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The create file details async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId)
        {
            if (fileCreateRequestViewModel.ResourceVersionId == 0)
            {
                fileCreateRequestViewModel.ResourceVersionId = await this.resourceRepository.CreateResourceAsync(fileCreateRequestViewModel.ResourceType, string.Empty, string.Empty, userId);
            }
            else
            {
                Resource resource = await this.resourceRepository.GetByResourceVersionIdAsync(fileCreateRequestViewModel.ResourceVersionId);
                if (resource.ResourceTypeEnum != fileCreateRequestViewModel.ResourceType)
                {
                    this.resourceVersionRepository.SetResourceType(fileCreateRequestViewModel.ResourceVersionId, fileCreateRequestViewModel.ResourceType, userId);
                }
            }

            File newFile = new File()
            {
                FileTypeId = fileCreateRequestViewModel.FileTypeId,
                FileName = fileCreateRequestViewModel.FileName,
                FilePath = fileCreateRequestViewModel.FilePath,
                FileSizeKb = fileCreateRequestViewModel.FileSize,
                FileChunkDetailId = fileCreateRequestViewModel.FileChunkDetailId,
            };

            switch (fileCreateRequestViewModel.ResourceType)
            {
                case ResourceTypeEnum.GenericFile:
                    await this.CreateGenericFileDetailsAsync(fileCreateRequestViewModel.ResourceVersionId, newFile, userId);
                    break;
                case ResourceTypeEnum.Image:
                    await this.CreateImageDetailsAsync(fileCreateRequestViewModel.ResourceVersionId, newFile, userId);
                    break;
                case ResourceTypeEnum.Video:
                    await this.CreateVideoDetailsAsync(fileCreateRequestViewModel.ResourceVersionId, newFile, userId);
                    break;
                case ResourceTypeEnum.Audio:
                    await this.CreateAudioDetailsAsync(fileCreateRequestViewModel.ResourceVersionId, newFile, userId);
                    break;
                case ResourceTypeEnum.Article:
                case ResourceTypeEnum.Embedded:
                case ResourceTypeEnum.Equipment:
                case ResourceTypeEnum.Scorm:
                    await this.CreateScormFileDetailsAsync(fileCreateRequestViewModel.ResourceVersionId, newFile, userId);
                    break;
                case ResourceTypeEnum.WebLink:
                case ResourceTypeEnum.Undefined:
                    return new LearningHubValidationResult(false, "Invalid ResourceType for file creation!");
                case ResourceTypeEnum.Html:
                    await this.CreateHtmlDetailsAsync(fileCreateRequestViewModel.ResourceVersionId, newFile, userId);
                    break;
            }

            var retVal = new LearningHubValidationResult(true);
            retVal.CreatedId = fileCreateRequestViewModel.ResourceVersionId;
            return retVal;
        }

        /// <summary>
        /// The check and remove resource provider permission if user do not have permission to the provider.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task CheckAndRemoveResourceVersionProviderAsync(int resourceVersionId, int userId)
        {
            List<int> userProviderList = new List<int>();

            // list of resource version provider
            var resourceProviderList = await this.providerService.GetByResourceVersionIdAsync(resourceVersionId);

            // list of user providers
            var userProviders = await this.providerService.GetByUserIdAsync(userId);

            if (userProviders != null && userProviders.Count > 0)
            {
                userProviderList.AddRange(userProviders.Select(n => n.Id).ToList());
            }

            // delete provider from resource version if user do not have permission.
            if (resourceProviderList != null && resourceProviderList.Count > 0)
            {
                foreach (ProviderViewModel resourceProvider in resourceProviderList)
                {
                    if (!userProviderList.Contains(resourceProvider.Id))
                    {
                        await this.resourceVersionProviderRepository.DeleteAsync(resourceVersionId, resourceProvider.Id, userId);
                    }
                }
            }
        }

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionEventViewModel">resourceVersionEventViewModel.</param>
        public void CreateResourceVersionEvent(ResourceVersionEventViewModel resourceVersionEventViewModel)
        {
            this.resourceVersionRepository.CreateResourceVersionEvent(resourceVersionEventViewModel.ResourceVersionId, resourceVersionEventViewModel.ResourceVersionEventType, resourceVersionEventViewModel.Details, resourceVersionEventViewModel.CreateUserId);
        }

        /// <summary>
        /// The create resource provider.
        /// </summary>
        /// <param name="providerId">provider id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateResourceVersionProviderAsync(int providerId, int resourceVersionId, int userId)
        {
            try
            {
                var resourceVersionProvider = new ResourceVersionProvider { ProviderId = providerId, ResourceVersionId = resourceVersionId };
                int id = await this.resourceVersionProviderRepository.CreateAsync(userId, resourceVersionProvider);

                var retVal = new LearningHubValidationResult(true);
                retVal.CreatedId = id;
                return retVal;
            }
            catch (Exception ex)
            {
                return new LearningHubValidationResult(false, $"Error creating resource provider: {ex.Message}");
            }
        }


        /// <summary>
        /// Set the resource catalogue.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="resourceVersion">The resourceVersion<see cref="ResourceVersion"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task SetNodeResource(int nodeId, int userId, ResourceVersion resourceVersion)
        {
            if (resourceVersion.VersionStatusEnum != VersionStatusEnum.Draft)
            {
                throw new Exception("NodeResource can only be changed for Draft resourceVersion records. ResourceVersionId: " + resourceVersion.Id.ToString() + ". ResourceId: " + resourceVersion.ResourceId.ToString() + ", VersionStatusEnum:" + resourceVersion.VersionStatusEnum.ToString());
            }

            // Note: This used to be done here in code, but has now been moved out to a stored proc.
            await this.nodeResourceRepository.CreateOrUpdateAsync(nodeId, resourceVersion.ResourceId, userId);
        }


        private LearningHubValidationResult ValidateRequestWithSourceResource(ResourceVersion sourceResourceVersion, DuplicateBlocksRequestModel requestModel)
        {
            var result = new LearningHubValidationResult(false, "Duplicate Blocks async: ");
            if (sourceResourceVersion == null)
            {
                result.IsValid = false;
                result.Details.Add("Resource not found");

                return result;
            }

            var sourceResource = sourceResourceVersion.Resource;

            if (sourceResource.ResourceTypeEnum != ResourceTypeEnum.Case && sourceResource.ResourceTypeEnum != ResourceTypeEnum.Assessment)
            {
                result.IsValid = false;
                result.Details.Add("Cannot duplicate a resource of type " + sourceResource.ResourceTypeEnum);

                return result;
            }

            if (sourceResource.ResourceTypeEnum == ResourceTypeEnum.Assessment && requestModel.DestinationResourceId == null && requestModel.AssessmentType == null)
            {
                result.IsValid = false;
                result.Details.Add("Undefined assessment type");

                return result;
            }

            result.IsValid = true;
            return result;
        }

        private LearningHubValidationResult ValidateRequestWithSourceBlockCollection(BlockCollection sourceBlockCollection, DuplicateBlocksRequestModel requestModel)
        {
            var result = new LearningHubValidationResult(false, "Duplicate Blocks async: ");
            if (sourceBlockCollection == null)
            {
                result.IsValid = false;
                result.Details.Add("Invalid source resource");

                return result;
            }

            if (requestModel.BlockIds.Any(id => sourceBlockCollection.Blocks.All(block => block.Id != id)))
            {
                result.IsValid = false;
                result.Details.Add("Invalid block IDs");

                return result;
            }

            result.IsValid = true;
            return result;
        }

        private async Task<LearningHubValidationResult> ValidateDestinationAndGetResourceVersionId(
    int destinationResourceId, int userId, ResourceTypeEnum sourceResourceType)
        {
            var result = new LearningHubValidationResult(false, "Duplicate Blocks async: ");
            var resourceVersion = await this.resourceVersionRepository.GetCurrentForResourceAsync(destinationResourceId);
            if (resourceVersion == null)
            {
                result.IsValid = false;
                result.Details.Add("Resource not found");

                return result;
            }

            var destResourceVersionId = resourceVersion.Id;
            if (sourceResourceType != resourceVersion.Resource.ResourceTypeEnum)
            {
                result.IsValid = false;
                result.Details.Add("Cannot copy block to a different resource type");

                return result;
            }

            if (resourceVersion.CreateUserId != userId)
            {
                result.IsValid = false;
                result.Details.Add("User is not allowed to duplicate blocks to the selected resource");

                return result;
            }

            if (resourceVersion.VersionStatusEnum != VersionStatusEnum.Draft)
            {
                destResourceVersionId =
                    await this.CreateNewDraftVersionForResource(destinationResourceId, resourceVersion, userId);
            }

            result.IsValid = true;
            result.CreatedId = destResourceVersionId;
            return result;
        }

        private async Task<int> CreateNewDraftVersionForResource(int resourceId, ResourceVersion currentResourceVersion, int userId)
        {
            var resourceVersionId = await this.resourceVersionRepository.CreateNextVersionAsync(resourceId, userId);

            if (currentResourceVersion.Resource.ResourceTypeEnum == ResourceTypeEnum.Case)
            {
                var caseViewModel = await this.GetCaseDetailsByIdAsync(currentResourceVersion.Id);
                caseViewModel.ResourceVersionId = resourceVersionId;
                await this.UpdateCaseResourceVersionAsync(caseViewModel, userId);
            }

            if (currentResourceVersion.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment)
            {
                var assessmentViewModel =
                    await this.GetAssessmentDetailsByIdAsync(currentResourceVersion.Id, userId);
                assessmentViewModel.ResourceVersionId = resourceVersionId;
                await this.UpdateAssessmentResourceVersionAsync(assessmentViewModel, userId);
            }

            return resourceVersionId;
        }

        private async Task<int> CreateNewResource(ResourceTypeEnum resourceType, string title, int userId, AssessmentTypeEnum? assessmentType)
        {
            var resourceVersionId = await this.resourceRepository.CreateResourceAsync(resourceType, title, string.Empty, userId);
            if (resourceType == ResourceTypeEnum.Case)
            {
                await this.caseResourceVersionRepository.CreateAsync(userId, new CaseResourceVersion
                {
                    ResourceVersionId = resourceVersionId,
                    BlockCollection = new BlockCollection(),
                });
            }
            else
            {
                await this.assessmentResourceVersionRepository.CreateAsync(userId, new AssessmentResourceVersion
                {
                    ResourceVersionId = resourceVersionId,
                    AssessmentContent = new BlockCollection(),
                    AssessmentType = assessmentType.Value,
                });
            }

            return resourceVersionId;
        }

        private List<string> CheckBlockFile(BlockCollectionViewModel? publishedBlock, BlockCollectionViewModel model)
        {
            var retVal = new List<string>();
            var caseBlockCollection = model;
            if (caseBlockCollection != null)
            {
                var allBlocks = caseBlockCollection.Blocks.ToList();
                if (allBlocks.Any())
                {
                    var existingAttachements = allBlocks.Where(x => x.BlockType == BlockType.Media && x.MediaBlock != null && x.MediaBlock.MediaType == MediaType.Attachment && x.MediaBlock.Attachment != null).ToList();
                    if (existingAttachements.Any())
                    {
                        foreach (var oldblock in existingAttachements)
                        {
                            var publishedEntry = (publishedBlock != null && publishedBlock.Blocks.Any()) ? publishedBlock.Blocks.FirstOrDefault(x => x.BlockType == BlockType.Media && x.MediaBlock != null && x.MediaBlock.MediaType == MediaType.Attachment && x.MediaBlock.Attachment != null && (x.MediaBlock.Attachment.File?.FilePath == oldblock.MediaBlock.Attachment?.File?.FilePath || x.MediaBlock.Attachment.File?.FileId == oldblock.MediaBlock.Attachment?.File?.FileId)) : null;
                            if (publishedEntry == null)
                            {
                                retVal.Add(oldblock.MediaBlock.Attachment?.File?.FilePath);
                            }
                        }
                    }

                    var existingVideos = allBlocks.Where(x => x.BlockType == BlockType.Media && x.MediaBlock != null && x.MediaBlock.MediaType == MediaType.Video && x.MediaBlock.Video != null).ToList();
                    if (existingVideos.Any())
                    {
                        foreach (var oldblock in existingVideos)
                        {
                            var publishedEntry = (publishedBlock != null && publishedBlock.Blocks.Any()) ? publishedBlock.Blocks.FirstOrDefault(x => x.BlockType == BlockType.Media && x.MediaBlock != null && x.MediaBlock.MediaType == MediaType.Video && x.MediaBlock.Video != null && (x.MediaBlock.Video.VideoFile?.File?.FilePath == oldblock.MediaBlock?.Video?.VideoFile?.File?.FilePath || x.MediaBlock.Video.VideoFile?.File?.FileId == oldblock.MediaBlock?.Video?.VideoFile?.File?.FileId)) : null;
                            if (publishedEntry == null)
                            {
                                if (!string.IsNullOrWhiteSpace(oldblock.MediaBlock.Video.File.FilePath))
                                {
                                    retVal.Add(oldblock.MediaBlock.Video.File.FilePath);
                                }

                                if (!string.IsNullOrWhiteSpace(oldblock.MediaBlock.Video?.File?.VideoFile?.File?.FilePath))
                                {
                                    retVal.Add(oldblock.MediaBlock.Video.File.VideoFile.File.FilePath);
                                }

                                if (oldblock.MediaBlock?.Video?.File?.VideoFile?.TranscriptFile?.File?.FilePath != null)
                                {
                                    retVal.Add(oldblock.MediaBlock.Video.File.VideoFile.TranscriptFile.File.FilePath);
                                }
                            }
                        }
                    }

                    var existingImages = allBlocks.Where(x => x.BlockType == BlockType.Media && x.MediaBlock != null && x.MediaBlock.MediaType == MediaType.Image && x.MediaBlock.Image != null).ToList();
                    if (existingImages.Any())
                    {
                        foreach (var oldblock in existingImages)
                        {
                            var publishedEntry = (publishedBlock != null && publishedBlock.Blocks.Any()) ? publishedBlock.Blocks.FirstOrDefault(x => x.BlockType == BlockType.Media && x.MediaBlock != null && x.MediaBlock.MediaType == MediaType.Image && x.MediaBlock.Image != null && (x.MediaBlock?.Image?.File?.FilePath == oldblock.MediaBlock?.Image?.File?.FilePath || x.MediaBlock?.Image?.File?.FileId == oldblock.MediaBlock?.Image?.File?.FileId)) : null;
                            if (publishedEntry == null)
                            {
                                retVal.Add(oldblock.MediaBlock?.Image?.File?.FilePath);
                            }
                        }
                    }

                    var existingImageCarousel = allBlocks.Where(x => x.BlockType == BlockType.ImageCarousel && x.ImageCarouselBlock != null && x.ImageCarouselBlock.ImageBlockCollection != null && x.ImageCarouselBlock.ImageBlockCollection.Blocks != null).ToList();
                    if (existingImageCarousel.Any())
                    {
                        foreach (var imageBlock in existingImageCarousel)
                        {
                            foreach (var oldblock in imageBlock.ImageCarouselBlock.ImageBlockCollection.Blocks)
                            {
                                var publishedEntry = (publishedBlock != null && publishedBlock.Blocks.Any()) ? publishedBlock.Blocks.FirstOrDefault(x => x.BlockType == BlockType.ImageCarousel && x.ImageCarouselBlock != null && x.ImageCarouselBlock.ImageBlockCollection != null && x.ImageCarouselBlock.ImageBlockCollection.Blocks != null && x.ImageCarouselBlock.ImageBlockCollection.Blocks.Where(x => x.MediaBlock?.Image?.File?.FilePath == oldblock.MediaBlock?.Image?.File?.FilePath || x.MediaBlock?.Image?.File?.FileId == oldblock.MediaBlock?.Image?.File?.FileId).Any()) : null;
                                if (publishedEntry == null)
                                {
                                    retVal.Add(oldblock.MediaBlock?.Image?.File?.FilePath);
                                }
                            }
                        }
                    }

                    var existingWholeSlideImages = allBlocks.Where(x => x.WholeSlideImageBlock != null && x.WholeSlideImageBlock.WholeSlideImageBlockItems.Any()).ToList();
                    if (existingWholeSlideImages.Any())
                    {
                        foreach (var wsi in existingWholeSlideImages)
                        {
                            foreach (var oldblock in wsi?.WholeSlideImageBlock?.WholeSlideImageBlockItems)
                            {
                                if (oldblock != null && (oldblock.WholeSlideImage.File.WholeSlideImageFile.Status == WholeSlideImageFileStatus.ProcessingComplete || oldblock.WholeSlideImage.File.WholeSlideImageFile.Status == WholeSlideImageFileStatus.ProcessingFailed))
                                {
                                    var publishedEntry = (publishedBlock != null && publishedBlock.Blocks.Any()) ? publishedBlock.Blocks.FirstOrDefault(x => x.WholeSlideImageBlock != null && x.WholeSlideImageBlock.WholeSlideImageBlockItems.Where(x => x.WholeSlideImage?.File?.FilePath == oldblock.WholeSlideImage?.File?.FilePath || x.WholeSlideImage?.File?.FileId == oldblock.WholeSlideImage?.File?.FileId).Any()) : null;
                                    if (publishedEntry == null)
                                    {
                                        retVal.Add(oldblock.WholeSlideImage?.File?.FilePath);
                                    }
                                }
                            }
                        }
                    }
                }

                var questionFiles = this.CheckQuestionBlock(caseBlockCollection);
                var publishedQuestionFiles = (publishedBlock != null && publishedBlock.Blocks.Any()) ? this.CheckQuestionBlock(publishedBlock) : new Dictionary<int, string>();
                if (questionFiles.Any() && !publishedQuestionFiles.Any())
                {
                    retVal.AddRange(questionFiles.Values.ToList());
                }
                else if (questionFiles.Any() && publishedQuestionFiles.Any())
                {
                    foreach (var file in questionFiles)
                    {
                        bool found = false;
                        var publishedEntry = publishedQuestionFiles.FirstOrDefault(x => (x.Key == file.Key || x.Value == file.Value) && (found = true));
                        if (!found)
                        {
                            retVal.Add(file.Value);
                        }
                    }
                }
            }

            return retVal.Where(x => x != null).ToList();
        }

        private Dictionary<int, string> CheckQuestionBlock(BlockCollectionViewModel model)
        {
            var filePath = new Dictionary<int, string>();
            if (model != null && model.Blocks.Any())
            {
                foreach (var block in model.Blocks)
                {
                    if (block.BlockType == BlockType.Question && block.QuestionBlock != null)
                    {
                        if (block.QuestionBlock.QuestionType == QuestionTypeEnum.MatchGame && block.QuestionBlock.Answers != null)
                        {
                            foreach (var answerBlock in block.QuestionBlock.Answers)
                            {
                                if (answerBlock.BlockCollection != null && answerBlock.BlockCollection.Blocks != null && answerBlock.BlockCollection.Blocks.Any())
                                {
                                    foreach (var imageBlock in answerBlock.BlockCollection.Blocks)
                                    {
                                        if (imageBlock.BlockType == BlockType.Media && imageBlock.MediaBlock != null && imageBlock.MediaBlock.Image.File != null)
                                        {
                                            filePath.Add(imageBlock.MediaBlock.Image.File.FileId, imageBlock.MediaBlock.Image.File.FilePath);
                                        }
                                    }
                                }
                            }
                        }

                        var questionBlockCollection = block.QuestionBlock.QuestionBlockCollection;
                        if (questionBlockCollection != null && questionBlockCollection.Blocks != null && questionBlockCollection.Blocks.Any())
                        {
                            foreach (var questionBlock in questionBlockCollection.Blocks)
                            {
                                if (questionBlock.BlockType == BlockType.Media && questionBlock.MediaBlock != null)
                                {
                                    if (questionBlock.MediaBlock.Image != null)
                                    {
                                        filePath.Add(questionBlock.MediaBlock.Image.File.FileId, questionBlock.MediaBlock.Image.File.FilePath);
                                    }

                                    if (questionBlock.MediaBlock.Video != null)
                                    {
                                        if (questionBlock.MediaBlock.Video.File != null)
                                        {
                                            filePath.Add(questionBlock.MediaBlock.Video.File.FileId, questionBlock.MediaBlock.Video.File.FilePath);
                                        }

                                        if (questionBlock.MediaBlock.Video.VideoFile != null)
                                        {
                                            if (questionBlock.MediaBlock.Video.VideoFile.TranscriptFile != null)
                                            {
                                                filePath.Add(questionBlock.MediaBlock.Video.VideoFile.TranscriptFile.File.FileId, questionBlock.MediaBlock.Video.VideoFile.TranscriptFile.File.FilePath);
                                            }

                                            if (questionBlock.MediaBlock.Video.VideoFile.CaptionsFile != null)
                                            {
                                                filePath.Add(questionBlock.MediaBlock.Video.VideoFile.CaptionsFile.File.FileId, questionBlock.MediaBlock.Video.VideoFile.CaptionsFile.File.FilePath);
                                            }
                                        }
                                    }
                                }
                                else if (questionBlock.BlockType == BlockType.WholeSlideImage && questionBlock.WholeSlideImageBlock != null && questionBlock.WholeSlideImageBlock.WholeSlideImageBlockItems.Any())
                                {
                                    var existingWholeSlideImages = questionBlock.WholeSlideImageBlock.WholeSlideImageBlockItems;
                                    if (existingWholeSlideImages.Any())
                                    {
                                        foreach (var wsi in existingWholeSlideImages)
                                        {
                                            if (wsi.WholeSlideImage != null && wsi.WholeSlideImage.File != null && wsi.WholeSlideImage.File.FileId > 0)
                                            {
                                                filePath.Add(wsi.WholeSlideImage.File.FileId, wsi.WholeSlideImage.File.FilePath);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var feedbackBlockCollection = block.QuestionBlock.FeedbackBlockCollection;
                        if (feedbackBlockCollection != null && feedbackBlockCollection.Blocks != null && feedbackBlockCollection.Blocks.Any())
                        {
                            foreach (var feedbackBlock in feedbackBlockCollection.Blocks)
                            {
                                if (feedbackBlock.BlockType == BlockType.Media && feedbackBlock.MediaBlock != null)
                                {
                                    if (feedbackBlock.MediaBlock.Image != null)
                                    {
                                        filePath.Add(feedbackBlock.MediaBlock.Image.File.FileId, feedbackBlock.MediaBlock.Image.File.FilePath);
                                    }

                                    if (feedbackBlock.MediaBlock.Video != null)
                                    {
                                        if (feedbackBlock.MediaBlock.Video.File != null)
                                        {
                                            filePath.Add(feedbackBlock.MediaBlock.Video.File.FileId, feedbackBlock.MediaBlock.Video.File.FilePath);
                                        }

                                        if (feedbackBlock.MediaBlock.Video.VideoFile != null)
                                        {
                                            if (feedbackBlock.MediaBlock.Video.VideoFile.TranscriptFile != null)
                                            {
                                                filePath.Add(feedbackBlock.MediaBlock.Video.VideoFile.TranscriptFile.File.FileId, feedbackBlock.MediaBlock.Video.VideoFile.TranscriptFile.File.FilePath);
                                            }

                                            if (feedbackBlock.MediaBlock.Video.VideoFile.CaptionsFile != null)
                                            {
                                                filePath.Add(feedbackBlock.MediaBlock.Video.VideoFile.CaptionsFile.File.FileId, feedbackBlock.MediaBlock.Video.VideoFile.CaptionsFile.File.FilePath);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return filePath;
        }

        private async Task<LearningHubValidationResult> ValidateResourceVersion(int resourceVersionId)
        {
            CaseResourceVersion caseResourceVersion = await this.caseResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            if (caseResourceVersion != null)
            {
                return await this.ValidateCaseResourceVersion(caseResourceVersion);
            }

            AssessmentResourceVersion assessmentResourceVersion = await this.assessmentResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            if (assessmentResourceVersion != null)
            {
                return await this.ValidateAssessmentResourceVersion(assessmentResourceVersion);
            }

            return new LearningHubValidationResult(true);
        }

        private async Task<LearningHubValidationResult> ValidateCaseResourceVersion(CaseResourceVersion caseResourceVersion)
        {
            return await this.ValidateQuestionsInBlockCollection(caseResourceVersion.BlockCollectionId);
        }

        private async Task<LearningHubValidationResult> ValidateAssessmentResourceVersion(AssessmentResourceVersion assessmentResourceVersion)
        {
            assessmentResourceVersion.AssessmentContent = await this.blockCollectionRepository.GetBlockCollection(assessmentResourceVersion.AssessmentContentId);
            assessmentResourceVersion.EndGuidance = await this.blockCollectionRepository.GetBlockCollection(assessmentResourceVersion.EndGuidanceId);

            var validator = new AssessmentValidator();
            var viewModel = this.mapper.Map<AssessmentViewModel>(assessmentResourceVersion);
            var validation = await validator.ValidateAsync(viewModel);
            var result = new LearningHubValidationResult(validation);
            if (!result.IsValid)
            {
                return result;
            }

            return await this.ValidateQuestionsInBlockCollection(assessmentResourceVersion.AssessmentContent);
        }

        private async Task<LearningHubValidationResult> ValidateQuestionsInBlockCollection(BlockCollection blockCollection)
        {
            BlockCollectionViewModel viewModel = this.mapper.Map<BlockCollectionViewModel>(blockCollection);
            if (viewModel != null)
            {
                return await QuestionValidationHelper.Validate(viewModel);
            }

            return new LearningHubValidationResult(true);
        }

        private async Task<LearningHubValidationResult> ValidateQuestionsInBlockCollection(int? blockCollectionId)
        {
            var blockCollection = await this.blockCollectionRepository.GetBlockCollection(blockCollectionId);
            return await this.ValidateQuestionsInBlockCollection(blockCollection);
        }

        /// <summary>
        /// The create generic file details async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="newFile">The newFile<see cref="File"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task CreateGenericFileDetailsAsync(int resourceVersionId, File newFile, int userId)
        {
            int fileId = await this.fileRepository.CreateAsync(userId, newFile);
            try
            {
                GenericFileResourceVersion genericFileResourceVersion = await this.genericFileResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId, true);

                if (genericFileResourceVersion == null)
                {
                    genericFileResourceVersion = new GenericFileResourceVersion()
                    {
                        ResourceVersionId = resourceVersionId,
                        FileId = fileId,
                    };
                    await this.genericFileResourceVersionRepository.CreateAsync(userId, genericFileResourceVersion);
                }
                else
                {
                    genericFileResourceVersion.File = null;
                    genericFileResourceVersion.FileId = fileId;
                    genericFileResourceVersion.ScormAiccContent = false;
                    genericFileResourceVersion.AuthoredYear = null;
                    genericFileResourceVersion.AuthoredMonth = null;
                    genericFileResourceVersion.AuthoredDayOfMonth = null;
                    genericFileResourceVersion.Deleted = false;
                    await this.genericFileResourceVersionRepository.UpdateAsync(userId, genericFileResourceVersion);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// The create scorm file details async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="newFile">The newFile<see cref="File"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task CreateScormFileDetailsAsync(int resourceVersionId, File newFile, int userId)
        {
            int fileId = await this.fileRepository.CreateAsync(userId, newFile);
            try
            {
                ScormResourceVersion scormResourceVersion = await this.scormResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId, true);

                scormResourceVersion.File = null;
                scormResourceVersion.FileId = fileId;
                scormResourceVersion.ScormResourceVersionManifest = null;
                scormResourceVersion.Deleted = false;
                await this.scormResourceVersionRepository.UpdateAsync(userId, scormResourceVersion);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// The create image details async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="newFile">The newFile<see cref="File"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task CreateImageDetailsAsync(int resourceVersionId, File newFile, int userId)
        {
            int fileId = await this.fileRepository.CreateAsync(userId, newFile);
            ImageResourceVersion imageResourceVersion = await this.imageResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId, true);

            if (imageResourceVersion == null)
            {
                imageResourceVersion = new ImageResourceVersion()
                {
                    ResourceVersionId = resourceVersionId,
                    ImageFileId = fileId,
                };
                await this.imageResourceVersionRepository.CreateAsync(userId, imageResourceVersion);
            }
            else
            {
                imageResourceVersion.File = null;
                imageResourceVersion.ImageFileId = fileId;
                imageResourceVersion.AltTag = null;
                imageResourceVersion.Description = null;
                imageResourceVersion.Deleted = false;
                await this.imageResourceVersionRepository.UpdateAsync(userId, imageResourceVersion);
            }
        }

        /// <summary>
        /// The create html details async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="newFile">The newFile<see cref="File"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task CreateHtmlDetailsAsync(int resourceVersionId, File newFile, int userId)
        {
            int fileId = await this.fileRepository.CreateAsync(userId, newFile);
            var htmlResourceVersion = await this.htmlResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId, true);

            if (htmlResourceVersion == null)
            {
                htmlResourceVersion = new HtmlResourceVersion()
                {
                    ResourceVersionId = resourceVersionId,
                    FileId = fileId,
                };
                await this.htmlResourceVersionRepository.CreateAsync(userId, htmlResourceVersion);
            }
            else
            {
                htmlResourceVersion.File = null;
                htmlResourceVersion.FileId = fileId;
                htmlResourceVersion.Deleted = false;
                await this.htmlResourceVersionRepository.UpdateAsync(userId, htmlResourceVersion);
            }
        }

        /// <summary>
        /// The create video details async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="newFile">The newFile<see cref="File"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task CreateVideoDetailsAsync(int resourceVersionId, File newFile, int userId)
        {
            int fileId = await this.fileRepository.CreateAsync(userId, newFile);
            VideoResourceVersion videoResourceVersion = await this.videoResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId, true);

            if (videoResourceVersion == null)
            {
                videoResourceVersion = new VideoResourceVersion()
                {
                    ResourceVersionId = resourceVersionId,
                    VideoFileId = fileId,
                };
                await this.videoResourceVersionRepository.CreateAsync(userId, videoResourceVersion);
            }
            else
            {
                videoResourceVersion.File = null;
                videoResourceVersion.VideoFileId = fileId;
                videoResourceVersion.TranscriptFile = null;
                videoResourceVersion.TranscriptFileId = null;
                videoResourceVersion.ClosedCaptionsFile = null;
                videoResourceVersion.ClosedCaptionsFileId = null;
                videoResourceVersion.DurationInMilliseconds = null;
                videoResourceVersion.Deleted = false;
                await this.videoResourceVersionRepository.UpdateAsync(userId, videoResourceVersion);
            }
        }

        /// <summary>
        /// The create audio details async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="newFile">The newFile<see cref="File"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task CreateAudioDetailsAsync(int resourceVersionId, File newFile, int userId)
        {
            int fileId = await this.fileRepository.CreateAsync(userId, newFile);
            AudioResourceVersion audioResourceVersion = await this.audioResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId, true);

            if (audioResourceVersion == null)
            {
                audioResourceVersion = new AudioResourceVersion()
                {
                    ResourceVersionId = resourceVersionId,
                    AudioFileId = fileId,
                };
                await this.audioResourceVersionRepository.CreateAsync(userId, audioResourceVersion);
            }
            else
            {
                audioResourceVersion.File = null;
                audioResourceVersion.AudioFileId = fileId;
                audioResourceVersion.TranscriptFile = null;
                audioResourceVersion.TranscriptFileId = null;
                audioResourceVersion.DurationInMilliseconds = null;
                audioResourceVersion.Deleted = false;
                await this.audioResourceVersionRepository.UpdateAsync(userId, audioResourceVersion);
            }
        }

    }
}
