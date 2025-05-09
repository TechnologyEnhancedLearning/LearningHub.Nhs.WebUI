﻿namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Activity
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;

    /// <summary>
    /// The AssessmentResourceActivityMatchQuestionRepository.
    /// </summary>
    public class AssessmentResourceActivityMatchQuestionRepository : GenericRepository<AssessmentResourceActivityMatchQuestion>, IAssessmentResourceActivityMatchQuestionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentResourceActivityMatchQuestionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public AssessmentResourceActivityMatchQuestionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Get Assessment Resource Activity by Assessment Resource Activity Id.
        /// </summary>
        /// <param name="id">The assessment resource activity id.</param>
        /// <returns>The Assessment Resource Activity.</returns>
        public IEnumerable<AssessmentResourceActivityMatchQuestion> GetByAssessmentResourceActivityIdAsync(int id)
        {
            return DbContext
                .AssessmentResourceActivityMatchQuestion
                .Where(q => q.AssessmentResourceActivityId == id);
        }
    }
}
