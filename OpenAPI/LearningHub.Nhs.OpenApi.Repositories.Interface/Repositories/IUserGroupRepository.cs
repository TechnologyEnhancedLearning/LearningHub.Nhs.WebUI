namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using LearningHub.Nhs.Models.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The UserGroupRepository interface.
    /// </summary>
    public interface IUserGroupRepository : IGenericRepository<UserGroup>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserGroup> GetByIdAsync(int id);

        /// <summary>
        /// The get by name async.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserGroup> GetByNameAsync(string name);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserGroup> GetByIdAsync(int id, bool includeRoles);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int userId, int userGroupId);


///////////

        /// <summary>
        /// Retrieves all active user groups.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the collection of active user groups.</returns>
        Task<List<UserGroup>> GetAllActiveAsync();


        /// <summary>
        /// Retrieves a user group for update operations.
        /// </summary>
        /// <param name="id">The identifier of the user group.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user group.</returns>
        Task<UserGroup> GetForUpdateAsync(int id);

        /// <summary>
        /// Determines whether a user group with the specified identifier exists.
        /// </summary>
        /// <param name="id">The identifier of the user group.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the user group exists.</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Determines whether a user group with the specified name exists.
        /// </summary>
        /// <param name="name">The name of the user group.</param>
        /// <param name="excludeUserGroupId">An optional user group identifier to exclude from the check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the name already exists.</returns>
        Task<bool> NameExistsAsync(string name, int? excludeUserGroupId = null);

        /// <summary>
        /// Retrieves all user groups associated with a specific user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user's groups.</returns>
        Task<List<UserGroup>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Retrieves a queryable collection of all user groups.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}" /> for user groups.</returns>
        IQueryable<UserGroup> GetAll();
    }
}
