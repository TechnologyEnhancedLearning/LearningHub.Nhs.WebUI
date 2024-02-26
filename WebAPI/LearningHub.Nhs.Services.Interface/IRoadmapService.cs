namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.RoadMap;

    /// <summary>
    /// The roadmap service.
    /// </summary>
    public interface IRoadmapService
    {
        /// <summary>
        /// The add roadmap async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roadmap">The roadmap.</param>
        /// <returns>The roadmap id.</returns>
        Task<int> AddRoadmapAsync(int userId, Roadmap roadmap);

        /// <summary>
        /// The update roadmap async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roadmap">The roadmap.</param>
        /// <returns>The task.</returns>
        Task UpdateRoadmapAsync(int userId, Roadmap roadmap);

        /// <summary>
        /// The get updates.
        /// </summary>
        /// <returns>The updates.</returns>
        List<Roadmap> GetUpdates();

        /// <summary>
        /// The get updates.
        /// </summary>
        /// <param name="numberOfResults">numberOfResults.</param>
        /// <returns>The updates.</returns>
        RoadMapResponseViewModel GetUpdates(int numberOfResults);

        /// <summary>
        /// The Update Ordering async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roadmapOrdering">The roadmap ordering.</param>
        /// <returns>The task.</returns>
        Task UpdateOrderingAsync(int userId, RoadmapOrdering roadmapOrdering);

        /// <summary>
        /// The get roadmap.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The roadmap.</returns>
        Task<Roadmap> GetRoadmap(int id);

        /// <summary>
        /// The delete roadmap.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="id">The roadmap id.</param>
        /// <returns>The task.</returns>
        Task DeleteRoadmap(int userId, int id);
    }
}
