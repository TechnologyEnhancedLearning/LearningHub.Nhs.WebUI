namespace LearningHub.NHS.OpenAPI.Controllers
{
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    /// <summary>
    /// Moodle operations.
    /// </summary>
    [Route("Moodle")]
    [ApiController]
    [Authorize]
    public class MoodleController : Controller
    {
        private readonly IMoodleApiService moodleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleController"/> class.
        /// </summary>
        /// <param name="moodleService">The moodle service.</param>
        public MoodleController(IMoodleApiService moodleService)
        {
            this.moodleService = moodleService;
        }

        /// <summary>
        /// The GetMoodleUserId.
        /// </summary>
        /// <param name="currentUserId">The LH user id.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetMoodleUserId/{currentUserId?}")]
        public async Task<IActionResult> GetMoodleUserId(int? currentUserId)
        {
            if (currentUserId.HasValue)
            {
                var moodleUser = await this.moodleService.GetMoodleUserIdByUsernameAsync(currentUserId.Value);
                return this.Ok(moodleUser);
            }
            else
            {
                return this.Ok(0);
            }
        }

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="currentUserId">Moodle user id.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetEnrolledCourses/{currentUserId?}")]
        public async Task<IActionResult> GetEnrolledCoursesAsync(int? currentUserId, int pageNumber = 0)
        {
            if (currentUserId.HasValue)
            {
                var entrolledCourses = await this.moodleService.GetEnrolledCoursesAsync(currentUserId.Value, pageNumber);
                return this.Ok(entrolledCourses);
            }
            else
            {
                return this.Ok(0);
            }
        }
    }
}
