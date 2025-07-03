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
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Services.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
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
        private readonly IResourceReferenceRepository resourceReferenceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkService"/> class.
        /// </summary>
        /// <param name="learningHubApiConfig">Learning Hub Api config details.</param>
        /// <param name="mapper">The mapper<see cref="IMapper"/>.</param>
        /// <param name="bookmarkRepository">The bookmarkRepository<see cref="IBookmarkRepository"/>.</param>
        public BookmarkService(IOptions<LearningHubApiConfig> learningHubApiConfig, IMapper mapper, IBookmarkRepository bookmarkRepository, IResourceReferenceRepository resourceReferenceRepository)
        {
            this.mapper = mapper;
            this.bookmarkRepository = bookmarkRepository;
            this.resourceReferenceRepository = resourceReferenceRepository;
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

        /// <inheritdoc/>
        public async Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(int currentUserId, int? parentId, bool? all = false)
        {
            var userBookmarks = this.bookmarkRepository.GetAll().Where(ub => ub.UserId == currentUserId && ub.Deleted == false).OrderBy(ub => ub.Position).ThenBy(ub => ub.AmendDate);

            var bookmarks = all == true ? userBookmarks : userBookmarks.Where(ub => ub.ParentId == parentId);

            var userBookmarkResult = this.mapper.Map<IEnumerable<UserBookmarkViewModel>>(bookmarks);

            foreach (var b in userBookmarkResult.Where(ub => ub.BookmarkTypeId == 1))
            {
                b.ChildrenCount = userBookmarks.Where(ub => ub.ParentId == b.Id).Count();
            }

            foreach (var b in userBookmarkResult.Where(ub => ub.ResourceReferenceId.HasValue))
            {
                var resourceRef = await this.resourceReferenceRepository.GetByOriginalResourceReferenceIdAsync(b.ResourceReferenceId.Value, false);
                b.ResourceTypeId = (int?)resourceRef.Resource.ResourceTypeEnum;
            }

            return userBookmarkResult;
        }

        public async Task<int> Create(int currentUserId, UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkModel = this.mapper.Map<UserBookmark>(bookmarkViewModel);
            var maxPosition = (await this.bookmarkRepository.GetAll().Where(ub => ub.UserId == currentUserId).MaxAsync(x => (int?)x.Position)) ?? 0;
            bookmarkModel.UserId = currentUserId;
            bookmarkModel.Position = maxPosition + 1;

            return await this.bookmarkRepository.CreateAsync(currentUserId, bookmarkModel);
        }

        /// <inheritdoc/>
        public async Task<int> Toggle(int currentUserId, UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkId = bookmarkViewModel.Id;
            var maxPosition = (await this.bookmarkRepository.GetAll().Where(ub => ub.UserId == currentUserId && ub.ParentId == null).MaxAsync(x => (int?)x.Position)) ?? 0;
            if (bookmarkId == 0)
            {
                var bookmarkModel = this.mapper.Map<UserBookmark>(bookmarkViewModel);
                bookmarkModel.UserId = currentUserId;
                bookmarkModel.Position = maxPosition + 1;
                bookmarkId = await this.bookmarkRepository.CreateAsync(currentUserId, bookmarkModel);
            }
            else
            {
                var bookmarkModel = await this.bookmarkRepository.GetById(bookmarkId);
                bookmarkModel.Deleted = !bookmarkModel.Deleted;
                if (bookmarkModel.Deleted)
                {
                    bookmarkModel.ParentId = null;
                }
                else
                {
                    bookmarkModel.Link = bookmarkViewModel.Link;
                    bookmarkModel.Title = bookmarkViewModel.Title;
                    bookmarkModel.Position = maxPosition + 1;
                }

                await this.bookmarkRepository.UpdateAsync(currentUserId, bookmarkModel);
            }

            return bookmarkId;
        }

        /// <inheritdoc/>
        public async Task<int> Edit(int currentUserId, UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkModel = await this.bookmarkRepository.GetById(bookmarkViewModel.Id);
            bookmarkModel.ParentId = bookmarkViewModel.ParentId;
            bookmarkModel.Position = bookmarkViewModel.Position;
            bookmarkModel.Title = bookmarkViewModel.Title;
            await this.bookmarkRepository.UpdateAsync(currentUserId, bookmarkModel);
            return bookmarkModel.Id;
        }

        /// <inheritdoc/>
        public async Task DeleteFolder(int bookmarkId, int userId)
        {
            await this.bookmarkRepository.DeleteFolder(bookmarkId, userId);
        }
    }
}
