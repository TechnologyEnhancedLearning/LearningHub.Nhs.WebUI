using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Role;
using LearningHub.Nhs.OpenApi.Models.ServiceModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    /// <summary>
    /// The user role repository.
    /// </summary>
    public interface IUserRoleRepository
    {
        /// <summary>
        /// Gets all role assignments for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A read-only list of user role assignments.</returns>
        Task<IReadOnlyList<UserRoleViewModel>> GetByUserIdAsync(int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all user assignments for a role.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A read-only list of user role assignments.</returns>
        Task<IReadOnlyList<UserRoleViewModel>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a role assignment by user and assignment ID.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The matching assignment, or <c>null</c> if not found.</returns>
        Task<UserRoleViewModel?> GetByIdAsync(int userId, int assignmentId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets active assignments for a role.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A read-only list of active role assignments.</returns>
        Task<IReadOnlyList<UserRole>> GetActiveByRoleIdAsync(int roleId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether a role has active assignments.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> if active assignments exist; otherwise, <c>false</c>.</returns>
        Task<bool> HasActiveAssignmentsAsync(int roleId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a unique role assignment.
        /// </summary>
        /// <param name="entity">The role assignment to create.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The created role assignment.</returns>
        Task<UserRole> CreateUniqueAsync(UserRole entity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the scope of a role assignment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="scope">The new scope value.</param>
        /// <param name="actorId">The identifier of the user performing the update.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The updated role assignment.</returns>
        Task<UserRole> UpdateScopeUniqueAsync(int userId, int assignmentId, RoleScope scope, int actorId, CancellationToken cancellationToken);

        /// <summary>
        /// Soft deletes a role assignment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="actorId">The identifier of the user performing the deletion.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> if the assignment was deleted; otherwise, <c>false</c>.</returns>
        Task<bool> SoftDeleteAsync(int userId, int assignmentId, int actorId, CancellationToken cancellationToken);
    }
}
