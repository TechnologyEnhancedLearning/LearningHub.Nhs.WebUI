// <copyright file="ReportServiceTests.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Entities.Reporting;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Report;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Report;
    using LearningHub.Nhs.Services.Report;
    using LearningHub.Nhs.Services.UnitTests.Helpers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;
    using User = LearningHub.Nhs.Models.Entities.User;

    /// <summary>
    /// The report service tests.
    /// </summary>
    public class ReportServiceTests
    {
        /// <summary>
        /// The mock caching service.
        /// </summary>
        private readonly Mock<ICachingService> mockCachingService;

        /// <summary>
        /// The mock reportRepository.
        /// </summary>
        private readonly Mock<IReportRepository> mockReportRepository;

        /// <summary>
        /// The mock clientRepository.
        /// </summary>
        private readonly Mock<IClientRepository> mockClientRepository;

        /// <summary>
        /// The mock logger.
        /// </summary>
        private readonly Mock<ILogger<ReportService>> mockLogger;

        /// <summary>
        /// The report service.
        /// </summary>
        private readonly IReportService reportService;

        /// <summary>
        /// mock settings.
        /// </summary>
        private readonly Mock<IOptions<Settings>> mockSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportServiceTests"/> class.
        /// </summary>
        public ReportServiceTests()
        {
            this.mockSettings = new Mock<IOptions<Settings>>();
            this.mockReportRepository = new Mock<IReportRepository>(MockBehavior.Strict);
            this.mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            this.mockCachingService = new Mock<ICachingService>(MockBehavior.Loose);
            this.mockLogger = new Mock<ILogger<ReportService>>(MockBehavior.Loose);
            this.SetupSettings();
            this.reportService = new ReportService(
                this.mockReportRepository.Object,
                this.mockClientRepository.Object,
                this.mockCachingService.Object,
                this.mockLogger.Object,
                this.mockSettings.Object,
                this.NewMapper());
        }

        /// <summary>
        /// The get by id async is valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_Valid()
        {
            // Arrange
            int reportId = 1;
            this.mockReportRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
                                        .ReturnsAsync(this.GetTestReport(reportId));

            // Act
            var report = await this.reportService.GetByIdAsync(reportId, true);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            Assert.IsType<ReportModel>(report);
            Assert.Equal(1, report.Id);
            Assert.Equal("Report-1", report.Name);
        }

        /// <summary>
        /// The get by id async is invalid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetByIdAsync_InValid()
        {
            // Arrange
            int reportId = 999;
            this.mockReportRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
                                        .ReturnsAsync((Report)null);

            // Act
            var report = await this.reportService.GetByIdAsync(reportId, true);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            Assert.Null(report);
        }

        /// <summary>
        /// The get client by client id async valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetClientByClientIdAsync_Valid()
        {
            // Arrange
            var clientId = this.TestClients().First().ClientId;
            this.mockClientRepository.Setup(r => r.GetAll())
                                        .Returns(this.TestClientsAsyncMock().Object);

            this.mockCachingService.Setup(r => r.GetAsync<List<ClientModel>>(It.IsAny<string>()))
                                         .ReturnsAsync(new CacheReadResponse<List<ClientModel>>()
                                         {
                                             ResponseEnum = CacheReadResponseEnum.NotFound,
                                         });

            this.mockCachingService.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<List<ClientModel>>))
                .ReturnsAsync(new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                });

            // Act
            var result = await this.reportService.GetClientByClientIdAsync(clientId);

            // Assert
            this.mockClientRepository.Verify(x => x.GetAll(), Times.Once());
            Assert.IsType<ClientModel>(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("lh-ui", result.Name);
        }

        /// <summary>
        /// The get client by client id async valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetClientByClientIdAsync_InValid()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            this.mockClientRepository.Setup(r => r.GetAll())
                                        .Returns(this.TestClientsAsyncMock().Object);

            this.mockCachingService.Setup(r => r.GetAsync<List<ClientModel>>(It.IsAny<string>()))
                                         .ReturnsAsync(new CacheReadResponse<List<ClientModel>>()
                                         {
                                             ResponseEnum = CacheReadResponseEnum.NotFound,
                                         });

            this.mockCachingService.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<List<ClientModel>>))
                .ReturnsAsync(new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                });

            // Act
            var result = await this.reportService.GetClientByClientIdAsync(clientId);

            // Assert
            this.mockClientRepository.Verify(x => x.GetAll(), Times.Once());
            Assert.Null(result);
        }

        /// <summary>
        /// The create report is valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateReport_Valid()
        {
            // Arrange
            var userId = 4;
            var clientId = 1;
            this.mockReportRepository.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<Report>()))
                                        .ReturnsAsync(101);

            var reportCreateModel = new ReportCreateModel
            {
                Name = "Report1",
                ReportTypeId = 1,
                ReportPages = new List<ReportPageModel>
                {
                    new ReportPageModel
                    {
                        ReportOrientationModeId = 1,
                        Html = "<h1>Test</h1>",
                    },
                },
            };

            // Act
            var result = await this.reportService.CreateAsync(userId, clientId, reportCreateModel);

            // Assert
            this.mockReportRepository.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<Report>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
            Assert.Equal(101, result.CreatedId);
        }

        /// <summary>
        /// The create report is invalid if input report data in invalid.
        /// </summary>
        /// <param name="reportCreateModel">report create model.</param>
        /// <returns>.</returns>
        [Theory]
        [ClassData(typeof(ReportCreateModelInValidTestData))]
        public async Task CreateReport_InValid(ReportCreateModel reportCreateModel)
        {
            // Arrange
            var userId = 4;
            var clientId = 1;
            this.mockReportRepository.Setup(r => r.CreateAsync(It.IsAny<int>(), It.IsAny<Report>()))
                                        .ReturnsAsync(101);

            // Act
            var result = await this.reportService.CreateAsync(userId, clientId, reportCreateModel);

            // Assert
            this.mockReportRepository.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<Report>()), Times.Never());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// The update report status is valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateReportStatusAsync_Valid()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
                                        .ReturnsAsync(testreport);

            this.mockReportRepository.Setup(r => r.UpdateAsync(It.IsAny<int>(), testreport)).Returns(Task.CompletedTask);

            var reportStatusUpdateModel = new ReportStatusUpdateModel
            {
                ReportId = 1,
                StatusId = 2,
            };

            // Act
            var result = await this.reportService.UpdateReportStatusAsync(reportStatusUpdateModel);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            this.mockReportRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Report>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// The update report status is invalid if report doesn't exists.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateReportStatusAsync_WhenReportDoesNotExists_InValid()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
                                         .ReturnsAsync((Report)null);

            this.mockReportRepository.Setup(r => r.UpdateAsync(It.IsAny<int>(), testreport)).Returns(Task.CompletedTask);

            var reportStatusUpdateModel = new ReportStatusUpdateModel
            {
                ReportId = 1,
                StatusId = 2,
            };

            // Act
            var result = await this.reportService.UpdateReportStatusAsync(reportStatusUpdateModel);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            this.mockReportRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Report>()), Times.Never());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// The update report status is invalid if report doesn't exists.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateReportStatusAsync_WhenReportExists_ThrowsException_InValid()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
                                         .ReturnsAsync(testreport);

            this.mockReportRepository.Setup(r => r.UpdateAsync(It.IsAny<int>(), testreport)).Throws(new Exception());

            var reportStatusUpdateModel = new ReportStatusUpdateModel
            {
                ReportId = 1,
                StatusId = 2,
            };

            // Act
            var result = await this.reportService.UpdateReportStatusAsync(reportStatusUpdateModel);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            this.mockReportRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Report>()), Times.Once());
            Assert.IsType<LearningHubValidationResult>(result);
            Assert.Equal("Error on updating report status for the report id: 1", result.Details[0]);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// Test if report exists and returns true.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ReportExistsAsync_WhenReportExists_ReturnsTrue()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                        .ReturnsAsync(testreport);

            // Act
            var result = await this.reportService.ReportExistsAsync(testreport.FileName, testreport.Hash);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            Assert.True(result);
        }

        /// <summary>
        /// Check if report doesn't report exists and returns false.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ReportExistsAsync_WhenReportDoesNotExists_ReturnsFalse()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                        .ReturnsAsync((Report)null);

            // Act
            var result = await this.reportService.ReportExistsAsync(testreport.FileName, testreport.Hash);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            Assert.False(result);
        }

        /// <summary>
        /// Test if report exists and returns status.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetReportStatusAsync_WhenReportExists_Valid()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                        .ReturnsAsync(testreport);

            // Act
            var result = await this.reportService.GetReportStatusAsync(testreport.FileName, testreport.Hash);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            Assert.IsType<ReportStatusModel>(result);
            Assert.Equal(testreport.ReportStatusId, result.Id);
        }

        /// <summary>
        /// Check if report doesn't report exists and returns null.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task ReportExistsAsync_WhenReportDoesNotExists_ReturnsNull()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                        .ReturnsAsync((Report)null);

            // Act
            var result = await this.reportService.GetReportStatusAsync(testreport.FileName, testreport.Hash);

            this.mockReportRepository.Verify(x => x.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            Assert.Null(result);
        }

        /// <summary>
        /// Test if report exists and returns file detail.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetFileDetailAsync_WhenReportExists_Valid()
        {
            // Arrange
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                        .ReturnsAsync(testreport);

            // Act
            var result = await this.reportService.GetByFileDetailAsync(testreport.FileName, testreport.Hash, false);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            Assert.IsType<ReportModel>(result);
            Assert.Equal(testreport.Id, result.Id);
            Assert.Equal(testreport.Name, result.Name);
            Assert.Equal(testreport.FileName, result.FileName);
        }

        /// <summary>
        /// Check if report doesn't report exists and returns null.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GetFileDetailAsync_WhenReportDoesNotExists_ReturnsNull()
        {
            var testreport = this.GetTestReport(1);

            this.mockReportRepository.Setup(r => r.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                        .ReturnsAsync((Report)null);

            // Act
            var result = await this.reportService.GetReportStatusAsync(testreport.FileName, testreport.Hash);

            // Assert
            this.mockReportRepository.Verify(x => x.GetByFileDetailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            Assert.Null(result);
        }

        /// <summary>
        /// The get report.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Report"/>.</returns>
        private Report GetTestReport(int id)
        {
            return this.TestReports().FirstOrDefault(l => l.Id == id);
        }

        /// <summary>
        /// The test client async mock.
        /// </summary>
        /// <returns>The <see cref="Mock"/>.</returns>
        private Mock<DbSet<Client>> TestClientsAsyncMock()
        {
            var records = this.TestClients().AsQueryable();

            var mockDbSet = new Mock<DbSet<Client>>();

            mockDbSet.As<IAsyncEnumerable<Client>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(new TestAsyncEnumerator<Client>(records.GetEnumerator()));

            mockDbSet.As<IQueryable<Client>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Client>(records.Provider));

            mockDbSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(records.Expression);
            mockDbSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(records.ElementType);
            mockDbSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => records.GetEnumerator());

            return mockDbSet;
        }

        /// <summary>
        /// Mock Client Objects.
        /// </summary>
        /// <returns>.</returns>
        private List<Client> TestClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    Id = 1,
                    Name = "lh-ui",
                    ClientId = Guid.Parse("FF6B5370-6274-4B09-A617-FD7B72E63AF9"),
                },
                new Client()
                {
                    Id = 2,
                    Name = "lh-admin",
                    ClientId = Guid.Parse("F70C3C29-C0AA-4398-A934-CCF42616B0F3"),
                },
            };
         }

        /// <summary>
        /// Mock Report Objects.
        /// </summary>
        /// <returns>.</returns>
        private List<Report> TestReports()
        {
            return new List<Report>()
            {
                new Report()
                {
                    Id = 1,
                    Name = "Report-1",
                    FileName = "Report1.pdf",
                    Hash = "e05e4a62381a0e5b206857a3334304b1930559667d5edfef916756ba4b8dacb3",
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
                    ReportTypeId = 1,
                    ReportStatusId = 2,
                    ReportPages = new List<ReportPage>()
                    {
                        new ReportPage()
                        {
                            ReportOrientationModeId = 1,
                            Html = "<html><body><h1>Test1</h1></body></html>",
                        },
                        new ReportPage()
                        {
                            ReportOrientationModeId = 2,
                            Html = "<html><body><h1>Test2</h1></body></html>",
                        },
                    },
                },
                new Report()
                {
                    Id = 1,
                    Name = "Report-2",
                    FileName = "Report2.pdf",
                    Hash = "e05e4a62381a0e5b206857a3334304b1930559667d5edfef916756ba4b8dacb4",
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
                    ReportTypeId = 2,
                    ReportStatusId = 1,
                    ReportPages = new List<ReportPage>()
                    {
                        new ReportPage()
                        {
                            ReportOrientationModeId = 1,
                            Html = "<html><body><h1>Test1</h1></body></html>",
                        },
                        new ReportPage()
                        {
                            ReportOrientationModeId = 2,
                            Html = "<html><body><h1>Test2</h1></body></html>",
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

        /// <summary>
        /// set up settings.
        /// </summary>
        private void SetupSettings()
        {
            this.mockSettings.Setup(r => r.Value)
                .Returns(new Settings());
        }

        /// <summary>
        /// ReportCreateModelInValidTestData.
        /// </summary>
        public class ReportCreateModelInValidTestData : TheoryData<ReportCreateModel>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReportCreateModelInValidTestData"/> class.
            /// </summary>
            public ReportCreateModelInValidTestData()
            {
                this.ReportCreateModels = new List<ReportCreateModel>
                {
                        new ReportCreateModel { ReportTypeId = 1 },
                        new ReportCreateModel { Name = string.Empty, ReportTypeId = 0 },
                        new ReportCreateModel { Name = "Report1", ReportTypeId = 0 },
                        new ReportCreateModel { Name = "Report2", ReportTypeId = 1 },
                };

                foreach (var item in this.ReportCreateModels)
                {
                    this.Add(item);
                }
            }

            /// <summary>
            /// Gets or sets CatalogueViewModels.
            /// </summary>
            public List<ReportCreateModel> ReportCreateModels { get; set; }
        }
    }
}