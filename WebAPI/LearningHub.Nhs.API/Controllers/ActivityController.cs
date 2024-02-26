namespace LearningHub.Nhs.Api.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Activity operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class ActivityController : ApiControllerBase
    {
        /// <summary>
        /// The activity service.
        /// </summary>
        private readonly IActivityService activityService;

        /// <summary>
        /// The resource service.
        /// </summary>
        private readonly IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityController"/> class.
        /// </summary>
        /// <param name="userService">The elfh user service.</param>
        /// <param name="activityService">The activity service.</param>
        /// <param name="resourceService">The resource service.</param>
        /// <param name="logger">The logger.</param>
        public ActivityController(
            IUserService userService,
            IActivityService activityService,
            IResourceService resourceService,
            ILogger<ActivityController> logger)
            : base(userService, logger)
        {
            this.resourceService = resourceService;
            this.activityService = activityService;
        }

        /// <summary>
        ///  Create Resource Activity.
        /// </summary>
        /// <param name="createResourceActivityViewModel">The details of the resource activity record.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateResourceActivity")]
        public async Task<IActionResult> CreateResourceActivity(CreateResourceActivityViewModel createResourceActivityViewModel)
        {
            var vr = await this.activityService.CreateResourceActivity(this.CurrentUserId, createResourceActivityViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create Media Resource Activity.
        /// </summary>
        /// <param name="createMediaResourceActivityViewModel">The media resource activity record view model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateMediaResourceActivity")]
        public async Task<IActionResult> CreateMediaResourceActivity(CreateMediaResourceActivityViewModel createMediaResourceActivityViewModel)
        {
            var vr = await this.activityService.CreateMediaResourceActivity(this.CurrentUserId, createMediaResourceActivityViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create Assessment Resource Activity.
        /// </summary>
        /// <param name="createAssessmentResourceActivityViewModel">The media resource activity record view model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateAssessmentResourceActivity")]
        public async Task<IActionResult> CreateAssessmentResourceActivity(CreateAssessmentResourceActivityViewModel createAssessmentResourceActivityViewModel)
        {
            var vr = await this.activityService.CreateAssessmentResourceActivity(this.CurrentUserId, createAssessmentResourceActivityViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create Media Resource Activity.
        /// </summary>
        /// <param name="createMediaResourceActivityInteractionModel">The media resource activity interaction record view model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateMediaResourceActivityInteraction")]
        public async Task<IActionResult> CreateMediaResourceActivityInteraction(CreateMediaResourceActivityInteractionViewModel createMediaResourceActivityInteractionModel)
        {
            var vr = await this.activityService.CreateMediaResourceActivityInteraction(this.CurrentUserId, createMediaResourceActivityInteractionModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create Assessment Resource Activity.
        /// </summary>
        /// <param name="createAssessmentResourceActivityInteractionModel">The assessment resource activity interaction record view model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateAssessmentResourceActivityInteraction")]
        public async Task<IActionResult> CreateAssessmentResourceActivityInteraction(CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionModel)
        {
            var assessmentResourceVersionId =
                await this.activityService.GetAssessmentResourceIdByActivity(
                    createAssessmentResourceActivityInteractionModel.AssessmentResourceActivityId);

            // Get AnswerInOrder without loading entire assessment object tree.
            var assessmentResourceVersion = await this.resourceService.GetAssessmentBasicDetailsByIdAsync(assessmentResourceVersionId);

            var vr = await this.activityService.CreateAssessmentResourceActivityInteraction(this.CurrentUserId, createAssessmentResourceActivityInteractionModel, assessmentResourceVersion.AnswerInOrder);

            if (vr.IsValid)
            {
                AssessmentViewModel assessmentViewModel = new AssessmentViewModel();

                // Only load the assessment if AnswerInOrder is true. Not needed otherwise.
                if (assessmentResourceVersion.AnswerInOrder)
                {
                    assessmentViewModel = await this.resourceService.GetAssessmentContentUpToQuestion(
                        assessmentResourceVersionId,
                        createAssessmentResourceActivityInteractionModel.QuestionNumber + 1,
                        createAssessmentResourceActivityInteractionModel.Answers.ToList());
                }

                return this.Ok(assessmentViewModel);
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Update Scorm Activity.
        /// </summary>
        /// <param name="updateScormActivityViewModel">The scorm activity.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("UpdateScormActivity")]
        public async Task<IActionResult> UpdateScormActivity(ScormActivityViewModel updateScormActivityViewModel)
        {
            var scormUpdateResponse = await this.activityService.UpdateScormActivity(this.CurrentUserId, updateScormActivityViewModel);

            if (scormUpdateResponse.IsValid)
            {
                return this.Ok(scormUpdateResponse);
            }
            else
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// Complete Scorm Activity.
        /// </summary>
        /// <param name="completeScormActivityViewModel">The scorm activity.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CompleteScormActivity")]
        public async Task<IActionResult> CompleteScormActivity(ScormActivityViewModel completeScormActivityViewModel)
        {
            var vr = await this.activityService.CompleteScormActivity(this.CurrentUserId, completeScormActivityViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Launch Scorm Activity.
        /// </summary>
        /// <param name="launchScormActivityViewModel">The scorm activity.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("launchScormActivity")]
        public async Task<IActionResult> LaunchScormActivity(LaunchScormActivityViewModel launchScormActivityViewModel)
        {
            var scormActivity = await this.activityService.LaunchScormActivity(this.CurrentUserId, launchScormActivityViewModel);

            return this.Ok(scormActivity);
        }

        /// <summary>
        /// This method cleans up incomplete media activities. Required if for any reason, the end of the user's activity was not recorded normally. For example - browser crash, power loss, connection loss.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CleanUpIncompleteActivities")]
        public async Task<IActionResult> CleanUpIncompleteActivitiesAsync([FromBody] int userId)
        {
            await this.activityService.CleanUpIncompleteActivitiesAsync(userId);

            return this.Ok();
        }

        /// <summary>
        /// This method cleans up incomplete media activities. Required if for any reason, the end of the user's activity was not recorded normally. For example - browser crash, power loss, connection loss.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("ResolveScormActivity/{userId}")]
        public async Task<IActionResult> ResolveScormActivity(int userId)
        {
            await this.activityService.ResolveScormActivity(userId);

            return this.Ok();
        }

        /// <summary>
        /// Launch Scorm Activity.
        /// </summary>
        /// <param name="scormActivityId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CheckUserScormActivitySuspendDataToBeCleared/{scormActivityId}/{resourceVersionId}")]
        public async Task<IActionResult> CheckUserScormActivitySuspendDataToBeCleared(int scormActivityId, int resourceVersionId)
        {
            var clear = await this.activityService.CheckUserScormActivitySuspendDataToBeCleared(scormActivityId, resourceVersionId);

            return this.Ok(clear);
        }
    }
}
