namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The MediaResourceActivityInteraction interface.
    /// </summary>
    public interface IMediaResourceActivityInteractionRepository : IGenericRepository<MediaResourceActivityInteraction>
    {
        /// <summary>
        /// Get Media Resource Activity Interaction By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>MediaResourceActivityInteraction.</returns>
        Task<MediaResourceActivityInteraction> GetByIdAsync(int id);

        /// <summary>
        /// Performs the analysis of media resource activity to populate the played segment data.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="mediaResourceActivityId">The mediaResourceActivityId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task CalculatePlayedMediaSegments(int userId, int resourceVersionId, int mediaResourceActivityId);
    }
}
