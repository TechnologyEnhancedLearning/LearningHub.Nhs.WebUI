// <copyright file="JobRoleController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="JobRoleController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JobRoleController : ControllerBase
    {
        private IJobRoleService jobRoleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRoleController"/> class.
        /// </summary>
        /// <param name="jobRoleService">Job role service.</param>
        public JobRoleController(IJobRoleService jobRoleService)
        {
            this.jobRoleService = jobRoleService;
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
            var jobRole = await this.jobRoleService.GetByIdAsync(id);
            return this.Ok(jobRole);
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
            var jobRoles = await this.jobRoleService.GetFilteredAsync(filter);
            return this.Ok(jobRoles);
        }

        /// <summary>
        /// The ValidateMedicalCouncilNumber.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <param name="medicalCouncilId">Medical council Id.</param>
        /// <param name="medicalCouncilNumber">Medical council number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("ValidateMedicalCouncilNumber/{lastName}/{medicalCouncilId}/{medicalCouncilNumber}")]
        public async Task<ActionResult> ValidateMedicalCouncilNumber(string lastName, int medicalCouncilId, string medicalCouncilNumber)
        {
            var errorMesssage = await this.jobRoleService.ValidateMedicalCouncilNumber(lastName, medicalCouncilId, medicalCouncilNumber);
            return this.Ok(errorMesssage);
        }
    }
}
