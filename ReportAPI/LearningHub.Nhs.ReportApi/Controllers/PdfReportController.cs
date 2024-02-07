// <copyright file="PdfReportController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="PdfReportController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PdfReportController : ControllerBase
    {
        /// <summary>
        /// Defines the bookmarkService.
        /// </summary>
        private readonly IPdfReportService pdfReportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfReportController"/> class.
        /// </summary>
        /// <param name="pdfReportService">The pdfReportService<see cref="IPdfReportService"/>.</param>
        public PdfReportController(IPdfReportService pdfReportService)
        {
            this.pdfReportService = pdfReportService;
        }

        /// <summary>
        /// get report.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <param name="hash">
        /// hash.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet("{fileName}/{hash}")]
        public async Task<IActionResult> Get(string fileName, string hash)
        {
            return this.Ok(await this.pdfReportService.GetPdfReportAsync(fileName, hash));
        }

        /// <summary>
        /// Create a new report.
        /// </summary>
        /// <param name="requestModel">
        /// The report request model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestModel requestModel)
        {
            var response = await this.pdfReportService.CreatePdfReportAsync(requestModel);

            return this.Ok(response);
        }

        /// <summary>
        /// Get report status record by id, filename and hash.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <param name="hash">
        /// hash.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("PdfReportStatus/{fileName}/{hash}")]
        public async Task<IActionResult> PdfReportStatus(string fileName, string hash)
        {
            var report = await this.pdfReportService.GetPdfReportStatusAsync(fileName, hash);

            return this.Ok(report);
        }

        /// <summary>
        /// Get report filename and hash.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <param name="hash">
        /// hash.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("PdfReportFile/{fileName}/{hash}")]
        public async Task<IActionResult> PdfReportFile(string fileName, string hash)
        {
            var blobModel = await this.pdfReportService.GetPdfReportFileAsync(fileName, hash);

            if (blobModel != null)
            {
                return this.File(blobModel.Content, blobModel.ContentType);
            }
            else
            {
                return this.Ok(this.Content($"Invalid request or file '{fileName}' not available."));
            }
        }

        /// <summary>
        /// Update report status.
        /// </summary>
        /// <param name="reportStatusUpdateModel">
        /// The report.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [Route("UpdateReportStatus")]
        public async Task<IActionResult> UpdateReportStatus([FromBody] ReportStatusUpdateModel reportStatusUpdateModel)
        {
           return this.Ok(await this.pdfReportService.UpdateReportStatusAsync(reportStatusUpdateModel));
        }
    }
}
