// <copyright file="AssessmentResourceActivityInteractionAnswerRepository.cs" company="NHS England">
// Copyright (c) NHS England.
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
    /// The media resource activity interaction repository.
    /// </summary>
    public class AssessmentResourceActivityInteractionAnswerRepository : GenericRepository<AssessmentResourceActivityInteractionAnswer>, IAssessmentResourceActivityInteractionAnswerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentResourceActivityInteractionAnswerRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public AssessmentResourceActivityInteractionAnswerRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<AssessmentResourceActivityInteractionAnswer> GetByIdAsync(int id)
        {
            return this.DbContext.AssessmentResourceActivityInteractionAnswer.Where(n => n.Id == id).SingleOrDefaultAsync();
        }
    }
}
