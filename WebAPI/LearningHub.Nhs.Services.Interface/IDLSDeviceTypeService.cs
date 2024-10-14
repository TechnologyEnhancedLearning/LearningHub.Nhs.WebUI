namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.DLS;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The DLSDeviceType interface.
    /// </summary>
    public interface IDLSDeviceTypeService
    {
        /// <summary>
        /// Get details of a DLSDeviceType.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public DeviceTypes Details(int id);

        /// <summary>
        /// Create a new DLSDeviceType.
        /// </summary>
        /// <param name="deviceType">The device type.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public void Create(string deviceType);

        /// <summary>
        /// Edit an existing DLSDeviceType.
        /// </summary>
        /// <param name = "id" > The id of the device type.</param>
        /// <param name = "deviceType" > The device type.</param>
        /// <returns>The<see cref="ActionResult"/>.</returns>
        public void Edit(int id, string deviceType);

        /// <summary>
        /// Delete an existing DLS DeviceType.
        /// </summary>
        /// <param name="id">The id of the device type.</param>
        /// <returns>The<see cref="ActionResult"/>.</returns>
        public void Delete(int id);
    }
}
