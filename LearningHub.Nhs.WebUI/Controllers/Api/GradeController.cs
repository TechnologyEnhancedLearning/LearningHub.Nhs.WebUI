// <copyright file="GradeController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="GradeController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private IGradeService gradeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeController"/> class.
        /// </summary>
        /// <param name="gradeService">Grade service.</param>
        public GradeController(IGradeService gradeService)
        {
            this.gradeService = gradeService;
        }

        /// <summary>
        /// The GetGradesForJobRoleAsync.
        /// </summary>
        /// <param name="jobRoleId">Job role id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetGradesForJobRole/{jobRoleId}")]
        public async Task<ActionResult> GetGradesForJobRoleAsync(int jobRoleId)
        {
            var grades = await this.gradeService.GetGradesForJobRoleAsync(jobRoleId);
            return this.Ok(grades);
        }
    }
}
