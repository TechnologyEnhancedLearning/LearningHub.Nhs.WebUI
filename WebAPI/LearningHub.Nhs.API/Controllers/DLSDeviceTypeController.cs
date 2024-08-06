namespace LearningHub.Nhs.Api.Controllers
{
    using LearningHub.Nhs.Api.DLSEntities;
    using LearningHub.Nhs.Repository;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The DLS Device Types Controller.
    /// </summary>
    public class DLSDeviceTypeController : Controller
    {
        private readonly DLSDbContext dlsDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DLSDeviceTypeController"/> class.
        /// </summary>
        /// <param name="context">The DLS DB context.</param>
        public DLSDeviceTypeController(DLSDbContext context)
        {
            this.dlsDbContext = context;
        }

        /// <summary>
        /// GET Details.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult Details(int id)
        {
            try
            {
                var deviceTypes = this.dlsDbContext.DeviceTypes.Find(id);

                return this.Ok(deviceTypes);
            }
            catch
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// POST Create a new DLSDeviceType.
        /// </summary>
        /// <param name="deviceType">The device type.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        public ActionResult Create(string deviceType)
        {
            try
            {
                var deviceTypes = new DeviceTypes
                {
                    DeviceType = deviceType,
                };
                this.dlsDbContext.DeviceTypes.Add(deviceTypes);

                this.dlsDbContext.SaveChanges();

                return this.Ok();
            }
            catch
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// POST Edit an existing DLSDeviceType.
        /// </summary>
        /// <param name = "id" > The id of the device type.</param>
        /// <param name = "deviceType" > The device type.</param>
        /// <returns>The<see cref="ActionResult"/>.</returns>
        [HttpPut]
        public ActionResult Edit(int id, string deviceType)
        {
            try
            {
                var deviceTypes = new DeviceTypes
                {
                    DeviceTypeId = id,
                    DeviceType = deviceType,
                };
                this.dlsDbContext.DeviceTypes.Update(deviceTypes);

                this.dlsDbContext.SaveChanges();

                return this.Ok();
            }
            catch
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// DELETE an existing DLS DeviceType.
        /// </summary>
        /// <param name="id">The id of the device type.</param>
        /// <returns>The<see cref="ActionResult"/>.</returns>
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                var deviceTypes = new DeviceTypes
                {
                    DeviceTypeId = id,
                };
                this.dlsDbContext.DeviceTypes.Remove(deviceTypes);

                this.dlsDbContext.SaveChanges();

                return this.Ok();
            }
            catch
            {
                return this.BadRequest();
            }
        }
    }
}
