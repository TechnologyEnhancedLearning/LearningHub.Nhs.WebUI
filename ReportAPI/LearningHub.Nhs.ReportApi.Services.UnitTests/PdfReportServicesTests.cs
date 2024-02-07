// <copyright file="PdfReportServicesTests.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoMapper;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using LearningHub.Nhs.ReportApi.Shared.Configuration;
    using LearningHub.Nhs.ReportApi.Shared.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    /// <summary>
    /// The pdf report service tests.
    /// </summary>
    public class PdfReportServicesTests
    {
        /// <summary>
        /// The mock logger.
        /// </summary>
        private readonly Mock<ILogger<PdfReportService>> mockLogger;

        /// <summary>
        /// The pdf report service.
        /// </summary>
        private readonly IPdfReportService pdfReportService;

        /// <summary>
        /// The azure blob service.
        /// </summary>
        private readonly Mock<IAzureBlobStorageService> mockAzureBlobStorageService;

        /// <summary>
        /// The service bus service.
        /// </summary>
        private readonly Mock<IServiceBusMessageService> mockServiceBusMessageService;

        /// <summary>
        /// The learning hub api facade.
        /// </summary>
        private readonly Mock<ILearningHubApiFacade> mockLearningHubApiFacade;

        /// <summary>
        /// mock settings.
        /// </summary>
        private readonly Mock<IOptions<Settings>> mockSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfReportServicesTests"/> class.
        /// </summary>
        public PdfReportServicesTests()
        {
            this.mockSettings = new Mock<IOptions<Settings>>();
            this.mockLogger = new Mock<ILogger<PdfReportService>>(MockBehavior.Loose);
            this.mockAzureBlobStorageService = new Mock<IAzureBlobStorageService>();
            this.mockServiceBusMessageService = new Mock<IServiceBusMessageService>();
            this.mockLearningHubApiFacade = new Mock<ILearningHubApiFacade>();

            this.SetupSettings();

            this.pdfReportService = new PdfReportService(
                this.mockAzureBlobStorageService.Object,
                this.mockServiceBusMessageService.Object,
                this.mockLearningHubApiFacade.Object,
                this.mockSettings.Object,
                this.NewMapper(),
                this.mockLogger.Object);
        }

        /// <summary>
        /// The create report is valid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task CreateReport_Valid()
        {
            // Arrange
            var reportModel = new ReportModel() { Id = 1, Name = "Report1", FileName = "Report1.pdf", Hash = "HashData", ReportTypeId = 1, ReportStatusId = 1 };

            this.mockLearningHubApiFacade.Setup(r => r.PostAsync<ReportModel, RequestModel>(It.IsAny<string>(), It.IsAny<RequestModel>()))
                                        .ReturnsAsync(reportModel);

            this.mockServiceBusMessageService.Setup(r => r.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                        .Returns(Task.CompletedTask);

            // Act
            var result = await this.pdfReportService.CreatePdfReportAsync(this.GetTestDataRequestModel());

            // Assert
            this.mockLearningHubApiFacade.Verify(x => x.PostAsync<ReportModel, RequestModel>(It.IsAny<string>(), It.IsAny<RequestModel>()), Times.Once());
            this.mockServiceBusMessageService.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());

            Assert.IsType<ResponseModel>(result);
            Assert.Equal("Report1.pdf", result.FileName);
            Assert.Equal("HashData", result.Hash);
        }

        /// <summary>
        /// Create report is invalid.
        /// </summary>
        [Fact]
        public void CreateReport_InValid()
        {
            // Arrange
            var reportModel = new ReportModel() { Id = 1, Name = "Report1", FileName = "Report1.pdf", Hash = "HashData", ReportTypeId = 1, ReportStatusId = 1 };
            this.mockLearningHubApiFacade.Setup(r => r.PostAsync<ReportModel, RequestModel>(It.IsAny<string>(), It.IsAny<RequestModel>()))
                                        .Throws(new Exception());

            // act and assert
            _ = Assert.ThrowsAsync<Exception>(async () => await this.pdfReportService.CreatePdfReportAsync(this.GetTestDataRequestModel()));

            // Assert
            this.mockLearningHubApiFacade.Verify(x => x.PostAsync<ReportModel, RequestModel>(It.IsAny<string>(), It.IsAny<RequestModel>()), Times.Once());

            // Mock.Verify(Services => Services.., Times.Never);
            this.mockServiceBusMessageService.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// The get pdf report status is valid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetPdfReportStatusAsync_Valid()
        {
            // Arrange
            var fileName = "report1.pdf";
            var hash = "hashdata";

            var reportStatusModel = new ReportStatusModel() { Id = 1, Status = "Ready" };

            this.mockLearningHubApiFacade.Setup(r => r.GetAsync<ReportStatusModel>(It.IsAny<string>()))
                                        .ReturnsAsync(reportStatusModel);

            // Act
            var result = await this.pdfReportService.GetPdfReportStatusAsync(fileName, hash);

            // Assert
            this.mockLearningHubApiFacade.Verify(r => r.GetAsync<ReportStatusModel>(It.IsAny<string>()), Times.Once());

            Assert.IsType<ReportStatusModel>(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Ready", result.Status);
        }

        /// <summary>
        /// The get pdf report status is invalid.
        /// </summary>
        [Fact]
        public void GetPdfReportStatusAsync_InValid()
        {
            // Arrange
            var fileName = "report1.pdf";
            var hash = "hashdata";

            var reportModel = new ReportModel() { Id = 1, Name = "Report1", FileName = "Report1.pdf", Hash = "HashData", ReportTypeId = 1, ReportStatusId = 1 };

            this.mockLearningHubApiFacade.Setup(r => r.GetAsync<ReportStatusModel>(It.IsAny<string>()))
                                        .Throws(new Exception());

            // act and assert
            _ = Assert.ThrowsAsync<Exception>(async () => await this.pdfReportService.GetPdfReportStatusAsync(fileName, hash));

            // Assert
            this.mockLearningHubApiFacade.Verify(r => r.GetAsync<ReportStatusModel>(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// The get pdf report is valid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetPdfReportAsync_Valid()
        {
            // Arrange
            var fileName = "report1.pdf";
            var hash = "hashdata";
            var reportModel = new ReportModel() { Id = 1, Name = "Report1", FileName = "Report1.pdf", Hash = "HashData", ReportTypeId = 1, ReportStatusId = 1 };

            this.mockLearningHubApiFacade.Setup(r => r.GetAsync<ReportModel>(It.IsAny<string>()))
                                        .ReturnsAsync(reportModel);

            // Act
            var result = await this.pdfReportService.GetPdfReportAsync(fileName, hash);

            // Assert
            this.mockLearningHubApiFacade.Verify(r => r.GetAsync<ReportModel>(It.IsAny<string>()), Times.Once());

            Assert.IsType<ReportModel>(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Report1.pdf", result.FileName);
        }

        /// <summary>
        /// The get pdf report is invalid.
        /// </summary>
        [Fact]
        public void GetPdfReportAsync_InValid()
        {
            // Arrange
            var fileName = "report1.pdf";
            var hash = "hashdata";

            var reportModel = new ReportModel() { Id = 1, Name = "Report1", FileName = "Report1.pdf", Hash = "HashData", ReportTypeId = 1, ReportStatusId = 1 };

            this.mockLearningHubApiFacade.Setup(r => r.GetAsync<ReportModel>(It.IsAny<string>()))
                                        .Throws(new Exception());

            // act and assert
            _ = Assert.ThrowsAsync<Exception>(async () => await this.pdfReportService.GetPdfReportAsync(fileName, hash));

            // Assert
            this.mockLearningHubApiFacade.Verify(r => r.GetAsync<ReportModel>(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// The get pdf file is valid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetPdfReportFileAsync_Valid()
        {
            // Arrange
            var fileName = "report1.pdf";
            var hash = "hashdata";

            byte[] myByteArray = new byte[10];
            MemoryStream stream = new MemoryStream();
            stream.Write(myByteArray, 0, myByteArray.Length);

            var blobModel = new BlobModel() { Name = "Report1", ContentType = "application/pdf", Content = stream };

            this.mockLearningHubApiFacade.Setup(r => r.GetAsync<object>(It.IsAny<string>()))
                                        .ReturnsAsync(true);

            this.mockAzureBlobStorageService.Setup(r => r.GetFile(It.IsAny<string>()))
                                        .ReturnsAsync(blobModel);

            // Act
            var result = await this.pdfReportService.GetPdfReportFileAsync(fileName, hash);

            // Assert
            this.mockLearningHubApiFacade.Verify(r => r.GetAsync<object>(It.IsAny<string>()), Times.Once());
            this.mockAzureBlobStorageService.Verify(r => r.GetFile(It.IsAny<string>()), Times.Once());

            Assert.IsType<BlobModel?>(result);
            Assert.Equal("Report1", result?.Name);
            Assert.Equal("application/pdf", result?.ContentType);
            Assert.Equal(stream, result?.Content);
        }

        /// <summary>
        /// The get pdf file returns null and is invalid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetPdfReportFileAsync_ReturnsNull_InValid()
        {
            // Arrange
            var fileName = "report1.pdf";
            var hash = "hashdata";

            BlobModel? blobModel = null;

            this.mockLearningHubApiFacade.Setup(r => r.GetAsync<object>(It.IsAny<string>()))
                                        .ReturnsAsync(true);

            this.mockAzureBlobStorageService.Setup(r => r.GetFile(It.IsAny<string>()))
                                        .ReturnsAsync(blobModel);

            // Act
            var result = await this.pdfReportService.GetPdfReportFileAsync(fileName, hash);

            // Assert
            this.mockLearningHubApiFacade.Verify(r => r.GetAsync<object>(It.IsAny<string>()), Times.Once());
            this.mockAzureBlobStorageService.Verify(r => r.GetFile(It.IsAny<string>()), Times.Once());

            Assert.Null(result);
        }

        /// <summary>
        /// The get pdf file throws exception if record does not exists.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task GetPdfReportFileAsync_RecordDoesNotExists_ThrowsException()
        {
            // Arrange
            var fileName = "report1.pdf";
            var hash = "hashdata";

            this.mockLearningHubApiFacade.Setup(r => r.GetAsync<object>(It.IsAny<string>()))
                                        .ReturnsAsync(false);

            // Act
            var result = await this.pdfReportService.GetPdfReportFileAsync(fileName, hash);

            // Assert
            this.mockLearningHubApiFacade.Verify(r => r.GetAsync<object>(It.IsAny<string>()), Times.Once());
            this.mockAzureBlobStorageService.Verify(r => r.GetFile(It.IsAny<string>()), Times.Never());

            this.mockLogger.Verify(
                x => x.Log(
               LogLevel.Error,
               It.IsAny<EventId>(),
               It.IsAny<It.IsAnyType>(),
               It.IsAny<Exception>(),
               (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

            Assert.Null(result);
        }

        /// <summary>
        /// The update report status is valid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task UpdateReportStatusAsync_Valid()
        {
            // Arrange
            var apiResponse = new ApiResponse { Success = true, ValidationResult = new LearningHubValidationResult { IsValid = true, Details = new List<string>(), CreatedId = 0, UpdatedIds = new List<int> { 1 } } };
            var reportStatusUpdateModel = new ReportStatusUpdateModel() { ReportId = 1, StatusId = 1 };

            this.mockLearningHubApiFacade.Setup(r => r.PostAsync<ApiResponse, ReportStatusUpdateModel>(It.IsAny<string>(), It.IsAny<ReportStatusUpdateModel>()))
                                        .ReturnsAsync(apiResponse);

            // Act
            var result = await this.pdfReportService.UpdateReportStatusAsync(reportStatusUpdateModel);

            // Assert
            this.mockLearningHubApiFacade.Verify(x => x.PostAsync<ApiResponse, ReportStatusUpdateModel>(It.IsAny<string>(), It.IsAny<ReportStatusUpdateModel>()), Times.Once());

            Assert.IsType<ApiResponse>(result);
            Assert.True(result.Success);
            Assert.True(result.ValidationResult.IsValid);
        }

        /// <summary>
        /// The update report status is invalid.
        /// </summary>
        [Fact]
        public void UpdateReportStatusAsync_InValid()
        {
            // Arrange
            var apiResponse = new ApiResponse { Success = true, ValidationResult = new LearningHubValidationResult { IsValid = true, Details = new List<string>(), CreatedId = 0, UpdatedIds = new List<int> { 1 } } };
            var reportStatusUpdateModel = new ReportStatusUpdateModel() { ReportId = 1, StatusId = 1 };

            this.mockLearningHubApiFacade.Setup(r => r.PostAsync<ApiResponse, ReportStatusUpdateModel>(It.IsAny<string>(), It.IsAny<ReportStatusUpdateModel>()))
                                        .ReturnsAsync(apiResponse);

            // act and assert
            _ = Assert.ThrowsAsync<Exception>(async () => await this.pdfReportService.UpdateReportStatusAsync(reportStatusUpdateModel));

            // Assert
            this.mockLearningHubApiFacade.Verify(x => x.PostAsync<ApiResponse, ReportStatusUpdateModel>(It.IsAny<string>(), It.IsAny<ReportStatusUpdateModel>()), Times.Once());
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

        private void SetupSettings()
        {
            var fixture = new Fixture();
            this.mockSettings.Setup(r => r.Value)
                .Returns(new Settings
                {
                    AzureBlobStorageSettings = new AzureBlobStorageSettings(),
                    AzureServiceBusSettings = new AzureServiceBusSettings
                    {
                        PdfReportCreateTopicName = "TopicName",
                    },
                });
        }

        /// <summary>
        /// Mock Report Request Create Object.
        /// </summary>
        /// <returns>.</returns>
        private RequestModel GetTestDataRequestModel()
        {
            var request = new LearningHub.Nhs.Models.Report.ReportCreate.RequestModel
            {
                ClientId = Guid.NewGuid(),
                UserId = 4,
                ReportCreateModel = new ReportCreateModel
                {
                    Name = "Report1",
                    ReportTypeId = 1,
                    ReportPages = new List<ReportPageModel>
                        {
                            new ReportPageModel
                            {
                                ReportOrientationModeId = 1,
                                Html = "<h1>Test1</h1>",
                            },
                            new ReportPageModel
                            {
                                ReportOrientationModeId = 1,
                                Html = "<h1>Test2</h1>",
                            },
                        },
                },
            };

            return request;
        }
    }
}