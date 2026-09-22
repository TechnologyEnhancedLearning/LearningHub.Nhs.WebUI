namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The user password validation token repository.
    /// </summary>
    public class UserPasswordValidationTokenRepository : GenericRepository<UserPasswordValidationToken>, IUserPasswordValidationTokenRepository
    {
       
        /// <summary>
        ///Initializes a new instance of the <see cref="UserPasswordValidationTokenRepository"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="tzOffsetManager">
        /// The Timezone offset manager.
        /// </param>
        public UserPasswordValidationTokenRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <inheritdoc/>
        public async Task<UserPasswordValidationToken> GetByToken(string lookup)
        {
            return await this.DbContext.UserPasswordValidationToken
                .Include(vt => vt.User)
                .Where(vt => vt.Lookup == lookup).AsNoTracking()
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CreateAsync(int userId, UserPasswordValidationToken userPasswordValidationToken)
        {
            await this.DbContext.Set<UserPasswordValidationToken>().AddAsync(userPasswordValidationToken);
            var createdDate = DateTimeOffset.Now;
            userPasswordValidationToken.CreateUserId = userId;
            userPasswordValidationToken.CreateDate = createdDate;

            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry(userPasswordValidationToken).State = EntityState.Detached;

            return userPasswordValidationToken.Id;
        }

        /// <inheritdoc/>
        public async Task ExpireUserPasswordValidationToken(string lookup)
        {
            var upvt = await this.GetByToken(lookup);
            this.DbContext.UserPasswordValidationToken.Remove(upvt);
            this.DbContext.SaveChanges();
        }
    }
}
