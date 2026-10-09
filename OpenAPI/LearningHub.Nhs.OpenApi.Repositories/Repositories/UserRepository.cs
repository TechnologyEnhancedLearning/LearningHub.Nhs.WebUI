namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using LearningHub.Nhs.Models.Dto;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The user repository.
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private const int SystemAdminUserGroup = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UserRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<User> GetByIdAsync(int id)
        {
            return await this.DbContext.User.Include(x => x.ProfessionalBody)
                .FirstOrDefaultAsync(user => user.Id == id && !user.Deleted);
        }

        /// <summary>
        /// The get by id include roles async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<User> GetByIdIncludeRolesAsync(int id)
        {
            return await DbContext.User.Include(u => u.UserUserGroup)
                .ThenInclude(u => u.UserGroup)
                .ThenInclude(u => u.RoleUserGroup)
                .ThenInclude(u => u.Scope)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// The get by username async.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<User> GetByUsernameAsync(string username, bool includeRoles)
        {
            if (includeRoles)
            {
                return await DbContext.User
                                        .Include(u => u.UserUserGroup)
                                        .ThenInclude(uug => uug.UserGroup)
                                        .ThenInclude(ug => ug.RoleUserGroup)
                                        .ThenInclude(rug => rug.Role)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(n => n.LegacyUserName == username && !n.Deleted);
            }
            else
            {
                return await DbContext.User.FirstOrDefaultAsync(n => n.LegacyUserName == username);
            }
        }

        /// <inheritdoc/>
        public bool IsAdminUser(int userId)
        {
            return DbContext.UserUserGroup
                .Any(uug => uug.UserId == userId &&
                            uug.UserGroupId == SystemAdminUserGroup);
        }

        /// <summary>
        /// The get user detail for the authentication.
        /// </summary>
        /// <param name = "username">
        /// username.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<UserAuthenticateDto> GetUserDetailForAuthentication(string username)
        {
            var param0 = new SqlParameter("@userName", SqlDbType.VarChar) { Value = username };

            var userAuthenticateDto = await this.DbContext.UserAuthenticateDto.FromSqlRaw("proc_UserDetailForAuthenticationByUserName @userName", param0).AsNoTracking().ToListAsync();

            return userAuthenticateDto.FirstOrDefault();
        }

        /// <summary>
        /// Gets a user by id, including soft-deleted users.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The user if found.</returns>
        public async Task<User?> GetByIdIncludingDeletedAsync(int id)
        {
            return await this.DbContext.User.FirstOrDefaultAsync(user => user.Id == id);
        }

        /// <summary>
        /// Checks whether an email address is available for use.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="excludeUserId">
        /// Optional user id to exclude from the check.
        /// </param>
        /// <returns>True when the email address is available.</returns>
        public async Task<bool> IsEmailAvailableAsync(string email, int? excludeUserId = null)
        {
            var query = this.DbContext.User.AsNoTracking().Where(user => user.EmailAddress == email);

            if (excludeUserId.HasValue)
            {
                query = query.Where(user => user.Id != excludeUserId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await DbContext.User.AsNoTracking()
                    .AnyAsync(
                        x => x.Id == userId &&
                             !x.Deleted);
        }
    }
}
