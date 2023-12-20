// <copyright file="LocationController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="LocationController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private ILocationService locationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationController"/> class.
        /// </summary>
        /// <param name="locationService">Location service.</param>
        public LocationController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        /// <summary>
        /// The GetById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var location = await this.locationService.GetByIdAsync(id);
            return this.Ok(location);
        }

        /// <summary>
        /// The GetFilteredAsync.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetFiltered/{filter}")]
        public async Task<ActionResult> GetFilteredAsync(string filter)
        {
            var locations = await this.locationService.GetFilteredAsync(filter);
            return this.Ok(locations);
        }
    }
}
