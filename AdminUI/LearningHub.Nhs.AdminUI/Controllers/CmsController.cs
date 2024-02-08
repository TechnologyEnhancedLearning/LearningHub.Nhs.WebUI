namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="CmsController" />.
    /// </summary>
    public class CmsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CmsController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        public CmsController(IWebHostEnvironment hostingEnvironment)
            : base(hostingEnvironment)
        {
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Edit Content Page.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("/cms/page/{*path}")]
        public IActionResult PageDetail()
        {
            return this.View();
        }
    }
}