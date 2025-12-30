using LearningHub.Nhs.Models.JiraRoadmap;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.HttpClients
{
    /// <summary>
    /// The IJiraApiHttpClient.
    /// </summary>
    public interface IJiraApiHttpClient
    {
        /// <summary>
        /// GetComponentMetadata.
        /// </summary>
        /// <returns></returns>
        Task<List<ComponentMetadata>> GetComponentMetadata();

        /// <summary>
        /// GetAllPublicIssues.
        /// </summary>
        /// <returns></returns>
        Task<JiraSearchResponse> GetAllPublicIssues(string jql);
    }
}
