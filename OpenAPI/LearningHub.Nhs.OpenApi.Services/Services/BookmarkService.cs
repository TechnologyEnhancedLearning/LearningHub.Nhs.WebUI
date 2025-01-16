namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Learning Hub Bookmark service.
    /// </summary>
    public class BookmarkService : IBookmarkService
    {
        private readonly IOptions<LearningHubApiConfig> learningHubApiConfig;
        private readonly IMapper mapper;
        private readonly IBookmarkRepository bookmarkRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkService"/> class.
        /// </summary>
        /// <param name="learningHubApiConfig">Learning Hub Api config details.</param>
        /// <param name="mapper">The mapper<see cref="IMapper"/>.</param>
        /// <param name="bookmarkRepository">The bookmarkRepository<see cref="IBookmarkRepository"/>.</param>
        public BookmarkService(IOptions<LearningHubApiConfig> learningHubApiConfig, IMapper mapper, IBookmarkRepository bookmarkRepository)
        {
            this.mapper = mapper;
            this.bookmarkRepository = bookmarkRepository;
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

        public async Task<int> Create(int currentUserId, UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkModel = this.mapper.Map<UserBookmark>(bookmarkViewModel);
            var maxPosition = (await this.bookmarkRepository.GetAll().Where(ub => ub.UserId == currentUserId).MaxAsync(x => (int?)x.Position)) ?? 0;
            bookmarkModel.UserId = currentUserId;
            bookmarkModel.Position = maxPosition + 1;

            return await this.bookmarkRepository.CreateAsync(currentUserId, bookmarkModel);
        }
   }
}
