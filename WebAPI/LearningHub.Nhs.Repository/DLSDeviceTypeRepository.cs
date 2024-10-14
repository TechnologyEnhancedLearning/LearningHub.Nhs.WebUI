namespace LearningHub.Nhs.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.DLS;
    using LearningHub.Nhs.Repository.Interface;

    /// <summary>
    /// Initializes a new instance of the <see cref="DLSDeviceTypeRepository"/> class.
    /// </summary>
    public class DLSDeviceTypeRepository : IDLSDeviceTypeRepository
    {
        private readonly DLSDbContext dlsDbContext;

        /// <summary>
        /// Initializes a new instance of the  <see cref="DLSDeviceTypeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The DLS DB Context.</param>
        public DLSDeviceTypeRepository(DLSDbContext dbContext)
        {
            this.dlsDbContext = dbContext;
        }

        /// <inheritdoc/>
        public DeviceTypes GetDetailsById(int id)
        {
            return this.dlsDbContext.DeviceTypes.Where(dt => dt.DeviceTypeID == id).FirstOrDefault();
        }

        /// <inheritdoc/>
        public void UpdateById(int id, string deviceType)
        {
            var deviceTypes = new DeviceTypes()
            {
                DeviceTypeID = id,
                DeviceType = deviceType,
            };
            this.dlsDbContext.DeviceTypes.Update(deviceTypes);
            this.dlsDbContext.SaveChanges();
        }

        /// <inheritdoc/>
        public void Create(string deviceType)
        {
            var deviceTypes = new DeviceTypes()
            {
                DeviceType = deviceType,
            };
            this.dlsDbContext.DeviceTypes.Add(deviceTypes);
            this.dlsDbContext.SaveChanges();
        }

        /// <inheritdoc/>
        public void Delete(int id)
        {
            var deviceTypes = new DeviceTypes()
            {
                DeviceTypeID = id,
            };
            this.dlsDbContext.DeviceTypes.Remove(deviceTypes);
            this.dlsDbContext.SaveChanges();
        }
    }
}
