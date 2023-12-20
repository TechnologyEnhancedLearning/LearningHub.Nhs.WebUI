// <copyright file="InternalSystemServiceTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Maintenance;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Maintenance;
    using LearningHub.Nhs.Repository.Interface.Maintenance;
    using LearningHub.Nhs.Services.Interface;
    using Moq;
    using Xunit;

    /// <summary>
    /// The InternalSystemServiceTests.
    /// </summary>
    public class InternalSystemServiceTests
    {
        /// <summary>
        /// The mock reportRepository.
        /// </summary>
        private readonly Mock<IInternalSystemRepository> mockInternalSystemRepository;

        /// <summary>
        /// The mock caching service.
        /// </summary>
        private readonly Mock<ICachingService> mockCachingService;

        /// <summary>
        /// The queue communicator service.
        /// </summary>
        private readonly Mock<IQueueCommunicatorService> mockQueueCommunicatorService;

        /// <summary>
        /// The internalSystemService.
        /// </summary>
        private readonly InternalSystemService internalSystemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSystemServiceTests"/> class.
        /// </summary>
        public InternalSystemServiceTests()
        {
            this.mockInternalSystemRepository = new Mock<IInternalSystemRepository>();
            this.mockQueueCommunicatorService = new Mock<IQueueCommunicatorService>();
            this.mockCachingService = new Mock<ICachingService>();

            this.internalSystemService = new InternalSystemService(
                this.mockInternalSystemRepository.Object,
                this.mockQueueCommunicatorService.Object,
                this.mockCachingService.Object,
                this.NewMapper());
        }

        /// <summary>
        /// The get by id async_ valid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetByIdAsync_Valid()
        {
            // Arrange
            this.mockCachingService.Setup(s => s.GetAsync<InternalSystem>($"InternalSystems"))
                .ReturnsAsync(new CacheReadResponse<InternalSystem>()
                {
                    ResponseEnum = CacheReadResponseEnum.NotFound,
                });

            this.mockInternalSystemRepository.Setup(r => r.GetAll())
                                        .Returns(await Task.FromResult(new List<InternalSystem>()
            {
                new InternalSystem() { Id = 1, Name = "Learning Hub" },
                new InternalSystem() { Id = 2, Name = "Queue One" },
                new InternalSystem() { Id = 3, Name = "Queue Two" },
            }.AsQueryable()));

            // Act
            var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.LearningHub);

            Assert.IsType<InternalSystemViewModel>(internalSystem);
            Assert.Equal(1, internalSystem.Id);
        }

        /// <summary>
        /// Gets all from repository.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetAllAsync_GetFromRepository()
        {
            // Arrange
            this.mockCachingService.Setup(s => s.GetAsync<InternalSystem>($"InternalSystems"))
                 .ReturnsAsync(new CacheReadResponse<InternalSystem>()
                 {
                     ResponseEnum = CacheReadResponseEnum.NotFound,
                 });

            this.mockInternalSystemRepository.Setup(r => r.GetAll())
                                        .Returns(await Task.FromResult(new List<InternalSystem>()
            {
                new InternalSystem() { Id = 1, Name = "Learning Hub" },
                new InternalSystem() { Id = 2, Name = "Queue One" },
                new InternalSystem() { Id = 3, Name = "Queue Two" },
                new InternalSystem() { Id = 3, Name = "Queue Three" },
            }.AsQueryable()));

            // Act
            var internalSystems = await this.internalSystemService.GetAllAsync();

            Assert.IsType<List<InternalSystemViewModel>>(internalSystems);
            Assert.Equal(4, internalSystems.Count);
        }

        /// <summary>
        /// Gets all from cache.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetAllAsync_GetFromCache()
        {
            // Arrange
            this.mockCachingService.Setup(s => s.GetAsync<List<InternalSystem>>($"InternalSystems"))
                .ReturnsAsync(new CacheReadResponse<List<InternalSystem>>()
                {
                    ResponseEnum = CacheReadResponseEnum.Found,
                    Item = new List<InternalSystem>()
                    {
                        new InternalSystem() { Id = 1, Name = "Learning Hub" },
                        new InternalSystem() { Id = 2, Name = "Queue One" },
                        new InternalSystem() { Id = 3, Name = "Queue Two" },
                    },
                });

            // Act
            var internalSystems = await this.internalSystemService.GetAllAsync();

            Assert.IsType<List<InternalSystemViewModel>>(internalSystems);
            Assert.Equal(3, internalSystems.Count);
        }

        /// <summary>
        /// Toggles the InternalSystem Offline status to make offline.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ToggleOfflineStatusAsync_TakeOffline()
        {
            // Arrange
            var userId = 213213;
            var internalSystems = new List<InternalSystem>()
            {
                new InternalSystem() { Id = 1, Name = "Learning Hub" },
                new InternalSystem() { Id = 2, Name = "Queue One" },
                new InternalSystem() { Id = 3, Name = "Queue Two" },
            };

            this.mockInternalSystemRepository.Setup(r => r.GetAll())
                                        .Returns(await Task.FromResult(internalSystems.AsQueryable()));

            var internalSystem = internalSystems.First();
            this.mockInternalSystemRepository.Setup(r => r.UpdateAsync(userId, internalSystem)).Returns(Task.CompletedTask);

            // Act
            var response = await this.internalSystemService.ToggleOfflineStatusAsync(1, userId);

            Assert.IsType<InternalSystemViewModel>(response);
            Assert.Equal(internalSystem.Id, response.Id);
            Assert.True(response.IsOffline);
        }

        /// <summary>
        /// Toggles the InternalSystem Offline status to go live.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ToggleOfflineStatusAsync_GoLive()
        {
            // Arrange
            var userId = 213213;
            var internalSystems = new List<InternalSystem>()
            {
                new InternalSystem() { Id = 1, Name = "Learning Hub", IsOffline = true },
                new InternalSystem() { Id = 2, Name = "Queue One" },
                new InternalSystem() { Id = 3, Name = "Queue Two" },
            };

            this.mockInternalSystemRepository.Setup(r => r.GetAll())
                                        .Returns(await Task.FromResult(internalSystems.AsQueryable()));

            var internalSystem = internalSystems.First();
            this.mockInternalSystemRepository.Setup(r => r.UpdateAsync(userId, internalSystem)).Returns(Task.CompletedTask);

            // Act
            var response = await this.internalSystemService.ToggleOfflineStatusAsync(1, userId);

            Assert.IsType<InternalSystemViewModel>(response);
            Assert.Equal(internalSystem.Id, response.Id);
            Assert.False(response.IsOffline);
        }

        /// <summary>
        /// The new mapper.
        /// </summary>
        /// <returns>
        /// The <see cref="IMapper"/>.
        /// </returns>
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
