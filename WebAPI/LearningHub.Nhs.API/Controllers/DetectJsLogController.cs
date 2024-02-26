namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// DetectJsLog operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class DetectJsLogController : ApiControllerBase
    {
        private readonly IDetectJsLogService detectJsLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectJsLogController"/> class.
        /// </summary>
        /// <param name="userService">IUserService.</param>
        /// <param name="detectJsLogService">IDetectJsLogService.</param>
        /// <param name="logger">The logger.</param>
        public DetectJsLogController(IUserService userService, IDetectJsLogService detectJsLogService, ILogger<DetectJsLogController> logger)
            : base(userService, logger)
        {
            this.detectJsLogService = detectJsLogService;
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="jsEnabled">Js enabled request count.</param>
        /// <param name="jsDisabled">Js disabled request count.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateAsync(long jsEnabled, long jsDisabled)
        {
            await this.detectJsLogService.UpdateAsync(jsEnabled, jsDisabled);
            return this.Ok();
        }
    }
}