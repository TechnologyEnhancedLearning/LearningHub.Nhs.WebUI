// <copyright file="RegionController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="RegionController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private IRegionService regionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionController"/> class.
        /// </summary>
        /// <param name="regionService">Region service.</param>
        public RegionController(IRegionService regionService)
        {
            this.regionService = regionService;
        }

        /// <summary>
        /// The GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAllAsync()
        {
            var regions = await this.regionService.GetAllAsync();
            return this.Ok(regions);
        }
    }
}
