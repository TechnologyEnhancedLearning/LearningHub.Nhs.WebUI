using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for managing user group reporter assignments.
    /// </summary>
    public class UserGroupReporterRepository : GenericRepository<UserGroupReporter>, IUserGroupReporterRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupReporterRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tzOffsetManager">The timezone offset manager.</param>
        public UserGroupReporterRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Retrieves all reporter assignments for a specified user group.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>A collection of reporter assignments with associated user information.</returns>
        public async Task<List<UserGroupReporter>> GetByUserGroupIdAsync(int userGroupId)
        {
            return await DbContext.UserGroupReporter
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
        /// Retrieves a reporter assignment for update operations with change tracking enabled.
        /// </summary>
        /// <param name="userId">The identifier of the reporter user.</param>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <returns>The matching reporter assignment if found; otherwise, <c>null</c>.</returns>
        public async Task<UserGroupReporter> GetForUpdateAsync(int userId, int userGroupId)
        {
            return await DbContext.UserGroupReporter
                .FirstOrDefaultAsync(
                    x => x.UserId == userId &&
                         x.UserGroupId == userGroupId &&
                         !x.Deleted &&
                         !x.UserGroup.Deleted);
        }

        /// <summary>
        /// Retrieves reporter assignments for the specified user group and collection of users.
        /// </summary>
        /// <param name="userGroupId">The identifier of the user group.</param>
        /// <param name="userIds">The identifiers of the users.</param>
        /// <returns>A collection of matching reporter assignments.</returns>
        public async Task<List<UserGroupReporter>> GetByUserGroupAndUserIdsForUpdateAsync(int userGroupId, IEnumerable<int> userIds)
        {
            var ids = userIds
                .Distinct()
                .ToList();

            return await DbContext.UserGroupReporter
                .Where(
                    x => x.UserGroupId == userGroupId &&
                         ids.Contains(x.UserId))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new reporter assignment.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the operation.</param>
        /// <param name="entity">The reporter assignment to create.</param>
        /// <returns>The identifier of the newly created reporter assignment.</returns>
        public async Task<int> CreateAsync(int currentUserId, UserGroupReporter entity)
        {
            entity.Deleted = false;
            entity.AmendUserId = currentUserId;
            entity.AmendDate = DateTime.UtcNow;

            DbContext.UserGroupReporter.Add(entity);

            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        /// <summary>
        /// Updates an existing reporter assignment.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the operation.</param>
        /// <param name="entity">The reporter assignment to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(int currentUserId, UserGroupReporter entity)
        {
            entity.AmendUserId = currentUserId;
            entity.AmendDate = DateTime.UtcNow;

            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbContext.UserGroupReporter.Update(entity);
            }

            await DbContext.SaveChangesAsync();
        }
    }
}
