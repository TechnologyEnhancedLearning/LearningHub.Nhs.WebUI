namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.DLS;

    /// <summary>
    /// The DLSDeviceTypeRepository interface.
    /// </summary>
    public interface IDLSDeviceTypeRepository
    {
        /// <summary>
        /// Get DLS Device Type details by Id.
        /// </summary>
        /// <param name="id">The Id of the details.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        DeviceTypes GetDetailsById(int id);

        /// <summary>
        /// Update DLS Device Type details by Id.
        /// </summary>
        /// <param name="id">The Id of the details.</param>
        /// <param name="deviceType">The string of the deviceType.</param>
        void UpdateById(int id, string deviceType);

        /// <summary>
        /// Add a DeviceTypes entry.
        /// </summary>
        /// <param name="deviceType">The string of the DeviceType.</param>
        void Create(string deviceType);

        /// <summary>
        /// Delete DLS Device type by id.
        /// </summary>
        /// <param name="id">The id of the DeviceType to delete.</param>
        void Delete(int id);
    }
}