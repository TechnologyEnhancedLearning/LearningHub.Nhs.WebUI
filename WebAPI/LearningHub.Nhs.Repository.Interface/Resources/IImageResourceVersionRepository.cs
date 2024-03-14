namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ImageResourceVersionRepository interface.
    /// </summary>
    public interface IImageResourceVersionRepository : IGenericRepository<ImageResourceVersion>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ImageResourceVersion> GetByIdAsync(int id);

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ImageResourceVersion> GetByResourceVersionIdAsync(int id, bool includeDeleted = false);
    }
}
