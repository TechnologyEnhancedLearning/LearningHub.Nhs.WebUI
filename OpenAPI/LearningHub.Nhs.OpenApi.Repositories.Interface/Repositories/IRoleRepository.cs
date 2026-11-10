using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Entities.Resource;
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
    /// The role repository interface.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A read-only list of roles.</returns>
        Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a role by ID.
        /// </summary>
        /// <param name="id">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The matching role, or <c>null</c> if not found.</returns>
        Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a tracked role by ID.
        /// </summary>
        /// <param name="id">The role identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The tracked role, or <c>null</c> if not found.</returns>
        Task<Role?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether a role code exists.
        /// </summary>
        /// <param name="normalizedCode">The normalized role code.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> if the code exists; otherwise, <c>false</c>.</returns>
        Task<bool> CodeExistsAsync(string normalizedCode, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether a user exists.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> if the user exists; otherwise, <c>false</c>.</returns>
        Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether the scope target exists.
        /// </summary>
        /// <param name="scope">The role scope to verify.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> if the scope target exists; otherwise, <c>false</c>.</returns>
        Task<bool> ScopeTargetExistsAsync(RoleScope scope, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a role with a unique code.
        /// </summary>
        /// <param name="entity">The role to create.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The created role.</returns>
        Task<Role> CreateUniqueAsync(Role entity, CancellationToken cancellationToken);

        /// <summary>
        /// Saves pending changes.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveChangesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Executes an operation in a serializable transaction.
        /// </summary>
        /// <typeparam name="T">The operation result type.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The result of the operation.</returns>
        Task<T> InSerializableTransactionAsync<T>(
            Func<Task<T>> operation,
            CancellationToken cancellationToken);
    }
}
