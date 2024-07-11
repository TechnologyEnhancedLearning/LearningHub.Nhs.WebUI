namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
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
    }
}
