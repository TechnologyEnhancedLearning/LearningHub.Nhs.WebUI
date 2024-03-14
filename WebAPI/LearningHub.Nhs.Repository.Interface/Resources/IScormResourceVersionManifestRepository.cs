namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ScormResourceVersionManfestRepository interface.
    /// </summary>
    public interface IScormResourceVersionManifestRepository : IGenericRepository<ScormResourceVersionManifest>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ScormResourceVersionManifest> GetByScormResourceVersionIdAsync(int id);
    }
}
