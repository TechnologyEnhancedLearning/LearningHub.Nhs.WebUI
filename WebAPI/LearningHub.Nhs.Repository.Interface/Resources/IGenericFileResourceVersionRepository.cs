namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// The GenericFileResourceVersionRepository interface.
    /// </summary>
    public interface IGenericFileResourceVersionRepository : IGenericRepository<GenericFileResourceVersion>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<GenericFileResourceVersion> GetByIdAsync(int id);

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<GenericFileResourceVersion> GetByResourceVersionIdAsync(int id, bool includeDeleted = false);

        /// <summary>
        /// The get by resource version id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The generic file resource version.</returns>
        GenericFileResourceVersion GetByResourceVersionId(int id, bool includeDeleted = false);
    }
}
