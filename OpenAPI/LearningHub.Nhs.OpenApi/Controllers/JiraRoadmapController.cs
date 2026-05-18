namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.JiraRoadmap;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The JiraRoadmapController.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("JiraRoadmap")]
    [ApiController]
    public class JiraRoadmapController : OpenApiControllerBase
    {
        private readonly IJiraRoadmapService jiraRoadmapService;

        /// <summary>
        ///  Initializes a new instance of the <see cref="JiraRoadmapController"/> class.
        /// </summary>
        /// <param name="jiraRoadmapService">The jiraRoadmap Service.</param>
        public JiraRoadmapController(IJiraRoadmapService jiraRoadmapService)
        {
            this.jiraRoadmapService = jiraRoadmapService;
        }

        /// <summary>
        /// The GetRoadmapTickets controller, tickets in jira.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetRoadmapTickets")]
        public async Task<IActionResult> GetRoadmapTickets()
        {
            var roadmapResponse = await this.GetRoadmapTicketsAsync();
            return this.Ok(roadmapResponse);
        }

        /// <summary>
        /// Get all tickets marked public and based on components in roadmap.
        /// </summary>
        private async Task<RoadmapResponseDto> GetRoadmapTicketsAsync()
        {
            return await this.jiraRoadmapService.GetPublicRoadmapTickets();
        }
    }
}
