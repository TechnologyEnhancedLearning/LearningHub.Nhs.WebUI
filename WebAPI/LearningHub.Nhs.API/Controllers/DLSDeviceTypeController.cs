namespace LearningHub.Nhs.Api.Controllers
{
    using LearningHub.Nhs.Models.DLS;
    using LearningHub.Nhs.Repository;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The DLS Device Types Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DLSDeviceTypeController : ApiControllerBase
    {
        /// <summary>
        /// The MyDLSDeviceType service.
        /// </summary>
        private readonly IDLSDeviceTypeService dlsDeviceTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DLSDeviceTypeController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="dlsDeviceTypeService">The DLS DeviceTypeService.</param>
        /// <param name="logger">The logger.</param>
        public DLSDeviceTypeController(IUserService userService, IDLSDeviceTypeService dlsDeviceTypeService, ILogger<DLSDeviceTypeController> logger)
            : base(userService, logger)
        {
            this.dlsDeviceTypeService = dlsDeviceTypeService;
        }

        /// <summary>
        /// Get Details.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Details(int id)
        {
            try
            {
                var deviceTypes = this.dlsDeviceTypeService.Details(id);

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
                this.dlsDeviceTypeService.Create(deviceType);

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
                this.dlsDeviceTypeService.Edit(id, deviceType);

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
                this.dlsDeviceTypeService.Delete(id);
                return this.Ok();
            }
            catch
            {
                return this.BadRequest();
            }
        }
    }
}
