namespace LearningHub.Nhs.Api.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Controllers;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Log;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// The log controller tests.
    /// </summary>
    public class LogControllerTests
    {
        private Mock<ILogger<LogController>> mockLogger;

        /// <summary>
        /// The get by id_ success.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetById_Success()
        {
            var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            var logServiceMock = new Mock<ILogService>(MockBehavior.Strict);
            this.mockLogger = new Mock<ILogger<LogController>>();

            logServiceMock.Setup(us => us.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(
                    new LogViewModel()
                    {
                        Id = 1,
                        Application = "LearningHub.Nhs.Api",
                        Logged = DateTime.Parse("2019-07-25T14:49:22.513"),
                        Level = "Info",
                        UserName = "test.user",
                    }));

            var controller = new LogController(userServiceMock.Object, logServiceMock.Object, this.mockLogger.Object);

            var response = await controller.GetById(1);

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;
            var logResult = okResult.Value as LogViewModel;

            Assert.Equal("LearningHub.Nhs.Api", logResult.Application);
        }

        /// <summary>
        /// The get page_ success.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetPage_Success()
        {
            var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            var logServiceMock = new Mock<ILogService>(MockBehavior.Strict);
            this.mockLogger = new Mock<ILogger<LogController>>();

            logServiceMock.Setup(us => us.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new PagedResultSet<LogBasicViewModel>()
                    {
                        TotalItemCount = 3,
                        Items = new List<LogBasicViewModel>()
                            {
                                new LogBasicViewModel()
                                {
                                    Id = 1,
                                    Application = "LearningHub.Nhs.Api",
                                    Logged = DateTime.Parse("2019-07-25T14:49:22.513"),
                                    Level = "Error",
                                    UserName = "test.user",
                                },
                                new LogBasicViewModel()
                                {
                                    Id = 2,
                                    Application = "LearningHub.Nhs.Auth",
                                    Logged = DateTime.Parse("2019-07-25T14:50:22.513"),
                                    Level = "Info",
                                    UserName = "test.user",
                                },
                                new LogBasicViewModel()
                                {
                                    Id = 3,
                                    Application = "LearningHub.Nhs.Api",
                                    Logged = DateTime.Parse("2019-07-25T14:52:22.513"),
                                    Level = "Trace",
                                    UserName = "test.user",
                                },
                            },
                    }));

            var controller = new LogController(userServiceMock.Object, logServiceMock.Object, this.mockLogger.Object);

            var response = await controller.GetPage(2, 3);

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;
            var logResult = okResult.Value as PagedResultSet<LogBasicViewModel>;

            Assert.Equal(3, logResult.Items.Count);
        }
    }
}
