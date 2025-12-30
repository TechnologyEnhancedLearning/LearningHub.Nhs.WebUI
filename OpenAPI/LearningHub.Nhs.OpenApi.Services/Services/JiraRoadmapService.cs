namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.JiraRoadmap;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Options;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// JiraRoadmapService.
    /// </summary>
    public class JiraRoadmapService : IJiraRoadmapService
    {
        private readonly IJiraApiHttpClient jiraApiHttpClient;
        private readonly JiraApiConfig jiraApiConfig;
        private readonly ICachingService cachingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraRoadmapService"/> class.
        /// </summary>
        /// <param name="jiraApiHttpClient">The http client.</param>
        /// <param name="jiraApiConfig">The jira api settings.</param>
        /// <param name="cachingService">The cache service.</param>
        public JiraRoadmapService(IJiraApiHttpClient jiraApiHttpClient, IOptions<JiraApiConfig> jiraApiConfig, ICachingService cachingService)
        {
            this.jiraApiHttpClient = jiraApiHttpClient;
            this.jiraApiConfig = jiraApiConfig.Value;
            this.cachingService = cachingService;
        }

        /// <summary>
        /// Get all issues marked public for roadmap.
        /// </summary>
        /// <returns></returns>
        public async Task<RoadmapResponseDto> GetPublicRoadmapIssues()
        {
            RoadmapResponseDto viewModel = new RoadmapResponseDto();

            string cacheKey = $"JiraApiResponse";
            var roadmapRespone = await this.cachingService.GetAsync<RoadmapResponseDto>(cacheKey);
            if (roadmapRespone.ResponseEnum == CacheReadResponseEnum.Found)
            {
                viewModel = roadmapRespone.Item;
            }
            else
            {
                var allComponents = await this.jiraApiHttpClient.GetComponentMetadata();
                var selectedComponents = allComponents
                    .Where(c => this.jiraApiConfig.ComponentIds.Contains(c.Id))
                    .ToDictionary(c => c.Id,c => c.Description);

                // Build URL query string
                var components = string.Join(",", selectedComponents.Keys);
                string filter = $"component in ({components}) AND 'customfield_11528' = \"'Make public '\"";
                string fields = this.Build(
                "customfield_11529",
                "customfield_11531",
                "status",
                "components");
                var jql = "?jql=" + filter + "&fields=" + fields.ToString() + "&maxResults="+ this.jiraApiConfig.MaxResults;

                var jiraResponse = await this.jiraApiHttpClient.GetAllPublicIssues(jql);

                if (jiraResponse?.Issues.Count > 0)
                {
                    var flatIssuesResponse = jiraResponse.Issues.Select(issue => new
                    {
                        Component = issue.Fields?.Components?.FirstOrDefault()?.Name,
                        ComponentDescription = selectedComponents.ContainsKey(issue.Fields?.Components?.FirstOrDefault()?.Id) ? selectedComponents[issue.Fields?.Components?.FirstOrDefault()?.Id] : null,
                        RoadmapState = this.MapToDisplay(issue.Fields?.Status?.Name),
                        Issue = new RoadmapIssueDto
                        {
                            Summary = issue.Fields?.Summary?.ToString(),
                            Description = issue.Fields?.Description?.ToString()
                        }
                    }).ToList();

                    var componentList = flatIssuesResponse.GroupBy(x => x.Component)
                        .Select(componentGroup => new RoadmapComponentDto
                        {
                            ComponentName = componentGroup.Key,
                            ComponentDescription = componentGroup.First().ComponentDescription,
                            States = componentGroup.GroupBy(x => x.RoadmapState).Select(stateGroup => new RoadmapStateDto
                            {
                                State = stateGroup.Key,
                                Issues = stateGroup.Select(x => x.Issue).ToList()
                            }).ToList()
                        }).ToList();

                    viewModel = new RoadmapResponseDto
                    {
                        Components = componentList
                    };

                    await this.cachingService.SetAsync(cacheKey, viewModel, 1440, false);
                    return viewModel;                    
                }
            }
            return viewModel;
        }

        private string Build(params string[] fields)
        {
            return string.Join(",", fields);
        }

        private string MapToDisplay(string jiraStatus)
        {
            return jiraStatus switch
            {
                // Recently Completed
                "Done" or
                "Ready for Release"
                    => "Recently Completed",

                // Working on Now
                "Blocked" or
                "Test Failed" or
                "Design Complete" or
                "In Progress" or
                "Review" or
                "Work Complete" or
                "Ready for Test" or
                "Testing In Progress"
                    => "Working on Now",

                // Working on Next
                "Next"
                    => "Working on Next",

                // Working on Later
                "To Do"
                    => "Working on Later",

                _ => "Working on Later"
            };
        }
    }
}
