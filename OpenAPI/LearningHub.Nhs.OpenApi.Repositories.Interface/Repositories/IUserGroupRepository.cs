namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

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
    }
}
