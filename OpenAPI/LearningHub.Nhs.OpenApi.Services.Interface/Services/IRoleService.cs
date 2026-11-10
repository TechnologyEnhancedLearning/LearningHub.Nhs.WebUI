namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using LearningHub.Nhs.Models.Role;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The role service.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A read-only list of roles.</returns>
        Task<IReadOnlyList<RoleViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a role by ID.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The matching role.</returns>
        Task<RoleViewModel> GetByIdAsync(int roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new role.
        /// </summary>
        /// <param name="actorId">The identifier of the user performing the action.</param>
        /// <param name="request">The role creation details.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The created role.</returns>
        Task<RoleViewModel> CreateAsync(int actorId, RoleCreateViewModel request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="actorId">The identifier of the user performing the action.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="request">The role update details.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The updated role.</returns>
        Task<RoleViewModel> UpdateAsync(int actorId, int roleId, RoleUpdateViewModel request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="actorId">The identifier of the user performing the action.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(int actorId, int roleId, CancellationToken cancellationToken = default);
    }
}
