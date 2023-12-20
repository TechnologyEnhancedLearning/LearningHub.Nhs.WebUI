// <copyright file="UserGroupRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

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
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserGroup> GetByIdAsync(int id)
        {
            return await this.DbContext.UserGroup.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserGroup> GetByIdAsync(int id, bool includeRoles)
        {
            if (includeRoles)
            {
                return await this.DbContext.UserGroup
                                        .Include(r => r.RoleUserGroup).ThenInclude(rug => rug.Role)
                                        .Include(r => r.RoleUserGroup)
                                        .ThenInclude(rug => rug.Scope)
                                        .ThenInclude(s => s.CatalogueNode)
                                        .ThenInclude(s => s.CurrentNodeVersion)
                                        .ThenInclude(s => s.CatalogueNodeVersion)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(n => n.Id == id);
            }
            else
            {
                return await this.DbContext.UserGroup.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
            }
        }

        /// <summary>
        /// The get by name async.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserGroup> GetByNameAsync(string name)
        {
            return await this.DbContext.UserGroup.AsNoTracking().FirstOrDefaultAsync(n => n.Name == name);
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int userId, int userGroupId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userGroupId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            await this.DbContext.Database.ExecuteSqlRawAsync("hierarchy.UserGroupDelete @p0, @p1, @p2", param0, param1, param2);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public new IQueryable<UserGroup> GetAll()
        {
            return this.DbContext.Set<UserGroup>()
                .Include(n => n.RoleUserGroup).ThenInclude(n => n.Scope);
        }
    }
}
