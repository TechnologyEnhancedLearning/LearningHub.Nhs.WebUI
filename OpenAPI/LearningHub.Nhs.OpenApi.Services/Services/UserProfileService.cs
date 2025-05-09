﻿namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The UserProfile service.
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        /// <summary>
        /// The UserProfile repository.
        /// </summary>
        private readonly IUserProfileRepository userProfileRepository;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UserProfileService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileService"/> class.
        /// </summary>
        /// <param name="userProfileRepository">The UserProfile repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public UserProfileService(
            IUserProfileRepository userProfileRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger)
        {
            this.userProfileRepository = userProfileRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserProfile> GetByIdAsync(int id)
        {
            return await userProfileRepository.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> CreateUserProfileAsync(int userId, UserProfile userProfile)
        {
            var retVal = await ValidateAsync(userProfile);

            if (retVal.IsValid)
            {
                retVal.CreatedId = await userProfileRepository.CreateAsync(userId, userProfile);
            }

            return retVal;
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> UpdateUserProfileAsync(int userId, UserProfile userProfile)
        {
            var retVal = await ValidateAsync(userProfile);

            if (retVal.IsValid)
            {
                await userProfileRepository.UpdateAsync(userId, userProfile);
                retVal.CreatedId = userProfile.Id;
            }

            return retVal;
        }

        private async Task<LearningHubValidationResult> ValidateAsync(UserProfile userProfile)
        {
            var userProfileValidator = new UserProfileValidator();
            var validationResult = await userProfileValidator.ValidateAsync(userProfile);

            var retVal = new LearningHubValidationResult(validationResult);

            return retVal;
        }
    }
}