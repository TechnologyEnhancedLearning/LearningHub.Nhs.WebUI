using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    /// <summary>
    /// UserExternalSystemRepository.
    /// </summary>
    public class UserExternalSystemRepository : GenericRepository<UserExternalSystem>, IUserExternalSystemRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserExternalSystemRepository"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="tzOffsetManager">
        /// The tzOffsetManager.
        /// </param>
        public UserExternalSystemRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <inheritdoc/>
        public UserExternalSystem GetUserExternalSystem(int userId, int externalSystemId)
        {
            return this.DbContext.UserExternalSystem.SingleOrDefault(
                x => x.UserId == userId && x.ExternalSystemId == externalSystemId);
        }

        /// <inheritdoc/>
        public async Task<UserExternalSystem> GetUserExternalSystemByUserIdandCodeAsync(int userId, string code)
        {
            return await this.DbContext.UserExternalSystem
                            .Where(t => t.UserId == userId && t.ExternalSystem.Code == code)
                            .FirstOrDefaultAsync();
        }
    }
}
