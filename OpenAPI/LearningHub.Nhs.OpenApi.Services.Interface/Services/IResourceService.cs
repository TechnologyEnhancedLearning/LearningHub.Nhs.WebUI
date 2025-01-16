namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// The ResourceService interface.
    /// </summary>
    public interface IResourceService
    {
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
    }
}
