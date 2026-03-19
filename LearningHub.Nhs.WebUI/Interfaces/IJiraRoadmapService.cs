namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.JiraRoadmap;

    /// <summary>
    /// Defines the <see cref="IJiraRoadmapService" />.
    /// </summary>
    public interface IJiraRoadmapService
    {
        /// <summary>
        /// Gets public roadmap issues from Jira Open API.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<RoadmapResponseDto> GetPublicRoadmapIssues();
    }
}
