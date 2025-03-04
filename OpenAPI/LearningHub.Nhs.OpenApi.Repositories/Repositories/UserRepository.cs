namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

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
            return await DbContext.User.FirstOrDefaultAsync(n => n.Id == id);
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
                                        .FirstOrDefaultAsync(n => n.UserName == username && !n.Deleted);
            }
            else
            {
                return await DbContext.User.FirstOrDefaultAsync(n => n.UserName == username);
            }
        }

        /// <inheritdoc/>
        public bool IsAdminUser(int userId)
        {
            return DbContext.UserUserGroup
                .Any(uug => uug.UserId == userId &&
                            uug.UserGroupId == SystemAdminUserGroup);
        }
    }
}
