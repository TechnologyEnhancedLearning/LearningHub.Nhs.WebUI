// <copyright file="ILearningHubReportApiClient.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>
namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Report;

    /// <summary>
    /// ILearningHubReportApiClient.
    /// </summary>
    public interface ILearningHubReportApiClient
    {
        /// <summary>
        /// Call to generate Pdf.
        /// </summary>
        /// <param name="strHTML">strHTML.</param>
        /// <param name="userId">userId.</param>
        /// <returns>Api response.</returns>
        Task<ReportModel> PdfReport(string strHTML, int userId);

        /// <summary>
        /// Call to generate Pdf.
        /// </summary>
        /// <param name="lstHTML">lstHTML.</param>
        /// <param name="userId">userId.</param>
        /// <returns>Api response.</returns>
        Task<ReportModel> PdfReport(List<string> lstHTML, int userId);

        /// <summary>
        /// Call to check if report is ready.
        /// </summary>
        /// <param name="pdfReportResponse">pdf Report Response.</param>
        /// <returns>Return PDF report status.</returns>
        Task<ReportStatusModel> PdfReportStatus(ReportModel pdfReportResponse);

        /// <summary>
        /// Call to get file.
        /// </summary>
        /// <param name="pdfReportResponse">pdfReportResponse.</param>
        /// <returns>Api response.</returns>
        Task<byte[]> GetPdfReportFile(ReportModel pdfReportResponse);
    }
}
