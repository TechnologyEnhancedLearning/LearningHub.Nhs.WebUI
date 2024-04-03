namespace LearningHub.Nhs.Repository
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The user repository.
    /// </summary>
    public class DetectJsLogRepository : GenericRepository<DetectJsLog>, IDetectJsLogRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetectJsLogRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public DetectJsLogRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(long jsEnabled, long jsDisabled)
        {
            var sql = @$"UPDATE hub.DetectJsLog 
                        SET JsEnabledRequest = JsEnabledRequest + {jsEnabled}, JsDisabledRequest = JsDisabledRequest + {jsDisabled}, AmendDate = SYSDATETIMEOFFSET() 
                        WHERE Id = 1";
            await this.DbContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}