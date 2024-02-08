namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="FileController" />.
    /// </summary>
    public class FileController : BaseController
    {
        private readonly IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="fileService">The fileService<see cref="IFileService"/>.</param>
        public FileController(IWebHostEnvironment hostingEnvironment, IFileService fileService)
        : base(hostingEnvironment)
        {
            this.fileService = fileService;
        }

        /// <summary>
        /// The DownloadFile.
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/>.</param>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("/file/download/{filePath}/{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFileAsync(string filePath, string fileName)
        {
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