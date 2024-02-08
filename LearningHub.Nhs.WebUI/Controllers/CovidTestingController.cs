namespace LearningHub.Nhs.WebUI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Defines the <see cref="CovidTestingController" />.
    /// </summary>
    public class CovidTestingController : Controller
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CovidTestingController"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public CovidTestingController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("self-swab")]
        public IActionResult Index()
        {
            this.ViewBag.Video1_locatorUri = this.configuration["AssetDetails:Video1_locatorUri"];
            this.ViewBag.Video2_locatorUri = this.configuration["AssetDetails:Video2_locatorUri"];
            this.ViewBag.ShowHomeLink = this.configuration["AssetDetails:ShowHomeLink"];
            this.ViewBag.AutoPlayMedia = this.configuration["AssetDetails:AutoPlayMedia"];

            return this.View();
        }

        /// <summary>
        /// The Redirect.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("selfswab")]
        public IActionResult Redirect()
        {
            return this.Redirect("self-swab");
        }
    }
}
