namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// Resource repository interface.
    /// </summary>
    public interface IResourceRepository
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceIds"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds);

        ///// <summary>
        ///// Gets resources from ids.
        ///// </summary>
        ///// <param name="resourceIds"><see cref="resourceIds"/>.</param>
        ///// <param name="currentUserId"></param>
        ///// <returns>Resources with details.</returns>
        //public Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds, int currentUserId);//qqqq

        /// <summary>
        /// Gets resource references by their original resource reference Ids.
        /// </summary>
        /// <param name="originalResourceReferenceIds"><see cref="originalResourceReferenceIds"/>.</param>
        /// <returns>Resource references.</returns>
        public Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
        IEnumerable<int> originalResourceReferenceIds);

        ///// <summary>
        ///// Gets resource references by their original resource reference Ids.
        ///// </summary>
        ///// <param name="originalResourceReferenceIds"><see cref="originalResourceReferenceIds"/>.</param>
        ///// <param name="currentUserId"></param>
        ///// <returns>Resource references.</returns>
        //public Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
        //    IEnumerable<int> originalResourceReferenceIds, int currentUserId);//qqqq
        /// <summary>
        /// delete me.
        /// </summary>
        public void QqqqTest();
        // qqqq
        /// <summary>
        /// Gets resource activity for resourceReferenceIds and userIds.
        /// </summary>
        /// <param name="resourceReferenceIds"><see cref="resourceReferenceIds"/>.</param>
        /// <param name="userIds"></param>
        /// <returns>ResourceActivity.</returns>
        Task<IEnumerable<ResourceActivity>> GetResourceActivityPerResourceMajorVersion(IEnumerable<int>? resourceReferenceIds, IEnumerable<int>? userIds);
    }
}
