namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user password validation token repository.
    /// </summary>
    public class EmailChangeValidationTokenRepository : GenericRepository<EmailChangeValidationToken>, IEmailChangeValidationTokenRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailChangeValidationTokenRepository"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="tzOffsetManager">
        /// The Timezone offset manager.
        /// </param>
        public EmailChangeValidationTokenRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The expire email change validation token.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ExpireEmailChangeValidationToken(string lookup)
        {
            var upvt = await GetByToken(lookup);
            DbContext.EmailChangeValidationToken.Remove(upvt);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// The get by token.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<EmailChangeValidationToken> GetByToken(string lookup)
        {
            return await DbContext.EmailChangeValidationToken
                .Include(vt => vt.User)
                .Where(vt => vt.Lookup == lookup)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// The get by token.
        /// </summary>
        /// <param name="userId">
        /// The lookup.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<EmailChangeValidationToken> GetLastIssuedEmailChangeValidationToken(int userId)
        {
            var validationToken = await DbContext.EmailChangeValidationToken
                .Where(vt => vt.UserId == userId).AsNoTracking()
                .OrderByDescending(vt => vt.Id).FirstOrDefaultAsync();

            return validationToken?.StatusId == (int)EmailChangeValidationTokenStatusEnum.Issued ? validationToken : null;
        }
    }
}