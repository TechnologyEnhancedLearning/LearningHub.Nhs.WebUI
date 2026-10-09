namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository implementation for managing user group membership data operations.
    /// </summary>
    public class UserUserGroupRepository : GenericRepository<UserUserGroup>, IUserUserGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserUserGroupRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tzOffsetManager">The timezone offset manager.</param>
        public UserUserGroupRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Retrieves a user group membership by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the membership record.</param>
        /// <returns>The matching membership record if found; otherwise, <c>null</c>.</returns>
        public async Task<UserUserGroup> GetByIdAsync(int id)
        {
            return await DbContext.UserUserGroup
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted);
        }

        /// <summary>
        /// Retrieves a membership record for the specified user and user group.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>The matching membership record if found; otherwise, <c>null</c>.</returns>
        public async Task<UserUserGroup> GetByUserIdandUserGroupIdAsync(int userId, int userGroupId)
        {
            return await DbContext.UserUserGroup
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId &&
                         x.UserGroupId == userGroupId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted);
        }

        /// <summary>
        /// Retrieves a membership record for update operations with change tracking enabled.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>The matching membership record if found; otherwise, <c>null</c>.</returns>
        public async Task<UserUserGroup> GetForUpdateAsync(int userId, int userGroupId)
        {
            return await DbContext.UserUserGroup
                .FirstOrDefaultAsync(
                    x => x.UserId == userId &&
                         x.UserGroupId == userGroupId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted);
        }

        /// <summary>
        /// Retrieves membership records for the specified user group and collection of users.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <param name="userIds">The identifiers of the users.</param>
        /// <returns>A collection of matching membership records.</returns>
        public async Task<List<UserUserGroup>> GetByUserGroupAndUserIdsForUpdateAsync(int userGroupId, IEnumerable<int> userIds)
        {
            var ids = userIds
                .Distinct()
                .ToList();

            return await DbContext.UserUserGroup
                .Where(
                    x => x.UserGroupId == userGroupId &&
                         ids.Contains(x.UserId))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all active membership records for a specified user group.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>A collection of membership records with associated user information.</returns>
        public async Task<List<UserUserGroup>> GetByUserGroupIdAsync(int userGroupId)
        {
            return await DbContext.UserUserGroup
                .AsNoTracking()
                .Include(x => x.User)
                .Where(
                    x => x.UserGroupId == userGroupId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted)
                .OrderBy(x => x.User.LegacyUserName)
                .ToListAsync();
        }

        /// <summary>
        /// Determines whether a user has been assigned to a specified user group.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns><c>true</c> if the user is assigned to the group; otherwise, <c>false</c>.</returns>
        public async Task<bool> HasUserBeenAssignedToUserGroup(int userId, int userGroupId)
        {
            return await DbContext.UserUserGroup
                .AnyAsync(
                    x => x.UserId == userId &&
                         x.UserGroupId == userGroupId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted);
        }

        /// <summary>
        /// Retrieves a queryable collection of active user group memberships.
        /// </summary>
        /// <returns>A queryable collection of user group memberships including related user and user group data.</returns>
        public new IQueryable<UserUserGroup> GetAll()
        {
            return DbContext.UserUserGroup
                .Include(x => x.User)
                .Include(x => x.UserGroup)
                .Where(
                    x => !x.Deleted &&
                         !x.UserGroup.Deleted);
        }
    }
}
