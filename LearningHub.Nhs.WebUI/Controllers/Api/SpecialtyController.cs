namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="SpecialtyController" />.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtyController : ControllerBase
    {
        private ISpecialtyService specialtyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialtyController"/> class.
        /// </summary>
        /// <param name="specialtyService">Specialty service.</param>
        public SpecialtyController(ISpecialtyService specialtyService)
        {
            this.specialtyService = specialtyService;
        }

        /// <summary>
        /// The GetSpecialties.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetSpecialties")]
        public async Task<ActionResult> GetSpecialties()
        {
            var specialties = await this.specialtyService.GetSpecialtiesAsync();
            return this.Ok(specialties);
        }
    }
}
