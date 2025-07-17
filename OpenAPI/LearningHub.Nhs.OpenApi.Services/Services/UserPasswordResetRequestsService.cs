namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The user service.
    /// </summary>
    public class UserPasswordResetRequestsService : IUserPasswordResetRequestsService
    {
        /// <summary>
        /// The user password reset requests repository.
        /// </summary>
        private readonly IUserPasswordResetRequestsRepository userPasswordResetRequestsRepository;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPasswordResetRequestsService"/> class.
        /// </summary>
        /// <param name="userPasswordResetRequestsRepository">The user password reset requests repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="userDetailsRepository">The userDetailsRepository.</param>
        public UserPasswordResetRequestsService(
            IUserPasswordResetRequestsRepository userPasswordResetRequestsRepository,
            IMapper mapper,
            ICachingService cachingService,
            ILogger<UserService> logger,
            IUserProfileRepository userDetailsRepository)
        {
            this.userPasswordResetRequestsRepository = userPasswordResetRequestsRepository;
            this.mapper = mapper;
            this.cachingService = cachingService;
            this.logger = logger;
        }

        /// <summary>
        /// The get by username async.
        /// </summary>
        /// <param name="emailAddress">The user name.</param>
        /// <param name="passwordRequestLimitingPeriod">The passwordRequestLimitingPeriod.</param>
        /// <param name="passwordRequestLimit">ThepasswordRequestLimit.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> CanRequestPasswordReset(string emailAddress, int passwordRequestLimitingPeriod, int passwordRequestLimit)
        {
            var result = await userPasswordResetRequestsRepository.CanRequestPasswordResetAsync(emailAddress, passwordRequestLimitingPeriod, passwordRequestLimit);

            return result;
        }

        /// <summary>
        /// CreateUserPasswordRequest.
        /// </summary>
        /// <param name="emailAddress">the emailAddress.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> CreateUserPasswordRequest(string emailAddress)
        {
        var result = await userPasswordResetRequestsRepository.CreatePasswordRequests(emailAddress);
        return result;
        }
    }
}
