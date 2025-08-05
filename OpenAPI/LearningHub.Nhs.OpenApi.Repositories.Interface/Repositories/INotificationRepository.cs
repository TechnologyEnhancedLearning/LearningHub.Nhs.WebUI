namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The NotificationRepository interface.
    /// </summary>
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Notification> GetByIdAsync(int id);

        /// <summary>
        /// The get all full.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        IQueryable<Notification> GetAllFull();
    }
}
