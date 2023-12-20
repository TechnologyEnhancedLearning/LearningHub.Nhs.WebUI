// <copyright file="IReportService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface.Report
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The ReportService interface.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Get by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeChildren">If the children entities should be loaded.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ReportModel> GetByIdAsync(int id, bool includeChildren);

        /// <summary>
        /// Create report.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <param name="clientId">client id.</param>
        /// <param name="reportCreateModel">report create model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateAsync(int userId, int clientId, ReportCreateModel reportCreateModel);

        /// <summary>
        /// Update report status.
        /// </summary>
        /// <param name="reportStatusUpdateModel">Report status update model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateReportStatusAsync(ReportStatusUpdateModel reportStatusUpdateModel);

        /// <summary>
        /// Get client model.
        /// </summary>
        /// <param name="clientId">client id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ClientModel> GetClientByClientIdAsync(Guid clientId);

        /// <summary>
        /// Get report exists by filename and hash.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> ReportExistsAsync(string fileName, string hash);

        /// <summary>
        /// Get report status by filename and hash.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ReportStatusModel> GetReportStatusAsync(string fileName, string hash);

        /// <summary>
        /// Get report by filename and hash.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <param name="includeChildren">Include children.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ReportModel> GetByFileDetailAsync(string fileName, string hash, bool includeChildren);
    }
}