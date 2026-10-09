namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using LearningHub.Nhs.Models.Dto;
    using LearningHub.Nhs.Models.Entities;
    using System.Threading.Tasks;

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

        /// <summary>
        /// The get user detail for the authentication.
        /// </summary>
        /// <param name = "username">
        /// username.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<UserAuthenticateDto> GetUserDetailForAuthentication(string username);

        Task<User> GetByIdIncludingDeletedAsync(int id);

        Task<bool> IsEmailAvailableAsync(string email, int? excludeUserId = null);
        Task<bool> UserExistsAsync(int userId);
    }
}
