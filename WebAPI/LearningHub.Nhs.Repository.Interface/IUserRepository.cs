namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The UserRepository interface.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<User> GetByIdAsync(int id);

        /// <summary>
        /// The get by id include roles async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<User> GetByIdIncludeRolesAsync(int id);

        /// <summary>
        /// The get by username async.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<User> GetByUsernameAsync(string username, bool includeRoles);

        /// <summary>
        /// Returns indication of whether the user in an Admin.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool IsAdminUser(int userId);
    }
}
