// <copyright file="RatingController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Rating operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class RatingController : ApiControllerBase
    {
        /// <summary>
        /// The rating service.
        /// </summary>
        private readonly IRatingService ratingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="ratingService">The rating service.</param>
        /// <param name="logger">The logger.</param>
        public RatingController(
            IUserService userService,
            IRatingService ratingService,
            ILogger<RatingController> logger)
            : base(userService, logger)
        {
            this.ratingService = ratingService;
        }

        /// <summary>
        /// Gets the rating summary for the entity.
        /// </summary>
        /// <param name="entityVersionId">The entity version id. Currently this is always a ResourceVersionId, but might be a NodeVersionId in the future.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetRatingSummary/{entityVersionId}")]
        public async Task<IActionResult> GetRatingSummary(int entityVersionId)
        {
            var ratingSummary = await this.ratingService.GetRatingSummary(this.CurrentUserId, entityVersionId);
            return this.Ok(ratingSummary);
        }

        /// <summary>
        /// Create a new rating.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateRating")]
        public async Task<IActionResult> CreateRating(RatingViewModel ratingViewModel)
        {
            var vr = await this.ratingService.CreateRating(this.CurrentUserId, ratingViewModel);

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
        /// Update a rating. Can be performed on ratings for any minor version of a major version of the resource.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("UpdateRating")]
        public async Task<IActionResult> UpdateRating(RatingViewModel ratingViewModel)
        {
            var vr = await this.ratingService.UpdateRating(this.CurrentUserId, ratingViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }
    }
}
