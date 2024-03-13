namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Contribute;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// Defines the <see cref="IResourceService" />.
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// The AcceptSensitiveContentAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> AcceptSensitiveContentAsync(int id);

        /// <summary>
        /// The GetArticleDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ArticleViewModel> GetArticleDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetAudioDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<AudioViewModel> GetAudioDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResourceHeaderViewModel> GetByIdAsync(int id);

        /// <summary>
        /// The GetFileTypeAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<FileTypeViewModel>> GetFileTypeAsync();

        /// <summary>
        /// The GetGenericFileDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<GenericFileViewModel> GetGenericFileDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetHtmlDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HtmlResourceViewModel> GetHtmlDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetScormDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ScormViewModel> GetScormDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetExternalContentDetailsAsync.
        /// </summary>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ExternalContentDetailsViewModel> GetExternalContentDetailsAsync(int resourceVersionId);

        /// <summary>
        /// The RecordExternalReferenceUserAgreementAsync.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> RecordExternalReferenceUserAgreementAsync(ExternalReferenceUserAgreementViewModel model);

        /// <summary>
        /// The GetImageDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ImageViewModel> GetImageDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetInformationByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResourceInformationViewModel> GetInformationByIdAsync(int id);

        /// <summary>
        /// The GetItemByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResourceItemViewModel> GetItemByIdAsync(int id);

        /// <summary>
        /// The GetLicencesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ResourceLicenceViewModel>> GetLicencesAsync();

        /// <summary>
        /// The GetLocationsByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CatalogueLocationsViewModel> GetLocationsByIdAsync(int id);

        /// <summary>
        /// The GetResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResourceDetailViewModel> GetResourceVersionAsync(int resourceVersionId);

        /// <summary>
        /// The GetResourceVersionViewModelAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResourceVersionViewModel> GetResourceVersionViewModelAsync(int resourceVersionId);

        /// <summary>
        /// The GetResourceVersionExtendedAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceDetailViewModel}"/>.</returns>
        Task<ResourceVersionExtendedViewModel> GetResourceVersionExtendedAsync(int resourceVersionId);

        /// <summary>
        /// The GetVersionHistoryByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResourceVersionHistoryViewModel> GetVersionHistoryByIdAsync(int id);

        /// <summary>
        /// The GetVideoDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<VideoViewModel> GetVideoDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetVideoFileContentAuthenticationTokenAsync.
        /// </summary>
        /// <param name="assetId">The assetId.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        Task<string> GetVideoFileContentAuthenticationTokenAsync(string assetId);

        /// <summary>
        /// The GetWeblinkDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<WebLinkViewModel> GetWeblinkDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetCaseDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CaseViewModel> GetCaseDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetAssessmentDetailsByIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<AssessmentViewModel> GetAssessmentDetailsByIdAsync(int resourceVersionId);

        /// <summary>
        /// The GetAssessmentContent.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The task.</returns>
        Task<AssessmentViewModel> GetAssessmentContent(int resourceVersionId);

        /// <summary>
        /// Retrieves the latest assessment progress by a given assessment activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentProgressViewModel> GetAssessmentProgressByActivity(int assessmentResourceActivityId);

        /// <summary>
        /// Retrieves the latest assessment progress by the resource version id.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentProgressViewModel> GetAssessmentProgressByResourceVersion(int resourceVersionId);

        /// <summary>
        /// The GetFileStatusDetailsAsync.
        /// </summary>
        /// <param name="fileIds">The File Ids.</param>
        /// <returns>The files.</returns>
        Task<List<FileViewModel>> GetFileStatusDetailsAsync(int[] fileIds);

        /// <summary>
        /// The UnpublishResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">Resource version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> UnpublishResourceVersionAsync(int resourceVersionId);

        /// <summary>
        /// The UserHasPublishedResourcesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UserHasPublishedResourcesAsync();

        /// <summary>
        /// The DuplicateResourceAsync.
        /// </summary>
        /// <param name="model">The DuplicateResourceRequestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> DuplicateResourceAsync(DuplicateResourceRequestModel model);

        /// <summary>
        /// Get All published resources id.
        /// </summary>
        /// <returns>The <see cref="T:Task{IEnumerable{int}}"/>.</returns>
        Task<IEnumerable<int>> GetAllPublishedResourceAsync();

        /// <summary>
        /// The DuplicateBlocksAsync.
        /// </summary>
        /// <param name="model">The duplicateBlocksRequestModel<see cref="DuplicateBlocksRequestModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> DuplicateBlocksAsync(DuplicateBlocksRequestModel model);

        /// <summary>
        /// The GetMyContributionsAsync.
        /// </summary>
        /// <param name="model">The myContributionsRequestViewModel<see cref="MyContributionsRequestViewModel"/>.</param>
        /// <returns>The <see cref="T:Task{IEnumerable{MyContributionsBasicDetailsViewModel}}"/>.</returns>
        Task<IEnumerable<MyContributionsBasicDetailsViewModel>> GetMyContributionsAsync(MyContributionsRequestViewModel model);

        /// <summary>
        /// The GetResourceVersionValidationResultAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionEventViewModel}}"/>.</returns>
        Task<ResourceVersionValidationResultViewModel> GetResourceVersionValidationResultAsync(int resourceVersionId);

        /// <summary>
        /// The RevertToDraft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> RevertToDraft(int resourceVersionId);

        /// <summary>
        /// The create resource provider.
        /// </summary>
        /// <param name="model">The model<see cref="ResourceVersionProviderViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateResourceVersionProviderAsync(ResourceVersionProviderViewModel model);

        /// <summary>
        /// Delete resource version provider.
        /// </summary>
        /// <param name="model">The model<see cref="ResourceVersionProviderViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> DeleteResourceVersionProviderAsync(ResourceVersionProviderViewModel model);

        /// <summary>
        /// The delete all resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> DeleteAllResourceVersionProviderAsync(int resourceVersionId);
    }
}
