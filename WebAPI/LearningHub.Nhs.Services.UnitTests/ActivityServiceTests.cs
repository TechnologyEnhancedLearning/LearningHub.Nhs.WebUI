namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.UnitTests.Helpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// The activity service tests.
    /// </summary>
    public class ActivityServiceTests
    {
        private readonly Mock<IResourceActivityRepository> mockResourceActivityRepository;
        private readonly Mock<IAssessmentResourceActivityRepository> mockAssessmentResourceActivityRepository;

        private readonly Mock<IAssessmentResourceActivityInteractionRepository>
            mockAssessmentResourceActivityInteractionRepository;

        private readonly Mock<IAssessmentResourceActivityInteractionAnswerRepository>
            mockAssessmentResourceActivityInteractionAnswerRepository;

        private readonly Mock<IMediaResourceActivityRepository> mockMediaResourceActivityRepository;

        private readonly Mock<IMediaResourceActivityInteractionRepository>
            mockMediaResourceActivityInteractionRepository;

        private readonly Mock<IResourceVersionRepository> mockResourceVersionRepository;
        private readonly Mock<IAssessmentResourceVersionRepository> mockAssessmentResourceVersionRepository;
        private readonly Mock<IBlockCollectionRepository> mockBlockCollectionRepository;
        private readonly Mock<IScormActivityRepository> mockScormActivityRepository;
        private readonly Mock<ITimezoneOffsetManager> mockTimezoneOffsetManager;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<ILogger<ActivityService>> mockLogger;
        private readonly IActivityService activityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityServiceTests"/> class.
        /// </summary>
        public ActivityServiceTests()
        {
            this.mockResourceActivityRepository = new Mock<IResourceActivityRepository>();
            this.mockAssessmentResourceActivityRepository = new Mock<IAssessmentResourceActivityRepository>();
            this.mockAssessmentResourceActivityInteractionRepository =
                new Mock<IAssessmentResourceActivityInteractionRepository>();
            this.mockAssessmentResourceActivityInteractionAnswerRepository =
                new Mock<IAssessmentResourceActivityInteractionAnswerRepository>();
            this.mockMediaResourceActivityRepository = new Mock<IMediaResourceActivityRepository>();
            this.mockMediaResourceActivityInteractionRepository =
                new Mock<IMediaResourceActivityInteractionRepository>();
            this.mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            this.mockAssessmentResourceActivityInteractionAnswerRepository =
                new Mock<IAssessmentResourceActivityInteractionAnswerRepository>();
            this.mockAssessmentResourceVersionRepository = new Mock<IAssessmentResourceVersionRepository>();
            this.mockBlockCollectionRepository = new Mock<IBlockCollectionRepository>();
            this.mockScormActivityRepository = new Mock<IScormActivityRepository>();
            this.mockTimezoneOffsetManager = new Mock<ITimezoneOffsetManager>();
            this.mockMapper = new Mock<IMapper>();
            this.mockLogger = new Mock<ILogger<ActivityService>>();
            this.activityService = new ActivityService(
                this.mockResourceActivityRepository.Object,
                this.mockAssessmentResourceActivityRepository.Object,
                this.mockAssessmentResourceActivityInteractionRepository.Object,
                this.mockMediaResourceActivityRepository.Object,
                this.mockMediaResourceActivityInteractionRepository.Object,
                null,
                this.mockResourceVersionRepository.Object,
                this.mockAssessmentResourceVersionRepository.Object,
                this.mockBlockCollectionRepository.Object,
                this.mockScormActivityRepository.Object,
                this.mockTimezoneOffsetManager.Object,
                this.mockLogger.Object,
                this.mockMapper.Object);
        }

        /// <summary>
        /// CreateResourceActivity validates ResourceVersionId.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateResourceActivity_Validates_ResourceVersionId()
        {
            // Arrange
            var createResourceActivityViewModel = new CreateResourceActivityViewModel
            {
                ResourceVersionId = 0,
            };
            var userId = 1;

            // Act
            var result = await this.activityService.CreateResourceActivity(userId, createResourceActivityViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateResourceActivity validates Activity start and end dates.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateResourceActivity_Validates_ActivityStartAndEndDates()
        {
            // Arrange
            var time = DateTimeOffset.Now;
            var createResourceActivityViewModel = new CreateResourceActivityViewModel
            {
                ResourceVersionId = 1,
                ActivityStart = time.AddDays(2),
                ActivityEnd = time,
            };
            var userId = 1;

            // Act
            var result = await this.activityService.CreateResourceActivity(userId, createResourceActivityViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateResourceActivity creates the activity as expected.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateResourceActivity_CreatesActivity()
        {
            // Arrange
            var userId = 1;
            this.mockResourceActivityRepository.Setup(s => s.CreateActivity(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<ActivityStatusEnum>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<DateTimeOffset>())).Returns(1);
            this.mockResourceVersionRepository.Setup(s => s.GetResourceType(1))
                .Returns(Task.FromResult(ResourceTypeEnum.Case));

            var createResourceActivityViewModel = new CreateResourceActivityViewModel
            {
                ResourceVersionId = 1,
                ActivityStart = DateTimeOffset.Now,
                ActivityEnd = DateTimeOffset.Now,
            };

            // Act
            var result = await this.activityService.CreateResourceActivity(userId, createResourceActivityViewModel);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(1, result.CreatedId);
        }

        /// <summary>
        /// CreateResourceActivity checks if there have been too many attempts for the given assessment.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateResourceActivity_ReturnsAnError_GivenTooManyAssessmentAttempts()
        {
            // Arrange
            var userId = 1;
            this.mockResourceActivityRepository.Setup(s => s.CreateActivity(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<ActivityStatusEnum>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<DateTimeOffset>())).Returns(1);
            this.mockResourceVersionRepository.Setup(s => s.GetResourceType(1))
                .Returns(Task.FromResult(ResourceTypeEnum.Assessment));
            this.mockResourceActivityRepository.Setup(s => s.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(Enumerable.Repeat(
                    new ResourceActivity
                    {
                        ResourceVersionId = 1,
                    }, 2)));
            this.mockAssessmentResourceVersionRepository.Setup(s => s.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion
                {
                    MaximumAttempts = 1,
                }));

            var createResourceActivityViewModel = new CreateResourceActivityViewModel
            {
                ResourceVersionId = 1,
                ActivityStart = DateTimeOffset.Now,
                ActivityEnd = DateTimeOffset.Now,
            };

            // Act
            var result = await this.activityService.CreateResourceActivity(userId, createResourceActivityViewModel);

            // Assert
            Assert.Contains(result.Details, d => d == "TooManyAttempts");
        }

        /// <summary>
        /// CreateResourceActivity creates the activity given a valid assessment.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateResourceActivity_CreatesActivity_ForValidAssessment()
        {
            // Arrange
            var userId = 1;
            this.mockResourceActivityRepository.Setup(s => s.CreateActivity(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<ActivityStatusEnum>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<DateTimeOffset>())).Returns(1);
            this.mockResourceVersionRepository.Setup(s => s.GetResourceType(1))
                .Returns(Task.FromResult(ResourceTypeEnum.Assessment));
            this.mockResourceActivityRepository.Setup(s => s.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(Enumerable.Repeat(
                    new ResourceActivity
                    {
                        ResourceVersionId = 1,
                    }, 2)));
            this.mockAssessmentResourceVersionRepository.Setup(s => s.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion
                {
                    MaximumAttempts = 3,
                }));

            var createResourceActivityViewModel = new CreateResourceActivityViewModel
            {
                ResourceVersionId = 1,
                ActivityStart = DateTimeOffset.Now,
                ActivityEnd = DateTimeOffset.Now,
            };

            // Act
            var result = await this.activityService.CreateResourceActivity(userId, createResourceActivityViewModel);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(1, result.CreatedId);
        }

        /// <summary>
        /// CreateMediaResourceActivity validates ResourceActivityId.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMediaResourceActivity_Validates_ResourceActivityId()
        {
            // Arrange
            var createMediaResourceActivityViewModel = new CreateMediaResourceActivityViewModel()
            {
                ResourceActivityId = 0,
            };
            var userId = 1;

            // Act
            var result =
                await this.activityService.CreateMediaResourceActivity(userId, createMediaResourceActivityViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateMediaResourceActivity validates ActivityStart.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMediaResourceActivity_Validates_ActivityStart()
        {
            // Arrange
            var createMediaResourceActivityViewModel = new CreateMediaResourceActivityViewModel()
            {
                ResourceActivityId = 1,
            };
            var userId = 1;

            // Act
            var result =
                await this.activityService.CreateMediaResourceActivity(userId, createMediaResourceActivityViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateMediaResourceActivity creates media resource activity.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMediaResourceActivity_CreatesNewMediaResourceActivity()
        {
            // Arrange
            var userId = 1;
            var mediaResourceActivity = new MediaResourceActivity
            {
                ActivityStart = DateTimeOffset.Now,
                ResourceActivityId = 1,
            };
            var createMediaResourceActivityViewModel = new CreateMediaResourceActivityViewModel()
            {
                ResourceActivityId = 1,
                ActivityStart = mediaResourceActivity.ActivityStart,
            };
            this.mockMapper.Setup(m => m.Map<MediaResourceActivity>(createMediaResourceActivityViewModel))
                .Returns(mediaResourceActivity);
            this.mockMediaResourceActivityRepository.Setup(r => r.CreateAsync(userId, mediaResourceActivity))
                .Returns(Task.FromResult(1));

            // Act
            var result =
                await this.activityService.CreateMediaResourceActivity(userId, createMediaResourceActivityViewModel);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(1, result.CreatedId);
        }

        /// <summary>
        /// CreateMediaResourceActivityInteraction validates MediaResourceActivityId.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMediaResourceActivityInteraction_Validates_MediaResourceActivityId()
        {
            // Arrange
            var createMediaResourceActivityInteractionViewModel = new CreateMediaResourceActivityInteractionViewModel()
            {
                MediaResourceActivityId = 0,
            };
            var userId = 1;

            // Act
            var result =
                await this.activityService.CreateMediaResourceActivityInteraction(userId, createMediaResourceActivityInteractionViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateMediaResourceActivityInteractionViewModel validates MediaResourceActivityType.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMediaResourceActivityInteractionViewModel_Validates_MediaResourceActivityType()
        {
            // Arrange
            var createMediaResourceActivityInteractionViewModel = new CreateMediaResourceActivityInteractionViewModel()
            {
                MediaResourceActivityId = 1,
            };
            var userId = 1;

            // Act
            var result =
                await this.activityService.CreateMediaResourceActivityInteraction(userId, createMediaResourceActivityInteractionViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateMediaResourceActivityInteraction creates media resource activity interaction.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateMediaResourceActivityInteraction_CreatesNewMediaResourceActivityInteraction()
        {
            // Arrange
            var userId = 1;
            var createMediaResourceActivityInteractionViewModel = new CreateMediaResourceActivityInteractionViewModel()
            {
                MediaResourceActivityId = 1,
                MediaResourceActivityType = MediaResourceActivityTypeEnum.End,
            };
            this.mockMediaResourceActivityInteractionRepository
                .Setup(r => r.CreateAsync(userId, It.IsAny<MediaResourceActivityInteraction>()))
                .Returns(Task.FromResult(1));

            // Act
            var result =
                await this.activityService.CreateMediaResourceActivityInteraction(userId, createMediaResourceActivityInteractionViewModel);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(1, result.CreatedId);
        }

        /// <summary>
        /// GetAssessmentResourceIdByActivity returns the corresponding resource version id.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetAssessmentResourceIdByActivity_ReturnsTheCorrespondingResourceVersionId()
        {
            // Arrange
            var assessmentResourceActivityId = 1;
            this.mockAssessmentResourceActivityRepository.Setup(r => r.GetByIdAsync(assessmentResourceActivityId))
                .Returns(Task.FromResult(new AssessmentResourceActivity
                {
                    ResourceActivity = new ResourceActivity
                    {
                        ResourceVersionId = 2,
                    },
                }));

            // Act
            var result = await this.activityService.GetAssessmentResourceIdByActivity(assessmentResourceActivityId);

            // Assert
            Assert.Equal(2, result);
        }

        /// <summary>
        /// GetLatestAssessmentResourceActivityByResourceVersionAndUserId returns the corresponding AssessmentResourceActivity.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task
            GetLatestAssessmentResourceActivityByResourceVersionAndUserId_ReturnsTheCorrespondingAssessmentResourceActivity()
        {
            // Arrange
            var resourceVersionId = 1;
            var userId = 1;
            var assessmentResourceActivity = new AssessmentResourceActivity
            {
                Id = 10,
            };
            this.mockAssessmentResourceActivityRepository
                .Setup(r => r.GetLatestAssessmentResourceActivity(resourceVersionId, userId))
                .Returns(Task.FromResult(assessmentResourceActivity));

            // Act
            var result =
                await this.activityService.GetLatestAssessmentResourceActivityByResourceVersionAndUserId(
                    resourceVersionId, userId);

            // Assert
            Assert.Equal(assessmentResourceActivity, result);
        }

        /// <summary>
        /// GetAttempts returns the corresponding number of attempts.
        /// </summary>
        [Fact]
        public void GetAttempts_ReturnsTheCorrespondingNumberOfAttempts()
        {
            // Arrange
            var resourceVersionId = 1;
            var userId = 1;
            var resourceActivities = new List<ResourceActivity>()
            {
                new ResourceActivity { ResourceVersionId = 1 },
                new ResourceActivity { ResourceVersionId = 1 },
                new ResourceActivity { ResourceVersionId = 2 },
            };
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(resourceActivities));

            // Act
            var result = this.activityService.GetAttempts(userId, resourceVersionId);

            // Assert
            Assert.Equal(2, result);
        }

        /// <summary>
        /// GetAnswersForAllTheAssessmentResourceActivities returns the answers for all the activities of a given assessment.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetAnswersForAllTheAssessmentResourceActivities_ReturnsTheAnswersForAllTheActivities()
        {
            // Arrange
            var resourceVersionId = 1;
            var userId = 1;
            var resourceActivities = new List<ResourceActivity>()
            {
                new ResourceActivity
                {
                    ResourceVersionId = 1,
                    AssessmentResourceActivity = new List<AssessmentResourceActivity>
                    {
                        new AssessmentResourceActivity { Id = 1, CreateDate = DateTimeOffset.Now },
                        new AssessmentResourceActivity { Id = 2, CreateDate = DateTimeOffset.Now },
                    },
                },
                new ResourceActivity
                {
                    ResourceVersionId = 1,
                    AssessmentResourceActivity = new List<AssessmentResourceActivity>
                    {
                        new AssessmentResourceActivity { Id = 3, CreateDate = DateTimeOffset.Now },
                    },
                },
                new ResourceActivity
                {
                    ResourceVersionId = 2,
                    AssessmentResourceActivity = new List<AssessmentResourceActivity>
                    {
                        new AssessmentResourceActivity { Id = 4, CreateDate = DateTimeOffset.Now },
                    },
                },
            };
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(resourceActivities));
            this.mockAssessmentResourceActivityInteractionRepository
                .Setup(r => r.GetInteractionsForAssessmentResourceActivity(It.IsAny<int>()))
                .Returns(Task.FromResult(Enumerable.Range(0, 2)
                    .Select(i =>
                        new AssessmentResourceActivityInteraction
                        {
                            QuestionBlock = new QuestionBlock() { Block = new Block { Order = i, }, },
                            Answers = new List<AssessmentResourceActivityInteractionAnswer>()
                            {
                                new AssessmentResourceActivityInteractionAnswer()
                                {
                                    QuestionAnswer = new QuestionAnswer
                                    {
                                        Order = i + 3,
                                    },
                                },
                            },
                        }).ToList()));
            var expectedResult = new Dictionary<int, Dictionary<int, IEnumerable<int>>>()
            {
                {
                    1, new Dictionary<int, IEnumerable<int>>
                    {
                        { 0, new List<int> { 3 } },
                        { 1, new List<int> { 4 } },
                    }
                },
                {
                    2, new Dictionary<int, IEnumerable<int>>
                    {
                        { 0, new List<int> { 3 } },
                        { 1, new List<int> { 4 } },
                    }
                },
                {
                    3, new Dictionary<int, IEnumerable<int>>
                    {
                        { 0, new List<int> { 3 } },
                        { 1, new List<int> { 4 } },
                    }
                },
            };

            // Act
            var result =
                await this.activityService.GetAnswersForAllTheAssessmentResourceActivities(userId, resourceVersionId);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// GetUserIdForActivity returns the corresponding user Id.
        /// </summary>
        [Fact]
        public void GetUserIdForActivity_ReturnsTheCorrespondingUserId()
        {
            // Arrange
            var assessmentResourceActivityId = 1;
            this.mockAssessmentResourceActivityRepository.Setup(r => r.GetByIdAsync(assessmentResourceActivityId))
                .Returns(Task.FromResult(new AssessmentResourceActivity { CreateUserId = 2 }));

            // Act
            var result = this.activityService.GetUserIdForActivity(assessmentResourceActivityId);

            // Assert
            Assert.Equal(2, result);
        }

        /// <summary>
        /// CreateAssessmentResourceActivity validates ResourceActivityId.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivity_Validates_ResourceActivityId()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityViewModel = new CreateAssessmentResourceActivityViewModel()
            {
                ResourceActivityId = 0,
            };

            // Act
            var result =
                await this.activityService.CreateAssessmentResourceActivity(userId, createAssessmentResourceActivityViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateAssessmentResourceActivity does not create an activity given more than the maximum number of attempts and no reason provided.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivity_ChecksIfNumberOfAttemptsGreaterThanThreshold()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityViewModel = new CreateAssessmentResourceActivityViewModel()
            {
                ResourceActivityId = 1,
            };
            var assessmentResourceActivity = new AssessmentResourceActivity()
            {
                ResourceActivityId = 1,
            };
            this.mockMapper.Setup(m => m.Map<AssessmentResourceActivity>(createAssessmentResourceActivityViewModel))
                .Returns(assessmentResourceActivity);
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockResourceVersionRepository.Setup(r => r.GetResourceType(1))
                .Returns(Task.FromResult(ResourceTypeEnum.Assessment));
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(Enumerable.Repeat(new ResourceActivity(), 3)));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion { MaximumAttempts = 2 }));

            // Act
            var result =
                await this.activityService.CreateAssessmentResourceActivity(userId, createAssessmentResourceActivityViewModel);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateAssessmentResourceActivity creates an activity given more the maximum number of attempts and reason provided.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task
            CreateAssessmentResourceActivity_CreatesActivity_IfAttemptsGreaterThanThresholdAndReasonProvided()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityViewModel = new CreateAssessmentResourceActivityViewModel()
            {
                ResourceActivityId = 1,
                ExtraAttemptReason = "test",
            };
            var assessmentResourceActivity = new AssessmentResourceActivity()
            {
                ResourceActivityId = 1,
            };
            this.mockMapper.Setup(m => m.Map<AssessmentResourceActivity>(createAssessmentResourceActivityViewModel))
                .Returns(assessmentResourceActivity);
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockResourceVersionRepository.Setup(r => r.GetResourceType(1))
                .Returns(Task.FromResult(ResourceTypeEnum.Assessment));
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(Enumerable.Repeat(new ResourceActivity(), 3)));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion { MaximumAttempts = 2 }));
            this.mockAssessmentResourceActivityRepository.Setup(s => s.CreateAsync(userId, assessmentResourceActivity))
                .Returns(Task.FromResult(10));

            // Act
            var result =
                await this.activityService.CreateAssessmentResourceActivity(userId, createAssessmentResourceActivityViewModel);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(10, result.CreatedId);
        }

        /// <summary>
        /// CreateAssessmentResourceActivity creates an activity if the number of attempts is less than the threshold.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivity_CreatesActivity_IfAttemptsLessThanThreshold()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityViewModel = new CreateAssessmentResourceActivityViewModel()
            {
                ResourceActivityId = 1,
            };
            var assessmentResourceActivity = new AssessmentResourceActivity()
            {
                ResourceActivityId = 1,
            };
            this.mockMapper.Setup(m => m.Map<AssessmentResourceActivity>(createAssessmentResourceActivityViewModel))
                .Returns(assessmentResourceActivity);
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockResourceVersionRepository.Setup(r => r.GetResourceType(1))
                .Returns(Task.FromResult(ResourceTypeEnum.Assessment));
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(Enumerable.Repeat(new ResourceActivity(), 2)));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion { MaximumAttempts = 2 }));
            this.mockAssessmentResourceActivityRepository.Setup(s => s.CreateAsync(userId, assessmentResourceActivity))
                .Returns(Task.FromResult(10));

            // Act
            var result =
                await this.activityService.CreateAssessmentResourceActivity(userId, createAssessmentResourceActivityViewModel);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(10, result.CreatedId);
        }

        /// <summary>
        /// CreateAssessmentResourceActivity creates an activity if no maximum number of attempts.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivity_CreatesActivity_IfNoMaximumNumberOfAttempts()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityViewModel = new CreateAssessmentResourceActivityViewModel()
            {
                ResourceActivityId = 1,
            };
            var assessmentResourceActivity = new AssessmentResourceActivity()
            {
                ResourceActivityId = 1,
            };
            this.mockMapper.Setup(m => m.Map<AssessmentResourceActivity>(createAssessmentResourceActivityViewModel))
                .Returns(assessmentResourceActivity);
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockResourceVersionRepository.Setup(r => r.GetResourceType(1))
                .Returns(Task.FromResult(ResourceTypeEnum.Assessment));
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(userId))
                .Returns(new TestAsyncEnumerable<ResourceActivity>(Enumerable.Repeat(new ResourceActivity(), 2)));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion()));
            this.mockAssessmentResourceActivityRepository.Setup(s => s.CreateAsync(userId, assessmentResourceActivity))
                .Returns(Task.FromResult(10));

            // Act
            var result =
                await this.activityService.CreateAssessmentResourceActivity(userId, createAssessmentResourceActivityViewModel);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(10, result.CreatedId);
        }

        /// <summary>
        /// CreateAssessmentResourceActivityInteraction validates AssessmentResourceActivityId.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivityInteraction_Validates_AssessmentResourceActivityId()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityInteractionViewModel =
                new CreateAssessmentResourceActivityInteractionViewModel()
                {
                    AssessmentResourceActivityId = 0,
                };

            // Act
            var result = await this.activityService.CreateAssessmentResourceActivityInteraction(userId, createAssessmentResourceActivityInteractionViewModel, true);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateAssessmentResourceActivityInteraction checks if the user has submitted an invalid answer.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivityInteraction_Checks_IfUserSubmittedInvalidAnswer()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityInteractionViewModel =
                new CreateAssessmentResourceActivityInteractionViewModel()
                {
                    AssessmentResourceActivityId = 1,
                    QuestionNumber = 1,
                    Answers = new List<int> { 0 },
                };
            this.mockAssessmentResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceActivity
                    { Id = 1, ResourceActivityId = 1, CreateUserId = 1 }));
            this.mockAssessmentResourceActivityInteractionRepository.Setup(r => r.GetAll())
                .Returns(new TestAsyncEnumerable<AssessmentResourceActivityInteraction>(
                    new List<AssessmentResourceActivityInteraction>()));
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion { AssessmentContentId = 1 }));
            this.mockBlockCollectionRepository.Setup(r => r.GetQuestionBlocks(1))
                .Returns(Task.FromResult(
                    new List<Block>
                    {
                        new Block { Order = 0, BlockType = BlockType.Question, QuestionBlock = new QuestionBlock() },
                        new Block { Order = 1, BlockType = BlockType.Question, QuestionBlock = new QuestionBlock() },
                        new Block { Order = 2, BlockType = BlockType.Question, QuestionBlock = new QuestionBlock() },
                    }));
            this.mockMapper.Setup(x => x.Map<QuestionBlockViewModel>(It.IsAny<QuestionBlock>()))
                .Returns(
                    new QuestionBlockViewModel()
                    {
                        QuestionType = QuestionTypeEnum.SingleChoice,
                        Answers = new List<QuestionAnswerViewModel>()
                        {
                            new QuestionAnswerViewModel { Id = 1, Order = 0, Status = QuestionAnswerStatus.Best },
                            new QuestionAnswerViewModel { Id = 2, Order = 1, Status = QuestionAnswerStatus.Incorrect },
                        },
                    });

            // Act
            var result =
                await this.activityService.CreateAssessmentResourceActivityInteraction(userId, createAssessmentResourceActivityInteractionViewModel, true);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateAssessmentResourceActivityInteraction checks if the user has already answered the question.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivityInteraction_Checks_IfUserHasAlreadyAnsweredTheQuestion()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityInteractionViewModel =
                new CreateAssessmentResourceActivityInteractionViewModel()
                {
                    AssessmentResourceActivityId = 1,
                    QuestionNumber = 0,
                    Answers = new List<int> { 0 },
                };
            this.mockAssessmentResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceActivity { Id = 1, ResourceActivityId = 1 }));
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion { AssessmentContentId = 1 }));
            this.mockBlockCollectionRepository.Setup(r => r.GetQuestionBlocks(1))
                .Returns(Task.FromResult(
                    new List<Block> { new Block { Order = 0, BlockType = BlockType.Question, QuestionBlock = new QuestionBlock() { } } }));
            this.mockAssessmentResourceActivityInteractionRepository
                .Setup(r => r.GetInteractionForQuestion(userId, 1, 0))
                .Returns(Task.FromResult(new AssessmentResourceActivityInteraction()));
            this.mockMapper.Setup(x => x.Map<QuestionBlockViewModel>(It.IsAny<QuestionBlock>()))
                .Returns(
                    new QuestionBlockViewModel()
                    {
                        QuestionType = QuestionTypeEnum.SingleChoice,
                        Answers = new List<QuestionAnswerViewModel>()
                        {
                            new QuestionAnswerViewModel { Id = 1, Order = 0, Status = QuestionAnswerStatus.Best },
                            new QuestionAnswerViewModel { Id = 2, Order = 1, Status = QuestionAnswerStatus.Incorrect },
                        },
                    });

            // Act
            var result =
                await this.activityService.CreateAssessmentResourceActivityInteraction(userId, createAssessmentResourceActivityInteractionViewModel, false);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateAssessmentResourceActivityInteraction checks if the assessment is already completed.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateAssessmentResourceActivityInteraction_Checks_IfAssessmentIsAlreadyCompleted()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityInteractionViewModel =
                new CreateAssessmentResourceActivityInteractionViewModel()
                {
                    AssessmentResourceActivityId = 1,
                    QuestionNumber = 0,
                    Answers = new List<int> { 0 },
                };
            this.mockAssessmentResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceActivity
                    { Id = 1, ResourceActivityId = 1, Score = 100 }));
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion { AssessmentContentId = 1 }));
            this.mockBlockCollectionRepository.Setup(r => r.GetQuestionBlocks(1))
                .Returns(Task.FromResult(
                    new List<Block> { new Block { Order = 0, BlockType = BlockType.Question, QuestionBlock = new QuestionBlock() { Id = 1 } } }));
            this.mockMapper.Setup(x => x.Map<QuestionBlockViewModel>(It.IsAny<QuestionBlock>()))
                .Returns(
                    new QuestionBlockViewModel()
                    {
                        QuestionType = QuestionTypeEnum.SingleChoice,
                        Answers = new List<QuestionAnswerViewModel>()
                        {
                            new QuestionAnswerViewModel { Id = 1, Order = 0, Status = QuestionAnswerStatus.Best },
                            new QuestionAnswerViewModel { Id = 2, Order = 1, Status = QuestionAnswerStatus.Incorrect },
                        },
                    });

            // Act
            var result = await this.activityService.CreateAssessmentResourceActivityInteraction(userId, createAssessmentResourceActivityInteractionViewModel, false);

            // Assert
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// CreateAssessmentResourceActivityInteraction does not complete the assessment if not all the questions are answered.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task
            CreateAssessmentResourceActivityInteraction_DoesNotCompleteTheAssessment_IfNotAllTheQuestionsAreAnswered()
        {
            // Arrange
            var userId = 1;
            var createAssessmentResourceActivityInteractionViewModel =
                new CreateAssessmentResourceActivityInteractionViewModel()
                {
                    AssessmentResourceActivityId = 1,
                    QuestionNumber = 0,
                    Answers = new List<int> { 0 },
                };
            this.mockAssessmentResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceActivity { Id = 1, ResourceActivityId = 1 }));
            this.mockResourceActivityRepository.Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(new ResourceActivity { ResourceVersionId = 1 }));
            this.mockAssessmentResourceVersionRepository.Setup(r => r.GetByResourceVersionIdAsync(1))
                .Returns(Task.FromResult(new AssessmentResourceVersion { AssessmentContentId = 1 }));

            this.mockBlockCollectionRepository.Setup(r => r.GetQuestionBlocks(1))
                .Returns(Task.FromResult(
                    Enumerable.Repeat(
                        new Block
                        {
                            Order = 0, BlockType = BlockType.Question,
                            QuestionBlock = new QuestionBlock
                                { Answers = new List<QuestionAnswer> { new QuestionAnswer { Id = 1, Order = 0 } } },
                        }, 2).ToList()));
            this.mockAssessmentResourceActivityInteractionRepository.Setup(r =>
                    r.CreateInteraction(userId, It.IsAny<AssessmentResourceActivityInteraction>()))
                .Returns(Task.FromResult(10));
            this.mockMapper.Setup(x => x.Map<QuestionBlockViewModel>(It.IsAny<QuestionBlock>()))
                .Returns(
                    new QuestionBlockViewModel()
                    {
                        QuestionType = QuestionTypeEnum.SingleChoice,
                        Answers = new List<QuestionAnswerViewModel>()
                        {
                            new QuestionAnswerViewModel { Id = 1, Order = 0, Status = QuestionAnswerStatus.Best },
                            new QuestionAnswerViewModel { Id = 2, Order = 1, Status = QuestionAnswerStatus.Incorrect },
                        },
                    });

            // Act
            var result = await this.activityService.CreateAssessmentResourceActivityInteraction(userId, createAssessmentResourceActivityInteractionViewModel, false);

            // Assert
            this.mockResourceActivityRepository.Verify(
                r =>
                    r.CreateActivity(userId, 1, It.IsAny<int>(), It.IsAny<int>(), ActivityStatusEnum.Completed, null, It.IsAny<DateTimeOffset>()), Moq.Times.Never);
            Assert.True(result.IsValid);
            Assert.Equal(10, result.CreatedId);
        }
    }
}