namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Admin;
    using LearningHub.Nhs.Models.Resource.AzureMediaAsset;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.Validation;

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
        /// The get resource by id async.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Resource}"/>.</returns>
        Task<Resource> GetResourceByIdAsync(int id);

        /// <summary>
        /// The get resource version by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceDetailViewModel}"/>.</returns>
        Task<ResourceDetailViewModel> GetResourceVersionByIdAsync(int resourceVersionId);

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
        /// The get extended resource version view model async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionExtendedViewModel}"/>.</returns>
        Task<ResourceVersionExtendedViewModel> GetResourceVersionExtendedViewModelAsync(int resourceVersionId, int userId);

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
        /// The GetExternalContentDetails.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">userId.</param>
        /// <returns>The <see cref="Task{ScormContentDetailsViewModel}"/>.</returns>
        ExternalContentDetailsViewModel GetExternalContentDetails(int resourceVersionId, int userId);

        /// <summary>
        /// The get image details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ImageViewModel}"/>.</returns>
        Task<ImageViewModel> GetImageDetailsByIdAsync(int resourceVersionId);

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
        /// The set resource type.
        /// </summary>
        /// <param name="resourceViewModel">The resourceViewModel<see cref="ResourceViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        void SetResourceType(ResourceViewModel resourceViewModel, int userId);

        /// <summary>
        /// The publish resource version.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> PublishResourceVersionAsync(PublishViewModel publishViewModel);

        /// <summary>
        /// Submits a published resource version to the Findwise search.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<(bool success, int resourceReferenceId)> SubmitResourceVersionToSearchAsync(int resourceVersionId, int userId);

        /// <summary>
        /// Submit resource version for prepare (pre-publish).
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SubmitResourceVersionForPrepare(PublishViewModel publishViewModel);

        /// <summary>
        /// The publishing media resource version.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SubmitResourceVersionForPublish(PublishViewModel publishViewModel);

        /// <summary>
        /// Set resource version to publishing status.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        LearningHubValidationResult SetResourceVersionPublishing(PublishViewModel publishViewModel);

        /// <summary>
        /// Set resource version to "failed to publish" status.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SetResourceVersionFailedToPublish(PublishViewModel publishViewModel);

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
        /// The update resource version async.
        /// </summary>
        /// <param name="resourceDetailViewModel">The resourceDetailViewModel<see cref="ResourceDetailViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateResourceVersionAsync(ResourceDetailViewModel resourceDetailViewModel, int userId);

        /// <summary>
        /// The get resource versions.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionViewModel}"/>.</returns>
        Task<List<ResourceVersionViewModel>> GetResourceVersionsAsync(int resourceId);

        /// <summary>
        /// Delete resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionAsync(int resourceVersionId, int userId);

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionEventViewModel">The resourceVersionEventViewModel<see cref="ResourceVersionEventViewModel"/>.</param>
        void CreateResourceVersionEvent(ResourceVersionEventViewModel resourceVersionEventViewModel);

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
        /// The publish update scorm detail async.
        /// </summary>
        /// <param name="scormViewModel">The scormPublishUpdateRequestViewModel<see cref="ScormPublishUpdateViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateScormPublishDetailAsync(ScormPublishUpdateViewModel scormViewModel);

        /// <summary>
        /// The publish update html resource detail async.
        /// </summary>
        /// <param name="viewModel">The HtmlResourcePublishUpdateViewModel<see cref="HtmlResourcePublishUpdateViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateHtmlResourcePublishDetailsAsync(HtmlResourcePublishUpdateViewModel viewModel);

        /// <summary>
        /// The RecordExternalReferenceUserAgreementAsync.
        /// </summary>
        /// <param name="viewModel">viewModel.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> RecordExternalReferenceUserAgreementAsync(ExternalReferenceUserAgreementViewModel viewModel, int currentUserId);

        /// <summary>
        /// The update image detail async.
        /// </summary>
        /// <param name="imageViewModel">The imageViewModel<see cref="ImageUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateImageDetailAsync(ImageUpdateRequestViewModel imageViewModel, int currentUserId);

        /// <summary>
        /// The publication is marked as submitted to search.
        /// </summary>
        /// <param name="publicationId">The publicationId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        void ConfirmSubmissionToSearch(int publicationId, int currentUserId);

        /// <summary>
        /// The update video detail async.
        /// </summary>
        /// <param name="videoViewModel">The videoViewModel<see cref="VideoUpdateRequestViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateVideoDetailAsync(VideoUpdateRequestViewModel videoViewModel, int currentUserId);

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
        /// The update article detail async.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateNewResourceVersionAsync(int resourceId, int currentUserId);

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
        /// The add resource version keyword async.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="ResourceKeywordViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> AddResourceVersionKeywordAsync(ResourceKeywordViewModel resourceKeywordViewModel, int userId);

        /// <summary>
        /// The delete resource version keyword async.
        /// </summary>
        /// <param name="resourceKeywordViewModel">The resourceKeywordViewModel<see cref="KeywordDeleteRequestModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionKeywordAsync(KeywordDeleteRequestModel resourceKeywordViewModel, int userId);

        /// <summary>
        /// The get embedded resource version by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{EmbedCodeViewModel}"/>.</returns>
        Task<EmbedCodeViewModel> GetEmbeddedResourceVersionByIdAsync(int resourceVersionId);

        /// <summary>
        /// The get embedded resource version by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> AcceptSensitiveContentAsync(int resourceVersionId, int currentUserId);

        /// <summary>
        /// The get equipment details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{EquipmentViewModel}"/>.</returns>
        Task<EquipmentViewModel> GetEquipmentDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The update embedded resource version async.
        /// </summary>
        /// <param name="embedCodeViewModel">The embedCodeViewModel<see cref="EmbedCodeViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateEmbeddedResourceVersionAsync(EmbedCodeViewModel embedCodeViewModel, int userId);

        /// <summary>
        /// The get web link resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{WebLinkViewModel}"/>.</returns>
        Task<WebLinkViewModel> GetWebLinkDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The update web link resource version async.
        /// </summary>
        /// <param name="webLinkViewModel">The webLinkViewModel<see cref="WebLinkViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateWebLinkResourceVersionAsync(WebLinkViewModel webLinkViewModel, int userId);

        /// <summary>
        /// The get case resource version async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CaseViewModel> GetCaseDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The update case resource version async.
        /// </summary>
        /// <param name="caseViewModel">The case view model.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateCaseResourceVersionAsync(CaseViewModel caseViewModel, int userId);

        /// <summary>
        /// Retrieves the entire assessment details given a resource version ID.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentViewModel> GetAssessmentDetailsByIdAsync(int resourceVersionId, int currentUserId);

        /// <summary>
        /// Retrieves the basic assessment details given a resource version ID, i.e. the content of the AssessmentResourceVersion table only.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentResourceVersion> GetAssessmentBasicDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// This method updates the database entry with the assessment details, passed down as a parameter.
        /// </summary>
        /// <param name="assessmentViewModel">The assessment view model.</param>
        /// <param name="currentUserId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateAssessmentResourceVersionAsync(AssessmentViewModel assessmentViewModel, int currentUserId);

        /// <summary>
        /// Retrieves the assessment details up to a given question, leaving out the feedback and answer types nullified.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="questionNumber">The question number.</param>
        /// <param name="answerOrders">The orders of the answers submitted.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentViewModel> GetAssessmentContentUpToQuestion(int resourceVersionId, int questionNumber, List<int> answerOrders);

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
        /// The GetFileStatusDetailsAsync.
        /// </summary>
        /// <param name="fileIds">The File Ids.</param>
        /// <returns>The files.</returns>
        Task<List<FileViewModel>> GetFileStatusDetailsAsync(int[] fileIds);

        /// <summary>
        /// The get file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailId">The fileChunkDetailId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{FileChunkDetailViewModel}"/>.</returns>
        Task<FileChunkDetailViewModel> GetFileChunkDetailAsync(int fileChunkDetailId);

        /// <summary>
        /// The get file async.
        /// </summary>
        /// <param name="fileId">The fileId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{File}"/>.</returns>
        Task<File> GetFileAsync(int fileId);

        /// <summary>
        /// The create file chunk detail async.
        /// </summary>
        /// <param name="fileChunkDetailCreateRequestViewModel">The fileChunkDetailCreateRequestViewModel<see cref="FileChunkDetailViewModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateFileChunkDetailAsync(FileChunkDetailViewModel fileChunkDetailCreateRequestViewModel, int currentUserId);

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
        /// The update file details async.
        /// </summary>
        /// <param name="fileUpdateRequestViewModel">The fileUpdateRequestViewModel<see cref="FileUpdateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateFileDetailsAsync(FileUpdateRequestViewModel fileUpdateRequestViewModel, int userId);

        /// <summary>
        /// The create file details for an article async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateArticleAttachedFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId);

        /// <summary>
        /// Create file details for a video attribute file async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateVideoAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId);

        /// <summary>
        /// The delete video attribute file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteVideoAttributeFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId);

        /// <summary>
        /// Create file details for a audio attribute file async.
        /// </summary>
        /// <param name="fileCreateRequestViewModel">The fileCreateRequestViewModel<see cref="FileCreateRequestViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateAudioAttributeFileDetailsAsync(FileCreateRequestViewModel fileCreateRequestViewModel, int userId);

        /// <summary>
        /// The delete audio attribute file async.
        /// </summary>
        /// <param name="fileDeleteRequestModel">The fileDeleteRequestModel<see cref="FileDeleteRequestModel"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteAudioAttributeFileAsync(FileDeleteRequestModel fileDeleteRequestModel, int currentUserId);

        /// <summary>
        /// The create scorm manifest detail async.
        /// </summary>
        /// <param name="scormManifestViewModel">The scormManifestViewModel<see cref="ScormManifestUpdateRequestViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SetScormManifestDetailAsync(ScormManifestUpdateRequestViewModel scormManifestViewModel);

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
        /// The get resource item view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="readOnly">The readOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{ResourceItemViewModel}"/>.</returns>
        Task<ResourceItemViewModel> GetResourceItemViewModelAsync(int resourceReferenceId, int userId, bool readOnly);

        /// <summary>
        /// The get resource information view model async.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceInformationViewModel}"/>.</returns>
        Task<ResourceInformationViewModel> GetResourceInformationViewModelAsync(int resourceReferenceId, int userId);

        /// <summary>
        /// The get my contributions view model async.
        /// </summary>
        /// <param name="catalogueId">The catalogueId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="MyContributionsTotalsViewModel"/>.</returns>
        MyContributionsTotalsViewModel GetMyContributionTotals(int catalogueId, int userId);

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
        /// The get resource card extended view model async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceCardExtendedViewModel}"/>.</returns>
        Task<ResourceCardExtendedViewModel> GetResourceCardExtendedViewModelAsync(int resourceVersionId, int userId);

        /// <summary>
        /// The get resource version events async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionEventViewModel}"/>.</returns>
        Task<List<ResourceVersionEventViewModel>> GetResourceVersionEventsAsync(int resourceVersionId);

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
        /// Get media asset view model by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{MediaAssetInputViewModel}"/>.</returns>
        Task<MediaAssetInputViewModel> GetMediaAssetInputViewModelAsync(int resourceVersionId);

        /// <summary>
        /// The create resource azure media asset.
        /// </summary>
        /// <param name="model">The model<see cref="MediaAssetOutputViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> CreateResourceAzureMediaAssetAsync(MediaAssetOutputViewModel model);

        /// <summary>
        /// The has published resources method.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>If the user has published resources.</returns>
        Task<bool> HasPublishedResourcesAsync(int userId);

        /// <summary>
        /// Get All published resources id.
        /// </summary>
        /// <returns>The <see cref="T:Task{IEnumerable{int}}"/>.</returns>
        Task<IEnumerable<int>> GetAllPublishedResourceAsync();

        /// <summary>
        /// Duplicate blocks.
        /// </summary>
        /// <param name="requestModel">The DuplicateBlocksRequestModel.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DuplicateBlocks(DuplicateBlocksRequestModel requestModel, int userId);

        /// <summary>
        /// The check and remove resource provider permission if user do not have permission to the provider.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task CheckAndRemoveResourceVersionProviderAsync(int resourceVersionId, int userId);

        /// <summary>
        /// The create resource provider.
        /// </summary>
        /// <param name="providerId">provider id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateResourceVersionProviderAsync(int providerId, int resourceVersionId, int userId);

        /// <summary>
        /// The delete resource provider.
        /// </summary>
        /// <param name="providerId">provider id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionProviderAsync(int providerId, int resourceVersionId, int userId);

        /// <summary>
        /// The delete all resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="userId">user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DeleteAllResourceVersionProviderAsync(int resourceVersionId, int userId);
    }
}
