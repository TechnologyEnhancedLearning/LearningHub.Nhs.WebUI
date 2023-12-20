// <copyright file="IPdfReportService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.ReportApi.Shared.Models;

    /// <summary>
    /// The IPdfReportService interface.
    /// </summary>
    public interface IPdfReportService
    {
        /// <summary>
        /// The create pdf.
        /// </summary>
        /// <param name="requestModel">
        /// The report request model.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResponseModel> CreatePdfReportAsync(RequestModel requestModel);

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
        Task<ReportStatusModel> GetPdfReportStatusAsync(string fileName, string hash);

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
        Task<ReportModel> GetPdfReportAsync(string fileName, string hash);

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
        Task<BlobModel?> GetPdfReportFileAsync(string fileName, string hash);

        /// <summary>
        /// The update report status.
        /// </summary>
        /// <param name="reportStatusUpdateModel">
        /// The response status update model.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> UpdateReportStatusAsync(ReportStatusUpdateModel reportStatusUpdateModel);
    }
}