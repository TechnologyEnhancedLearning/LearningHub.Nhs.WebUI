namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoMapper;
    using EntityFrameworkCore.Testing.Common;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.UnitTests.Helpers;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    /// <summary>
    /// The provider service tests.
    /// </summary>
    public class ProviderServiceTests
    {
        private readonly Mock<IProviderRepository> mockProviderRepo;
        private readonly ProviderService providerService;
        private Provider provider = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderServiceTests"/> class.
        /// </summary>
        public ProviderServiceTests()
        {
            this.mockProviderRepo = new Mock<IProviderRepository>();
            this.providerService = new ProviderService(this.mockProviderRepo.Object, this.NewMapper());
        }

        /// <summary>
        /// The get by id async invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_InValid()
        {
            this.mockProviderRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .ReturnsAsync((Provider)null);

            var provider = await this.providerService.GetByIdAsync(1);

            Assert.Null(provider);
        }

        /// <summary>
        /// The get by id async valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_Valid()
        {
            int providerId = 2;

            this.mockProviderRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .ReturnsAsync(this.GetProvider(providerId));

            var provider = await this.providerService.GetByIdAsync(1);

            Assert.IsType<ProviderViewModel>(provider);
            Assert.Equal(2, provider.Id);
            Assert.Equal("Minded", provider.Name);
            Assert.Equal("Minded Provider", provider.Description);
        }

        /// <summary>
        /// The get all async valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetAllAsync_Valid()
        {
            this.mockProviderRepo.Setup(r => r.GetAll())
                .Returns(new AsyncEnumerable<Provider>(this.TestProviderAsyncMock().Object));

            var providers = await this.providerService.GetAllAsync();

            Assert.IsType<List<ProviderViewModel>>(providers);
            Assert.Equal(2, providers.Count);
            Assert.Equal(1, providers.First().Id);
            Assert.Equal(2, providers.Last().Id);
        }

        /// <summary>
        /// The get by user id async valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByUserIdAsync_Valid()
        {
            this.mockProviderRepo.Setup(r => r.GetProvidersByUserIdAsync(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<Provider>(this.TestProviderAsyncMock().Object));

            var providers = await this.providerService.GetByUserIdAsync(1);

            Assert.IsType<List<ProviderViewModel>>(providers);
            Assert.Equal(2, providers.Count);
            Assert.Equal(1, providers.First().Id);
            Assert.Equal(2, providers.Last().Id);
        }

        /// <summary>
        /// The get by resource version id async valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByResourceVersionIdAsync_Valid()
        {
            this.mockProviderRepo.Setup(r => r.GetProvidersByResourceIdAsync(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<Provider>(this.TestProviderAsyncMock().Object));

            var providers = await this.providerService.GetByResourceVersionIdAsync(1);

            Assert.IsType<List<ProviderViewModel>>(providers);
            Assert.Equal(2, providers.Count);
            Assert.Equal(1, providers.First().Id);
            Assert.Equal(2, providers.Last().Id);
        }

        /// <summary>
        /// The test providers async mock.
        /// </summary>
        /// <returns>The <see cref="Mock"/>.</returns>
        private Mock<DbSet<Provider>> TestProviderAsyncMock()
        {
            var logRecords = this.TestProviders().AsQueryable();
            var mockLogDbSet = new Mock<DbSet<Provider>>();

            mockLogDbSet.As<IAsyncEnumerable<Provider>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(new TestAsyncEnumerator<Provider>(logRecords.GetEnumerator()));

            mockLogDbSet.As<IQueryable<Provider>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Provider>(logRecords.Provider));

            mockLogDbSet.As<IQueryable<Provider>>().Setup(m => m.Expression).Returns(logRecords.Expression);
            mockLogDbSet.As<IQueryable<Provider>>().Setup(m => m.ElementType).Returns(logRecords.ElementType);
            mockLogDbSet.As<IQueryable<Provider>>().Setup(m => m.GetEnumerator()).Returns(() => logRecords.GetEnumerator());

            return mockLogDbSet;
        }

        /// <summary>
        /// The get provider.
        /// </summary>
        /// <param name="logId">The log id.</param>
        /// <returns>The <see cref="Provider"/>.</returns>
        private Provider GetProvider(int logId)
        {
            return this.TestProviders().FirstOrDefault(l => l.Id == logId);
        }

        /// <summary>
        /// The test providers.
        /// </summary>
        /// <returns>The Provider list.</returns>
        private List<Provider> TestProviders()
        {
            return new List<Provider>()
            {
                new Provider()
                {
                    Id = 1,
                    Name = "elearning for healthcare (elfh)",
                    Description = "elearning for healthcare (elfh) Provider",
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T15:27:39.3235637+01:00"),
                    AmendUser = new User()
                    {
                        Id = 1003866,
                        UserName = "test.user",
                    },
                    CreateUser = new User()
                    {
                        Id = 1003866,
                        UserName = "test.user",
                    },
                },
                new Provider()
                {
                    Id = 2,
                    Name = "Minded",
                    Description = "Minded Provider",
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T15:27:39.3235637+01:00"),
                    AmendUser = new User()
                    {
                        Id = 1003866,
                        UserName = "test.user",
                    },
                    CreateUser = new User()
                    {
                        Id = 1003866,
                        UserName = "test.user",
                    },
                },
            };
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

        private void SetupProvider()
        {
            this.mockProviderRepo.Setup(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<Provider>()))
                .Callback<int, Provider>((i, obj) => this.provider = obj)
                .ReturnsAsync(new Fixture().Create<int>());
        }
    }
}