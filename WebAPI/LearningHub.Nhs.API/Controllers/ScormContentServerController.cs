// <copyright file="ScormContentServerController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

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
    public class ScormContentServerController : ApiControllerBase
    {
        /// <summary>
        /// The scorm content server service.
        /// </summary>
        private IScormContentServerService scormContentServerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentServerController"/> class.
        /// </summary>
        /// <param name="userService">The UserService<see cref="IUserService"/>.</param>
        /// <param name="scormContentServerService">The scormContentServerService<see cref="IScormContentServerService"/>.</param>
        /// <param name="logger">The logger.</param>
        public ScormContentServerController(
            IUserService userService,
            IScormContentServerService scormContentServerService,
            ILogger<ScormContentServerController> logger)
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
        [Route("GetScormContentDetailsByExternalUrl")]
        public IActionResult GetScormContentDetailsByExternalUrl([FromBody] string externalUrl)
        {
            string decodedUrl = HttpUtility.UrlDecode(externalUrl);

            var details = this.scormContentServerService.GetScormContentDetailsByExternalUrl(decodedUrl);

            return this.Ok(details);
        }

        /// <summary>
        /// The GetScormContentDetailsByExternalIdentifier.
        /// </summary>
        /// <param name="externalReference">The externalIdentifier<see cref="Guid"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetScormContentDetailsByExternalReference/{externalReference}")]
        public IActionResult GetScormContentDetailsByExternalReference(string externalReference)
        {
            var details = this.scormContentServerService.GetScormContentDetailsByExternalReference(externalReference);

            return this.Ok(details);
        }

        /// <summary>
        /// The LogScormResourceReferenceEventAsync.
        /// </summary>
        /// <param name="scormResourceReferenceEventViewModel">The ScormResourceReferenceEventViewModel<see cref="ScormResourceReferenceEventViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("LogScormResourceReferenceEvent")]
        public async Task<IActionResult> LogScormResourceReferenceEventAsync([FromBody] ScormResourceReferenceEventViewModel scormResourceReferenceEventViewModel)
        {
            await this.scormContentServerService.LogScormResourceReferenceEventAsync(scormResourceReferenceEventViewModel);
            return this.Ok();
        }
    }
}
