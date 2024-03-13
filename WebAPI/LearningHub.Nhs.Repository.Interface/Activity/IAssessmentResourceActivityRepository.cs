namespace LearningHub.Nhs.Repository.Interface.Activity
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The AssessmentResourceActivity interface.
    /// </summary>
    public interface IAssessmentResourceActivityRepository : IGenericRepository<AssessmentResourceActivity>
    {
        /// <summary>
        /// Get Assessment Resource Activity By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>AssessmentResourceActivity.</returns>
        Task<AssessmentResourceActivity> GetByIdAsync(int id);

        /// <summary>
        /// Gets the latest assessment resource activity for the given resource version id and user id.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The assessment resource activity task.</returns>
        Task<AssessmentResourceActivity> GetLatestAssessmentResourceActivity(int resourceVersionId, int userId);
    }
}
