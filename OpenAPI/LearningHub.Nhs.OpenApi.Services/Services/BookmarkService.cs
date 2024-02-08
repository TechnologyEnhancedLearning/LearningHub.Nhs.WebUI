namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Learning Hub Bookmark service.
    /// </summary>
    public class BookmarkService : IBookmarkService
    {
        private readonly IOptions<LearningHubApiConfig> learningHubApiConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkService"/> class.
        /// </summary>
        /// <param name="learningHubApiConfig">Learning Hub Api config details.</param>
        public BookmarkService(IOptions<LearningHubApiConfig> learningHubApiConfig)
        {
            this.learningHubApiConfig = learningHubApiConfig;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(string authHeader)
        {
            LearningHubApiHttpClient bookmarkClient = new LearningHubApiHttpClient(this.learningHubApiConfig);

            string requestUrl = "Bookmark/GetAllByParent";
            var response = await bookmarkClient.GetData(requestUrl, authHeader);

            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return new List<UserBookmarkViewModel>();
            }

            return JsonConvert.DeserializeObject<List<UserBookmarkViewModel>>(
                response.Content.ReadAsStringAsync().Result);
        }
    }
}
