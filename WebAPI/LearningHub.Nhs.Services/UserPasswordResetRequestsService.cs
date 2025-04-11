namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO.Enumeration;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Constants;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Extensions;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

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
            var result = await this.userPasswordResetRequestsRepository.CanRequestPasswordResetAsync(emailAddress, passwordRequestLimitingPeriod, passwordRequestLimit);

            return result;
        }

        /// <summary>
        /// CreateUserPasswordRequest.
        /// </summary>
        /// <param name="emailAddress">the emailAddress.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> CreateUserPasswordRequest(string emailAddress)
        {
        var result = await this.userPasswordResetRequestsRepository.CreatePasswordRequests(emailAddress);
        return result;
        }
    }
}
