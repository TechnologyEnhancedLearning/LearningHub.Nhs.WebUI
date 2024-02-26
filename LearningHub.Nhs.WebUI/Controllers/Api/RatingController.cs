namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The rating controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : BaseApiController
    {
        private readonly IUserService userService;
        private IRatingService ratingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingController"/> class.
        /// </summary>
        /// <param name="ratingService">The rating service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="logger">Logger.</param>
        public RatingController(IRatingService ratingService, IUserService userService, ILogger<RatingController> logger)
            : base(logger)
        {
            this.ratingService = ratingService;
            this.userService = userService;
        }

        /// <summary>
        /// Gets the rating summary for the entity.
        /// </summary>
        /// <param name="entityVersionId">The entity version id. Currently this is always a ResourceVersionId, but might be a NodeVersionId in the future.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetRatingSummary/{entityVersionId}")]
        public async Task<ActionResult> GetRatingSummary(int entityVersionId)
        {
            var ratingSummary = await this.ratingService.GetRatingSummaryAsync(entityVersionId);
            return this.Ok(ratingSummary);
        }

        /// <summary>
        /// Create a new rating.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateRating")]
        public async Task<ActionResult> CreateRating([FromBody] RatingViewModel ratingViewModel)
        {
            var userEmployment = await this.userService.GetPrimaryUserEmploymentForUser(this.CurrentUserId);
            ratingViewModel.JobRoleId = userEmployment.JobRoleId.GetValueOrDefault();
            ratingViewModel.LocationId = userEmployment.LocationId;

            var validationResult = await this.ratingService.CreateRatingAsync(ratingViewModel);

            return this.Ok(validationResult);
        }

        /// <summary>
        /// Update a new rating.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("UpdateRating")]
        public async Task<ActionResult> UpdateRating([FromBody] RatingViewModel ratingViewModel)
        {
            var userEmployment = await this.userService.GetPrimaryUserEmploymentForUser(this.CurrentUserId);
            ratingViewModel.JobRoleId = userEmployment.JobRoleId.GetValueOrDefault();
            ratingViewModel.LocationId = userEmployment.LocationId;
            var validationResult = await this.ratingService.UpdateRatingAsync(ratingViewModel);

            return this.Ok(validationResult);
        }
    }
}