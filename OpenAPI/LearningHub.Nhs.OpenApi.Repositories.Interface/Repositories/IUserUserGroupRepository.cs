namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using LearningHub.Nhs.Models.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines repository operations for managing user group memberships.
    /// </summary>
    public interface IUserUserGroupRepository : IGenericRepository<UserUserGroup>
    {
        /// <summary>
        /// Retrieves a user group membership by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the membership record.</param>
        /// <returns>The matching user group membership, or <c>null</c> if not found.</returns>
        Task<UserUserGroup> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a user group membership for a specified user and user group.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>The matching user group membership, or <c>null</c> if not found.</returns>
        Task<UserUserGroup> GetByUserIdandUserGroupIdAsync(int userId, int userGroupId);

        /// <summary>
        /// Retrieves a user group membership for update operations with change tracking enabled.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>The matching user group membership, or <c>null</c> if not found.</returns>
        Task<UserUserGroup> GetForUpdateAsync(int userId, int userGroupId);

        /// <summary>
        /// Retrieves user group memberships for the specified user group and users with change tracking enabled.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <param name="userIds">The identifiers of the users.</param>
        /// <returns>A collection of matching user group memberships.</returns>
        Task<List<UserUserGroup>> GetByUserGroupAndUserIdsForUpdateAsync(int userGroupId, IEnumerable<int> userIds);

        /// <summary>
        /// Retrieves all memberships associated with a specified user group.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>A collection of user group memberships.</returns>
        Task<List<UserUserGroup>> GetByUserGroupIdAsync(int userGroupId);

        /// <summary>
        /// Determines whether a user has been assigned to a specified user group.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns><c>true</c> if the user has been assigned to the user group; otherwise, <c>false</c>.</returns>
        Task<bool> HasUserBeenAssignedToUserGroup(int userId, int userGroupId);

        /// <summary>
        /// Retrieves a queryable collection of user group memberships.
        /// </summary>
        /// <returns>A queryable collection of user group memberships.</returns>
        IQueryable<UserUserGroup> GetAll();
    }
}
