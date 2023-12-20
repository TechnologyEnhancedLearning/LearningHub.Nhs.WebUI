// <copyright file="InternalSystemController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Maintenance;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Internal System Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class InternalSystemController : ApiControllerBase
    {
        private readonly IUserService userService;
        private readonly IInternalSystemService internalSystemService;
        private readonly ILogger<InternalSystemController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSystemController"/> class.
        /// </summary>
        /// <param name="userService">The userService.</param>
        /// <param name="internalSystemService">The internalSystemService.</param>
        /// <param name="logger">The logger.</param>
        public InternalSystemController(
            IUserService userService,
            IInternalSystemService internalSystemService,
            ILogger<InternalSystemController> logger)
            : base(userService, logger)
        {
            this.userService = userService;
            this.internalSystemService = internalSystemService;
            this.logger = logger;
        }

        /// <summary>
        /// Gets all internal systems.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await this.internalSystemService.GetAllAsync();
            return this.Ok(result);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var internalSystem = await this.internalSystemService.GetByIdAsync(id);

            return this.Ok(internalSystem);
        }

        /// <summary>
        /// Toggles the internalSystem offline status.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut]
        [Route("ToggleOfflineStatus/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleOfflineStatus(int id)
        {
            var internalSystem = await this.internalSystemService.ToggleOfflineStatusAsync(id, this.CurrentUserId);
            return this.Ok(internalSystem);
        }
    }
}
