// <copyright file="ReportController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Report;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Report operations.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ApiControllerBase
    {
        /// <summary>
        /// The report service.
        /// </summary>
        private readonly IReportService reportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="reportService">
        /// The activity service.
        /// </param>
        /// <param name="logger">The logger.</param>
        public ReportController(
            IUserService userService,
            IReportService reportService,
            ILogger<ReportController> logger)
            : base(userService, logger)
        {
            this.reportService = reportService;
        }

        /// <summary>
        /// get report.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{fileName}/{hash}")]
        public async Task<IActionResult> Get(string fileName, string hash)
        {
            return this.Ok(await this.reportService.GetByFileDetailAsync(fileName, hash, true));
        }

        /// <summary>
        /// Get report exists.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("ReportExists/{fileName}/{hash}")]
        public async Task<IActionResult> GetReportExists(string fileName, string hash)
        {
            var exists = await this.reportService.ReportExistsAsync(fileName, hash);

            return this.Ok(exists);
        }

        /// <summary>
        /// Get report status.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="hash">hash.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("ReportStatus/{fileName}/{hash}")]
        public async Task<IActionResult> GetReportStatus(string fileName, string hash)
        {
            return this.Ok(await this.reportService.GetReportStatusAsync(fileName, hash));
        }

        /// <summary>
        /// Create a new report.
        /// </summary>
        /// <param name="requestModel">The report request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestModel requestModel)
        {
            var client = await this.reportService.GetClientByClientIdAsync(requestModel.ClientId);

            if (client == null)
            {
                this.BadRequest(new ApiResponse(false, new LearningHubValidationResult(false, "Client Id provided is invalid")));
            }

            var vr = await this.reportService.CreateAsync(requestModel.UserId, client.Id, requestModel.ReportCreateModel);

            if (vr.IsValid)
            {
                var response = await this.reportService.GetByIdAsync(vr.CreatedId.Value, true);
                return this.Ok(response);
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Update report status.
        /// </summary>
        /// <param name="reportStatusUpdateModel">The report.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateReportStatus")]
        public async Task<IActionResult> UpdateReportStatus([FromBody] ReportStatusUpdateModel reportStatusUpdateModel)
        {
            var vr = await this.reportService.UpdateReportStatusAsync(reportStatusUpdateModel);
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }
    }
}