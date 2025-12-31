using LearningHub.Nhs.Models.JiraRoadmap;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// IJiraRoadmapService.
    /// </summary>
    public interface IJiraRoadmapService
    {
        /// <summary>
        /// Get all issues marked public for roadmap.
        /// </summary>
        /// <returns></returns>
        Task<RoadmapResponseDto> GetPublicRoadmapIssues();
    }
}
