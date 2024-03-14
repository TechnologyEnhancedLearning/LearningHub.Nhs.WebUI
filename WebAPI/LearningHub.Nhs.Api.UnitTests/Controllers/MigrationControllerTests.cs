namespace LearningHub.Nhs.Api.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Controllers;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// The migration controller tests.
    /// </summary>
    public class MigrationControllerTests
    {
        /// <summary>
        /// The mock elfh user service.
        /// </summary>
        private Mock<IUserService> mockUserService;

        /// <summary>
        /// The mock migration service.
        /// </summary>
        private Mock<IMigrationService> mockMigrationService;

        /// <summary>
        /// The mock form file.
        /// </summary>
        private Mock<IFormFile> mockFormFile;

        /// <summary>
        /// The migration controller.
        /// </summary>
        private MigrationController migrationController;

        private Mock<ILogger<MigrationController>> mockLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationControllerTests"/> class.
        /// </summary>
        public MigrationControllerTests()
        {
            this.mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            this.mockMigrationService = new Mock<IMigrationService>(MockBehavior.Strict);
            this.mockLogger = new Mock<ILogger<MigrationController>>();

            this.migrationController = new MigrationController(this.mockUserService.Object, this.mockMigrationService.Object, this.mockLogger.Object);
            this.migrationController.ControllerContext = this.SetContollerContext();

            this.mockFormFile = new Mock<IFormFile>(MockBehavior.Loose);
            this.mockFormFile.Setup(x => x.Length).Returns(999);
        }

        /// <summary>
        /// The create from json file_ returns bad request if file is null.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonFile_ReturnsBadRequestIfFileIsNull()
        {
            // Arrange

            // Act
            var result = await this.migrationController.CreateFromJsonFile(null, 9, "my-container", 99);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var typedResult = result as BadRequestObjectResult;
            Assert.Equal("Invalid file", typedResult.Value);
        }

        /// <summary>
        /// The create from json file_ returns bad request if file is empty.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonFile_ReturnsBadRequestIfFileIsEmpty()
        {
            // Arrange
            this.mockFormFile.Setup(x => x.Length).Returns(0);

            // Act
            var result = await this.migrationController.CreateFromJsonFile(this.mockFormFile.Object, 9, "my-container", 99);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var typedResult = result as BadRequestObjectResult;
            Assert.Equal("Invalid file", typedResult.Value);
        }

        /// <summary>
        /// The create from json file_ returns bad request if service returns invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonFile_ReturnsBadRequestIfServiceReturnsInvalid()
        {
            // Arrange
            this.mockMigrationService.Setup(x => x.CreateFromJsonString(It.IsAny<string>(), 9, "my-container", 99, 999))
                .Returns(Task.FromResult<LearningHubValidationResult>(new LearningHubValidationResult(false)));

            // Act
            var result = await this.migrationController.CreateFromJsonFile(this.mockFormFile.Object, 9, "my-container", 99);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// The create from json file_ returns ok if service returns valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromJsonFile_ReturnsOkIfServiceReturnsValid()
        {
            // Arrange
            this.mockMigrationService.Setup(x => x.CreateFromJsonString(It.IsAny<string>(), 9, "my-container", 99, 999))
                .Returns(Task.FromResult<LearningHubValidationResult>(new LearningHubValidationResult(true)));

            // Act
            var result = await this.migrationController.CreateFromJsonFile(this.mockFormFile.Object, 9, "my-container", 99);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// The CreateFromStagingTables returns bad request if file is empty.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromStagingTables_ReturnsBadRequestIfFileIsEmpty()
        {
            // Arrange
            this.mockFormFile.Setup(x => x.Length).Returns(0);

            // Act
            var result = await this.migrationController.CreateFromStagingTables(this.mockFormFile.Object, 9, "my-container");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var typedResult = result as BadRequestObjectResult;
            Assert.Equal("Invalid file", typedResult.Value);
        }

        /// <summary>
        /// The CreateFromStagingTables returns bad request if service returns invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromStagingTables_ReturnsBadRequestIfServiceReturnsInvalid()
        {
            // Arrange
            this.mockMigrationService.Setup(x => x.CreateFromStagingTables(It.IsAny<byte[]>(), 9, "my-container", 999))
                .Returns(Task.FromResult<LearningHubValidationResult>(new LearningHubValidationResult(false)));

            // Act
            var result = await this.migrationController.CreateFromStagingTables(this.mockFormFile.Object, 9, "my-container");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// The CreateFromStagingTables returns ok if service returns valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateFromStagingTables_ReturnsOkIfServiceReturnsValid()
        {
            // Arrange
            this.mockMigrationService.Setup(x => x.CreateFromStagingTables(It.IsAny<byte[]>(), 9, "my-container", 999))
                .Returns(Task.FromResult<LearningHubValidationResult>(new LearningHubValidationResult(true)));

            // Act
            var result = await this.migrationController.CreateFromStagingTables(this.mockFormFile.Object, 9, "my-container");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// The set contoller context.
        /// </summary>
        /// <returns>The <see cref="ControllerContext"/>.</returns>
        private ControllerContext SetContollerContext()
        {
            IList<Claim> claimCollection = new List<Claim>
                    {
                        new Claim("given_name", "TestUser"),
                        new Claim("sub", "999"),
                    };

            var context = new ControllerContext();
            context.HttpContext = new DefaultHttpContext();
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claimCollection));

            return context;
        }
    }
}
