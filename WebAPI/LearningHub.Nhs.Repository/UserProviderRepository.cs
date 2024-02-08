namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user provider repository.
    /// </summary>
    public class UserProviderRepository : GenericRepository<UserProvider>, IUserProviderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProviderRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UserProviderRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <inheritdoc />
        public async Task<UserProvider> GetByIdAsync(int id)
        {
            return await this.DbContext.UserProvider.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
        }

        /// <summary>
        /// The get list by user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<UserProvider>> GetByUserIdAsync(int userId)
        {
            return await this.DbContext.UserProvider.Include(n => n.Provider).AsNoTracking()
                .Where(n => n.UserId == userId && !n.Deleted).ToListAsync();
        }

        /// <summary>
        /// The update provider list by user id async.
        /// </summary>
        /// <param name="userProviderUpdateModel">The user provider update model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateUserProviderAsync(UserProviderUpdateViewModel userProviderUpdateModel)
        {
            var existingUserProviders = this.DbContext.UserProvider.Where(n => n.UserId == userProviderUpdateModel.UserId);

            // Delete User Providers
            foreach (var existingUserProvider in existingUserProviders)
            {
                if (userProviderUpdateModel.ProviderIds == null || !userProviderUpdateModel.ProviderIds.Contains(existingUserProvider.ProviderId))
                {
                    this.SetAuditFieldsForDelete(userProviderUpdateModel.UserId, existingUserProvider);
                    existingUserProvider.RemovalDate = existingUserProvider.AmendDate;
                }
            }

            // Insert User Provider
            if (userProviderUpdateModel.ProviderIds != null)
            {
                foreach (var providerId in userProviderUpdateModel.ProviderIds)
                {
                    var existingUserProvider = existingUserProviders.Where(n => n.ProviderId == providerId).SingleOrDefault();

                    if (existingUserProvider == null)
                    {
                        var userProvider = new UserProvider()
                        {
                            UserId = userProviderUpdateModel.UserId,
                            ProviderId = providerId,
                        };

                        this.SetAuditFieldsForCreate(userProviderUpdateModel.UserId, userProvider);
                        this.DbContext.UserProvider.Add(userProvider);
                    }
                }
            }

            await this.DbContext.SaveChangesAsync();
        }
    }
}
