using LearningHub.Nhs.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    /// <summary>
    /// Defines repository operations for managing user group reporter assignments.
    /// </summary>
    public interface IUserGroupReporterRepository : IGenericRepository<UserGroupReporter>
    {
        /// <summary>
        /// Retrieves all reporter assignments for a specified user group.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>A collection of reporter assignments associated with the user group.</returns>
        Task<List<UserGroupReporter>> GetByUserGroupIdAsync(int userGroupId);

        /// <summary>
        /// Retrieves a reporter assignment for update operations with change tracking enabled.
        /// </summary>
        /// <param name="userId">The identifier of the reporter user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>The matching reporter assignment if found; otherwise, <c>null</c>.</returns>
        Task<UserGroupReporter> GetForUpdateAsync(int userId, int userGroupId);

        /// <summary>
        /// Retrieves reporter assignments for the specified user group and collection of users with change tracking enabled.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <param name="userIds">The identifiers of the users.</param>
        /// <returns>A collection of matching reporter assignments.</returns>
        Task<List<UserGroupReporter>> GetByUserGroupAndUserIdsForUpdateAsync(int userGroupId, IEnumerable<int> userIds);

        /// <summary>
        /// Creates a new reporter assignment.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the operation.</param>
        /// <param name="entity">The reporter assignment to create.</param>
        /// <returns>The identifier of the newly created reporter assignment.</returns>
        Task<int> CreateAsync(int currentUserId, UserGroupReporter entity);

        /// <summary>
        /// Updates an existing reporter assignment.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the operation.</param>
        /// <param name="entity">The reporter assignment to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateAsync(int currentUserId, UserGroupReporter entity);
    }
}
