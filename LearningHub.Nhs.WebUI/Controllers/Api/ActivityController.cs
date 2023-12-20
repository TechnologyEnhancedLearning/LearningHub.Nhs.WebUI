// <copyright file="ActivityController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The activity controller.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService activityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityController"/> class.
        /// </summary>
        /// <param name="activityService">The activityService<see cref="IActivityService"/>.</param>
        public ActivityController(IActivityService activityService)
        {
            this.activityService = activityService;
        }

        /// <summary>
        /// Creates the assessment resource activity.
        /// </summary>
        /// <param name="createAssessmentResourceActivityViewModel">The createAssessmentResourceActivityViewModel<see cref="CreateMediaResourceActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateAssessmentResourceActivity")]
        public async Task<ActionResult> CreateAssessmentResourceActivity([FromBody] CreateAssessmentResourceActivityViewModel createAssessmentResourceActivityViewModel)
        {
            var validationResult = await this.activityService.CreateAssessmentResourceActivityAsync(createAssessmentResourceActivityViewModel);

            return this.Ok(validationResult);
        }

        /// <summary>
        /// Creates a single assessment resource activity interaction.
        /// </summary>
        /// <param name="createAssessmentResourceActivityInteractionModel">The assessment resource activity interaction record view model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateAssessmentResourceActivityInteraction")]
        public async Task<IActionResult> CreateAssessmentResourceActivityInteraction([FromBody] CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionModel)
        {
            var validationResult = await this.activityService.CreateAssessmentResourceActivityInteractionAsync(createAssessmentResourceActivityInteractionModel);

            return this.Ok(validationResult);
        }

        /// <summary>
        /// The create media resource activity.
        /// </summary>
        /// <param name="createMediaResourceActivityViewModel">The createMediaResourceActivityViewModel<see cref="CreateMediaResourceActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateMediaResourceActivity")]
        public async Task<ActionResult> CreateMediaResourceActivity([FromBody] CreateMediaResourceActivityViewModel createMediaResourceActivityViewModel)
        {
            var validationResult = await this.activityService.CreateMediaResourceActivityAsync(createMediaResourceActivityViewModel);

            return this.Ok(validationResult);
        }

        /// <summary>
        /// The create media resource activity interaction.
        /// </summary>
        /// <param name="createMediaResourceActivityInteractionViewModel">The createMediaResourceActivityInteractionViewModel<see cref="CreateMediaResourceActivityInteractionViewModel"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateMediaResourceActivityInteraction")]
        public async Task<ActionResult> CreateMediaResourceActivityInteraction([FromBody] CreateMediaResourceActivityInteractionViewModel createMediaResourceActivityInteractionViewModel)
        {
            var validationResult = await this.activityService.CreateMediaResourceActivityInteractionAsync(createMediaResourceActivityInteractionViewModel);

            return this.Ok(validationResult);
        }

        /// <summary>
        /// The create resource activity.
        /// </summary>
        /// <param name="createResourceActivityViewModel">The createResourceActivityViewModel<see cref="CreateResourceActivityViewModel"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost]
        [Route("CreateResourceActivity")]
        public async Task<ActionResult> CreateResourceActivity([FromBody] CreateResourceActivityViewModel createResourceActivityViewModel)
        {
            var validationResult = await this.activityService.CreateResourceActivityAsync(createResourceActivityViewModel);

            return this.Ok(validationResult);
        }

        /// <summary>
        /// A single web service call that updates the .
        /// </summary>
        /// <param name="combinedViewModel">The create resource activity interaction view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("CreateResourceActivityAndMediaInteraction")]
        public async Task<ActionResult> CreateResourceActivityAndMediaInteraction([FromBody] CreateResourceActivityAndInteractionViewModel combinedViewModel)
        {
            // Create the media interaction first.
            var validationResult = await this.activityService.CreateMediaResourceActivityInteractionAsync(combinedViewModel.CreateMediaResourceActivityInteractionViewModel);

            // If that succeeded record the activity end.
            if (validationResult.IsValid)
            {
                validationResult = await this.activityService.CreateResourceActivityAsync(combinedViewModel.CreateResourceActivityViewModel);
            }

            return this.Ok(validationResult);
        }
    }
}
