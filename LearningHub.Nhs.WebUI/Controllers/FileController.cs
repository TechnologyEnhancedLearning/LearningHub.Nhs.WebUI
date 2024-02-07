// <copyright file="FileController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="FileController" />.
    /// </summary>
    public class FileController : BaseController
    {
        private readonly IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="httpClientFactory">The httpClientFactory.</param>
        /// <param name="fileService">The fileService.</param>
        public FileController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<ResourceController> logger,
            IOptions<Settings> settings,
            IHttpClientFactory httpClientFactory,
            IFileService fileService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.fileService = fileService;
        }

        /// <summary>
        /// The DownloadFile.
        /// </summary>
        /// <param name="filePath">The filePath.</param>
        /// <param name="fileName">The fileName.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("/file/download/{filePath}/{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFileAsync(string filePath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName))
            {
                return this.BadRequest("Invalid file path");
            }

            var file = await this.fileService.DownloadFileAsync(filePath, fileName);
            if (file != null)
            {
                return this.File(file.Content, file.ContentType);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }
    }
}