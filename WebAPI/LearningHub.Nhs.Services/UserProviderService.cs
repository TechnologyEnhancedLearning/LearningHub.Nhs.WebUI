// <copyright file="UserProviderService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// The user provider service.
    /// </summary>
    public class UserProviderService : IUserProviderService
    {
        /// <summary>
        /// The user provider repository.
        /// </summary>
        private readonly IUserProviderRepository userProviderRepository;

        /// <summary>
        ///  mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProviderService"/> class.
        /// </summary>
        /// <param name="userProviderRepository">The user provider repository.</param>
        /// <param name="mapper">The mapper.</param>
        public UserProviderService(IUserProviderRepository userProviderRepository, IMapper mapper)
        {
            this.userProviderRepository = userProviderRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<LearningHubValidationResult> UpdateUserProviderAsync(UserProviderUpdateViewModel userProviderUpdateModel)
        {
            try
            {
                await this.userProviderRepository.UpdateUserProviderAsync(userProviderUpdateModel);
            }
            catch (Exception ex)
            {
                return new LearningHubValidationResult(false, $"Error updating provided by permission to user: {ex.Message}");
            }

            return new LearningHubValidationResult(true);
        }
    }
}