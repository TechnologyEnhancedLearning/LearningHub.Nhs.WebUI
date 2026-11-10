namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The user group repository.
    /// </summary>
    public class UserGroupRepository : GenericRepository<UserGroup>, IUserGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UserGroupRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Retrieves all active user groups.
        /// </summary>
        /// <returns>A collection of active user groups ordered by name.</returns>
        public async Task<List<UserGroup>> GetAllActiveAsync()
        {
            return await DbContext.UserGroup
            .AsNoTracking()
            .Where(x => !x.Deleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
        }

        /// <summary>
        /// Retrieves a user group by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the user group.</param>
        /// <returns>The user group if found; otherwise, <c>null</c>.</returns>
        public async Task<UserGroup> GetByIdAsync(int id)
        {
            return await DbContext.UserGroup
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         !x.Deleted);
        }

        /// <summary>
        /// Retrieves a user group by its identifier and optionally includes related role information.
        /// </summary>
        /// <param name="id">The identifier of the user group.</param>
        /// <param name="includeRoles">A value indicating whether related role and scope information should be included.</param>
        /// <returns>The user group if found; otherwise, <c>null</c>.</returns>
        public async Task<UserGroup> GetByIdAsync(
            int id,
            bool includeRoles)
        {
            if (!includeRoles)
            {
                return await GetByIdAsync(id);
            }

            return await DbContext.UserGroup
                .Include(x => x.RoleUserGroup
                    .Where(rug => !rug.Deleted))
                    .ThenInclude(rug => rug.Role)
                .Include(x => x.RoleUserGroup
                    .Where(rug => !rug.Deleted))
                    .ThenInclude(rug => rug.Scope)
                        .ThenInclude(scope => scope.CatalogueNode)
                            .ThenInclude(node => node.CurrentNodeVersion)
                                .ThenInclude(version =>
                                    version.CatalogueNodeVersion)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         !x.Deleted);
        }

        /// <summary>
        /// Retrieves a user group by its name.
        /// </summary>
        /// <param name="name">The name of the user group.</param>
        /// <returns>The user group if found; otherwise, <c>null</c>.</returns>
        public async Task<UserGroup> GetByNameAsync(string name)
        {
            return await DbContext.UserGroup
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => !x.Deleted &&
                         x.Name == name);
        }

        ///// <summary>
        ///// The delete async.
        ///// </summary>
        ///// <param name="userId">The user id.</param>
        ///// <param name="userGroupId">The user group id.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //public async Task DeleteAsync(int userId, int userGroupId)
        //{
        //    var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userGroupId };
        //    var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
        //    var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

        //    await DbContext.Database.ExecuteSqlRawAsync("hierarchy.UserGroupDelete @p0, @p1, @p2", param0, param1, param2);
        //}

        /// <summary>
        /// Soft deletes a user group.
        /// </summary>
        /// <param name="userId">The identifier of the user performing the deletion.</param>
        /// <param name="userGroupId">The identifier of the user group to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(
            int userId,
            int userGroupId)
        {
            var entity = await DbContext.UserGroup
                .FirstOrDefaultAsync(
                    x => x.Id == userGroupId &&
                         !x.Deleted);

            if (entity == null)
            {
                return;
            }

            entity.Deleted = true;

            await base.UpdateAsync(
                userId,
                entity);
        }

        /// <summary>
        /// Retrieves a queryable collection of active user groups, including their associated role and scope information.
        /// </summary>
        /// <returns>A queryable collection of active user groups.</returns>
        public new IQueryable<UserGroup> GetAll()
        {
            return DbContext.UserGroup
                .Where(x => !x.Deleted)
                .Include(x => x.RoleUserGroup
                    .Where(rug => !rug.Deleted))
                .ThenInclude(rug => rug.Scope);
        }





        /// <summary>
        /// Retrieves a user group for update operations with change tracking enabled.
        /// </summary>
        /// <param name="id">The identifier of the user group.</param>
        /// <returns>The user group if found; otherwise, <c>null</c>.</returns>
        public async Task<UserGroup> GetForUpdateAsync(int id)
            {
                return await DbContext.UserGroup
                    .FirstOrDefaultAsync(
                        x => x.Id == id &&
                             !x.Deleted);
            }


            

            /// <summary>
            /// Determines whether a user group exists.
            /// </summary>
            /// <param name="id">The identifier of the user group.</param>
            /// <returns><c>true</c> if the user group exists; otherwise, <c>false</c>.</returns>
            public async Task<bool> ExistsAsync(int id)
            {
                return await DbContext.UserGroup
                    .AnyAsync(
                        x => x.Id == id &&
                             !x.Deleted);
            }

            /// <summary>
            /// Determines whether a user group with the specified name already exists.
            /// </summary>
            /// <param name="name">The user group name to check.</param>
            /// <param name="excludeUserGroupId">The identifier of a user group to exclude from the check.</param>
            /// <returns><c>true</c> if the name already exists; otherwise, <c>false</c>.</returns>
            public async Task<bool> NameExistsAsync(
                string name,
                int? excludeUserGroupId = null)
            {
                IQueryable<UserGroup> query =
                    DbContext.UserGroup.Where(
                        x => !x.Deleted &&
                             x.Name == name);

                if (excludeUserGroupId.HasValue)
                {
                    query = query.Where(
                        x => x.Id !=
                             excludeUserGroupId.Value);
                }

                return await query.AnyAsync();
            }

            /// <summary>
            /// Retrieves all user groups to which a specified user belongs.
            /// </summary>
            /// <param name="userId">The identifier of the user.</param>
            /// <returns>A collection of user groups associated with the user.</returns>
            public async Task<List<UserGroup>> GetByUserIdAsync(
                int userId)
            {
                return await DbContext.UserGroup
                    .AsNoTracking()
                    .Where(group =>
                        !group.Deleted &&
                        group.UserUserGroup.Any(
                            membership =>
                                membership.UserId == userId &&
                                !membership.Deleted))
                    .OrderBy(group => group.Name)
                    .ToListAsync();
            }


        }
    }

