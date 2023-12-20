// <copyright file="NotificationServiceTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

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
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.UnitTests.Helpers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// The notification service tests.
    /// </summary>
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> mockNotificationRepo;
        private readonly Mock<IOptions<Settings>> mockSettings;
        private readonly NotificationService notificationService;
        private Notification notification = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationServiceTests"/> class.
        /// </summary>
        public NotificationServiceTests()
        {
            this.mockSettings = new Mock<IOptions<Settings>>();
            this.mockNotificationRepo = new Mock<INotificationRepository>();
            this.SetupSettings();

            this.notificationService = new NotificationService(this.mockNotificationRepo.Object, this.mockSettings.Object, this.NewMapper());
        }

        /// <summary>
        /// The get by id async_ valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_Valid()
        {
            int notificationId = 8;

            this.mockNotificationRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .ReturnsAsync(this.GetNotification(notificationId));

            var notification = await this.notificationService.GetByIdAsync(notificationId);

            Assert.IsType<NotificationViewModel>(notification);
            Assert.Equal(8, notification.Id);
            Assert.Equal("e-Den enhanced CPD forms Test123", notification.Title);
        }

        /// <summary>
        /// The get by id async_ invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_Invalid()
        {
            this.mockNotificationRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .ReturnsAsync((Notification)null);

            var notification = await this.notificationService.GetByIdAsync(999);

            Assert.Null(notification);
        }

        /// <summary>
        /// The get page async_ valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Valid()
        {
            int page = 3;
            int pageSize = 5;

            this.mockNotificationRepo.Setup(r => r.GetAllFull())
                .Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notifications = await this.notificationService.GetPageAsync(page, pageSize);

            Assert.IsType<List<NotificationViewModel>>(notifications.Items);
            Assert.Equal(5, notifications.Items.Count);
            Assert.Equal(18, notifications.TotalItemCount);
            Assert.Equal(14, notifications.Items.First().Id);
            Assert.Equal(18, notifications.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ id_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Id_Asc()
        {
            int page = 2;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Id", "A", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(4, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(8, notificationsPage.Items.First().Id);
            Assert.Equal(11, notificationsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ id_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Id_Desc()
        {
            int page = 2;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Id", "D", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(4, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(17, notificationsPage.Items.First().Id);
            Assert.Equal(14, notificationsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ title_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Title_Asc()
        {
            int page = 2;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Title", "A", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(4, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(15, notificationsPage.Items.First().Id);
            Assert.Equal(19, notificationsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ title_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Title_Desc()
        {
            int page = 2;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Title", "D", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(4, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(3, notificationsPage.Items.First().Id);
            Assert.Equal(11, notificationsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ start date_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_StartDate_Asc()
        {
            int page = 5;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "StartDate", "A", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(2, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(27, notificationsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ start date_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_StartDate_Desc()
        {
            int page = 5;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "StartDate", "D", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(2, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(3, notificationsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ end date_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_EndDate_Asc()
        {
            int page = 1;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "EndDate", "A", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(4, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(27, notificationsPage.Items.First().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ end date_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_EndDate_Desc()
        {
            int page = 5;
            int pageSize = 4;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "EndDate", "D", string.Empty);

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(2, notificationsPage.Items.Count);
            Assert.Equal(18, notificationsPage.TotalItemCount);
            Assert.Equal(27, notificationsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ filter_ title.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_Title()
        {
            int page = 1;
            int pageSize = 5;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"Title\",\"Value\":\"Healthcare\"}]");

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Equal(2, notificationsPage.Items.Count);
            Assert.Equal(2, notificationsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ filter_ start date.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_StartDate()
        {
            int page = 1;
            int pageSize = 5;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"StartDate\",\"Value\":\"08 Aug 2019\"}]");

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Single(notificationsPage.Items);
            Assert.Equal(1, notificationsPage.TotalItemCount);
            Assert.Equal(27, notificationsPage.Items.First().Id);
        }

        /// <summary>
        /// The get page async_ filter_ end date.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_EndDate()
        {
            int page = 1;
            int pageSize = 5;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"EndDate\",\"Value\":\"31 Dec 2020\"}]");

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Single(notificationsPage.Items);
            Assert.Equal(1, notificationsPage.TotalItemCount);
            Assert.Equal(3, notificationsPage.Items.First().Id);
        }

        /// <summary>
        /// The get page async_ filter_ ceated by.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_CeatedBy()
        {
            int page = 1;
            int pageSize = 5;

            this.mockNotificationRepo.Setup(r => r.GetAllFull()).Returns(new AsyncEnumerable<Notification>(this.TestNotificationsAsyncMock().Object));

            var notificationsPage = await this.notificationService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"CreatedBy\",\"Value\":\"test1.user\"}]");

            Assert.IsType<List<NotificationViewModel>>(notificationsPage.Items);
            Assert.Single(notificationsPage.Items);
            Assert.Equal(1, notificationsPage.TotalItemCount);
            Assert.Equal(6, notificationsPage.Items.First().Id);
        }

        /// <summary>
        /// The create_ valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Create_Valid()
        {
            this.mockNotificationRepo.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()))
                                        .ReturnsAsync(1001);

            int userId = 27;
            var notification = new NotificationViewModel
            {
                Id = 0,
                Title = "A test Notification",
                Message = "A very interesting test message!",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.CreateAsync(userId, notification);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
            Assert.Equal(1001, result.CreatedId);
        }

        /// <summary>
        /// The create_ no title.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Create_NoTitle()
        {
            this.mockNotificationRepo.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()))
                                        .ReturnsAsync(1001);

            int userId = 27;
            var notification = new NotificationViewModel()
            {
                Id = 0,
                Message = "A very interesting test message!",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.CreateAsync(userId, notification);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
            Assert.Equal("Title is mandatory.", result.Details[0]);
        }

        /// <summary>
        /// The create_ no message.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Create_NoMessage()
        {
            this.mockNotificationRepo.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()))
                                        .ReturnsAsync(1001);

            int userId = 27;
            var notification = new NotificationViewModel()
            {
                Id = 0,
                Title = "A test Notification",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.CreateAsync(userId, notification);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
            Assert.Equal("Message is mandatory.", result.Details[0]);
        }

        /// <summary>
        /// The Create_TitleTooLong.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task Create_TitleTooLong()
        {
            this.mockNotificationRepo.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()))
                                        .ReturnsAsync(1001);

            string title = string.Empty;
            for (int i = 0; i < 301; i++)
            {
                title += "a";
            }

            int userId = 27;
            var notification = new NotificationViewModel()
            {
                Id = 0,
                Title = title,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.CreateAsync(userId, notification);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
            Assert.Equal("Title cannot exceed 300 characters.", result.Details[0]);
        }

        /// <summary>
        /// The create_ valid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task Create_TitleOnLimit()
        {
            this.mockNotificationRepo.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()))
                                        .ReturnsAsync(1001);

            string title = string.Empty;
            for (int i = 0; i < 300; i++)
            {
                title += "a";
            }

            int userId = 27;
            var notification = new NotificationViewModel
            {
                Id = 0,
                Title = title,
                Message = "A very interesting test message!",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.CreateAsync(userId, notification);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
            Assert.Equal(1001, result.CreatedId);
        }

        /// <summary>
        /// The edit_ no title.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Edit_NoTitle()
        {
            int userId = 27;
            var notification = new NotificationViewModel
            {
                Id = 1001,
                Message = "A very interesting test message!",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.UpdateAsync(userId, notification);

            this.mockNotificationRepo.Verify(ur => ur.UpdateAsync(It.IsAny<int>(), It.IsAny<Notification>()), Times.Never);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
            Assert.Equal("Title is mandatory.", result.Details[0]);
        }

        /// <summary>
        /// The edit_ no message.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Edit_NoMessage()
        {
            int userId = 27;
            var notification = new NotificationViewModel
            {
                Id = 1001,
                Title = "A test Notification",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.UpdateAsync(userId, notification);

            this.mockNotificationRepo.Verify(ur => ur.UpdateAsync(It.IsAny<int>(), It.IsAny<Notification>()), Times.Never);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
            Assert.Equal("Message is mandatory.", result.Details[0]);
        }

        /// <summary>
        /// The edit_ valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Edit_Valid()
        {
            int userId = 27;
            var notification = new NotificationViewModel
            {
                Id = 1001,
                Title = "A test Notification",
                Message = "A very interesting test message!",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                UserDismissable = false,
            };
            var result = await this.notificationService.UpdateAsync(userId, notification);

            this.mockNotificationRepo.Verify(ur => ur.UpdateAsync(It.IsAny<int>(), It.IsAny<Notification>()), Times.Once);

            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// The CreatePermisssionNotification method for read only user creates priority notification.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreatePermisssionNotificationAsync_ForReadOnlyUser_CreatesPriorityNotification()
        {
            // Arrange
            this.SetupNotification();

            // Act
            await this.notificationService.CreatePermisssionNotificationAsync(new Fixture().Create<int>(), true);

            // Assert
            this.mockNotificationRepo.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()), Times.Once);

            Assert.True(this.notification.Title == "Resource Access Title");
            Assert.True(this.notification.Message == "Resource Readonly Access, Support Contact");
            Assert.True(this.notification.NotificationTypeEnum == Models.Enums.NotificationTypeEnum.UserPermission);
            Assert.True(this.notification.NotificationPriorityEnum == Models.Enums.NotificationPriorityEnum.Priority);
        }

        /// <summary>
        /// The CreatePermisssionNotification method for resource contribute user creates priority notification.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreatePermisssionNotificationAsync_ForResourceContributeUser_CreatesPriorityNotification()
        {
            // Arrange
            this.SetupNotification();

            // Act
            await this.notificationService.CreatePermisssionNotificationAsync(new Fixture().Create<int>(), false);

            // Assert
            this.mockNotificationRepo.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()), Times.Once);

            Assert.True(this.notification.Title == "Resource Access Title");
            Assert.True(this.notification.Message == "Resource Contribute Access, Support Contact");
            Assert.True(this.notification.NotificationTypeEnum == Models.Enums.NotificationTypeEnum.UserPermission);
            Assert.True(this.notification.NotificationPriorityEnum == Models.Enums.NotificationPriorityEnum.Priority);
        }

        /// <summary>
        /// The CreateResourcePublishedNotification method creates general notification.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateResourcePublishedNotificationAsync_WhenCalled_CreatesGeneralNotification()
        {
            // Arrange
            var fixture = new Fixture();
            var resourceTitle = fixture.Create<string>();
            var resourceReferenceId = fixture.Create<int>();
            this.SetupNotification();

            // Act
            await this.notificationService.CreateResourcePublishedNotificationAsync(
                            fixture.Create<int>(), resourceTitle, resourceReferenceId);

            // Assert
            this.mockNotificationRepo.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()), Times.Once);

            Assert.True(this.notification.Title == "Resource Published Title" + resourceTitle);
            Assert.True(this.notification.Message == $"Resource Published, {resourceReferenceId}");
            Assert.True(this.notification.NotificationTypeEnum == Models.Enums.NotificationTypeEnum.ResourcePublished);
            Assert.True(this.notification.NotificationPriorityEnum == Models.Enums.NotificationPriorityEnum.General);
        }

        /// <summary>
        /// The CreatePublishFailedNotification method creates priority notification.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreatePublishFailedNotificationAsync_WhenCalled_CreatesPriorityNotification()
        {
            // Arrange
            var fixture = new Fixture();
            var resourceTitle = fixture.Create<string>();
            var resourceVersionId = fixture.Create<int>();
            this.SetupNotification();

            // Act
            await this.notificationService.CreatePublishFailedNotificationAsync(fixture.Create<int>(), resourceTitle);

            // Assert
            this.mockNotificationRepo.Verify(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()), Times.Once);

            Assert.True(this.notification.Title == "Resource Publish Failed Title" + resourceTitle);
            Assert.True(this.notification.Message == $"Resource Publish Failed, {resourceTitle}, Support Contact");
            Assert.True(this.notification.NotificationTypeEnum == Models.Enums.NotificationTypeEnum.PublishFailed);
            Assert.True(this.notification.NotificationPriorityEnum == Models.Enums.NotificationPriorityEnum.Priority);
        }

        /// <summary>
        /// The get notification.
        /// </summary>
        /// <param name="logId">The log id.</param>
        /// <returns>The <see cref="Notification"/>.</returns>
        private Notification GetNotification(int logId)
        {
            return this.TestNotifications().FirstOrDefault(l => l.Id == logId);
        }

        /// <summary>
        /// The test notifications async mock.
        /// </summary>
        /// <returns>The <see cref="Mock"/>.</returns>
        private Mock<DbSet<Notification>> TestNotificationsAsyncMock()
        {
            var logRecords = this.TestNotifications().AsQueryable();

            var mockLogDbSet = new Mock<DbSet<Notification>>();

            mockLogDbSet.As<IAsyncEnumerable<Notification>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(new TestAsyncEnumerator<Notification>(logRecords.GetEnumerator()));

            mockLogDbSet.As<IQueryable<Notification>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Notification>(logRecords.Provider));

            mockLogDbSet.As<IQueryable<Notification>>().Setup(m => m.Expression).Returns(logRecords.Expression);
            mockLogDbSet.As<IQueryable<Notification>>().Setup(m => m.ElementType).Returns(logRecords.ElementType);
            mockLogDbSet.As<IQueryable<Notification>>().Setup(m => m.GetEnumerator()).Returns(() => logRecords.GetEnumerator());

            return mockLogDbSet;
        }

        /// <summary>
        /// The test notifications.
        /// </summary>
        /// <returns>The Notification list.</returns>
        private List<Notification> TestNotifications()
        {
            return new List<Notification>()
            {
                new Notification()
                {
                    Id = 3,
                    Title = "Removal of redundant Workforce Development Programmes",
                    Message = "Please be aware that the following programmes will be removed from the e-Learning for Healthcare (e-LfH) Hub on 31 March 2018: \r\nFire Safety\r\nHealth &amp; Safety\r\nInfection Control\r\nManual Handling.\r\nThese programmes have been replaced by updated courses held within the new Statutory and Mandatory programme available \r\nhere. The courses have been re-developed in a new format allowing for improved viewing using a mobile phone, tablet as well as laptop or desktop devices. The new programme can be enrolled on by individual users via the 'My Account', 'Enrolment' area once logged in.\r\nPlease be aware that any learning activity that you have undertaken on the e-LfH Hub will be retained and you can access this information via the ‘My Activity’, ‘Reports’ section of the e-LfH Hub. However, if you require a certificate, please access and download before 31March 2018.\r\nIf you have any questions or queries relating to the removal of these programmes, please contact the support desk via this link.\r\nThe e-LfH Support Team",
                    StartDate = DateTime.Parse("2010-01-01T00:00:00Z"),
                    EndDate = DateTime.Parse("2020-12-31T00:00:00Z"),
                    UserDismissable = false,
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 5,
                    Title = "e-FM knowledge sessions and assessments restructure",
                    Message = "Further to our recent notification, on the upcoming restructure of our 'knowledge sessions', eFM would like to announce that this restructure will take place on Monday 26th March.\r\nFrom this date onwards, the current set up of 5 modules and 1 assessment module, will be replaced by 4 Learning Paths, based on levels of complexity.\r\nLevel 1 - Pathophysiology; Normal Fetal Heart Rate Pattern\r\nLevel 2 - Risk factors for Fetal Hypoxia\r\nLevel 3 - Pathophysiology; Complex Fetal Heart Rate Pattern\r\nLevel 4 - Complex Pregnancies\r\nEach of the above Learning Paths will have a dedicated assessment session built into it. Successful completion of the assessment (80% pass mark), will be required to unlock the following level's assessment session. All knowledge sessions will still be available to all users, at any time.\r\nThe Learning Paths will be structured so that users can gain certification in 2 ways. 1) By completing all the 'knowledge sessions' in each Learning Path, and 2) by successfully passing the assessment. This will result in 2 certificates being available per Learning Path, offering the user different pathways when attempting the learning.\r\nThere will be no changes to the Case Studies, and how they are accessed.\r\nIf there are any queries relating to this restructure then please do get in touch via richard.bryant@eintegrity.org",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T17:27:53.2131149+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 6,
                    Title = "Introducing Learning Paths on eFM",
                    Message = "eFM is pleased to announce that the new learning path structure is live and available to use. As of today, Monday 26th March 2018, the knowledge sessions and assessments will now appear as a series of learning paths based on levels of complexity. The four learning paths are as follows:\r\nLevel 1 - Pathophysiology: Normal Fetal Heart Rate Pattern\r\nLevel 2 - Risk factors for Fetal Hypoxia\r\nLevel 3 - Pathophysiology: Complex Fetal Heart Rate Pattern\r\nLevel 4 - Complex Pregnancies\r\nAll previous knowledge sessions are still available and all user records are still accessible, the sessions are now presented in an way that, we believe, is easier to navigate and interact with. All knowledge sessions, in all learning paths can be accessed as before, however, all users will need to work through the assessments in order and pass at 80% or higher, for the proceeding levels’ assessment to be unlocked.\r\nEach learning path will consist of one course for knowledge sessions and one course for assessment. This presents users with the opportunity to generate two certificates for each learning path.\r\nCertification is now available upon course completion, or passing the assessment sessions, and can be generated via the certificate icon at learning path level. The previous method of generating certificates, via My Activity, will no longer be available.\r\nIf you experience any difficulties accessing, or navigating the content, please contact richard.bryant@eintegrity.org or email support@e-lfh.org.uk",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003867,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-07T18:24:13.2391991+01:00"),
                    AmendUser = new User()
                    {
                        Id = 1003867,
                        UserName = "test.user",
                    },
                    CreateUser = new User()
                    {
                        Id = 1003867,
                        UserName = "test1.user",
                    },
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 7,
                    Title = "NHS England (NHSE) - User Research",
                    Message = "We are conducting user research to help inform the development of NHS England’s (NHSE) Learning Solution and e-Learning for Healthcare Hub to ensure they meet the needs of users. This user research will focus on key areas that will underpin future development.Could you help us? We promise it won’t take up too much of your time. The research will use methods including interviews, surveys, focus groups etc. and you can choose which of these you wish to take part in. If you can help, please click on the link below and provide your details and we’ll be in touch with you soon:\r\nhttps://healtheducationyh.onlinesurveys.ac.uk/user-research\r\nMany thanks,\r\nThe Learning Solution Project Team",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-07T18:25:18.5786314+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 8,
                    Title = "e-Den enhanced CPD forms Test123",
                    Message = "In January 2018, the General Dental Council\r\n(GDC) introduced the Enhanced CPD Scheme for dentists, and dental care professionals will join the scheme in August 2018.\r\ne-Den sessions can be counted as verifiable CPD for the Enhanced CPD scheme.\r\n\r\nOnce you have completed an e-Den module, you\r\nmust first generate a course completion certificate from the e-LfH Hub (help article).\r\nThen complete the relevant e-Den module Enhanced CPD form, making reference to\r\nthe course completion certificate. Both the relevant e-Den module Enhanced CPD\r\nform and the accompanying e-LfH Hub course completion certificate should be\r\nkept as evidence in your records to demonstrate you have completed verifiable\r\nCPD, should the GDC request it. \r\n\r\nYou can download the relevant module Enhanced\r\nCPD forms from the e-learning resources section at the bottom of the e-Den programme page",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-07T18:26:17.7186472+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 9,
                    Title = "e-LfH and General Data Protection Regulations (GDPR)",
                    Message = "You may have heard about the new General Data Protection Regulation that comes into effect on 25 May 2018. To align with these regulations we have updated our terms and conditions which references NHS England's (NHSE) Privacy Notice. Why we collect your data We collect your data to enable us to create an account for you allowing you access to our e-learning programmes and other related systems. Your account profile data, including your place of work, is necessary for reporting purposes. How we collect your data We do this at the time of registration for your account and whenever you update your account profile. We may share your data We may share data with your organisation via our Administrator Place of Work Reporting e.g. a Trust, Foundation School or company depending on your role. Note that permissions for administrators at your organisation to run activity reports are only granted once they have been verified. Sharing data to other external systems is only done once we receive explicit consent from you. For example, if you are linking your learning activity to an ePortfolio, then we will ask your permission. We will never sell your data or pass it on for commercial gain in any way. Know your rights Under the new General Data Protection Regulations (GDPR), you have many rights regarding your personal data, including seeing what data we hold for you, your right for erasure and also withdrawing consent of data processing. Note that withdrawing consent will result in your account being anonymised and access to the e-LfH Hub removed. Click here to view our updated Terms and Conditions</a> which includes a link to NHS England’s (NHSE) Privacy Notice If you have any queries relating to GDPR including request for erasure and withdrawing consent, please contact us at enquiries@e-lfh.org.uk",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-07T18:27:56.6667656+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 10,
                    Title = "NW – Apprentices",
                    Message = "The NW - Apprentices Programme will be removed from the Hub on 1st March 2018 and you should therefore complete the programme before this date if you wish to obtain a certificate. For future learning the National Statutory and Mandatory content is available on the Hub on the following link: LinkX\r\n",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:32:02.2115246+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 11,
                    Title = "NW - Healthcare Students Programme",
                    Message = "This is a reminder that on the 1st March 2018, the NW – Healthcare Students courses Yr1, Yr2 and Yr3 are due to be removed and the content\r\nreplaced. If you have completed any of the three courses within the Programme but not downloaded the certificate, you\r\nshould do this before 28th February 2018, after this date the e-Learning for Healthcare system will maintain a record of your activity undertaken on the current courses under ‘My Activity’ but a certificate will not be available.\r\n",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:33:12.7089247+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 12,
                    Title = "e-FM knowledge sessions and assessments restructure",
                    Message = "Further to our recent notification, on the upcoming restructure of our 'knowledge sessions', eFM would like to announce that this restructure will take place on Monday 26th March. From this date onwards, the current set up of 5 modules and 1 assessment module, will be replaced by 4 Learning Paths, based on levels of complexity.\r\nLevel 1 - Pathophysiology; Normal Fetal Heart Rate Pattern\r\nLevel 2 - Risk factors for Fetal Hypoxia\r\nLevel 3 - Pathophysiology; Complex Fetal Heart Rate Pattern\r\nLevel 4 - Complex Pregnancies<br>&nbsp;\r\nEach of the above Learning Paths will have a dedicated assessment session built into it. Successful completion of the assessment (80% pass mark), will be required to unlock the following level's assessment session. All knowledge sessions will still be available to all users, at any time.\r\nThe Learning Paths will be structured so that users can gain certification in 2 ways. 1) By completing all the 'knowledge sessions' in each Learning Path, and 2) by successfully passing the assessment. This will result in 2 certificates being available per Learning Path, offering the user different pathways when attempting the learning.\r\nThere will be no changes to the Case Studies, and how they are accessed.\r\nIf there are any queries relating to this restructure then please do get in touch via richard.bryant@eintegrity.org",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:34:37.0242968+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 13,
                    Title = "NW - Healthcare Partners",
                    Message = "This is a reminder that on the 1st March 2018, the NW – Healthcare Partners Programme will be removed and you should therefore complete the relevant course before this date if you wish to obtain a certificate. For future learning the National Statutory and Mandatory content is available on the Hub on the following link: LinkA\r\n",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:36:04.4066992+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 14,
                    Title = "NW - Social Care Students Programme",
                    Message = "This is a reminder that on the 1st March 2018, the NW – Social Care Students courses Yr1, Yr2 and Yr3 are due to be removed and the content replaced. If you have completed any of the three courses within the Programme but not downloaded the certificate, you should do this before 28th February 2018, after this date the e-Learning for Healthcare system will maintain a record of your activity undertaken on the current\r\ncourses under ‘My Activity’ but a certificate will not be available.",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:37:06.6400011+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 15,
                    Title = "e-FM knowledge sessions and assessments restructure",
                    Message = "Further to our recent notification, on the upcoming restructure of our 'knowledge sessions', eFM would like to announce that this restructure will take place on Monday 26th March. From this date onwards, the current set up of 5 modules and 1 assessment module, will be replaced by 4 Learning Paths, based on levels of complexity.\r\nLevel 1 - Pathophysiology; Normal Fetal Heart Rate Pattern\r\nLevel 2 - Risk factors for Fetal Hypoxia\r\nLevel 3 - Pathophysiology; Complex Fetal Heart Rate Pattern\r\nLevel 4 - Complex Pregnancies\r\nEach of the above Learning Paths will have a dedicated assessment session built into it. Successful completion of the assessment (80% pass mark), will be required to unlock the following level's assessment session. All knowledge sessions will still be available to all users, at any time.\r\nThe Learning Paths will be structured so that users can gain certification in 2 ways. 1) By completing all the 'knowledge sessions' in each Learning Path, and 2) by successfully passing the assessment. This will result in 2 certificates being available per Learning Path, offering the user different pathways when attempting the learning.\r\nThere will be no changes to the Case Studies, and how they are accessed.\r\nIf there are any queries relating to this restructure then please do get in touch via richard.bryant@eintegrity.org",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:38:20.3135332+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 16,
                    Title = "System Downtime 8:30am Friday 2nd March 2018",
                    Message = "Please note that the e-LfH Hub will be unavailable from 8:30am on Friday 2nd March 2018 for a period of 30 minutes. This is due to a planned system update.\r\nApologies for any inconvenience this short period of downtime may cause.\r\nBest regards,\r\ne-LfH support team</span>",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:39:17.5591736+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 17,
                    Title = "Updates in Anaesthesia, Critical Care and Pain Management",
                    Message = "Our Updates events are three-day meetings consisting of lectures and topical discussion. The meeting is intended for doctors engaged in clinical anaesthesia, pain management and intensive care medicine (i.e. consultants, trainees, staff and associate specialist grades or their overseas equivalent) who would benefit from a refresher of the latest updates in areas of practice they may be exposed to regularly or only occasionally. Sessions include:\r\nPerioperative blood management\r\n",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:40:15.5709996+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 18,
                    Title = "Completing Sessions on mobile and tablet devices",
                    Message = "Following feedback into the helpdesk, we are aware of a number of learners having difficulty obtaining completed statuses for their e-learning sessions.\r\nPlease note that if you are using a mobile or tablet device to carry out your e-learning, you will need to close the window or tab of the e-learning session within the browser for the completion status to be passed back to your activity record. If you leave the session open, you may lose that activity for that attempt resulting in you having to re-take the session. If you experience further difficulty, as an alternative, please try a desktop or laptop device using a recognised internet browser such as Chrome, FireFox or Windows Edge. \r\nTo view our technical checker and further support information, please view our support website here\r\n\r\nBest regards,\r\nThe e-LfH Support Team",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:41:08.0203536+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 19,
                    Title = "NHS Cervical Sampler - Course Restructure",
                    Message = "On the 8th January 2018, the NHS Cervical Sampler course is due to be removed and replaced. The content will remain as it is currently but is being restructured into 12 sessions to enable a better learning experience for the user. If you require a certificate and have completed the course but not downloaded the certificate, you should do this before 8th January 2018. After 8th January 2018 the e-Learning for Healthcare system will maintain a record of your activity completed on the current course under ‘My Activity’, 'Reports' and this can be accessed and details printed off as required. PLEASE NOTE: If you have not fully completed the current NHS Cervical Sampler course by 08/01/2018 when the course is replaced, you will not be able to complete this and obtain a certificate for it. You would need to undertake your learning in the replacement course and complete this if you required a certificate to be available.\r\nPHE Screening Project Team",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:42:00.7447835+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 20,
                    Title = "The ‘West Midlands - Making Every Contact Count’ resources have moved.",
                    Message = "These resources have now been combined into a single programme for Making Every Contact Count nationally, found here: http://portal.e-lfh.org.uk/Component/Details/432821. You have been automatically enrolled on this new programme. Please access the ‘West Midlands - Making Every Contact Count’ resources from this new location. Note that your learning history will not be affected by this change in location. Thank you for your understanding.",
                    StartDate = DateTime.Parse("2018-02-03T00:00:00Z"),
                    EndDate = DateTime.Parse("2021-12-20T00:00:00Z"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-07-30T00:00:00+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T10:42:41.0615264+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
                },
                new Notification()
                {
                    Id = 27,
                    Title = "Test Notification",
                    Message = "This is a test notification\r\n123\r\nABC",
                    StartDate = DateTime.Parse("2019-08-08T00:00:00+01:00"),
                    EndDate = DateTime.Parse("2019-08-15T00:00:00+01:00"),
                    UserDismissable = false,
                    Deleted = false,
                    CreateUserId = 1003866,
                    CreateDate = DateTime.Parse("2019-08-08T18:10:03.5869174+01:00"),
                    AmendUserId = 1003866,
                    AmendDate = DateTime.Parse("2019-08-08T18:10:03.5869174+01:00"),
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
                    NotificationTypeEnum = NotificationTypeEnum.SystemUpdate,
                    NotificationPriorityEnum = NotificationPriorityEnum.General,
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

        private void SetupSettings()
        {
            this.mockSettings.Setup(r => r.Value)
                .Returns(new Settings
                {
                    SupportContact = "Support Contact",
                    Notifications = new NotificationSettings
                    {
                        ResourcePublishedTitle = "Resource Published Title",
                        ResourcePublished = "Resource Published, [ResourceReferenceId]",
                        ResourcePublishFailedTitle = "Resource Publish Failed Title",
                        ResourcePublishFailed = "Resource Publish Failed, [ResourceTitle], [SupportContact]",
                        ResourceAccessTitle = "Resource Access Title",
                        ResourceReadonlyAccess = "Resource Readonly Access, [SupportContact]",
                        ResourceContributeAccess = "Resource Contribute Access, [SupportContact]",
                    },
                });
        }

        private void SetupNotification()
        {
            this.mockNotificationRepo.Setup(m => m.CreateAsync(It.IsAny<int>(), It.IsAny<Notification>()))
                .Callback<int, Notification>((i, obj) => this.notification = obj)
                .ReturnsAsync(new Fixture().Create<int>());
        }
    }
}