namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Moodle Bridge operations.
    /// </summary>
    [Route("MoodleBridge")]
    [ApiController]
    [Authorize]
    public class MoodleBridgeController : Controller
    {
        private readonly IMoodleBridgeApiService moodleBridgeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleBridgeController"/> class.
        /// </summary>
        /// <param name="moodleBridgeApiService">The moodle bridge service.</param>
        public MoodleBridgeController(IMoodleBridgeApiService moodleBridgeApiService)
        {
            this.moodleBridgeApiService = moodleBridgeApiService;
        }

        /// <summary>
        /// The GetMoodle Instances UserIds.
        /// </summary>
        /// <param name="email">The LH user email.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetUserInstancesByEmail/{email}")]
        public async Task<IActionResult> GetUserInstancesByEmail(string email)
        {
                var moodleUser = await this.moodleBridgeApiService.GetUserInstancesByEmail(email);
                return this.Ok(moodleUser);
        }

        /// <summary>
        /// The GetMoodle Instances UserIds.
        /// </summary>
        /// <param name="email">The LH user email.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdateEmail")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailaddressViewModel updateEmailaddressViewModel)
        {
            var emailUpdateResponse = await this.moodleBridgeApiService.UpdateEmail(updateEmailaddressViewModel);
            return this.Ok(emailUpdateResponse);
        }

        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAllMoodleCategories")]
        public async Task<IActionResult> GetAllMoodleCategoriesAsync()
        {
            var moodleCategories = await this.moodleBridgeApiService.GetAllMoodleCategoriesAsync();
            return this.Ok(moodleCategories);
        }
    }
}
