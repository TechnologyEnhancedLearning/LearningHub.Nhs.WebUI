namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using EntityFrameworkCore.Testing.Common;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Log;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.UnitTests.Helpers;
    using Microsoft.Azure.Management.Media.Models;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// The log service tests.
    /// </summary>
    public class LogServiceTests
    {
        /// <summary>
        /// The get by id async_ valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_Valid()
        {
            int logId = 18389;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .Returns(Task.FromResult(this.GetLog(logId)));

            var logService = new LogService(logRepositoryMock.Object, this.NewMapper());

            var log = await logService.GetByIdAsync(logId);

            Assert.IsType<LogViewModel>(log);
            Assert.Equal(18389, log.Id);
            Assert.Equal("Error", log.Level);
            Assert.Contains("Data is Null.", log.Exception);
            Assert.Equal(1, log.UserId);
        }

        /// <summary>
        /// The get by id async_ invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_Invalid()
        {
            int logId = 999;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                        .Returns(Task.FromResult<Log>(null));

            var logService = new LogService(logRepositoryMock.Object, this.NewMapper());

            var log = await logService.GetByIdAsync(logId);

            Assert.Null(log);
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
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize);

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal(18377, logsPage.Items.First().Id);
            Assert.Equal(18381, logsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ id_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Id_Asc()
        {
            int page = 3;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                            logRepositoryMock,
                            page,
                            pageSize,
                            string.Empty,
                            "Id",
                            "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal(18377, logsPage.Items.First().Id);
            Assert.Equal(18381, logsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ id_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Id_Desc()
        {
            int page = 3;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Id",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal(18384, logsPage.Items.First().Id);
            Assert.Equal(18380, logsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ sorted_ application_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Application_Asc()
        {
            int page = 5;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Application",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Application", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal("LearningHub.Nhs.Api", logsPage.Items.First().Application);
            Assert.Equal("LearningHub.Nhs.Api", logsPage.Items.Last().Application);
        }

        /// <summary>
        /// The get page async_ sorted_ application_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Application_Desc()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Application",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Application", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal("LearningHub.Nhs.Auth", logsPage.Items.First().Application);
            Assert.Equal("LearningHub.Nhs.Api", logsPage.Items.Last().Application);
        }

        /// <summary>
        /// The get page async_ sorted_ logged_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Logged_Asc()
        {
            int page = 4;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Logged",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Logged", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal(DateTime.Parse("2019-07-25 14:50:14.613"), logsPage.Items.First().Logged);
            Assert.Equal(DateTime.Parse("2019-07-25 14:50:53.503"), logsPage.Items.Last().Logged);
        }

        /// <summary>
        /// The get page async_ sorted_ logged_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Logged_Desc()
        {
            int page = 2;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Logged",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Logged", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal(DateTime.Parse("2019-07-25 14:51:05.737"), logsPage.Items.First().Logged);
            Assert.Equal(DateTime.Parse("2019-07-25 14:50:53.493"), logsPage.Items.Last().Logged);
        }

        /// <summary>
        /// The get page async_ sorted_ level_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Level_Asc()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Level",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Level", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal("Error", logsPage.Items.First().Level);
            Assert.Equal("Info", logsPage.Items.Last().Level);
        }

        /// <summary>
        /// The get page async_ sorted_ level_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Level_Desc()
        {
            int page = 5;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Level",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Level", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal("Info", logsPage.Items.First().Level);
            Assert.Equal("Info", logsPage.Items.Last().Level);
        }

        /// <summary>
        /// The get page async_ sorted_ message_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Message_Asc()
        {
            int page = 3;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Message",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Message", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal("url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing action method LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api) - Validation state: Valid", logsPage.Items.First().Message);
            Assert.Equal("url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing ObjectResult, writing value of type 'LearningHub.Nhs.Api.Shared.LoginResult'.", logsPage.Items.Last().Message);
        }

        /// <summary>
        /// The get page async_ sorted_ message_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Message_Desc()
        {
            int page = 2;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Message",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Message", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.StartsWith("url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Executed endpoint 'LearningHub.Nhs.Api.Controllers.ElfhUserController.GetUserIdByUsername (LearningHub.Nhs.Api)'", logsPage.Items.First().Message);
            Assert.Equal("url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Route matched with {action = \"AuthenticateElfhHubAsync\", controller = \"Authentication\"}. Executing controller action with signature System.Threading.Tasks.Task`1[Microsoft.AspNetCore.Mvc.IActionResult] AuthenticateElfhHubAsync(elfhHub.Nhs.Model.Login) on controller LearningHub.Nhs.Api.Controllers.AuthenticationController (LearningHub.Nhs.Api).", logsPage.Items.Last().Message);
        }

        /// <summary>
        /// The get page async_ sorted_ logger_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Logger_Asc()
        {
            int page = 2;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Logger",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Logger", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ sorted_ logger_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Logger_Desc()
        {
            int page = 5;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Logger",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Logger", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ sorted_ callsite_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Callsite_Asc()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Callsite",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Callsite", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ sorted_ callsite_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Callsite_Desc()
        {
            int page = 2;
            int pageSize = 15;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Callsite",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Callsite", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(13, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ sorted_ exception_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Exception_Asc()
        {
            int page = 5;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Exception",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Exception", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ sorted_ exception_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_Exception_Desc()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "Exception",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Exception", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ sorted_ user name_ asc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_UserName_Asc()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "UserName",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "UserName", "A", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Null(logsPage.Items.First().UserName);
            Assert.Equal("test.user1", logsPage.Items.Last().UserName);
        }

        /// <summary>
        /// The get page async_ sorted_ user name_ desc.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Sorted_UserName_Desc()
        {
            int page = 2;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                string.Empty,
                "UserName",
                "D");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "UserName", "D", string.Empty);

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(28, logsPage.TotalItemCount);
            Assert.Equal("test.user3", logsPage.Items.First().UserName);
            Assert.Equal("test.user2", logsPage.Items.Last().UserName);
        }

        /// <summary>
        /// The get page async_ filter_ id.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_Id()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                "[{\"Column\":\"Id\",\"Value\":\"18389\"}]",
                "Id",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"Id\",\"Value\":\"18389\"}]");

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Single(logsPage.Items);
            Assert.Equal(1, logsPage.TotalItemCount);
            Assert.Equal(18389, logsPage.Items.First().Id);
        }

        /// <summary>
        /// The get page async_ filter_ id_ not found.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_Id_NotFound()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                "[{\"Column\":\"Id\",\"Value\":\"999\"}]",
                "Id",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"Id\",\"Value\":\"999\"}]");

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Empty(logsPage.Items);
            Assert.Equal(0, logsPage.TotalItemCount);
        }

        /// <summary>
        /// The get page async_ filter_ level_ message.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_Level_Message()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                "[{\"Column\":\"Level\",\"Value\":\"Error\"},{\"Column\":\"Message\",\"Value\":\"object\"}]",
                "Id",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"Level\",\"Value\":\"Error\"},{\"Column\":\"Message\",\"Value\":\"object\"}]");

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Single(logsPage.Items);
            Assert.Equal(1, logsPage.TotalItemCount);
            Assert.Equal(18391, logsPage.Items.First().Id);
        }

        /// <summary>
        /// The get page async_ filter_ application_ level_ call site.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_Application_Level_CallSite()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                "[{\"Column\":\"Level\",\"Value\":\"Info\"},{\"Column\":\"Application\",\"Value\":\".Api\"},{\"Column\":\"CallSite\",\"Value\":\"Microsoft.AspNetCore.Mvc\"}]",
                "Id",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"Level\",\"Value\":\"Info\"},{\"Column\":\"Application\",\"Value\":\".Api\"},{\"Column\":\"CallSite\",\"Value\":\"Microsoft.AspNetCore.Mvc\"}]");

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Equal(5, logsPage.Items.Count);
            Assert.Equal(14, logsPage.TotalItemCount);
            Assert.Equal(18366, logsPage.Items.First().Id);
            Assert.Equal(18370, logsPage.Items.Last().Id);
        }

        /// <summary>
        /// The get page async_ filter_ logged_ exception.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_Logged_Exception()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                "[{\"Column\":\"Logged\",\"Value\":\"25 Jul 2019\"},{\"Column\":\"Exception\",\"Value\":\"Could not load\"}]",
                "Id",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"Logged\",\"Value\":\"25 Jul 2019\"},{\"Column\":\"Exception\",\"Value\":\"Could not load\"}]");

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Single(logsPage.Items);
            Assert.Equal(1, logsPage.TotalItemCount);
            Assert.Equal(18392, logsPage.Items.First().Id);
        }

        /// <summary>
        /// The get page async_ filter_ application_ user name.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPageAsync_Filter_Application_UserName()
        {
            int page = 1;
            int pageSize = 5;
            var logRepositoryMock = new Mock<ILogRepository>(MockBehavior.Strict);

            logRepositoryMock.Setup(r => r.GetAll()).Returns(this.TestLogsAsyncMock().Object);
            var mapperMock = this.GetMapperMock(
                logRepositoryMock,
                page,
                pageSize,
                "[{\"Column\":\"Application\",\"Value\":\".Auth\"},{\"Column\":\"UserName\",\"Value\":\"test.user3\"}]",
                "Id",
                "A");

            var logService = new LogService(logRepositoryMock.Object, mapperMock.Object);

            var logsPage = await logService.GetPageAsync(page, pageSize, "Id", "A", "[{\"Column\":\"Application\",\"Value\":\".Auth\"},{\"Column\":\"UserName\",\"Value\":\"test.user3\"}]");

            Assert.IsType<List<LogBasicViewModel>>(logsPage.Items);
            Assert.Single(logsPage.Items);
            Assert.Equal(1, logsPage.TotalItemCount);
            Assert.Equal(18391, logsPage.Items.First().Id);
        }

        /// <summary>
        /// The get log.
        /// </summary>
        /// <param name="logId">The log id.</param>
        /// <returns>The <see cref="Log"/>.</returns>
        private Log GetLog(int logId)
        {
            return this.TestLogs().FirstOrDefault(l => l.Id == logId);
        }

        /// <summary>
        /// The test logs async mock.
        /// </summary>
        /// <returns>The <see cref="Mock"/>.</returns>
        private Mock<DbSet<Log>> TestLogsAsyncMock()
        {
            var logRecords = this.TestLogs().AsQueryable();

            var mockLogDbSet = new Mock<DbSet<Log>>();

            mockLogDbSet.As<IAsyncEnumerable<Log>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(new TestAsyncEnumerator<Log>(logRecords.GetEnumerator()));

            mockLogDbSet.As<IQueryable<Log>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Log>(logRecords.Provider));

            mockLogDbSet.As<IQueryable<Log>>().Setup(m => m.Expression).Returns(logRecords.Expression);
            mockLogDbSet.As<IQueryable<Log>>().Setup(m => m.ElementType).Returns(logRecords.ElementType);
            mockLogDbSet.As<IQueryable<Log>>().Setup(m => m.GetEnumerator()).Returns(() => logRecords.GetEnumerator());

            return mockLogDbSet;
        }

        /// <summary>
        /// The test logs.
        /// </summary>
        /// <returns>The Log list.</returns>
        private List<Log> TestLogs()
        {
            return new List<Log>()
            {
                new Log()
                {
                    Id = 18366,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:49:22.513"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing action method LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api) - Validation state: Valid",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ActionMethodExecuting",
                    Exception = string.Empty,
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18367,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.400"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executed action method LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api), returned result Microsoft.AspNetCore.Mvc.OkObjectResult in 51873.9656ms.",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ActionMethodExecuted",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18368,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.437"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing ObjectResult, writing value of type 'LearningHub.Nhs.Api.Shared.LoginResult'.",
                    Logger = "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ObjectResultExecuting",
                    Exception = string.Empty,
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 18369,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.437"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing ObjectResult, writing value of type 'LearningHub.Nhs.Api.Shared.LoginResult'.",
                    Logger = "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ObjectResultExecuting",
                    Exception = string.Empty,
                    UserName = null,
                    UserId = 0,
                },
                new Log()
                {
                    Id = 18370,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.470"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executed action LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api) in 52187.9833ms",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ExecutedAction",
                    Exception = string.Empty,
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18371,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.470"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executed endpoint 'LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api)'",
                    Logger = "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                    Callsite = "Microsoft.AspNetCore.Routing.EndpointMiddleware+Log.ExecutedEndpoint",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18372,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.493"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Request finished in 52322.8612ms 200 application/json; charset=utf-8",
                    Logger = "Microsoft.AspNetCore.Hosting.Internal.WebHost",
                    Callsite = "Microsoft.AspNetCore.Hosting.Internal.HostingApplicationDiagnostics.LogRequestFinished",
                    Exception = string.Empty,
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 19373,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.437"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing ObjectResult, writing value of type 'LearningHub.Nhs.Api.Shared.LoginResult'.",
                    Logger = "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ObjectResultExecuting",
                    Exception = string.Empty,
                    UserName = null,
                    UserId = 0,
                },
                new Log()
                {
                    Id = 18374,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.517"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action:  | Request starting HTTP/1.1 GET https://localhost:44384/api/ElfhUser/GetUserIdByUsername/dave.brown  ",
                    Logger = "Microsoft.AspNetCore.Hosting.Internal.WebHost",
                    Callsite = "Microsoft.AspNetCore.Hosting.Internal.HostingApplicationDiagnostics.LogRequestStarting",
                    Exception = string.Empty,
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18375,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.517"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Executing endpoint 'LearningHub.Nhs.Api.Controllers.ElfhUserController.GetUserIdByUsername (LearningHub.Nhs.Api)'",
                    Logger = "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                    Callsite = "Microsoft.AspNetCore.Routing.EndpointMiddleware+Log.ExecutingEndpoint",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18376,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.533"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Route matched with {action = \"GetUserIdByUsername\", controller = \"ElfhUser\"}. Executing controller action with signature System.Threading.Tasks.Task`1[Microsoft.AspNetCore.Mvc.IActionResult] GetUserIdByUsername(System.String) on controller LearningHub.Nhs.Api.Controllers.ElfhUserController (LearningHub.Nhs.Api).",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ExecutingAction",
                    Exception = string.Empty,
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 18377,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.550"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Executing action method LearningHub.Nhs.Api.Controllers.ElfhUserController.GetUserIdByUsername (LearningHub.Nhs.Api) - Validation state: Valid",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ActionMethodExecuting",
                    Exception = string.Empty,
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18378,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.570"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Executed action method LearningHub.Nhs.Api.Controllers.ElfhUserController.GetUserIdByUsername (LearningHub.Nhs.Api), returned result Microsoft.AspNetCore.Mvc.OkObjectResult in 15.1122ms.",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ActionMethodExecuted",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18379,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.613"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Executing ObjectResult, writing value of type 'System.Int32'.",
                    Logger = "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ObjectResultExecuting",
                    Exception = string.Empty,
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 18380,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.613"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Executed action LearningHub.Nhs.Api.Controllers.ElfhUserController.GetUserIdByUsername (LearningHub.Nhs.Api) in 79.9654ms",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ExecutedAction",
                    Exception = string.Empty,
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18381,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.613"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Executed endpoint 'LearningHub.Nhs.Api.Controllers.ElfhUserController.GetUserIdByUsername (LearningHub.Nhs.Api)'",
                    Logger = "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                    Callsite = "Microsoft.AspNetCore.Routing.EndpointMiddleware+Log.ExecutedEndpoint",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18382,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:14.627"),
                    Level = "Info",
                    Message = "url: https://localhost/api/ElfhUser/GetUserIdByUsername/dave.brown | action: GetUserIdByUsername | Request finished in 110.0866ms 200 application/json; charset=utf-8",
                    Logger = "Microsoft.AspNetCore.Hosting.Internal.WebHost",
                    Callsite = "Microsoft.AspNetCore.Hosting.Internal.HostingApplicationDiagnostics.LogRequestFinished",
                    Exception = string.Empty,
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 18383,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:53.493"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action:  | Request starting HTTP/1.1 POST https://localhost:44384/api/Authentication/Authenticate application/json 51",
                    Logger = "Microsoft.AspNetCore.Hosting.Internal.WebHost",
                    Callsite = "Microsoft.AspNetCore.Hosting.Internal.HostingApplicationDiagnostics.LogRequestStarting",
                    Exception = string.Empty,
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18384,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:53.493"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing endpoint 'LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api)'",
                    Logger = "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                    Callsite = "Microsoft.AspNetCore.Routing.EndpointMiddleware+Log.ExecutingEndpoint",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18385,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:53.503"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Route matched with {action = \"AuthenticateElfhHubAsync\", controller = \"Authentication\"}. Executing controller action with signature System.Threading.Tasks.Task`1[Microsoft.AspNetCore.Mvc.IActionResult] AuthenticateElfhHubAsync(elfhHub.Nhs.Model.Login) on controller LearningHub.Nhs.Api.Controllers.AuthenticationController (LearningHub.Nhs.Api).",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ExecutingAction",
                    Exception = string.Empty,
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 18386,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:50:53.503"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executing action method LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api) - Validation state: Valid",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ActionMethodExecuting",
                    Exception = string.Empty,
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18387,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:51:05.633"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executed action LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api) in 12124.4532ms",
                    Logger = "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
                    Callsite = "Microsoft.AspNetCore.Mvc.Internal.MvcCoreLoggerExtensions.ExecutedAction",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18388,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:51:05.737"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Executed endpoint 'LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync (LearningHub.Nhs.Api)'",
                    Logger = "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                    Callsite = "Microsoft.AspNetCore.Routing.EndpointMiddleware+Log.ExecutedEndpoint",
                    Exception = string.Empty,
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 18389,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:51:05.970"),
                    Level = "Error",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Data is Null. This method or property cannot be called on Null values.",
                    Logger = "LearningHub.Nhs.Api.Middleware.ExceptionMiddleware",
                    Callsite = "LearningHub.Nhs.Api.Middleware.ExceptionMiddleware+<HandleExceptionAsync>d__5.MoveNext(C:\\e-lfh\\GIT\\LearningHub.Nhs.WebAPI\\LearningHub.Nhs.API\\Middleware\\ExceptionMiddleware.cs:44)",
                    Exception = "System.Data.SqlTypes.SqlNullValueException: Data is Null. This method or property cannot be called on Null values.\r\n   at System.Data.SqlClient.SqlBuffer.get_String()\r\n   at System.Data.SqlClient.SqlDataReader.GetString(Int32 i)\r\n   at lambda_method(Closure , DbDataReader )\r\n   at Microsoft.EntityFrameworkCore.Storage.Internal.TypedRelationalValueBufferFactory.Create(DbDataReader dataReader)\r\n   at Microsoft.EntityFrameworkCore.Query.Internal.AsyncQueryingEnumerable`1.AsyncEnumerator.BufferlessMoveNext(DbContext _, Boolean buffer, CancellationToken cancellationToken)\r\n   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)\r\n   at Microsoft.EntityFrameworkCore.Query.Internal.AsyncQueryingEnumerable`1.AsyncEnumerator.MoveNext(CancellationToken cancellationToken)\r\n   at System.Linq.AsyncEnumerable.FirstOrDefault_[TSource](IAsyncEnumerable`1 source, CancellationToken cancellationToken) in D:\\a\\1\\s\\Ix.NET\\Source\\System.Interactive.Async\\First.cs:line 144\r\n   at Microsoft.EntityFrameworkCore.Query.Internal.AsyncLinqOperatorProvider.TaskResultAsyncEnumerable`1.Enumerator.MoveNext(CancellationToken cancellationToken)\r\n   at System.Linq.AsyncEnumerable.SelectEnumerableAsyncIterator`2.MoveNextCore(CancellationToken cancellationToken) in D:\\a\\1\\s\\Ix.NET\\Source\\System.Interactive.Async\\Select.cs:line 106\r\n   at System.Linq.AsyncEnumerable.AsyncIterator`1.MoveNext(CancellationToken cancellationToken) in D:\\a\\1\\s\\Ix.NET\\Source\\System.Interactive.Async\\AsyncIterator.cs:line 98\r\n   at Microsoft.EntityFrameworkCore.Query.Internal.AsyncLinqOperatorProvider.ExceptionInterceptor`1.EnumeratorExceptionInterceptor.MoveNext(CancellationToken cancellationToken)\r\n   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.ExecuteSingletonAsyncQuery[TResult](QueryContext queryContext, Func`2 compiledQuery, IDiagnosticsLogger`1 logger, Type contextType)\r\n   at LearningHub.Nhs.Repository.UserRepository.GetByUsernameAsync(String username, Boolean includeRoles) in C:\\e-lfh\\GIT\\LearningHub.Nhs.WebAPI\\LearningHub.Nhs.Repository\\UserRepository.cs:line 35\r\n   at elfhHub.Nhs.Services.UserService.SyncLHUserAsync(String userName) in C:\\e-lfh\\GIT\\LearningHub.Nhs.WebAPI\\elfhHub.Nhs.Services\\UserService.cs:line 89\r\n   at LearningHub.Nhs.Api.Controllers.AuthenticationController.AuthenticateElfhHubAsync(Login login) in C:\\e-lfh\\GIT\\LearningHub.Nhs.WebAPI\\LearningHub.Nhs.API\\Controllers\\AuthenticationController.cs:line 47\r\n   at Microsoft.AspNetCore.Mvc.Internal.ActionMethodExecutor.TaskOfIActionResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)\r\n   at System.Threading.Tasks.ValueTask`1.get_Result()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeActionMethodAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeNextActionFilterAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeInnerFilterAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeNextResourceFilter()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Rethrow(ResourceExecutedContext context)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeFilterPipelineAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeAsync()\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)\r\n   at Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware.Invoke(HttpContext httpContext)\r\n   at LearningHub.Nhs.Api.Middleware.ExceptionMiddleware.Invoke(HttpContext context) in C:\\e-lfh\\GIT\\LearningHub.Nhs.WebAPI\\LearningHub.Nhs.API\\Middleware\\ExceptionMiddleware.cs:line 34",
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18390,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:51:06.027"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action: AuthenticateElfhHubAsync | Request finished in 12532.8528ms 500 application/json",
                    Logger = "Microsoft.AspNetCore.Hosting.Internal.WebHost",
                    Callsite = "Microsoft.AspNetCore.Hosting.Internal.HostingApplicationDiagnostics.LogRequestFinished",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
                },
                new Log()
                {
                    Id = 18391,
                    Application = "LearningHub.Nhs.Auth",
                    Logged = DateTime.Parse("2019-07-25T14:51:06.867"),
                    Level = "Error",
                    Message = "Object reference not set to an instance of an object.",
                    Logger = "LearningHub.Nhs.Auth.Middleware.ExceptionMiddleware",
                    Callsite = "LearningHub.Nhs.Auth.Middleware.ExceptionMiddleware+<HandleExceptionAsync>d__5.MoveNext(C:\\e-lfh\\GIT\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\Middleware\\ExceptionMiddleware.cs:44)",
                    Exception = "System.NullReferenceException: Object reference not set to an instance of an object.\r\n   at LearningHub.Nhs.Auth.Controllers.AccountController.Login(LoginInputModel model, String button) in C:\\e-lfh\\GIT\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\Controllers\\AccountController.cs:line 129\r\n   at Microsoft.AspNetCore.Mvc.Internal.ActionMethodExecutor.TaskOfIActionResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)\r\n   at System.Threading.Tasks.ValueTask`1.get_Result()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeActionMethodAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeNextActionFilterAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeInnerFilterAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeNextResourceFilter()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Rethrow(ResourceExecutedContext context)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeFilterPipelineAsync()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeAsync()\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)\r\n   at Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware.Invoke(HttpContext httpContext)\r\n   at Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware.Invoke(HttpContext context)\r\n   at IdentityServer4.Hosting.IdentityServerMiddleware.Invoke(HttpContext context, IEndpointRouter router, IUserSession session, IEventService events) in C:\\local\\identity\\server4\\IdentityServer4\\src\\IdentityServer4\\src\\Hosting\\IdentityServerMiddleware.cs:line 72\r\n   at IdentityServer4.Hosting.MutualTlsTokenEndpointMiddleware.Invoke(HttpContext context, IAuthenticationSchemeProvider schemes) in C:\\local\\identity\\server4\\IdentityServer4\\src\\IdentityServer4\\src\\Hosting\\MtlsTokenEndpointMiddleware.cs:line 60\r\n   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.Cors.Infrastructure.CorsMiddleware.InvokeCore(HttpContext context)\r\n   at IdentityServer4.Hosting.BaseUrlMiddleware.Invoke(HttpContext context) in C:\\local\\identity\\server4\\IdentityServer4\\src\\IdentityServer4\\src\\Hosting\\BaseUrlMiddleware.cs:line 36\r\n   at LearningHub.Nhs.Auth.Middleware.ExceptionMiddleware.Invoke(HttpContext httpContext) in C:\\e-lfh\\GIT\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\Middleware\\ExceptionMiddleware.cs:line 35",
                    UserName = "test.user3",
                    UserId = 3,
                },
                new Log()
                {
                    Id = 18392,
                    Application = "LearningHub.Nhs.Auth",
                    Logged = DateTime.Parse("2019-07-25T14:51:07.177"),
                    Level = "Error",
                    Message = "An unhandled exception has occurred while executing the request.",
                    Logger = "Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware",
                    Callsite = "Microsoft.AspNetCore.Diagnostics.Internal.DiagnosticsLoggerExtensions.UnhandledException",
                    Exception = "System.IO.FileNotFoundException: Could not load file or assembly 'FluentValidation, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7de548da2fbae0f0'. The system cannot find the file specified.\r\nFile name: 'FluentValidation, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7de548da2fbae0f0'\r\n   at System.Signature.GetSignature(Void* pCorSig, Int32 cCorSig, RuntimeFieldHandleInternal fieldHandle, IRuntimeMethodInfo methodHandle, RuntimeType declaringType)\r\n   at System.Reflection.RuntimeConstructorInfo.get_Signature()\r\n   at System.Reflection.RuntimeConstructorInfo.GetParametersNoCopy()\r\n   at System.Reflection.RuntimeConstructorInfo.GetParameters()\r\n   at Newtonsoft.Json.Utilities.ReflectionUtils.<>c.<GetDefaultConstructor>b__11_0(ConstructorInfo c)\r\n   at System.Linq.Enumerable.SingleOrDefault[TSource](IEnumerable`1 source, Func`2 predicate)\r\n   at Newtonsoft.Json.Utilities.ReflectionUtils.GetDefaultConstructor(Type t, Boolean nonPublic)\r\n   at Newtonsoft.Json.Utilities.ReflectionUtils.HasDefaultConstructor(Type t, Boolean nonPublic)\r\n   at Newtonsoft.Json.Serialization.DefaultContractResolver.InitializeContract(JsonContract contract)\r\n   at Newtonsoft.Json.Serialization.DefaultContractResolver.CreateObjectContract(Type objectType)\r\n   at Newtonsoft.Json.Serialization.DefaultContractResolver.CreateContract(Type objectType)\r\n   at System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd(TKey key, Func`2 valueFactory)\r\n   at Newtonsoft.Json.Serialization.DefaultContractResolver.ResolveContract(Type type)\r\n   at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.CalculatePropertyValues(JsonWriter writer, Object value, JsonContainerContract contract, JsonProperty member, JsonProperty property, JsonContract& memberContract, Object& memberValue)\r\n   at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeObject(JsonWriter writer, Object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)\r\n   at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)\r\n   at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.Serialize(JsonWriter jsonWriter, Object value, Type objectType)\r\n   at Newtonsoft.Json.JsonSerializer.SerializeInternal(JsonWriter jsonWriter, Object value, Type objectType)\r\n   at Newtonsoft.Json.JsonConvert.SerializeObjectInternal(Object value, Type type, JsonSerializer jsonSerializer)\r\n   at Newtonsoft.Json.JsonConvert.SerializeObject(Object value)\r\n   at LearningHub.Nhs.Auth.Middleware.ExceptionMiddleware.HandleExceptionAsync(HttpContext context, Exception exception) in C:\\e-lfh\\GIT\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\Middleware\\ExceptionMiddleware.cs:line 62\r\n   at LearningHub.Nhs.Auth.Middleware.ExceptionMiddleware.Invoke(HttpContext httpContext) in C:\\e-lfh\\GIT\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\LearningHub.Nhs.Auth\\Middleware\\ExceptionMiddleware.cs:line 39\r\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)\r\n\r\n",
                    UserName = "test.user1",
                    UserId = 1,
                },
                new Log()
                {
                    Id = 18393,
                    Application = "LearningHub.Nhs.Api",
                    Logged = DateTime.Parse("2019-07-25T14:51:47.773"),
                    Level = "Info",
                    Message = "url: https://localhost/api/Authentication/Authenticate | action:  | Request starting HTTP/1.1 POST https://localhost:44384/api/Authentication/Authenticate application/json 51",
                    Logger = "Microsoft.AspNetCore.Hosting.Internal.WebHost",
                    Callsite = "Microsoft.AspNetCore.Hosting.Internal.HostingApplicationDiagnostics.LogRequestStarting",
                    Exception = string.Empty,
                    UserName = "test.user2",
                    UserId = 2,
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

        ////[Fact]
        ////public void CreateCSVFromTestData()
        ////{
        ////    var lstData = TestLogs();
        ////    var sb = new System.Text.StringBuilder();
        ////    sb.AppendLine($"Id,Application,Logged,Level,Message,Logger,Callsite,Exception");
        ////    foreach (var data in lstData)
        ////    {
        ////        sb.AppendLine($"\"{data.Id}\",\"{data.Application}\",\"{data.Logged}\",\"{data.Level}\",\"{data.Message}\",\"{data.Logger}\",\"{data.Callsite}\",\"{data.Exception}\"");
        ////    }
        ////    var op = sb.ToString();

        /// <summary>
        /// The filter items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<Log> FilterItems(IQueryable<Log> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "id":
                        int enteredId = 0;
                        int.TryParse(filter.Value, out enteredId);
                        items = items.Where(l => l.Id == enteredId);
                        break;
                    case "application":
                        items = items.Where(x => x.Application.Contains(filter.Value));
                        break;
                    case "logged":
                        DateTime enteredDate = DateTime.MinValue;
                        DateTime.TryParse(filter.Value, out enteredDate);

                        if (enteredDate != DateTime.MinValue)
                        {
                            items = items.Where(l => l.Logged >= enteredDate && l.Logged < enteredDate.AddDays(1));
                        }

                        break;
                    case "level":
                        items = items.Where(l => l.Level.Contains(filter.Value));
                        break;
                    case "message":
                        items = items.Where(l => l.Message.Contains(filter.Value));
                        break;
                    case "logger":
                        items = items.Where(l => l.Logger.Contains(filter.Value));
                        break;
                    case "callsite":
                        items = items.Where(l => l.Callsite.Contains(filter.Value));
                        break;
                    case "exception":
                        items = items.Where(l => l.Exception.Contains(filter.Value));
                        break;
                    case "username":
                        items = items.Where(l => l.UserName.Contains(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// The order items items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<Log> OrderItemsItems(IQueryable<Log> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "application":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Application);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Application);
                    }

                    break;
                case "logged":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Logged);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Logged);
                    }

                    break;
                case "level":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Level);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Level);
                    }

                    break;
                case "message":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Message);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Message);
                    }

                    break;
                case "logger":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Logger);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Logger);
                    }

                    break;
                case "callsite":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Callsite);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Callsite);
                    }

                    break;
                case "exception":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Exception);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Exception);
                    }

                    break;
                case "username":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.UserName);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.UserName);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Id);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Id);
                    }

                    break;
            }

            return items;
        }

        private Mock<IMapper> GetMapperMock(Mock<ILogRepository> logRepositoryMock, int page, int pageSize, string filter = "", string sortColumn = "", string sortDirection = "")
        {
            var items = logRepositoryMock.Object.GetAll();
            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);
            items = this.FilterItems(items, filterCriteria);
            items = this.OrderItemsItems(items, sortColumn, sortDirection);
            items = items.Skip((page - 1) * pageSize).Take(pageSize);
            var mappedItems = this.NewMapper().ProjectTo<LogBasicViewModel>(items);
            var mapResult = new AsyncEnumerable<LogBasicViewModel>(mappedItems);
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(s => s.ProjectTo<LogBasicViewModel>(It.IsAny<IQueryable<Log>>(), It.IsAny<object>()))
                .Returns(mapResult);
            return mapperMock;
        }
    }
}
