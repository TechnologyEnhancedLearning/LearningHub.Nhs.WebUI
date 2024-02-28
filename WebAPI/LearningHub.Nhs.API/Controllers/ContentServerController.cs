namespace LearningHub.Nhs.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The ScormContentServerController. Used by the SCORM content server web app to retrieve mappings of ESR links to internal content folders.
    /// This is separate from the ResourceController because these methods need to be called anonymously via the AuthorizeOrCallFromLH attribute.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContentServerController : ApiControllerBase
    {
        /// <summary>
        /// The scorm content server service.
        /// </summary>
        private IScormContentServerService scormContentServerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentServerController"/> class.
        /// </summary>
        /// <param name="userService">The UserService<see cref="IUserService"/>.</param>
        /// <param name="scormContentServerService">The scormContentServerService<see cref="IScormContentServerService"/>.</param>
        /// <param name="logger">The logger.</param>
        public ContentServerController(
            IUserService userService,
            IScormContentServerService scormContentServerService,
            ILogger<ContentServerController> logger)
            : base(userService, logger)
        {
            this.scormContentServerService = scormContentServerService;
        }

        /// <summary>
        /// The GetScormContentDetailsByEsrLinkUrl.
        /// </summary>
        /// <param name="externalUrl">The externalUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("GetContentDetailsByExternalUrl")]
        public IActionResult GetContentDetailsByExternalUrl([FromBody] string externalUrl)
        {
            string decodedUrl = HttpUtility.UrlDecode(externalUrl);

            var details = this.scormContentServerService.GetContentDetailsByExternalUrl(decodedUrl);

            return this.Ok(details);
        }

        /// <summary>
        /// The GetContentDetailsByExternalIdentifier.
        /// </summary>
        /// <param name="externalReference">The externalIdentifier<see cref="Guid"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetContentDetailsByExternalReference/{externalReference}")]
        public IActionResult GetContentDetailsByExternalReference(string externalReference)
        {
            var details = this.scormContentServerService.GetContentDetailsByExternalReference(externalReference);

            return this.Ok(details);
        }

        /// <summary>
        /// The LogResourceReferenceEventAsync.
        /// </summary>
        /// <param name="resourceReferenceEventViewModel">The ResourceReferenceEventViewModel<see cref="ResourceReferenceEventViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("LogResourceReferenceEvent")]
        public async Task<IActionResult> LogResourceReferenceEventAsync([FromBody] ResourceReferenceEventViewModel resourceReferenceEventViewModel)
        {
            await this.scormContentServerService.LogResourceReferenceEventAsync(resourceReferenceEventViewModel);
            return this.Ok();
        }
    }
}
