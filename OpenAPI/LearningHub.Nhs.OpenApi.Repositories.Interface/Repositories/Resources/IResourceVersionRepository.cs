namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;

    /// <summary>
    /// The ResourceVersionRepository interface.
    /// </summary>
    public interface IResourceVersionRepository : IGenericRepository<ResourceVersion>
    {
        /// <summary>
        /// The get all admin search.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        IQueryable<ResourceVersion> GetAllAdminSearch(int userId);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeEvents">The include events.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetByIdAsync(int id, bool includeEvents);

        /// <summary>
        /// The get basic by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetBasicByIdAsync(int id);

        /// <summary>
        /// The contribution totals.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="catalogueId">The catalogue id.</param>
        /// <returns>The MyContributionsTotalsViewModel.</returns>
        MyContributionsTotalsViewModel GetMyContributionTotals(int userId, int catalogueId);

        /// <summary>
        /// Get resource cards.
        /// </summary>
        /// <param name="includeEvents">The include events.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<ResourceVersion>> GetResourceCards(bool includeEvents);

        /// <summary>
        /// The get current for resource async.
        /// </summary>
        /// <param name="resourceId">The resource Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetCurrentForResourceAsync(int resourceId);

        /// <summary>
        /// The get current resource for resourceid async.
        /// </summary>
        /// <param name="resourceId">The resource Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetCurrentResourceDetailsAsync(int resourceId);

        /// <summary>
        /// The get resource version details by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetByResourceVersionByIdAsync(int resourceVersionId);

        /// <summary>
        /// The check dev id already exists in the table async.
        /// </summary>
        /// <param name="devId">The devId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> DoesDevIdExistsAync(string devId);

        /// <summary>
        /// The get current published for resource async.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetCurrentPublicationForResourceAsync(int resourceId);

        /// <summary>
        /// The get current for resource reference id async.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetCurrentForResourceReferenceIdAsync(int resourceReferenceId);

        /// <summary>
        /// The get current resource for resource reference id async.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetCurrentResourceForResourceReferenceIdAsync(int resourceReferenceId);

        /// <summary>
        /// The get current published for resource reference id async.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersion> GetCurrentPublicationForResourceReferenceIdAsync(int resourceReferenceId);

        /// <summary>
        /// The set resource type.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="resourceTypeEnum">The resource type enum.</param>
        /// <param name="userId">The user id.</param>
        void SetResourceType(int resourceVersionId, ResourceTypeEnum resourceTypeEnum, int userId);

        /// <summary>
        /// Gets the resource type of a ResourceVersion.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceTypeEnum> GetResourceType(int resourceVersionId);

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="isMajorRevision">The is major revision.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="publicationDate">The publication date. Set to null if not giving the resource a publication date in the past. This parameter is intended for use by the migration tool.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The publication id.</returns>
        int Publish(int resourceVersionId, bool isMajorRevision, string notes, DateTimeOffset? publicationDate, int userId);

        /// <summary>
        /// The unpublish.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="details">The details.</param>
        /// <param name="userId">The user id.</param>
        void Unpublish(int resourceVersionId, string details, int userId);

        /// <summary>
        /// The revert to draft.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        void RevertToDraft(int resourceVersionId, int userId);

        /// <summary>
        /// Delete a resource version.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        void Delete(int resourceVersionId, int userId);

        /// <summary>
        /// The get resource versions.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<ResourceVersion>> GetResourceVersionsAsync(int resourceId);

        ///// <summary>
        ///// The create activity.
        ///// </summary>
        ///// <param name="userId">
        ///// The user id.
        ///// </param>
        ///// <param name="resourceVersionId">
        ///// The resource version id.
        ///// </param>
        ///// <param name="activityStatusEnum">
        ///// The activity status enum.
        ///// </param>
        ///// <param name="activityStart">
        ///// The activity start.
        ///// </param>
        ///// <param name="activityEnd">
        ///// The activity end.
        ///// </param>
        ////void CreateActivity(
        //    int userId,
        //    int resourceVersionId,
        //    ActivityStatusEnum activityStatusEnum,
        //    DateTimeOffset activityStart,
        //    DateTimeOffset activityEnd);

        /// <summary>
        /// Check if a user has completed the activity corresponding to the resource version.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>A boolean.</returns>
        Task<bool> HasUserCompletedActivity(int userId, int resourceVersionId);

        /// <summary>
        /// Return whether a version at a specific status currently exists.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="draft">The resource version status id.</param>
        /// <returns>Whether the resource version exists or not.</returns>
        Task<bool> DoesVersionExist(int resourceId, VersionStatusEnum draft);

        /// <summary>
        /// Return whether a version at a specific status currently exists.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns the new resource version id.</returns>
        Task<int> CreateNextVersionAsync(int resourceId, int userId);

        /// <summary>
        /// Duplicates a resource version.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns the new resource version id.</returns>
        Task<int> CreateDuplicateVersionAsync(int resourceId, int userId);

        /// <summary>
        /// The publishing.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        void Publishing(int resourceVersionId, int userId);

        /// <summary>
        /// "Failed to publish".
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        void FailedToPublish(int resourceVersionId, int userId);

        /// <summary>
        /// The submit publishing.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        void SubmitForPublishing(int resourceVersionId, int userId);

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <param name="resourceVersionEventType">resourceVersionEventType.</param>
        /// <param name="details">details.</param>
        /// <param name="userId">user id.</param>
        void CreateResourceVersionEvent(int resourceVersionId, ResourceVersionEventTypeEnum resourceVersionEventType, string details, int userId);

        /// <summary>
        /// Get Contributions.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceContributionsRequestViewModel">The ResourceContributionsRequestViewModel.</param>
        /// <returns>A list of contributed resources.</returns>
        List<ResourceContributionDto> GetContributions(int userId, ResourceContributionsRequestViewModel resourceContributionsRequestViewModel);

        /// <summary>
        /// The GetResourceVersionForIdList.
        /// </summary>
        /// <param name="resourceVersionIds">List of resource version ids.</param>
        /// <returns>The resource version list.</returns>
        Task<List<ResourceVersion>> GetResourceVersionsForSearchSubmission(List<int> resourceVersionIds);

        /// <summary>
        /// Gets resources for dashboard based on type.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>resources.</returns>
        (int resourceCount, List<DashboardResourceDto> resources) GetResources(string dashboardType, int pageNumber, int userId);

        /// <summary>
        /// Copy the blocks from source to destination.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="sourceBlockCollectionId">The source block collection id.</param>
        /// <param name="destinationBlockCollectionId">The destination block collection id.</param>
        /// <param name="blocks">The blocks to be duplicated.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> FractionalDuplication(int userId, int sourceBlockCollectionId, int destinationBlockCollectionId, List<int> blocks);

        /// <summary>
        /// Gets external content details.
        /// </summary>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ExternalContentDetailsViewModel> GetExternalContentDetails(int resourceVersionId, int userId);

        /// <summary>
        /// To update dev id details.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceVersionDevIdViewModel">The resourceVersionDevIdViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateDevIdAsync(int userId, ResourceVersionDevIdViewModel resourceVersionDevIdViewModel);
    }
}
