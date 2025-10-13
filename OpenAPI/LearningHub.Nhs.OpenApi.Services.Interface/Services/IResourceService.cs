namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Admin;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using FileChunkDetailViewModel = LearningHub.Nhs.Models.Resource.Contribute.FileChunkDetailViewModel;
    using FileCreateRequestViewModel = LearningHub.Nhs.Models.Resource.Contribute.FileCreateRequestViewModel;
    using FileDeleteRequestModel = LearningHub.Nhs.Models.Resource.Contribute.FileDeleteRequestModel;

    /// <summary>
    /// The ResourceService interface.
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// The get all resource licences async.
        /// </summary>
        /// <returns>The ResourceLicenceViewModel list.</returns>
        Task<List<ResourceLicenceViewModel>> GetResourceLicencesAsync();

        /// <summary>
        /// The get resource version view model by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionViewModel}"/>.</returns>
        Task<ResourceVersionViewModel> GetResourceVersionViewModelAsync(int resourceVersionId);

        /// <summary>
        /// The get Resource version view model by video file Id async.
        /// </summary>
        /// <param name="fileId">The video fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionViewModel}"/>.</returns>
        Task<ResourceVersionViewModel> GetResourceVersionForVideoAsync(int fileId);
        /// <summary>
        /// The get Resource version view model by whole slide image file Id async.
        /// </summary>
        /// <param name="fileId">The video fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionViewModel}"/>.</returns>
        Task<ResourceVersionViewModel> GetResourceVersionForWholeSlideImageAsync(int fileId);

        /// <summary>
        /// The get resource version view model by resource reference id async.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersionViewModel> GetResourceVersionByResourceReferenceAsync(int resourceReferenceId);

        /// <summary>
        /// Get file directory for unpublished or deleted versions.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="deletedResource">.</param>
        /// <returns>The <see cref="List{String}"/>.</returns>
        Task<List<string>> GetObsoleteResourceFile(int resourceVersionId, bool deletedResource = false);

        /// <summary>
        /// The unpublish resource version.
        /// </summary>
        /// <param name="unpublishViewModel">The unpublishViewModel<see cref="UnpublishViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns><see cref="Task"/>LearningHubValidationResult.</returns>
        Task<LearningHubValidationResult> UnpublishResourceVersion(UnpublishViewModel unpublishViewModel, int userId);

        /// <summary>
        /// The revert publishing resource version to draft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> RevertToDraft(int resourceVersionId, int userId);

        /// <summary>
        /// The get embedded resource version by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> AcceptSensitiveContentAsync(int resourceVersionId, int currentUserId);

        /// <summary>
        /// The get resource versions.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionViewModel}"/>.</returns>
        Task<List<ResourceVersionViewModel>> GetResourceVersionsAsync(int resourceId);


        /// <summary>
        /// The get video details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{VideoViewModel}"/>.</returns>
        Task<VideoViewModel> GetVideoDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The get audio details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{AudioViewModel}"/>.</returns>
        Task<AudioViewModel> GetAudioDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The get article details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ArticleViewModel}"/>.</returns>
        Task<ArticleViewModel> GetArticleDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The add resource version keyword async.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="ResourceKeywordViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> AddResourceVersionKeywordAsync(ResourceKeywordViewModel resourceKeywordViewModel, int userId);


        /// <summary>
        /// The GetResourceVersionValidationResultAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionEventViewModel}"/>.</returns>
        Task<ResourceVersionValidationResultViewModel> GetResourceVersionValidationResultAsync(int resourceVersionId);

        /// <summary>
        /// The get resource version flags async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionFlagViewModel}"/>.</returns>
        Task<List<ResourceVersionFlagViewModel>> GetResourceVersionFlagsAsync(int resourceVersionId);


        /// <summary>
        /// Retrieves the entire assessment details given a resource version ID.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentViewModel> GetAssessmentDetailsByIdAsync(int resourceVersionId, int currentUserId);

        /// <summary>
        /// Retrieves the assessment details up to the first question, leaving out the feedback and answer types nullified.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentViewModel> GetInitialAssessmentContent(int resourceVersionId);

        /// <summary>
        /// Retrieves the assessment progress.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="numberOfAttempts">The number of attempts made.</param>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <param name="assessmentResourceActivitiesWithAnswers">The assessment resource activities.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentProgressViewModel> GetAssessmentProgress(
            int resourceVersionId,
            int numberOfAttempts,
            int assessmentResourceActivityId,
            Dictionary<int, Dictionary<int, IEnumerable<int>>> assessmentResourceActivitiesWithAnswers);

        /// <summary>
        /// The get resource information view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceInformationViewModel}"/>.</returns>
        Task<ResourceInformationViewModel> GetResourceInformationViewModelAsync(int resourceReferenceId, int userId);

        /// <summary>
        /// The get resource item view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="readOnly">The readOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{ResourceItemViewModel}"/>.</returns>
        Task<ResourceItemViewModel> GetResourceItemViewModelAsync(int resourceReferenceId, int userId, bool readOnly);


        /// <summary>
        /// The get my contributions view model async.
        /// </summary>
        /// <param name="catalogueId">The catalogueId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="MyContributionsTotalsViewModel"/>.</returns>
        MyContributionsTotalsViewModel GetMyContributionTotals(int catalogueId, int userId);

        /// <summary>
        /// The get resource card extended view model async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceCardExtendedViewModel}"/>.</returns>
        Task<ResourceCardExtendedViewModel> GetResourceCardExtendedViewModelAsync(int resourceVersionId, int userId);

        /// <summary>
        /// The get my contributions view model async.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="resourceContributionsRequestViewModel">The resourceContributionsRequestViewModel<see cref="ResourceContributionsRequestViewModel"/>.</param>
        /// <param name="readOnly">The readOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="List{ContributedResourceCardViewModel}"/>.</returns>
        List<ContributedResourceCardViewModel> GetContributions(int userId, ResourceContributionsRequestViewModel resourceContributionsRequestViewModel, bool readOnly);

        /// <summary>
        /// The get my contributions view model async.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="myContributionsRequestViewModel">The myContributionsRequestViewModel<see cref="MyContributionsRequestViewModel"/>.</param>
        /// <returns>The <see cref="List{MyContributionsBasicDetailsViewModel}"/>.</returns>
        List<MyContributionsBasicDetailsViewModel> GetMyContributions(int userId, MyContributionsRequestViewModel myContributionsRequestViewModel);

        /// <summary>
        /// The get my resource view model async.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{MyResourceViewModel}"/>.</returns>
        Task<MyResourceViewModel> GetMyResourceViewModelAsync(int userId);

        /// <summary>
        /// The has published resources method.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>If the user has published resources.</returns>
        Task<bool> HasPublishedResourcesAsync(int userId);

        /// <summary>
        /// The get resource version by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceDetailViewModel}"/>.</returns>
        Task<ResourceDetailViewModel> GetResourceVersionByIdAsync(int resourceVersionId);

        /// <summary>
        /// The get resource by activityStatusIds async.
        /// </summary>
        /// <param name="activityStatusIds">activityStatusIds.</param>
        /// <param name="currentUserId">c.</param>
        /// <returns>The <see cref="Task"/>the resourceMetaDataViewModel corresponding to the resource reference.</returns>
        Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferenceByActivityStatus(List<int> activityStatusIds, int currentUserId);

        /// <summary>
        /// The get resource by id async.
        /// </summary>
        /// <param name="originalResourceReferenceId">The original resource reference id.</param>
        /// <param name="currentUserId">.</param>
        /// <returns>The <see cref="Task"/>the resourceMetaDataViewModel corresponding to the resource reference.</returns>
        Task<ResourceReferenceWithResourceDetailsViewModel> GetResourceReferenceByOriginalId(int originalResourceReferenceId, int? currentUserId);

        /// <summary>
        /// The get resource references for certificates
        /// </summary>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The ResourceReferenceWithResourceDetailsViewModel<see cref="Task"/>the resourceMetaDataViewModel corresponding to the resource reference.</returns>
        Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferencesForCertificates(int currentUserId);

        /// <summary>
        /// The get resources by Ids endpoint.
        /// </summary>
        /// <param name="originalResourceReferenceIds">The original resource reference Ids.</param>
        /// <param name="currentUserId">.</param>
        /// <returns><see cref="Task"/>The resourceReferenceMetaDataViewModel.</returns>
        Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds(List<int> originalResourceReferenceIds, int? currentUserId);


        /// <summary>
        /// The get resource by id async.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Resource}"/>.</returns>
        Task<Resource> GetResourceByIdAsync(int id);

        /// <summary>
        /// The get generic file details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{GenericFileViewModel}"/>.</returns>
        Task<GenericFileViewModel> GetGenericFileDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The get html resource details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{HtmlViewModel}"/>.</returns>
        Task<HtmlResourceViewModel> GetHtmlDetailsByIdAsync(int resourceVersionId);


        /// <summary>
        /// The get scorm details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ScormViewModel}"/>.</returns>
        Task<ScormViewModel> GetScormDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The get web link resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{WebLinkViewModel}"/>.</returns>
        Task<WebLinkViewModel> GetWebLinkDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The get case resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CaseViewModel> GetCaseDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetExternalContentDetails.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">userId.</param>
        /// <returns>The <see cref="Task{ExternalContentDetailsViewModel}"/>.</returns>
        Task<ExternalContentDetailsViewModel> GetExternalContentDetails(int resourceVersionId, int userId);

        /// <summary>
        /// The get case resource version async.
        /// </summary>
        /// <param name="excludeResourceVersionId">The resource version id.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>The <see cref="List{T}"/>.</returns>
        Task<List<string>> GetResourceBlockCollectionsFilePathAsync(int excludeResourceVersionId, ResourceTypeEnum resourceType);

        /// <summary>
        /// The GetFileStatusDetailsAsync.
        /// </summary>
        /// <param name="fileIds">The File Ids.</param>
        /// <returns>The files.</returns>
        Task<List<FileViewModel>> GetFileStatusDetailsAsync(int[] fileIds);

        /// <summary>
        /// The get file async.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{File}"/>.</returns>
        Task<File> GetFileAsync(int fileId);

        /// <summary>
        /// The get image details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ImageViewModel}"/>.</returns>
        Task<ImageViewModel> GetImageDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The create file details for an article async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateArticleAttachedFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId);

        /// <summary>
        /// Create file details for a audio attribute file async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateAudioAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId);

        /// <summary>
        /// Create file details for a video attribute file async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateVideoAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId);

        /// <summary>
        /// Duplicate blocks.
        /// </summary>
        /// <param name="requestModel">The DuplicateBlocksRequestModel.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DuplicateBlocks(DuplicateBlocksRequestModel requestModel, int userId);

        /// <summary>
        /// The create resource async.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateResourceAsync(ResourceDetailViewModel resourceDetailViewModel, int userId);

        /// <summary>
        /// The duplicate resource async.
        /// </summary>
        /// <param name="duplicateResourceRequestModel">The duplicateResourceRequestModel<see cref="DuplicateResourceRequestModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DuplicateResourceAsync(DuplicateResourceRequestModel duplicateResourceRequestModel, int userId);

        /// <summary>
        /// The update article detail async.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateNewResourceVersionAsync(int resourceId, int currentUserId);

        /// <summary>
        /// The create resource version validation result async.
        /// </summary>
        /// <param name="validationResultViewModel">The validationResultViewModel<see cref="ResourceVersionValidationResultViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateResourceVersionValidationResultAsync(ResourceVersionValidationResultViewModel validationResultViewModel);

        /// <summary>
        /// The get resource header view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceHeaderViewModel}"/>.</returns>
        Task<ResourceHeaderViewModel> GetResourceHeaderViewModelAsync(int resourceReferenceId);

        /// <summary>
        /// The set resource type.
        /// </summary>
        /// <param name="resourceViewModel">The resourceViewModel<see cref="ResourceViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        void SetResourceType(ResourceViewModel resourceViewModel, int userId);


        /// <summary>
        /// The publishing media resource version.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SubmitResourceVersionForPublish(PublishViewModel publishViewModel);

        /// <summary>
        /// Submits a published resource version to the Findwise search.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<(bool success, int resourceReferenceId)> SubmitResourceVersionToSearchAsync(int resourceVersionId, int userId);


        /// <summary>
        /// The update resource version async.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateResourceVersionAsync(ResourceDetailViewModel resourceDetailViewModel, int userId);

        /// <summary>
        /// Delete resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionAsync(int resourceVersionId, int userId);


        /// <summary>
        /// The update generic file detail async.
        /// </summary>
        /// <param name="genericFileViewModel">The genericFileViewModel<see cref="GenericFileUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateGenericFileDetailAsync(GenericFileUpdateRequestViewModel genericFileViewModel, int currentUserId);

        /// <summary>
        /// The update scorm detail async.
        /// </summary>
        /// <param name="scormViewModel">The ScormUpdateRequestViewModel<see cref="ScormUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateScormDetailAsync(ScormUpdateRequestViewModel scormViewModel, int currentUserId);

        /// <summary>
        /// The update HTML detail async.
        /// </summary>
        /// <param name="htmlResourceViewModel">Html resource update view model.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateHtmlDetailAsync(HtmlResourceUpdateRequestViewModel htmlResourceViewModel, int currentUserId);

        /// <summary>
        /// The update image detail async.
        /// </summary>
        /// <param name="imageViewModel">The imageViewModel<see cref="ImageUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateImageDetailAsync(ImageUpdateRequestViewModel imageViewModel, int currentUserId);

        /// <summary>
        /// The update video detail async.
        /// </summary>
        /// <param name="videoViewModel">The videoViewModel<see cref="VideoUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateVideoDetailAsync(VideoUpdateRequestViewModel videoViewModel, int currentUserId);

        /// <summary>
        /// The delete audio attribute file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteAudioAttributeFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId);

        /// <summary>
        /// The delete video attribute file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteVideoAttributeFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId);

        /// <summary>
        /// The update audio detail async.
        /// </summary>
        /// <param name="audioViewModel">The audioViewModel<see cref="AudioUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateAudioDetailAsync(AudioUpdateRequestViewModel audioViewModel, int currentUserId);

        /// <summary>
        /// The update article detail async.
        /// </summary>
        /// <param name="articleViewModel">The articleViewModel<see cref="ArticleUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateArticleDetailAsync(ArticleUpdateRequestViewModel articleViewModel, int currentUserId);

        /// <summary>
        /// The delete article file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteArticleFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId);

        /// <summary>
        /// The add resource version author async.
        /// </summary>
        /// <param name="resourceAuthorViewModel">The resourceAuthorViewModel<see cref="ResourceAuthorViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> AddResourceVersionAuthorAsync(ResourceAuthorViewModel resourceAuthorViewModel, int userId);

        /// <summary>
        /// The delete resource version author async.
        /// </summary>
        /// <param name="resourceAuthorViewModel">The resourceAuthorViewModel<see cref="AuthorDeleteRequestModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionAuthorAsync(AuthorDeleteRequestModel resourceAuthorViewModel, int userId);

        /// <summary>
        /// The delete resource version keyword async.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="KeywordDeleteRequestModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionKeywordAsync(KeywordDeleteRequestModel resourceKeywordViewModel, int userId);


        /// <summary>
        /// The save resource version flag async.
        /// </summary>
        /// <param name="resourceVersionFlagViewModel">The resourceVersionFlagViewModel<see cref="ResourceVersionFlagViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SaveResourceVersionFlagAsync(
            ResourceVersionFlagViewModel resourceVersionFlagViewModel,
            int userId);

        /// <summary>
        /// The delete resource version flag async.
        /// </summary>
        /// <param name="resourceVersionFlagId">The resourceVersionFlagId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteResourceVersionFlagAsync(int resourceVersionFlagId, int userId);

        /// <summary>
        /// Returns a list of basic resource info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="currentUserId">The userId<see cref="int"/>.</param>
        /// <param name="pagingRequestModel">The request detail <see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{ResourceAdminSearchResultViewModel}"/>.</returns>
        Task<PagedResultSet<ResourceAdminSearchResultViewModel>> GetResourceAdminSearchFilteredPageAsync(int currentUserId, PagingRequestModel pagingRequestModel);

        /// <summary>
        /// Transfer Resource Ownership.
        /// </summary>
        /// <param name="transferResourceOwnershipViewModel">The transferResourceOwnershipViewModel<see cref="TransferResourceOwnershipViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> TransferResourceOwnership(TransferResourceOwnershipViewModel transferResourceOwnershipViewModel, int userId);

        /// <summary>
        /// The update web link resource version async.
        /// </summary>
        /// <param name="webLinkViewModel">The webLinkViewModel<see cref="WebLinkViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateWebLinkResourceVersionAsync(WebLinkViewModel webLinkViewModel, int userId);

        /// <summary>
        /// The update case resource version async.
        /// </summary>
        /// <param name="caseViewModel">The case view model.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateCaseResourceVersionAsync(CaseViewModel caseViewModel, int userId);

        /// <summary>
        /// This method updates the database entry with the assessment details, passed down as a parameter.
        /// </summary>
        /// <param name="assessmentViewModel">The assessment view model.</param>
        /// <param name="currentUserId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateAssessmentResourceVersionAsync(AssessmentViewModel assessmentViewModel, int currentUserId);


        /// <summary>
        /// The create file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">The fileChunkDetailCreateRequestViewModel<see cref="FileChunkDetailViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateFileChunkDetailAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel, int currentUserId);

        /// <summary>
        /// The get file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileChunkDetailViewModel}"/>.</returns>
        Task<FileChunkDetailViewModel> GetFileChunkDetailAsync(int fileChunkDetailId);

        /// <summary>
        /// The delete file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteFileChunkDetailAsync(int fileChunkDetailId, int currentUserId);


        /// <summary>
        /// The create file details async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId);

        /// <summary>
        /// The get resource version events async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionEventViewModel}"/>.</returns>
        Task<List<ResourceVersionEventViewModel>> GetResourceVersionEventsAsync(int resourceVersionId);

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionEventViewModel">The resourceVersionEventViewModel<see cref="ResourceVersionEventViewModel"/>.</param>
        void CreateResourceVersionEvent(ResourceVersionEventViewModel resourceVersionEventViewModel);

        /// <summary>
        /// The get extended resource version view model async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionExtendedViewModel}"/>.</returns>
        Task<ResourceVersionExtendedViewModel> GetResourceVersionExtendedViewModelAsync(int resourceVersionId, int userId);

        /// <summary>
        /// The GetResourceVersionDevIdDetailsAync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionDevIdViewModel}"/>.</returns>
        Task<ResourceVersionDevIdViewModel> GetResourceVersionDevIdDetailsAync(int resourceVersionId);

        /// <summary>
        /// The get resource version dev id async.
        /// </summary>
        /// <param name="devId">The devId<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        Task<bool> DoesDevIdExistsAync(string devId);

        /// <summary>
        /// Update Dev Id details.
        /// </summary>
        /// <param name="resourceVersionDevIdViewModel">The resourceVersionDevIdViewModel.</param>
        /// <param name="currentUserId">The currentUserId.</param>
        /// <returns>The <see cref="List{ResourceVersionDevIdViewModel}"/>.</returns>
        Task UpdateDevIdDetailsAsync(ResourceVersionDevIdViewModel resourceVersionDevIdViewModel, int currentUserId);

    }
}
