namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The AssessmentResourceActivityInteractionAnswer interface.
    /// </summary>
    public interface IAssessmentResourceActivityInteractionAnswerRepository : IGenericRepository<AssessmentResourceActivityInteractionAnswer>
    {
        /// <summary>
        /// Get Assessment Resource Activity Interaction Answer By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>AssessmentResourceActivityInteractionAnswer.</returns>
        Task<AssessmentResourceActivityInteractionAnswer> GetByIdAsync(int id);
    }
}
