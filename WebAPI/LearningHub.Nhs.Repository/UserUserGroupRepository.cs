// <copyright file="UserUserGroupRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user user group repository.
    /// </summary>
    public class UserUserGroupRepository : GenericRepository<UserUserGroup>, IUserUserGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserUserGroupRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UserUserGroupRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserUserGroup> GetByIdAsync(int id)
        {
            return await this.DbContext.UserUserGroup.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// The get by user user group details by user id and group id async.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="userGroupId">The userGroupId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserUserGroup> GetByUserIdandUserGroupIdAsync(int userId, int userGroupId)
        {
          return await this.DbContext.UserUserGroup.AsNoTracking().FirstOrDefaultAsync(n => n.UserId == userId && n.UserGroupId == userGroupId && !n.Deleted);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public new IQueryable<UserUserGroup> GetAll()
        {
            return this.DbContext.Set<UserUserGroup>()
                .Include(n => n.User)
                .Include(n => n.UserGroup)
                .Where(n => n.Deleted == false);
        }
    }
}
