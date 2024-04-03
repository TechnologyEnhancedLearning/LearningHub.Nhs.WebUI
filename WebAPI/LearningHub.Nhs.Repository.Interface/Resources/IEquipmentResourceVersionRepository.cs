namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The EquipmentResourceVersionRepository interface.
    /// </summary>
    public interface IEquipmentResourceVersionRepository : IGenericRepository<EquipmentResourceVersion>
    {
        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<EquipmentResourceVersion> GetByResourceVersionIdAsync(int id);
    }
}
