// <copyright file="UserProviderServiceTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// The user provider service tests.
    /// </summary>
    public class UserProviderServiceTests
    {
        private readonly Mock<IUserProviderRepository> mockUserProviderRepo;
        private readonly Mock<IOptions<Settings>> mockSettings;
        private readonly IUserProviderService userProviderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProviderServiceTests"/> class.
        /// </summary>
        public UserProviderServiceTests()
        {
            this.mockUserProviderRepo = new Mock<IUserProviderRepository>();
            this.userProviderService = new UserProviderService(this.mockUserProviderRepo.Object, this.NewMapper());
        }

        /// <summary>
        /// The update user provider async invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateUserProviderAsync_InValid()
        {
            // Arrange
            this.mockUserProviderRepo.Setup(r => r.UpdateUserProviderAsync(It.IsAny<UserProviderUpdateViewModel>()))
                                        .Throws(new Exception());

            var testModel = new UserProviderUpdateViewModel { ProviderIds = null, UserId = 1 };

            // Act
            var result = await this.userProviderService.UpdateUserProviderAsync(testModel);

            // Assert
            this.mockUserProviderRepo.Verify(x => x.UpdateUserProviderAsync(It.IsAny<UserProviderUpdateViewModel>()), Times.Once());
            Assert.Contains("Error updating provided by permission to user:", result.Details[0]);
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// The update user provider async valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateUserProviderAsync_Valid()
        {
            // Arrange
            this.mockUserProviderRepo.Setup(r => r.UpdateUserProviderAsync(It.IsAny<UserProviderUpdateViewModel>()))
                                        .Returns(Task.CompletedTask);

            var testModel = new UserProviderUpdateViewModel { ProviderIds = new List<int> { 1, 2 }, UserId = 1 };

            // Act
            var result = await this.userProviderService.UpdateUserProviderAsync(testModel);

            // Assert
            this.mockUserProviderRepo.Verify(x => x.UpdateUserProviderAsync(It.IsAny<UserProviderUpdateViewModel>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// The new mapper.
        /// </summary>
        /// <returns>The <see cref="IMapper"/>.</returns>
        private IMapper NewMapper()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            return mapper;
        }
    }
}