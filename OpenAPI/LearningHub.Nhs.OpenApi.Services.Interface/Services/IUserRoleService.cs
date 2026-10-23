namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using LearningHub.Nhs.Models.Role;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// The User role service.
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>
        /// Gets all role assignments for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A read-only list of user role assignments.</returns>
        Task<IReadOnlyList<UserRoleViewModel>> GetForUserAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all user assignments for a role.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A read-only list of user role assignments.</returns>
        Task<IReadOnlyList<UserRoleViewModel>> GetForRoleAsync(int roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a role assignment by user and assignment ID.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The matching role assignment.</returns>
        Task<UserRoleViewModel> GetAsync(int userId, int assignmentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="actorId">The identifier of the user performing the action.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The role assignment details.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The created role assignment.</returns>
        Task<UserRoleViewModel> AssignAsync(int actorId, int userId, UserRoleCreateViewModel request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the scope of a role assignment.
        /// </summary>
        /// <param name="actorId">The identifier of the user performing the action.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="request">The scope update details.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The updated role assignment.</returns>
        Task<UserRoleViewModel> UpdateScopeAsync(int actorId, int userId, int assignmentId, UserRoleScopeUpdateViewModel request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a role assignment from a user.
        /// </summary>
        /// <param name="actorId">The identifier of the user performing the action.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UnassignAsync(int actorId, int userId, int assignmentId, CancellationToken cancellationToken = default);
    }
}
