// <copyright file="CountryController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="CountryController" />.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private ICountryService countryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryController"/> class.
        /// </summary>
        /// <param name="countryService">Counntry service.</param>
        public CountryController(ICountryService countryService)
        {
            this.countryService = countryService;
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
            var country = await this.countryService.GetByIdAsync(id);
            return this.Ok(country);
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
            var countries = await this.countryService.GetFilteredAsync(filter);
            return this.Ok(countries);
        }
    }
}
