// <copyright file="AssessmentResourceActivityInteractionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Activity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The media resource activity interaction repository.
    /// </summary>
    public class AssessmentResourceActivityInteractionRepository : GenericRepository<AssessmentResourceActivityInteraction>, IAssessmentResourceActivityInteractionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentResourceActivityInteractionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public AssessmentResourceActivityInteractionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<AssessmentResourceActivityInteraction> GetByIdAsync(int id)
        {
            return this.DbContext.AssessmentResourceActivityInteraction.Where(n => n.Id == id).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Creates an assessment activity interaction.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="interaction">The interaction.</param>
        /// <returns>The task.</returns>
        public async Task<int> CreateInteraction(int userId, AssessmentResourceActivityInteraction interaction)
        {
            foreach (AssessmentResourceActivityInteractionAnswer answer in interaction.Answers)
            {
                this.SetAuditFieldsForCreate(userId, answer);
            }

            return await this.CreateAsync(userId, interaction);
        }

        /// <summary>
        /// Get the assessment resource activity interaction for the given user, activity, and question block.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <param name="questionBlockId">The question block id.</param>
        /// <returns>AssessmentResourceActivityInteraction.</returns>
        public async Task<AssessmentResourceActivityInteraction> GetInteractionForQuestion(int userId, int assessmentResourceActivityId, int questionBlockId)
        {
            return await this.DbContext
                .AssessmentResourceActivityInteraction
                .FirstOrDefaultAsync(i => i.CreateUserId == userId && i.AssessmentResourceActivityId == assessmentResourceActivityId && i.QuestionBlockId == questionBlockId);
        }

        /// <summary>
        /// Gets all the interactions for a given assessment resource activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <returns>The list of interactions.</returns>
        public async Task<List<AssessmentResourceActivityInteraction>> GetInteractionsForAssessmentResourceActivity(int assessmentResourceActivityId)
        {
           return await this.DbContext
                .AssessmentResourceActivityInteraction
                .Include(i => i.Answers)
                .ThenInclude(a => a.QuestionAnswer)
                .Include(i => i.Answers)
                .Include(a => a.QuestionBlock)
                .ThenInclude(q => q.Block)
                .Include(a => a.QuestionBlock)
                .ThenInclude(q => q.Answers)
                .Where(i => i.AssessmentResourceActivityId == assessmentResourceActivityId)
                .ToListAsync();
        }
    }
}
