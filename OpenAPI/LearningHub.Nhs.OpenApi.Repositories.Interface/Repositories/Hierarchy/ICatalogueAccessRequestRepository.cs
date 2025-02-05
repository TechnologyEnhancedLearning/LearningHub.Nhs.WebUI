namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// The ICatalogueAccessRequestRepository interface.
    /// </summary>
    public interface ICatalogueAccessRequestRepository : IGenericRepository<CatalogueAccessRequest>
    {
        /// <summary>
        /// The GetByUserIdAndCatalogueId.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogueAccessRequest.</returns>
        CatalogueAccessRequest GetByUserIdAndCatalogueId(int catalogueNodeId, int userId);

        /// <summary>
        /// The GetAllByUserIdAndCatalogueId.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogueAccessRequest.</returns>
        IQueryable<CatalogueAccessRequest> GetAllByUserIdAndCatalogueId(int catalogueNodeId, int userId);

        /// <summary>
        /// The CreateCatalogueAccessRequestAsync.
        /// </summary>
        /// <param name="currentUserId">The currentUserId.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="message">The message.</param>
        /// <param name="roleId">The roleId.</param>
        /// <param name="catalogueManageAccessUrl">The catalogueManageAccessUrl.</param>
        /// <param name="accessType">The accessType.</param>
        /// <returns>The task.</returns>
        Task CreateCatalogueAccessRequestAsync(
            int currentUserId,
            string reference,
            string message,
            int roleId,
            string catalogueManageAccessUrl,
            string accessType);
    }
}
