namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user group attribute repository.
    /// </summary>
    public class UserGroupAttributeRepository : GenericRepository<UserGroupAttribute>, IUserGroupAttributeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupAttributeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UserGroupAttributeRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserGroupAttribute> GetByIdAsync(int id)
        {
            return await DbContext.UserGroupAttribute.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// The get by user group id async.
        /// </summary>
        /// <param name="userGroupId">The user group id.</param>
        /// <param name="attributeId">The attribute id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserGroupAttribute> GetByUserGroupIdAttributeId(int userGroupId, int attributeId)
        {
            return await DbContext.UserGroupAttribute.Where(n => n.UserGroupId == userGroupId && n.AttributeId == attributeId && n.Deleted == false).FirstOrDefaultAsync();
        }
    }
}
