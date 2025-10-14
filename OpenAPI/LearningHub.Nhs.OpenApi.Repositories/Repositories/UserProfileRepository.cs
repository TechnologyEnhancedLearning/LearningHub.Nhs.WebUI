namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The UserDetailsRepository class.
    /// </summary>
    public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UserProfileRepository(LearningHubDbContext context, ITimezoneOffsetManager tzOffsetManager)
            : base(context, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The userProfile.</returns>
        public async Task<UserProfile> GetByIdAsync(int id)
        {
            return await DbContext.UserProfile.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// The GetByEmailAddressAsync.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>The userProfile.</returns>
        public async Task<UserProfile> GetByEmailAddressAsync(string emailAddress)
        {
            return await DbContext.UserProfile.AsNoTracking().SingleOrDefaultAsync(x => x.EmailAddress == emailAddress);
        }

        /// <inheritdoc/>
        public async Task<UserProfile> GetByUsernameAsync(string userName)
        {
            return await DbContext.UserProfile.AsNoTracking().SingleOrDefaultAsync(x => x.UserName == userName);
        }
    }
}
