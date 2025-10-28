namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The activity controller.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MyLearningController : ControllerBase
    {
        /// <summary>
        /// The _activity service.
        /// </summary>
        private IMyLearningService myLearningService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningController"/> class.
        /// </summary>
        /// <param name="myLearningService">The myLearning service.</param>
        public MyLearningController(IMyLearningService myLearningService)
        {
            this.myLearningService = myLearningService;
        }

        /// <summary>
        /// Gets the detailed activity data.
        /// </summary>
        /// <param name="requestModel">The request model - filter settings.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("GetActivityDetailed")]
        public async Task<ActionResult> GetActivityDetailed([FromBody] MyLearningRequestModel requestModel)
        {
            var activity = await this.myLearningService.GetActivityDetailed(requestModel);

            return this.Ok(activity);
        }

        /// <summary>
        /// Gets the detailed activity data.
        /// </summary>
        /// <param name="requestModel">The request model - filter settings.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("GetUserRecentMyLearningActivities")]
        public async Task<ActionResult> GetUserRecentMyLearningActivities([FromBody] MyLearningRequestModel requestModel)
        {
            var activity = await this.myLearningService.GetUserRecentMyLearningActivities(requestModel);

            return this.Ok(activity);
        }

        /// <summary>
        /// Gets the detailed activity data.
        /// </summary>
        /// <param name="requestModel">The request model - filter settings.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("GetUserLearningHistory")]
        public async Task<ActionResult> GetUserLearningHistory([FromBody] MyLearningRequestModel requestModel)
        {
            var activity = await this.myLearningService.GetUserLearningHistory(requestModel);

            return this.Ok(activity);
        }

        /// <summary>
        /// Gets the played segment data for the progress modal in My Learning screen.
        /// </summary>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("GetPlayedSegments/{resourceId}/{majorVersion}")]
        public async Task<IActionResult> GetPlayedSegments(int resourceId, int majorVersion)
        {
            var activityModel = await this.myLearningService.GetPlayedSegments(resourceId, majorVersion);
            return this.Ok(activityModel);
        }

        /// <summary>
        /// The CheckCertificateAvailability.
        /// </summary>
        /// <param name="resourceReferenceId">The reesource reference id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("CheckCertificateAvailability/{resourceReferenceId}")]
        public async Task<bool> CheckCertificateAvailabilityAsync(int resourceReferenceId)
        {
            var certDetails = await this.myLearningService.GetResourceCertificateDetails(resourceReferenceId);
            if (certDetails.Item2 != null && certDetails.Item2.IsCurrentResourceVersion)
            {
                var activityDetailedItemViewModel = new ActivityDetailedItemViewModel(certDetails.Item2);
                if (activityDetailedItemViewModel != null && ViewActivityHelper.CanDownloadCertificate(activityDetailedItemViewModel))
                {
                    return true;
                }
            }

            return false;
        }
    }
}