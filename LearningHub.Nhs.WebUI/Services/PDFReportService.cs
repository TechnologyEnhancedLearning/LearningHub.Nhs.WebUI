// <copyright file="PDFReportService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The PDFReportService.
    /// </summary>
    public class PDFReportService : IPDFReportService
    {
        private readonly ILearningHubReportApiClient learningHubReportApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFReportService"/> class.
        /// </summary>
        /// <param name="learningHubReportApiClient">learningHubReportApiClient.</param>
        /// <param name="logger">logger.</param>
        public PDFReportService(
                ILearningHubReportApiClient learningHubReportApiClient,
                ILogger<PDFReportService> logger)
        {
            this.learningHubReportApiClient = learningHubReportApiClient;
        }

        /// <summary>
        /// Call to generate Pdf.
        /// </summary>
        /// <param name="strHTML">strHTML.</param>
        /// <param name="userId">userId.</param>
        /// <returns>Api response.</returns>
        public Task<ReportModel> PdfReport(string strHTML, int userId)
        {
            return this.learningHubReportApiClient.PdfReport(strHTML, userId);
        }

        /// <summary>
        /// Call to generate Pdf.
        /// </summary>
        /// <param name="lstHTML">lstHTML.</param>
        /// <param name="userId">userId.</param>
        /// <returns>Api response.</returns>
        public Task<ReportModel> PdfReport(List<string> lstHTML, int userId)
        {
            return this.learningHubReportApiClient.PdfReport(lstHTML, userId);
        }

        /// <summary>
        /// Call to check if report is ready.
        /// </summary>
        /// <param name="pdfReportResponse">pdf Report Response.</param>
        /// <returns>Return PDF report status.</returns>
        public Task<ReportStatusModel> PdfReportStatus(ReportModel pdfReportResponse)
        {
            return this.learningHubReportApiClient.PdfReportStatus(pdfReportResponse);
        }

        /// <summary>
        /// Call to get file.
        /// </summary>
        /// <param name="pdfReportResponse">pdfReportResponse.</param>
        /// <returns>Api response.</returns>
        public Task<byte[]> GetPdfReportFile(ReportModel pdfReportResponse)
        {
            return this.learningHubReportApiClient.GetPdfReportFile(pdfReportResponse);
        }
    }
}
