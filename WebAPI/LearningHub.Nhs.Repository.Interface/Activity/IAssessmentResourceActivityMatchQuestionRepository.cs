namespace LearningHub.Nhs.Repository.Interface.Activity
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The IAssessmentResourceActivityMatchQuestionRepository interface.
    /// </summary>
    public interface IAssessmentResourceActivityMatchQuestionRepository : IGenericRepository<AssessmentResourceActivityMatchQuestion>
    {
        /// <summary>
        /// Get Assessment Resource Activity by Assessment Resource Activity Id.
        /// </summary>
        /// <param name="id">The assessment resource activity id.</param>
        /// <returns>The Assessment Resource Activity.</returns>
        IEnumerable<AssessmentResourceActivityMatchQuestion> GetByAssessmentResourceActivityIdAsync(int id);
    }
}
