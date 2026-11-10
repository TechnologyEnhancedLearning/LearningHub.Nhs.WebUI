namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Constants;
    using LearningHub.Nhs.Models.Dto;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.ProfessionalBody;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Extensions;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

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
        private readonly IProfessionalBodyRepository professionalBodyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="professionalBodyRepository">The userDetailsRepository.</param>
        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ICachingService cachingService,
            ILogger<UserService> logger,
            IProfessionalBodyRepository professionalBodyRepository)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.cachingService = cachingService;
            this.logger = logger;
            this.professionalBodyRepository = professionalBodyRepository;
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
        /// Gets a user by id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The user if found.</returns>
        public async Task<UserViewModel> GetByIdAsync(int id)
        {
            var user = await this.userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            return this.mapper.Map<UserViewModel>(user);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<User> GetAuthUserByIdAsync(int id)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(id);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<UserAuthenticateDto> GetUserDetailForAuthenticateAsync(string userName)
        {
            return await this.userRepository.GetUserDetailForAuthentication(userName);
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

        /// <inheritdoc/>
        public async Task RecordSuccessfulSigninAsync(int id, CancellationToken token = default)
        {
            var user = await this.userRepository.GetByIdAsync(id);

            if (user.PasswordLifeCounter != 0 || user.SecurityLifeCounter != 0)
            {
                user.PasswordLifeCounter = 0;
                user.SecurityLifeCounter = 0;

                await this.userRepository.UpdateAsync(id, user);

                await this.InvalidateUserCacheAsync(user.Id, user.LegacyUserName, token);
            }
        }

        /// <inheritdoc/>
        public async Task RecordUnsuccessfulSigninAsync(int id, CancellationToken token = default)
        {
            var user = await this.userRepository.GetByIdAsync(id);

            user.PasswordLifeCounter++;

            await this.userRepository.UpdateAsync(id, user);

            await this.InvalidateUserCacheAsync(user.Id, user.LegacyUserName, token);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The user creation request.</param>
        /// <param name="currentUserId">
        /// The id of the user performing the operation.
        /// </param>
        /// <returns>The created user.</returns>
        public async Task<UserViewModel> CreateAsync(CreateUserRequest request, int currentUserId)
        {
            var email = request.PrimaryEmail.Trim();

            if (!await this.userRepository.IsEmailAvailableAsync(email))
            {
                throw new ValidationException("The email address is already in use.");
            }

            await this.ValidateProfessionalRegistrationAsync(request.ProfessionalBodyId, request.ProfessionalRegistrationNumber);

            var user = new User
            {
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                EmailAddress = email,
                RecoveryEmailAddress =
                    request.RecoveryEmail?.Trim(),
                ProfessionalBodyId =
                    request.ProfessionalBodyId,
                ProfessionalRegistrationNumber =
                    request.ProfessionalRegistrationNumber?.Trim(),
                Active = true,
            };

            var validationResult = await this.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                throw new ValidationException("The supplied user details are invalid.");
            }

            var userId = await this.userRepository.CreateAsync(currentUserId, user);

            user.Id = userId;

            return this.mapper.Map<UserViewModel>(user);
        }

        /// <summary>
        /// Partially updates a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="request">The patch request.</param>
        /// <param name="currentUserId">
        /// The id of the user performing the update.
        /// </param>
        /// <returns>The updated user, or null when not found.</returns>
        public async Task<UserViewModel?> PatchAsync(int id, PatchUserRequest request, int currentUserId)
        {
            var user = await this.userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            if (request.FirstName != null)
            {
                user.FirstName = request.FirstName.Trim();
            }

            if (request.LastName != null)
            {
                user.LastName = request.LastName.Trim();
            }

            if (request.PrimaryEmail != null)
            {
                var email = request.PrimaryEmail.Trim();

                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new ValidationException("Primary email cannot be empty.");
                }

                // Only check availability if the email has actually changed.
                if (!string.Equals(user.EmailAddress,email,StringComparison.OrdinalIgnoreCase))
                {
                    var emailAvailable = await this.userRepository.IsEmailAvailableAsync(email,id);

                    if (!emailAvailable)
                    {
                        throw new ValidationException("The email address is already in use.");
                    }
                }

                user.EmailAddress = email;
            }

            if (request.RecoveryEmail != null)
            {
                user.RecoveryEmailAddress = request.RecoveryEmail.Trim();
            }

            var professionalBodyId = request.ProfessionalBodyId ?? user.ProfessionalBodyId;

            var registrationNumber = request.ProfessionalRegistrationNumber ?? user.ProfessionalRegistrationNumber;

            if (request.ProfessionalBodyId.HasValue ||
                request.ProfessionalRegistrationNumber != null)
            {
                await this.ValidateProfessionalRegistrationAsync(
                    professionalBodyId,
                    registrationNumber);

                if (request.ProfessionalBodyId.HasValue)
                {
                    user.ProfessionalBodyId = request.ProfessionalBodyId.Value;
                }

                if (request.ProfessionalRegistrationNumber != null)
                {
                    user.ProfessionalRegistrationNumber = request.ProfessionalRegistrationNumber.Trim();
                }
            }

            var validationResult = await this.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                throw new ValidationException("The supplied user details are invalid.");
            }

            await this.userRepository.UpdateAsync(currentUserId,user);

            await this.InvalidateUserCacheAsync(user.Id,user.LegacyUserName,CancellationToken.None);

            return this.mapper.Map<UserViewModel>(user);
        }

        /// <summary>
        /// Soft deletes a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="currentUserId">
        /// The id of the user performing the operation.
        /// </param>
        /// <returns>True when the user was deleted.</returns>
        public async Task<bool> SoftDeleteAsync(int id,int currentUserId)
        {
            var user = await this.userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            user.Deleted = true;

            await this.userRepository.UpdateAsync(currentUserId,user);

            await this.InvalidateUserCacheAsync(user.Id,user.LegacyUserName,CancellationToken.None);

            return true;
        }

        /// <summary>
        /// Restores a soft-deleted user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="currentUserId">
        /// The id of the user performing the operation.
        /// </param>
        /// <returns>True when the user was restored.</returns>
        public async Task<bool> RestoreAsync(int id, int currentUserId)
        {
            var user = await this.userRepository.GetByIdIncludingDeletedAsync(id);

            if (user == null || !user.Deleted)
            {
                return false;
            }

            user.Deleted = false;

            await this.userRepository.UpdateAsync(currentUserId,user);

            await this.InvalidateUserCacheAsync(user.Id,user.LegacyUserName,CancellationToken.None);

            return true;
        }

        /// <summary>
        /// Checks whether an email address is available.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="excludeUserId">
        /// Optional user id to exclude.
        /// </param>
        /// <returns>True when the email is available.</returns>
        public async Task<bool> IsEmailAvailableAsync(string email, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return await this.userRepository.IsEmailAvailableAsync(email.Trim(),excludeUserId);
        }

        /// <summary>
        /// Searches users.
        /// </summary>
        /// <param name="request">The search request.</param>
        /// <returns>A paged collection of users.</returns>
        public async Task<PagedResultSet<UserViewModel>> SearchAsync(UserSearchRequest request)
        {
            var page = request.Page <= 0 ? 1 : request.Page;

            var pageSize = request.PageSize <= 0 ? 20   : Math.Min(request.PageSize, 100);

            var query = this.userRepository.GetAll().Where(user => !user.Deleted);

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var searchTerm = request.Query.Trim();

                query = query.Where(user => 
                    (user.FirstName != null && user.FirstName.Contains(searchTerm)) ||
                    (user.LastName != null &&  user.LastName.Contains(searchTerm)) ||
                    (user.EmailAddress != null && user.EmailAddress.Contains(searchTerm)) ||
                    (user.LegacyUserName != null && user.LegacyUserName.Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var email = request.Email.Trim();

                query = query.Where(user => user.EmailAddress != null && user.EmailAddress.Contains(email));
            }

            if (request.Active.HasValue)
            {
                query = query.Where(user => (user.Active ?? false) == request.Active.Value);
            }

            var totalItemCount = await query.CountAsync();

            query = ApplySearchOrder(query,request.Sort,request.Direction);

            var users = await this.mapper
                .ProjectTo<UserViewModel>(
                    query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize))
                .ToListAsync();

            return new PagedResultSet<UserViewModel>
            {
                Items = users,
                TotalItemCount = totalItemCount,
            };
        }

        public async Task<UserViewModel> ClearProfessionalRegistrationAsync(int id, int currentUserId)
        {
            var user = await this.userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            user.ProfessionalBodyId = null;
            user.ProfessionalRegistrationNumber = null;

            await this.userRepository.UpdateAsync(currentUserId, user);

            await this.InvalidateUserCacheAsync(user.Id, user.LegacyUserName, CancellationToken.None);

            return this.mapper.Map<UserViewModel>(user);
        }

        public async Task<IReadOnlyList<ProfessionalBodyViewModel>> GetProfessionalBodiesAsync()
        {
            var professionalBodies =
                await this.professionalBodyRepository.GetAllAsync();

            return professionalBodies
                .Select(x => new ProfessionalBodyViewModel
                {
                    Id = x.Id,
                    Name = x.ProfessionalBodyName,
                    OrderByNumber = x.OrderByNumber,
                    RegexPattern = x.RegexPattern,
                })
                .ToList();
        }

        private static IQueryable<User> ApplySearchOrder(IQueryable<User> query, string? sort, string? direction)
        {
            var descending =
                string.Equals(
                    direction,
                    "desc",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    direction,
                    "d",
                    StringComparison.OrdinalIgnoreCase);

            return sort?.Trim().ToLowerInvariant() switch
            {
                "firstname" => descending
                    ? query.OrderByDescending(user => user.FirstName)
                    : query.OrderBy(user => user.FirstName),

                "lastname" => descending
                    ? query.OrderByDescending(user => user.LastName)
                    : query.OrderBy(user => user.LastName),

                "email" or "primaryemail" => descending
                    ? query.OrderByDescending(user => user.EmailAddress)
                    : query.OrderBy(user => user.EmailAddress),

                "active" => descending
                    ? query.OrderByDescending(user => user.Active)
                    : query.OrderBy(user => user.Active),

                "id" => descending
                    ? query.OrderByDescending(user => user.Id)
                    : query.OrderBy(user => user.Id),

                _ => query.OrderBy(user => user.Id),
            };
        }

    

        private async Task ValidateProfessionalRegistrationAsync(int? professionalBodyId, string? registrationNumber)
        {
            // No professional body selected.
            if (!professionalBodyId.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(registrationNumber))
                {
                    throw new ValidationException(
                        "A professional body must be selected when a professional registration number is supplied.");
                }

                return;
            }

            var professionalBody =
                await this.professionalBodyRepository
                    .GetByIdAsync(professionalBodyId.Value);

            if (professionalBody == null)
            {
                throw new ValidationException(
                    "The selected professional body does not exist.");
            }

            // Registration number itself remains optional.
            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(
                professionalBody.RegexPattern))
            {
                return;
            }

            try
            {
                var isValid = Regex.IsMatch(
                    registrationNumber.Trim(),
                    professionalBody.RegexPattern,
                    RegexOptions.CultureInvariant |
                    RegexOptions.IgnoreCase,
                    TimeSpan.FromMilliseconds(250));

                if (!isValid)
                {
                    throw new ValidationException(
                        $"The professional registration number is not valid for {professionalBody.ProfessionalBodyName}.");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                this.logger.LogError(
                    "Professional body regex timed out for ProfessionalBodyId {ProfessionalBodyId}.",
                    professionalBody.Id);

                throw new InvalidOperationException(
                    "The professional registration validation rule is invalid.");
            }
            catch (ArgumentException ex)
            {
                this.logger.LogError(
                    ex,
                    "Invalid regex configured for ProfessionalBodyId {ProfessionalBodyId}.",
                    professionalBody.Id);

                throw new InvalidOperationException(
                    "The professional registration validation rule is invalid.");
            }
        }

        /// <inheritdoc/>
        private async Task InvalidateUserCacheAsync(int userId, string userName, CancellationToken cancellationToken)
        {
            if (userId == 0 || string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await Task.WhenAll(
               this.cachingService.RemoveAsync($"{CacheKeys.UserLoadByUserId}:{userId}"),
               this.cachingService.RemoveAsync($"{CacheKeys.UserLoadByUserName}:{userName}"));
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
                        items = items.Where(x => x.LegacyUserName.Contains(filter.Value));
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
                        items = items.OrderByDescending(x => x.LegacyUserName);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.LegacyUserName);
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
