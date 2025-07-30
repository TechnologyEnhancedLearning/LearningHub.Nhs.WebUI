namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user password reset requests repository.
    /// </summary>
    public class UserPasswordResetRequestsRepository : GenericRepository<PasswordResetRequests>, IUserPasswordResetRequestsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserPasswordResetRequestsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="tzOffsetManager">
        /// The Timezone offset manager.
        /// </param>
        public UserPasswordResetRequestsRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// To check user can request a password reset.
        /// </summary>
        /// <param name="emailAddress">
        /// The lookup.
        /// </param>
        /// <param name="passwordRequestLimitingPeriod">The passwordRequestLimitingPeriod.</param>
        /// <param name="passwordRequestLimit">ThepasswordRequestLimit.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> CanRequestPasswordResetAsync(string emailAddress, int passwordRequestLimitingPeriod, int passwordRequestLimit)
        {
            // passwordRequestLimitingPeriod is in minutes.
            var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-passwordRequestLimitingPeriod);

            // Get the count of password reset requests for the user in the last 1 minute
            var recentRequests = await DbContext.PasswordResetRequests
                .Where(r => r.EmailAddress == emailAddress && r.RequestTime >= oneMinuteAgo)
                .CountAsync();

            return recentRequests < passwordRequestLimit;
        }

        /// <summary>
        /// CreatePasswordRequests.
        /// </summary>
        /// <param name="emailAddress">The emailAddress.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> CreatePasswordRequests(string emailAddress)
        {
            try
            {
                var passwordResetRequests = new PasswordResetRequests
                {
                    EmailAddress = emailAddress,
                    RequestTime = DateTime.UtcNow,
                    Deleted = false,
                    CreateUserId = 4,
                    CreateDate = DateTime.UtcNow,
                    AmendUserId = 4,
                    AmendDate = DateTime.UtcNow,
                };

                await DbContext.PasswordResetRequests.AddAsync(passwordResetRequests);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
            return false;
            }
        }
    }
}