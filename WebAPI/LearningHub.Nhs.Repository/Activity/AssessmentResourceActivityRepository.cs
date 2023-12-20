// <copyright file="AssessmentResourceActivityRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Activity
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The activity repository.
    /// </summary>
    public class AssessmentResourceActivityRepository : GenericRepository<AssessmentResourceActivity>, IAssessmentResourceActivityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentResourceActivityRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public AssessmentResourceActivityRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<AssessmentResourceActivity> GetByIdAsync(int id)
        {
            return this.DbContext.AssessmentResourceActivity
                .Include(a => a.ResourceActivity)
                .Include(a => a.AssessmentResourceActivityInteractions)
                .Include(a => a.MatchQuestions)
                .ThenInclude(a => a.FirstMatchAnswer)
                .Where(n => n.Id == id)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Gets the latest assessment resource activity for the given resource version id and user id.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The assessment resource activity task.</returns>
        public async Task<AssessmentResourceActivity> GetLatestAssessmentResourceActivity(int resourceVersionId, int userId)
        {
            return await this.DbContext.AssessmentResourceActivity
                .OrderByDescending(a => a.CreateDate)
                .Include(a => a.ResourceActivity)
                .FirstOrDefaultAsync(n => n.ResourceActivity.ResourceVersionId == resourceVersionId && n.CreateUserId == userId);
        }
    }
}
