// <copyright file="ReportService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Constants;
    using LearningHub.Nhs.Models.Entities.Reporting;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Enums.Report;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Report;
    using LearningHub.Nhs.Services.Helpers;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Report;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The report service.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IMapper mapper;
        private readonly Settings settings;
        private readonly IReportRepository reportRepository;
        private readonly IClientRepository clientRepository;
        private readonly ICachingService cachingService;
        private readonly ILogger<ReportService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportService"/> class.
        /// </summary>
        /// <param name="reportRepository">The report repository.</param>
        /// <param name="clientRepository">The client repository.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="mapper">The mapper.</param>
        public ReportService(
            IReportRepository reportRepository,
            IClientRepository clientRepository,
            ICachingService cachingService,
            ILogger<ReportService> logger,
            IOptions<Settings> settings,
            IMapper mapper)
        {
            this.reportRepository = reportRepository;
            this.clientRepository = clientRepository;
            this.cachingService = cachingService;
            this.settings = settings.Value;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Get by id.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="includeChildren">If the children entities should be loaded.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ReportModel> GetByIdAsync(int id, bool includeChildren)
        {
            var report = await this.reportRepository.GetByIdAsync(id, includeChildren);

            return this.mapper.Map<ReportModel>(report);
        }

        /// <summary>
        /// Create report.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <param name="clientId">client id.</param>
        /// <param name="reportCreateModel">report create model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAsync(int userId, int clientId, ReportCreateModel reportCreateModel)
        {
            var report = this.mapper.Map<Report>(reportCreateModel);

            report.ClientId = clientId;
            report.Hash = HashGeneratorHelper.SHA256ToString(System.Guid.NewGuid().ToString());
            report.ReportStatusId = (int)Status.Pending;
            report.FileName = this.GetUniqueFileNameByReportType(report.ReportTypeId);

            var retVal = await this.ValidateAsync(report);

            if (retVal.IsValid)
            {
                retVal.CreatedId = await this.reportRepository.CreateAsync(userId, report);
            }

            return retVal;
        }

        /// <summary>
        /// The update report status async.
        /// </summary>
        /// <param name="reportStatusUpdateModel">The report status update model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateReportStatusAsync(ReportStatusUpdateModel reportStatusUpdateModel)
        {
            var report = await this.reportRepository.GetByIdAsync(reportStatusUpdateModel.ReportId, false);

            if (report == null)
            {
                return new LearningHubValidationResult(false, $"No report found for the given report id: {reportStatusUpdateModel.ReportId}");
            }

            try
            {
                report.ReportStatusId = reportStatusUpdateModel.StatusId;
                await this.reportRepository.UpdateAsync(report.CreateUserId, report);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new LearningHubValidationResult(false, $"Error on updating report status for the report id: {reportStatusUpdateModel.ReportId}");
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Get client.
        /// </summary>
        /// <param name="clientId">client id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ClientModel> GetClientByClientIdAsync(Guid clientId)
        {
            string cacheKey = CacheKeys.ReportClients;
            List<ClientModel> reportClientModelList;
            var retVal = await this.cachingService.GetAsync<List<ClientModel>>(cacheKey);

            if (retVal.ResponseEnum == CacheReadResponseEnum.Found)
            {
                reportClientModelList = retVal.Item;
            }
            else
            {
                var clients = await this.clientRepository.GetAll().ToListAsync();
                reportClientModelList = this.mapper.Map<List<ClientModel>>(clients);
                await this.cachingService.SetAsync(cacheKey, reportClientModelList);
            }

            return reportClientModelList.Where(x => x.ClientId == clientId).SingleOrDefault();
        }

        /// <summary>
        /// Get report exists by filename and hash.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> ReportExistsAsync(string fileName, string hash)
        {
            return await this.reportRepository.GetByFileDetailAsync(fileName, hash) != null;
        }

        /// <summary>
        /// Get report status by filename and hash.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ReportStatusModel> GetReportStatusAsync(string fileName, string hash)
        {
            var report = await this.reportRepository.GetByFileDetailAsync(fileName, hash);
            return this.mapper.Map<ReportStatusModel>(report);
        }

        /// <summary>
        /// Get report file detail by filename and hash.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <param name="includeChildren">Include children.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ReportModel> GetByFileDetailAsync(string fileName, string hash, bool includeChildren)
        {
            var report = await this.reportRepository.GetByFileDetailAsync(fileName, hash, includeChildren);
            return this.mapper.Map<ReportModel>(report);
        }

        /// <summary>
        /// Get unique file name.
        /// </summary>
        /// <param name="reportTypeId">report type.</param>
        /// <returns>filename.</returns>
        private string GetUniqueFileNameByReportType(int reportTypeId)
        {
            return $"{Enum.GetName(typeof(Models.Enums.Report.ReportType), reportTypeId)}_{DateTime.Now.Ticks}.pdf";
        }

        /// <summary>
        /// Validate report.
        /// </summary>
        /// <param name="report">Report.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<LearningHubValidationResult> ValidateAsync(Report report)
        {
            var reportValidator = new ReportValidator();
            var clientValidationResult = await reportValidator.ValidateAsync(report);

            var retVal = new LearningHubValidationResult(clientValidationResult);

            return retVal;
        }
    }
}
