// <copyright file="MyLearningServiceTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using EntityFrameworkCore.Testing.Common;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.UnitTests.Helpers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// The migration service tests.
    /// </summary>
    public class MyLearningServiceTests
    {
        /// <summary>
        /// The mock resourceActivityRepository.
        /// </summary>
        private readonly Mock<IResourceActivityRepository> mockResourceActivityRepository;

        /// <summary>
        /// The mock mediaResourcePlayedSegmentRepository.
        /// </summary>
        private readonly Mock<IMediaResourcePlayedSegmentRepository> mockMediaResourcePlayedSegmentRepository;

        /// <summary>
        /// The My Learning service.
        /// </summary>
        private readonly IMyLearningService myLearningService;

        /// <summary>
        /// The mock mockCatalogueNodeVersionRepository.
        /// </summary>
        private readonly Mock<ICatalogueNodeVersionRepository> mockCatalogueNodeVersionRepository;

        /// <summary>
        /// The mock mockscormActivityRepository.
        /// </summary>
        private readonly Mock<IScormActivityRepository> mockscormActivityRepository;

        /// <summary>
        /// The mock mockmediaResourceActivityRepository.
        /// </summary>
        private readonly Mock<IMediaResourceActivityRepository> mockmediaResourceActivityRepository;

        /// <summary>
        /// The mock settings.
        /// </summary>
        private readonly Mock<IOptions<Settings>> mockSettings;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningServiceTests"/> class.
        /// </summary>
        public MyLearningServiceTests()
        {
            this.mockResourceActivityRepository = new Mock<IResourceActivityRepository>(MockBehavior.Strict);
            this.mockMediaResourcePlayedSegmentRepository = new Mock<IMediaResourcePlayedSegmentRepository>(MockBehavior.Strict);
            this.mockmediaResourceActivityRepository = new Mock<IMediaResourceActivityRepository>(MockBehavior.Strict);
            this.mockCatalogueNodeVersionRepository = new Mock<ICatalogueNodeVersionRepository>(MockBehavior.Strict);
            this.mockscormActivityRepository = new Mock<IScormActivityRepository>(MockBehavior.Strict);
            this.mockSettings = new Mock<IOptions<Settings>>(MockBehavior.Loose);

            this.myLearningService = new MyLearningService(
                this.mockResourceActivityRepository.Object,
                this.mockMediaResourcePlayedSegmentRepository.Object,
                null,
                null,
                this.mockCatalogueNodeVersionRepository.Object,
                this.NewMapper(),
                this.mockSettings.Object,
                this.mockscormActivityRepository.Object,
                this.mockmediaResourceActivityRepository.Object);

            this.settings = new Settings
            {
                DetailedMediaActivityRecordingStartDate = default(DateTimeOffset),
            };
            this.mockSettings.Setup(x => x.Value).Returns(this.settings);
        }

        /// <summary>
        /// The GetActivityDetailedReturnsCorrectlyPopulatedReturnObject.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedReturnsCorrectlyPopulatedReturnObject()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel { Take = 10 };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(6, result.TotalCount);
            Assert.Equal(6, result.Activities.Count);
            Assert.Equal("Audio", result.Activities.ElementAt(0).Title);
            Assert.Equal("1.0", result.Activities.ElementAt(0).Version);
            Assert.Equal(100, result.Activities.ElementAt(0).ResourceDurationMilliseconds);
            Assert.Equal(99, result.Activities.ElementAt(0).CompletionPercentage);
            Assert.Equal(data.ElementAt(0).ActivityStart, result.Activities.ElementAt(0).ActivityDate);
        }

        /// <summary>
        /// The GetActivityDetailedWithDefaultRequestParamsReturnsAllActivityData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedWithDefaultRequestParamsReturnsAllActivityData()
        {
            // Arrange
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(this.TestActivitiesAsyncMock().Object));

            var request = new MyLearningRequestModel { Take = 10 };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(6, result.TotalCount);
            Assert.Equal(6, result.Activities.Count);
            Assert.Equal("Audio", result.Activities.ElementAt(0).Title);
            Assert.Equal("Video", result.Activities.ElementAt(1).Title);
            Assert.Equal("Weblink", result.Activities.ElementAt(2).Title);
            Assert.Equal("Article", result.Activities.ElementAt(3).Title);
            Assert.Equal("GenericFile", result.Activities.ElementAt(4).Title);
            Assert.Equal("Image", result.Activities.ElementAt(5).Title);
        }

        /// <summary>
        /// The GetActivityDetailedSkipAndTakeReturnsCorrectActivities.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedSkipAndTakeReturnsCorrectActivities()
        {
            // Arrange
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(this.TestActivitiesAsyncMock().Object));

            var request = new MyLearningRequestModel
            {
                Skip = 2,
                Take = 3,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(6, result.TotalCount);
            Assert.Equal(3, result.Activities.Count);
            Assert.Equal("Weblink", result.Activities.ElementAt(0).Title);
            Assert.Equal("Article", result.Activities.ElementAt(1).Title);
            Assert.Equal("GenericFile", result.Activities.ElementAt(2).Title);
        }

        /// <summary>
        /// The GetActivityDetailedTextFilterReturnsTitleMatch.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedTextFilterReturnsTitleMatch()
        {
            // Arrange
            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(this.TestActivitiesAsyncMock().Object));

            var request = new MyLearningRequestModel
            {
                SearchText = "Video",
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Video", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedTextFilterReturnsDescriptionMatch.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedTextFilterReturnsDescriptionMatch()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;
            data.ElementAt(2).ResourceVersion.Description = "myDescription"; // Element at pos 2 = weblink.

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                SearchText = "myDescription",
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Weblink", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedTextFilterReturnsKeywordMatch.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedTextFilterReturnsKeywordMatch()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;
            data.ElementAt(3).ResourceVersion.ResourceVersionKeyword.ElementAt(0).Keyword = "myKeyword"; // Element at pos 3 = article.

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                SearchText = "myKeyword",
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Article", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedArticleFilterReturnsCorrectResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedArticleFilterReturnsCorrectResource()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                Article = true,
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Article", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedAudioFilterReturnsCorrectResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedAudioFilterReturnsCorrectResource()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                Audio = true,
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Audio", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedFileFilterReturnsCorrectResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedFileFilterReturnsCorrectResource()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                File = true,
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("GenericFile", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedImageFilterReturnsCorrectResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedImageFilterReturnsCorrectResource()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                Image = true,
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Image", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedVideoFilterReturnsCorrectResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedVideoFilterReturnsCorrectResource()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                Video = true,
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Video", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedWeblinkFilterReturnsCorrectResource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedWeblinkFilterReturnsCorrectResource()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                Weblink = true,
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Weblink", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedMultipleResourceTypeFiltersWorkTogether.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedMultipleResourceTypeFiltersWorkTogether()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                Article = true,
                Audio = true,
                File = true,
                Image = true,
                Video = true,
                Weblink = true,
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(6, result.TotalCount);
            Assert.Equal(6, result.Activities.Count);
            Assert.Equal("Audio", result.Activities.ElementAt(0).Title);
            Assert.Equal("Video", result.Activities.ElementAt(1).Title);
            Assert.Equal("Weblink", result.Activities.ElementAt(2).Title);
            Assert.Equal("Article", result.Activities.ElementAt(3).Title);
            Assert.Equal("GenericFile", result.Activities.ElementAt(4).Title);
            Assert.Equal("Image", result.Activities.ElementAt(5).Title);
        }

        /// <summary>
        /// The GetActivityDetailedCompleteFilterReturnsCorrectMatches.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedCompleteFilterReturnsCorrectMatches()
        {
            // Arrange
            var data = this.TestActivitiesAsyncMock().Object;

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(6, result.TotalCount);
            Assert.Equal(6, result.Activities.Count);
            Assert.Equal("Audio", result.Activities.ElementAt(0).Title);
            Assert.Equal("Video", result.Activities.ElementAt(1).Title);
            Assert.Equal("Weblink", result.Activities.ElementAt(2).Title);
            Assert.Equal("Article", result.Activities.ElementAt(3).Title);
            Assert.Equal("GenericFile", result.Activities.ElementAt(4).Title);
            Assert.Equal("Image", result.Activities.ElementAt(5).Title);
        }

        /// <summary>
        /// The GetActivityDetailedThisWeekFilterReturnsCorrectMatches.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedThisWeekFilterReturnsCorrectMatches()
        {
            // Arrange
            var now = DateTime.Now;
            var data = this.TestActivitiesAsyncMock().Object;

            data.ElementAt(0).ActivityStart = now;
            data.ElementAt(1).ActivityStart = now.AddDays(-1);
            data.ElementAt(2).ActivityStart = now.AddDays(-2);
            data.ElementAt(3).ActivityStart = now.AddDays(-3);
            data.ElementAt(4).ActivityStart = now.AddDays(-4);
            data.ElementAt(5).ActivityStart = now.AddDays(-5);

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                TimePeriod = "thisWeek",
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            // Awkward to write asserts as depends on what day of week the test is run on.
            var dayOfWeekToday = now.DayOfWeek;
            var daysSinceMonday = (dayOfWeekToday > 0) ? (int)dayOfWeekToday : 7; // convert Sunday from 0 to 7.

            Assert.Equal(daysSinceMonday, result.TotalCount);
            Assert.Equal(daysSinceMonday, result.Activities.Count);
        }

        /// <summary>
        /// The GetActivityDetailedThisMonthFilterReturnsCorrectMatches.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedThisMonthFilterReturnsCorrectMatches()
        {
            // Arrange
            var now = DateTime.Now;
            var data = this.TestActivitiesAsyncMock().Object;

            data.ElementAt(0).ActivityStart = new DateTime(now.Year, now.Month, 1).AddDays(-4); // Too early.
            data.ElementAt(1).ActivityStart = new DateTime(now.Year, now.Month, 1).AddDays(-3); // Too early.
            data.ElementAt(2).ActivityStart = new DateTime(now.Year, now.Month, 1).AddDays(-2); // Too early.
            data.ElementAt(3).ActivityStart = new DateTime(now.Year, now.Month, 1).AddDays(-1); // Too early.
            data.ElementAt(4).ActivityStart = new DateTime(now.Year, now.Month, 1); // During "this month".
            data.ElementAt(5).ActivityStart = now; // During "this month".

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                TimePeriod = "thisMonth",
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Activities.Count);
            Assert.Equal("GenericFile", result.Activities.ElementAt(0).Title);
            Assert.Equal("Image", result.Activities.ElementAt(1).Title);
        }

        /// <summary>
        /// The GetActivityDetailedLast12MonthsFilterReturnsCorrectMatches.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedLast12MonthsFilterReturnsCorrectMatches()
        {
            // Arrange
            var now = DateTime.Now;
            var data = this.TestActivitiesAsyncMock().Object;

            data.ElementAt(0).ActivityStart = now.AddYears(-1).AddDays(-4); // Too early.
            data.ElementAt(1).ActivityStart = now.AddYears(-1).AddDays(-3); // Too early.
            data.ElementAt(2).ActivityStart = now.AddYears(-1).AddDays(-2); // Too early.
            data.ElementAt(3).ActivityStart = now.AddYears(-1).AddDays(-1); // Too early.
            data.ElementAt(4).ActivityStart = now.AddYears(-1); // During "last 12 months.
            data.ElementAt(5).ActivityStart = now.AddYears(-1).AddDays(1); // During "last 12 months".

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                TimePeriod = "last12Months",
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Activities.Count);
            Assert.Equal("GenericFile", result.Activities.ElementAt(0).Title);
            Assert.Equal("Image", result.Activities.ElementAt(1).Title);
        }

        /// <summary>
        /// The GetActivityDetailedStartDateFilterReturnsCorrectMatches.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedStartDateFilterReturnsCorrectMatches()
        {
            // Arrange
            var now = DateTime.Now;
            var data = this.TestActivitiesAsyncMock().Object;

            data.ElementAt(0).ActivityStart = new DateTime(2020, 10, 1);
            data.ElementAt(1).ActivityStart = new DateTime(2020, 10, 2);
            data.ElementAt(2).ActivityStart = new DateTime(2020, 10, 3);
            data.ElementAt(3).ActivityStart = new DateTime(2020, 10, 4);
            data.ElementAt(4).ActivityStart = new DateTime(2020, 10, 5);
            data.ElementAt(5).ActivityStart = new DateTime(2020, 10, 6);

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                TimePeriod = "dateRange",
                StartDate = new DateTime(2020, 10, 6),
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Activities);
            Assert.Equal("Image", result.Activities.ElementAt(0).Title);
        }

        /// <summary>
        /// The GetActivityDetailedEndDateFilterReturnsCorrectMatches.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedEndDateFilterReturnsCorrectMatches()
        {
            // Arrange
            var now = DateTime.Now;
            var data = this.TestActivitiesAsyncMock().Object;

            data.ElementAt(0).ActivityStart = new DateTime(2020, 10, 1);
            data.ElementAt(1).ActivityStart = new DateTime(2020, 10, 2);
            data.ElementAt(2).ActivityStart = new DateTime(2020, 10, 3);
            data.ElementAt(3).ActivityStart = new DateTime(2020, 10, 4);
            data.ElementAt(4).ActivityStart = new DateTime(2020, 10, 5);
            data.ElementAt(5).ActivityStart = new DateTime(2020, 10, 6);

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                TimePeriod = "dateRange",
                EndDate = new DateTime(2020, 10, 2),
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Activities.Count);
            Assert.Equal("Audio", result.Activities.ElementAt(0).Title);
            Assert.Equal("Video", result.Activities.ElementAt(1).Title);
        }

        /// <summary>
        /// The GetActivityDetailedStartAndEndDateFilterReturnsCorrectMatches.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetActivityDetailedStartAndEndDateFilterReturnsCorrectMatches()
        {
            // Arrange
            var now = DateTime.Now;
            var data = this.TestActivitiesAsyncMock().Object;

            data.ElementAt(0).ActivityStart = new DateTime(2020, 10, 1);
            data.ElementAt(1).ActivityStart = new DateTime(2020, 10, 2);
            data.ElementAt(2).ActivityStart = new DateTime(2020, 10, 3);
            data.ElementAt(3).ActivityStart = new DateTime(2020, 10, 4);
            data.ElementAt(4).ActivityStart = new DateTime(2020, 10, 5);
            data.ElementAt(5).ActivityStart = new DateTime(2020, 10, 6);

            this.mockResourceActivityRepository.Setup(r => r.GetByUserId(It.IsAny<int>()))
                .Returns(new AsyncEnumerable<ResourceActivity>(data));

            var request = new MyLearningRequestModel
            {
                TimePeriod = "dateRange",
                StartDate = new DateTime(2020, 10, 4),
                EndDate = new DateTime(2020, 10, 6),
                Take = 10,
            };

            // Act
            var result = await this.myLearningService.GetActivityDetailed(1, request);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Activities.Count);
            Assert.Equal("Article", result.Activities.ElementAt(0).Title);
            Assert.Equal("GenericFile", result.Activities.ElementAt(1).Title);
            Assert.Equal("Image", result.Activities.ElementAt(2).Title);
        }

        /// <summary>
        /// The test countries async mock.
        /// </summary>
        /// <returns>The <see cref="Mock"/>.</returns>
        private Mock<DbSet<ResourceActivity>> TestActivitiesAsyncMock()
        {
            var records = this.GetActivityData().AsQueryable();

            var mockDbSet = new Mock<DbSet<ResourceActivity>>();

            mockDbSet.As<IAsyncEnumerable<ResourceActivity>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(new TestAsyncEnumerator<ResourceActivity>(records.GetEnumerator()));

            mockDbSet.As<IQueryable<ResourceActivity>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<ResourceActivity>(records.Provider));

            mockDbSet.As<IQueryable<ResourceActivity>>().Setup(m => m.Expression).Returns(records.Expression);
            mockDbSet.As<IQueryable<ResourceActivity>>().Setup(m => m.ElementType).Returns(records.ElementType);
            mockDbSet.As<IQueryable<ResourceActivity>>().Setup(m => m.GetEnumerator()).Returns(() => records.GetEnumerator());

            return mockDbSet;
        }

        private List<ResourceActivity> GetActivityData()
        {
            var now = DateTime.Now;
            return new List<ResourceActivity>
            {
                new ResourceActivity
                {
                    ActivityStart = now,
                    DurationSeconds = 99,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    NodePathId = 1,
                    Resource = new Resource
                    {
                        ResourceTypeEnum = ResourceTypeEnum.Audio,
                        ResourceReference = new List<ResourceReference>
                        {
                            new ResourceReference { Id = 1, NodePathId = 1 },
                        },
                    },
                    ResourceVersion = new ResourceVersion
                    {
                        Title = "Audio",
                        AudioResourceVersion = new AudioResourceVersion
                        {
                            DurationInMilliseconds = 100,
                        },
                        Description = "description",
                        ResourceVersionKeyword = new List<ResourceVersionKeyword>
                        {
                            new ResourceVersionKeyword { Keyword = "keyword" },
                        },
                    },
                    MediaResourceActivity = new List<MediaResourceActivity>
                    {
                        new MediaResourceActivity
                        {
                            PercentComplete = 99,
                            SecondsPlayed = 10,
                        },
                    },
                },
                new ResourceActivity
                {
                    ActivityStart = now,
                    DurationSeconds = 99,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    NodePathId = 1,
                    Resource = new Resource
                    {
                        ResourceTypeEnum = ResourceTypeEnum.Video,
                        ResourceReference = new List<ResourceReference>
                        {
                            new ResourceReference { Id = 1, NodePathId = 1 },
                        },
                    },
                    ResourceVersion = new ResourceVersion
                    {
                        Title = "Video",
                        VideoResourceVersion = new VideoResourceVersion
                        {
                            DurationInMilliseconds = 100,
                        },
                        Description = "description",
                        ResourceVersionKeyword = new List<ResourceVersionKeyword>
                        {
                            new ResourceVersionKeyword { Keyword = "keyword" },
                        },
                    },
                    MediaResourceActivity = new List<MediaResourceActivity>
                    {
                        new MediaResourceActivity
                        {
                            PercentComplete = 100,
                            SecondsPlayed = 10,
                        },
                    },
                },
                new ResourceActivity
                {
                    ActivityStart = now,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    NodePathId = 1,
                    Resource = new Resource
                    {
                        ResourceTypeEnum = ResourceTypeEnum.WebLink,
                        ResourceReference = new List<ResourceReference>
                        {
                            new ResourceReference { Id = 1, NodePathId = 1 },
                        },
                    },
                    ResourceVersion = new ResourceVersion
                    {
                        Title = "Weblink",
                        Description = "description",
                        ResourceVersionKeyword = new List<ResourceVersionKeyword>
                        {
                            new ResourceVersionKeyword { Keyword = "keyword" },
                        },
                    },
                },
                new ResourceActivity
                {
                    ActivityStart = now,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    NodePathId = 1,
                    Resource = new Resource
                    {
                        ResourceTypeEnum = ResourceTypeEnum.Article,
                        ResourceReference = new List<ResourceReference>
                        {
                            new ResourceReference { Id = 1, NodePathId = 1 },
                        },
                    },
                    ResourceVersion = new ResourceVersion
                    {
                        Title = "Article",
                        Description = "description",
                        ResourceVersionKeyword = new List<ResourceVersionKeyword>
                        {
                            new ResourceVersionKeyword { Keyword = "keyword" },
                        },
                    },
                },
                new ResourceActivity
                {
                    ActivityStart = now,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    NodePathId = 1,
                    Resource = new Resource
                    {
                        ResourceTypeEnum = ResourceTypeEnum.GenericFile,
                        ResourceReference = new List<ResourceReference>
                        {
                            new ResourceReference { Id = 1, NodePathId = 1 },
                        },
                    },
                    ResourceVersion = new ResourceVersion
                    {
                        Title = "GenericFile",
                        Description = "description",
                        ResourceVersionKeyword = new List<ResourceVersionKeyword>
                        {
                            new ResourceVersionKeyword { Keyword = "keyword" },
                        },
                    },
                },
                new ResourceActivity
                {
                    ActivityStart = now,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    NodePathId = 1,
                    Resource = new Resource
                    {
                        ResourceTypeEnum = ResourceTypeEnum.Image,
                        ResourceReference = new List<ResourceReference>
                        {
                            new ResourceReference { Id = 1, NodePathId = 1 },
                        },
                    },
                    ResourceVersion = new ResourceVersion
                    {
                        Title = "Image",
                        Description = "description",
                        ResourceVersionKeyword = new List<ResourceVersionKeyword>
                        {
                            new ResourceVersionKeyword { Keyword = "keyword" },
                        },
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
    }
}
