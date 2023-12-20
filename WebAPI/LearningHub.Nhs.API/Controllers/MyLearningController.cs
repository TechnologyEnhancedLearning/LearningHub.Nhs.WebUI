// <copyright file="MyLearningController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// MyLearning operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyLearningController : ApiControllerBase
    {
        /// <summary>
        /// The MyLearning service.
        /// </summary>
        private readonly IMyLearningService myLearningService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="myLearningService">
        /// The myLearning service.
        /// </param>
        /// <param name="logger">The logger.</param>
        public MyLearningController(
            IUserService userService,
            IMyLearningService myLearningService,
            ILogger<MyLearningController> logger)
            : base(userService, logger)
        {
            this.myLearningService = myLearningService;
        }

        /// <summary>
        /// Gets the activity records for the detailed activity tab of My Learning screen.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("GetActivityDetailed")]
        public async Task<IActionResult> GetActivityDetailed([FromBody] MyLearningRequestModel requestModel)
        {
            var activityModel = await this.myLearningService.GetActivityDetailed(this.CurrentUserId, requestModel);
            return this.Ok(activityModel);
        }

        /// <summary>
        /// Gets the played segment data for the progress modal in My Learning screen.
        /// </summary>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetPlayedSegments/{resourceId}/{majorVersion}")]
        public async Task<IActionResult> GetPlayedSegments(int resourceId, int majorVersion)
        {
            var segments = await this.myLearningService.GetPlayedSegments(this.CurrentUserId, resourceId, majorVersion);
            return this.Ok(segments);
        }

        /// <summary>
        /// Gets the resource certificate details of a resource reference.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <param name="minorVersion">The minorVersion.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetResourceCertificateDetails/{resourceReferenceId}")]
        [Route("GetResourceCertificateDetails/{resourceReferenceId}/{majorVersion}/{minorVersion}/{userId}")]
        public async Task<IActionResult> GetResourceCertificateDetails(int resourceReferenceId, int majorVersion = 0, int minorVersion = 0, int userId = 0)
        {
            var certificateDetails = await this.myLearningService.GetResourceCertificateDetails((userId == 0) ? this.CurrentUserId : (int)userId, resourceReferenceId, majorVersion, minorVersion);
            return this.Ok(certificateDetails);
        }
    }
}
