namespace LearningHub.Nhs.Services.UnitTests
{
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoMapper;
    ////using elfhHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// The migration service tests.
    /// </summary>
    public class RatingServiceTests
    {
        /// <summary>
        /// The mock userService.
        /// </summary>
        private readonly Mock<IUserService> mockUserService;

        /// <summary>
        /// The mock resourceService.
        /// </summary>
        private readonly Mock<IResourceService> mockResourceService;

        /// <summary>
        /// The mock resourceVersionRepository.
        /// </summary>
        private readonly Mock<IResourceVersionRepository> mockResourceVersionRepository;

        /// <summary>
        /// The mock resourceRepository.
        /// </summary>
        private readonly Mock<IResourceRepository> mockResourceRepository;

        /// <summary>
        /// The mock resourceVersionRatingSummaryRepository.
        /// </summary>
        private readonly Mock<IResourceVersionRatingSummaryRepository> mockResourceVersionRatingSummaryRepository;

        /// <summary>
        /// The mock resourceVersionRatingRepository.
        /// </summary>
        private readonly Mock<IResourceVersionRatingRepository> mockResourceVersionRatingRepository;

        /////// <summary>
        /////// The mock userEmploymentRepository.
        /////// </summary>
        ////private readonly Mock<IUserEmploymentRepository> mockUserEmploymentRepository;

        /// <summary>
        /// The mock publicationRepository.
        /// </summary>
        private readonly Mock<IPublicationRepository> mockPublicationRepository;

        /// <summary>
        /// The mock cachingService.
        /// </summary>
        private readonly Mock<ICachingService> mockCachingService;

        /// <summary>
        /// The mock logger.
        /// </summary>
        private readonly Mock<ILogger<ActivityService>> mockLogger;

        /// <summary>
        /// The rating service.
        /// </summary>
        private readonly IRatingService ratingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingServiceTests"/> class.
        /// </summary>
        public RatingServiceTests()
        {
            this.mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            this.mockResourceService = new Mock<IResourceService>(MockBehavior.Strict);
            this.mockResourceVersionRepository = new Mock<IResourceVersionRepository>(MockBehavior.Strict);
            this.mockResourceRepository = new Mock<IResourceRepository>(MockBehavior.Strict);
            this.mockResourceVersionRatingSummaryRepository = new Mock<IResourceVersionRatingSummaryRepository>(MockBehavior.Strict);
            this.mockResourceVersionRatingRepository = new Mock<IResourceVersionRatingRepository>(MockBehavior.Strict);
            ////this.mockUserEmploymentRepository = new Mock<IUserEmploymentRepository>(MockBehavior.Strict);
            this.mockPublicationRepository = new Mock<IPublicationRepository>(MockBehavior.Strict);
            this.mockCachingService = new Mock<ICachingService>(MockBehavior.Strict);
            this.mockLogger = new Mock<ILogger<ActivityService>>(MockBehavior.Loose);

            this.ratingService = new RatingService(
                this.mockUserService.Object,
                this.mockResourceService.Object,
                this.mockResourceVersionRepository.Object,
                this.mockResourceRepository.Object,
                this.mockResourceVersionRatingSummaryRepository.Object,
                this.mockResourceVersionRatingRepository.Object,
                ////this.mockUserEmploymentRepository.Object,
                this.mockPublicationRepository.Object,
                this.mockCachingService.Object,
                this.mockLogger.Object,
                this.NewMapper());
        }

        /// <summary>
        /// The GetRatingSummaryReturnsCorrectRatingPercentagesAndTrueBooleans.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetRatingSummaryReturnsCorrectRatingPercentagesAndTrueBooleans()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;

            this.mockResourceVersionRatingSummaryRepository.Setup(r => r.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<ResourceVersionRatingSummary>(
                    new ResourceVersionRatingSummary
                    {
                        AverageRating = 3.7m,
                        Rating1StarCount = 1,
                        Rating2StarCount = 2,
                        Rating3StarCount = 3,
                        Rating4StarCount = 4,
                        Rating5StarCount = 5,
                        RatingCount = 15,
                        ResourceVersionId = resourceVersionId,
                    }));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(new ResourceVersionRating { Rating = 5 }));

            // Act
            var result = await this.ratingService.GetRatingSummary(userId, resourceVersionId);

            // Assert
            Assert.Equal(3.7, result.AverageRating);
            Assert.Equal(6, result.Rating1StarPercent);
            Assert.Equal(13, result.Rating2StarPercent);
            Assert.Equal(20, result.Rating3StarPercent);
            Assert.Equal(26, result.Rating4StarPercent);
            Assert.Equal(33, result.Rating5StarPercent);
            Assert.Equal(15, result.RatingCount);
            Assert.Equal(5, result.UserRating);
            Assert.True(result.UserIsContributor);
            Assert.True(result.UserCanRate);
            Assert.True(result.UserHasAlreadyRated);
        }

        /// <summary>
        /// The GetRatingSummaryReturnsFalseBooleans.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetRatingSummaryReturnsFalseBooleans()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;

            this.mockResourceVersionRatingSummaryRepository.Setup(r => r.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<ResourceVersionRatingSummary>(new ResourceVersionRatingSummary()));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(false));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var result = await this.ratingService.GetRatingSummary(userId, resourceVersionId);

            // Assert
            Assert.Equal(0, result.UserRating);
            Assert.False(result.UserIsContributor);
            Assert.False(result.UserCanRate);
            Assert.False(result.UserHasAlreadyRated);
        }

        /**********************/
        /* CreateRating Tests */
        /**********************/

        /// <summary>
        /// The CreateRatingRecalculatesAverageRatingCorrectly.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateRatingRecalculatesAverageRatingCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            int userId = 1;
            int resourceVersionId = 2;
            int jobRoleId = 3;
            int locationId = 4;
            int userRating = 5;
            int ratingId = 6;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            this.mockResourceVersionRatingRepository.Setup(r => r.CreateAsync(userId, It.Is<ResourceVersionRating>(y =>
                    y.UserId == userId &&
                    y.Rating == userRating &&
                    y.ResourceVersionId == resourceVersionId &&
                    y.JobRoleId == jobRoleId &&
                    y.LocationId == locationId)))
                .Returns(Task.FromResult<int>(ratingId));

            this.mockResourceVersionRatingSummaryRepository.Setup(r => r.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<ResourceVersionRatingSummary>(new ResourceVersionRatingSummary()));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetRatingCountsForResourceVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<int[]>(new int[] { 1, 2, 3, 4, 5 }));

            this.mockResourceVersionRatingSummaryRepository.Setup(r => r.UpdateAsync(userId, It.Is<ResourceVersionRatingSummary>(y =>
                    y.Rating1StarCount == 1 &&
                    y.Rating2StarCount == 2 &&
                    y.Rating3StarCount == 3 &&
                    y.Rating4StarCount == 4 &&
                    y.Rating5StarCount == 5 &&
                    y.RatingCount == 15 &&
                    y.AverageRating == 3.7m)))
                .Returns(Task.CompletedTask);

            this.mockResourceService.Setup(r => r.SubmitResourceVersionToSearchAsync(resourceVersionId, userId))
                .ReturnsAsync((true, fixture.Create<int>()));

            this.mockCachingService.Setup(s => s.RemoveAsync($"RatingSummaryBasic:{resourceVersionId}"))
                .ReturnsAsync(new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                });

            this.mockCachingService.Setup(s => s.GetAsync<RatingSummaryBasicViewModel>($"RatingSummaryBasic:{resourceVersionId}"))
                .ReturnsAsync(new CacheReadResponse<RatingSummaryBasicViewModel>()
                {
                    ResponseEnum = CacheReadResponseEnum.Found,
                });

                // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating, JobRoleId = jobRoleId, LocationId = locationId };
            var result = await this.ratingService.CreateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// The CreateRatingValidationPreventsOldVersionBeingRated.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateRatingValidationPreventsOldVersionBeingRated()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(false));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.CreateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("This version of the resource is not the current version and cannot be rated", result.Details[0]);
        }

        /// <summary>
        /// The CreateRatingValidationPreventsContributorAddingARating.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateRatingValidationPreventsContributorAddingARating()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.CreateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("A contributor cannot rate their own resources", result.Details[0]);
        }

        /// <summary>
        /// The CreateRatingValidationPreventsRatingBeingAddedIfUserHasntAccessedTheResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateRatingValidationPreventsRatingBeingAddedIfUserHasntAccessedTheResource()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(false));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.CreateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("You have to access the resource before you can rate it", result.Details[0]);
        }

        /// <summary>
        /// The CreateRatingValidationPreventsRatingBeingAddedIfUserHasntAccessedTheResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateRatingValidationPreventsRatingBeingAddedIfUserAlreadyRatedTheResource()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(new ResourceVersionRating { Rating = userRating }));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.CreateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("You have already rated this resource", result.Details[0]);
        }

        /**********************/
        /* UpdateRating Tests */
        /**********************/

        /// <summary>
        /// The UpdateRatingRecalculatesAverageRatingCorrectly.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateRatingRecalculatesAverageRatingCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            int userId = 1;
            int resourceVersionId = 2;
            int jobRoleId = 3;
            int locationId = 4;
            int userRating = 5;
            int oldRatingId = 6;
            int ratingId = 7;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(new ResourceVersionRating { Id = oldRatingId }));

            // Set up rating record creation calls.
            this.mockResourceVersionRatingRepository.Setup(r => r.UpdateAsync(userId, It.Is<ResourceVersionRating>(y =>
                    y.Id == oldRatingId &&
                    y.Deleted == true)))
                .Returns(Task.CompletedTask);

            this.mockResourceVersionRatingRepository.Setup(r => r.CreateAsync(userId, It.Is<ResourceVersionRating>(y =>
                    y.UserId == userId &&
                    y.Rating == userRating &&
                    y.ResourceVersionId == resourceVersionId &&
                    y.JobRoleId == jobRoleId &&
                    y.LocationId == locationId)))
                .Returns(Task.FromResult<int>(ratingId));

            this.mockResourceVersionRatingSummaryRepository.Setup(r => r.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<ResourceVersionRatingSummary>(new ResourceVersionRatingSummary()));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetRatingCountsForResourceVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<int[]>(new int[] { 1, 2, 3, 4, 5 }));

            this.mockResourceVersionRatingSummaryRepository.Setup(r => r.UpdateAsync(userId, It.Is<ResourceVersionRatingSummary>(y =>
                    y.Rating1StarCount == 1 &&
                    y.Rating2StarCount == 2 &&
                    y.Rating3StarCount == 3 &&
                    y.Rating4StarCount == 4 &&
                    y.Rating5StarCount == 5 &&
                    y.RatingCount == 15 &&
                    y.AverageRating == 3.7m)))
                .Returns(Task.CompletedTask);

            this.mockResourceService.Setup(r => r.SubmitResourceVersionToSearchAsync(resourceVersionId, userId))
                .ReturnsAsync((true, fixture.Create<int>()));

            this.mockCachingService.Setup(r => r.RemoveAsync($"RatingSummaryBasic:{resourceVersionId}"))
                .ReturnsAsync(new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                });

            this.mockCachingService.Setup(r => r.GetAsync<RatingSummaryBasicViewModel>($"RatingSummaryBasic:{resourceVersionId}"))
                .ReturnsAsync(new CacheReadResponse<RatingSummaryBasicViewModel>()
                {
                    ResponseEnum = CacheReadResponseEnum.Found,
                });

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating, JobRoleId = jobRoleId, LocationId = locationId };
            var result = await this.ratingService.UpdateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// The UpdateRatingValidationPreventsOldVersionBeingRated.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateRatingValidationPreventsOldVersionBeingRated()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(false));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.UpdateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("This version of the resource is not the current version and cannot be rated", result.Details[0]);
        }

        /// <summary>
        /// The UpdateRatingValidationPreventsContributorAddingARating.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateRatingValidationPreventsContributorAddingARating()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.UpdateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("A contributor cannot rate their own resources", result.Details[0]);
        }

        /// <summary>
        /// The UpdateRatingValidationPreventsRatingBeingAddedIfUserHasntAccessedTheResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateRatingValidationPreventsRatingBeingAddedIfUserHasntAccessedTheResource()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(false));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.UpdateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("You have to access the resource before you can rate it", result.Details[0]);
        }

        /// <summary>
        /// The UpdateRatingValidationPreventsRatingBeingUpdatedIfUserHasntAlreadyRatedTheResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateRatingValidationPreventsRatingBeingUpdatedIfUserHasntAlreadyRatedTheResource()
        {
            // Arrange
            int userId = 1;
            int resourceVersionId = 2;
            int userRating = 5;

            // Set up validation calls.
            this.mockResourceRepository.Setup(r => r.IsCurrentVersionAsync(resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockPublicationRepository.Setup(p => p.GetByResourceVersionIdAsync(resourceVersionId))
                .Returns(Task.FromResult<Publication>(new Publication { CreateUserId = userId + 1 }));

            this.mockResourceVersionRepository.Setup(r => r.HasUserCompletedActivity(userId, resourceVersionId))
                .Returns(Task.FromResult<bool>(true));

            this.mockResourceVersionRatingRepository.Setup(r => r.GetUsersPreviousRatingForSameMajorVersionAsync(resourceVersionId, userId))
                .Returns(Task.FromResult<ResourceVersionRating>(null));

            // Act
            var ratingViewModel = new RatingViewModel { EntityVersionId = resourceVersionId, Rating = userRating };
            var result = await this.ratingService.UpdateRating(userId, ratingViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal("A previous rating for this resource was not found", result.Details[0]);
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
