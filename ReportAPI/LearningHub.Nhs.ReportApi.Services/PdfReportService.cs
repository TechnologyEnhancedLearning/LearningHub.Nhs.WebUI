namespace LearningHub.Nhs.ReportApi.Services
{
    using System.IO;
    using System.Text;
    using AutoMapper;
    using Azure.Messaging.ServiceBus;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Reporting;
    using LearningHub.Nhs.Models.Enums.Report;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.ReportApi.Services;
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using LearningHub.Nhs.ReportApi.Shared.Configuration;
    using LearningHub.Nhs.ReportApi.Shared.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The PdfReportService.
    /// </summary>
    public class PdfReportService : BaseService<PdfReportService>, IPdfReportService
    {
        private readonly IAzureBlobStorageService azureBlobStorageService;
        private readonly IServiceBusMessageService serviceBusMessageService;
        private readonly Settings settings;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfReportService"/> class.
        /// </summary>
        /// <param name="azureBlobStorageService">The azure blob storage service.</param>
        /// <param name="learningHubApiFacade">The learningHubApiFacade.</param>
        /// <param name="serviceBusMessageService">The service bus service.</param>
        /// <param name="serviceBusClient">The service bus client.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        /// <param name="logger">The logger<see cref="ILogger{PdfReportService}"/>.</param>
        public PdfReportService(
            IAzureBlobStorageService azureBlobStorageService,
            IServiceBusMessageService serviceBusMessageService,
            ILearningHubApiFacade learningHubApiFacade,
            IOptions<Settings> settings,
            IMapper mapper,
            ILogger<PdfReportService> logger)
            : base(learningHubApiFacade, logger)
        {
            this.azureBlobStorageService = azureBlobStorageService;
            this.serviceBusMessageService = serviceBusMessageService;
            this.settings = settings.Value;
            this.mapper = mapper;
        }

        /// <summary>
        /// The create pdf.
        /// </summary>
        /// <param name="requestModel">
        /// The report request model.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResponseModel> CreatePdfReportAsync(RequestModel requestModel)
        {
            var request = "Report";
            var report = await this.LearningHubApiFacade.PostAsync<ReportModel, RequestModel>(request, requestModel);
            var response = this.mapper.Map<ResponseModel>(report);
            var topic = this.settings.AzureServiceBusSettings.PdfReportCreateTopicName;
            var jsonString = JsonConvert.SerializeObject(response);
            var messageId = $"pdfreport-{report.Id}";

            await this.serviceBusMessageService.SendMessageAsync(topic,  messageId, jsonString);

            return response;
        }

        /// <summary>
        /// The get pdf report status.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <param name="hash">
        /// hash.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ReportStatusModel> GetPdfReportStatusAsync(string fileName, string hash)
        {
            var request = $"Report/ReportStatus/{fileName}/{hash}";
            return await this.LearningHubApiFacade.GetAsync<ReportStatusModel>(request);
        }

        /// <summary>
        /// The get pdf report.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <param name="hash">
        /// hash.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ReportModel> GetPdfReportAsync(string fileName, string hash)
        {
            var request = $"Report/{fileName}/{hash}";
            return await this.LearningHubApiFacade.GetAsync<ReportModel>(request);
        }

        /// <summary>
        /// The get pdf report file.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <param name="hash">
        /// hash.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<BlobModel?> GetPdfReportFileAsync(string fileName, string hash)
        {
            BlobModel? blobModel = null;

            var request = $"Report/ReportExists/{fileName}/{hash}";
            var reportRecordExists = await this.LearningHubApiFacade.GetAsync<object>(request);

            if ((bool)reportRecordExists)
            {
                blobModel = await this.azureBlobStorageService.GetFile(fileName);
            }
            else
            {
               this.Logger.LogError($"Invalid file request,FileName: {fileName} Hash: {hash} was not found.");
            }

            return blobModel;
        }

        /// <summary>
        /// The update report status.
        /// </summary>
        /// <param name="reportStatusUpdateModel">
        /// The response status update model.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> UpdateReportStatusAsync(ReportStatusUpdateModel reportStatusUpdateModel)
        {
            var request = "Report/UpdateReportStatus";
            return await this.LearningHubApiFacade.PostAsync<ApiResponse, ReportStatusUpdateModel>(request, reportStatusUpdateModel);
        }
    }
}