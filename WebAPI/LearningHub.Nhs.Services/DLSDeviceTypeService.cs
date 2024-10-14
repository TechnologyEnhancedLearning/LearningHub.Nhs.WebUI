namespace LearningHub.Nhs.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.DLS;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The DLS Device Type Service.
    /// </summary>
    public class DLSDeviceTypeService : IDLSDeviceTypeService
    {
        private readonly IDLSDeviceTypeRepository dlsDeviceTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DLSDeviceTypeService"/> class.
        /// </summary>
        /// <param name="dlsDeviceTypeRepository">The DLSDeviceTypeRepository.</param>
        public DLSDeviceTypeService(
           IDLSDeviceTypeRepository dlsDeviceTypeRepository)
        {
            this.dlsDeviceTypeRepository = dlsDeviceTypeRepository;
        }

        /// <inheritdoc/>
        public void Create(string deviceType)
        {
            this.dlsDeviceTypeRepository.Create(deviceType);
        }

        /// <inheritdoc/>
        public void Delete(int id)
        {
            this.dlsDeviceTypeRepository.Delete(id);
        }

        /// <inheritdoc/>
        // public ActionResult Details(int id)
        public DeviceTypes Details(int id)
        {
            return this.dlsDeviceTypeRepository.GetDetailsById(id);
        }

        /// <inheritdoc/>
        public void Edit(int id, string deviceType)
        {
            this.dlsDeviceTypeRepository.UpdateById(id, deviceType);
        }
    }
}
