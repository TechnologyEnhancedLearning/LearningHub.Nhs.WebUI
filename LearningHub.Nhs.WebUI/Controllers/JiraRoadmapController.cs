namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="JiraRoadmapController"/>.
    /// </summary>
    public class JiraRoadmapController : BaseController
    {
        private readonly IJiraRoadmapService jiraRoadmapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraRoadmapController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">hostingEnvironment.</param>
        /// <param name="logger">logger.</param>
        /// <param name="settings">settings.</param>
        /// <param name="httpClientFactory">httpClientFactory.</param>
        /// <param name="jiraRoadmapService">jiraRoadmapService.</param>
        public JiraRoadmapController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<ResourceController> logger,
            IOptions<Settings> settings,
            IHttpClientFactory httpClientFactory,
            IJiraRoadmapService jiraRoadmapService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.jiraRoadmapService = jiraRoadmapService;
        }

        /// <summary>
        /// Returns public roadmap issues (data only endpoint for WebUI).
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("getRoadmapIssues")]
        public async Task<IActionResult> GetRoadmapIssues()
        {
            var roadmapResponse = await this.jiraRoadmapService.GetPublicRoadmapIssues();
            return this.Json(roadmapResponse);
        }
     }
}
