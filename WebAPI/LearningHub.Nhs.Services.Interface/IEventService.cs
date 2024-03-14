namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The RoleService interface.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Event> GetByIdAsync(int id);

        /// <summary>
        /// The save async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="eventEntity">The event view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAsync(int userId, Event eventEntity);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="eventEntity">The event entity.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateAsync(int userId, Event eventEntity);
    }
}
