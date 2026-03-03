namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Constants;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Extensions;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The user service.
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// The user repository.
        /// </summary>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly ICachingService cachingService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UserService> logger;
        private readonly IUserProfileRepository userDetailsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="userDetailsRepository">The userDetailsRepository.</param>
        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ICachingService cachingService,
            ILogger<UserService> logger,
            IUserProfileRepository userDetailsRepository)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.cachingService = cachingService;
            this.logger = logger;
            this.userDetailsRepository = userDetailsRepository;
        }

        /// <summary>
        /// The get active content async.
        /// </summary>
        /// <param name="userId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActiveContentViewModel}"/>.</returns>
        public async Task<List<ActiveContentViewModel>> GetActiveContentAsync(int userId)
        {
            var retVal = new List<ActiveContentViewModel>();

            string cacheKey = $"{CacheKeys.ActiveContent}:{userId}";
            var cacheResponse = await cachingService.GetAsync<List<ActiveContentViewModel>>(cacheKey);
            if (cacheResponse.ResponseEnum == CacheReadResponseEnum.Found)
            {
                retVal = cacheResponse.Item;
            }

            return retVal;
        }

        /// <summary>
        /// The add active content.
        /// </summary>
        /// <param name="activeContentViewModel">The active content view model<see cref="ActiveContentViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> AddActiveContent(ActiveContentViewModel activeContentViewModel, int userId)
        {
            var activeContent = await GetActiveContentAsync(userId);

            if (activeContent.Any(ac => ac.ResourceId == activeContentViewModel.ResourceId))
            {
                return new LearningHubValidationResult(false);
            }

            activeContent.Add(activeContentViewModel);

            string cacheKey = $"{CacheKeys.ActiveContent}:{userId}";
            await cachingService.SetAsync(cacheKey, activeContent);
            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The release active content.
        /// </summary>
        /// <param name="activeContentReleaseViewModel">The active content view model<see cref="ActiveContentReleaseViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> ReleaseActiveContent(ActiveContentReleaseViewModel activeContentReleaseViewModel)
        {
            string cacheKey = $"{CacheKeys.ActiveContent}:{activeContentReleaseViewModel.UserId}";
            if (activeContentReleaseViewModel.ReleaseAll)
            {
                await cachingService.RemoveAsync(cacheKey);
            }
            else
            {
                var activeContent = await GetActiveContentAsync(activeContentReleaseViewModel.UserId);
                var ac = activeContent.Where(ac1 => ac1.ScormActivityId == activeContentReleaseViewModel.ScormActivityId).FirstOrDefault();
                if (ac != null)
                {
                    activeContent.Remove(ac);

                    await cachingService.SetAsync(cacheKey, activeContent);
                }
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Returns a list of basic user info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The preset filter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PagedResultSet<UserAdminBasicViewModel>> GetUserAdminBasicPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "")
        {
            var presetFilterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(presetFilter);
            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<UserAdminBasicViewModel> result = new PagedResultSet<UserAdminBasicViewModel>();

            var items = userRepository.GetAll();

            items = this.PresetFilterItems(items, presetFilterCriteria);
            items = this.FilterItems(items, filterCriteria);

            items = items.ContainsWithLikeQuery();

            result.TotalItemCount = items.Count();

            items = this.OrderItems(items, sortColumn, sortDirection);

            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            result.Items = await mapper.ProjectTo<UserAdminBasicViewModel>(items).ToListAsync();

            return result;
        }

        /// <summary>
        /// The get by username async.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<User> GetByUsernameAsync(string userName)
        {
            var user = await userRepository.GetByUsernameAsync(userName, true);

            return user;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserLHBasicViewModel> GetByIdAsync(int id)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(id);
                return mapper.Map<UserLHBasicViewModel>(user);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public bool IsAdminUser(int userId)
        {
            return userRepository.IsAdminUser(userId);
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> CreateUserAsync(int userId, UserCreateViewModel userCreateViewModel)
        {
            var user = mapper.Map<User>(userCreateViewModel);

            var retVal = await ValidateAsync(user);

            if (retVal.IsValid)
            {
                retVal.CreatedId = await userRepository.CreateAsync(userId, user);
            }

            return retVal;
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> UpdateUserAsync(int userId, UserUpdateViewModel userUpdateViewModel)
        {
            var user = mapper.Map<User>(userUpdateViewModel);

            var retVal = await ValidateAsync(user);

            if (retVal.IsValid)
            {
                await userRepository.UpdateAsync(userId, user);
                retVal.CreatedId = userUpdateViewModel.Id;
            }

            return retVal;
        }

        private IQueryable<User> PresetFilterItems(IQueryable<User> items, List<PagingColumnFilter> presetFilterCriteria)
        {
            if (presetFilterCriteria == null || presetFilterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in presetFilterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "usergroupid_exclude":
                        items = items.Where(x => !x.UserUserGroup.Any(uug => uug.UserGroupId == int.Parse(filter.Value)));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        private IQueryable<User> FilterItems(IQueryable<User> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "id":
                        int enteredId = 0;
                        int.TryParse(filter.Value, out enteredId);
                        items = items.Where(x => x.Id == enteredId);
                        break;
                    case "username":
                        items = items.Where(x => x.UserName.Contains(filter.Value));
                        break;
                    case "excludeusergroupid":
                        items = items.Where(x => !x.UserUserGroup.Any(uug => uug.UserGroupId == int.Parse(filter.Value)));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        private IQueryable<User> OrderItems(IQueryable<User> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "username":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.UserName);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.UserName);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Id);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Id);
                    }

                    break;
            }

            return items;
        }

        private async Task<LearningHubValidationResult> ValidateAsync(User user)
        {
            var notificationValidator = new UserValidator();
            var clientValidationResult = await notificationValidator.ValidateAsync(user);

            var retVal = new LearningHubValidationResult(clientValidationResult);

            return retVal;
        }
    }
}
