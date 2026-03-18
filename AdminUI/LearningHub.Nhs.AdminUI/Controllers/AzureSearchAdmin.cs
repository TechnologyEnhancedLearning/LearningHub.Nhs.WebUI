namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Helpers;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.FeatureManagement;

    /// <summary>
    /// Controller for Azure Search administration.
    /// </summary>
    public class AzureSearchAdminController : BaseController
    {
        private readonly IAzureSearchAdminService azureSearchAdminService;
        private readonly IFeatureManager featureManager;
        private readonly ILogger<AzureSearchAdminController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchAdminController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="azureSearchAdminService">The Azure Search admin service.</param>
        /// <param name="featureManager">The feature manager.</param>
        /// <param name="logger">The logger.</param>
        public AzureSearchAdminController(
            IWebHostEnvironment hostingEnvironment,
            IAzureSearchAdminService azureSearchAdminService,
            IFeatureManager featureManager,
            ILogger<AzureSearchAdminController> logger)
            : base(hostingEnvironment)
        {
            this.azureSearchAdminService = azureSearchAdminService;
            this.featureManager = featureManager;
            this.logger = logger;
        }

        /// <summary>
        /// Displays the Azure Search Admin dashboard.
        /// </summary>
        /// <returns>The view.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!await this.featureManager.IsEnabledAsync(FeatureFlags.AzureSearch))
            {
                return this.NotFound();
            }

            var viewModel = new AzureSearchAdminViewModel
            {
                Indexers = await this.azureSearchAdminService.GetIndexersStatusAsync(),
                Indexes = await this.azureSearchAdminService.GetIndexesStatusAsync(),
            };

            return this.View(viewModel);
        }

        /// <summary>
        /// Triggers an indexer run.
        /// </summary>
        /// <param name="indexerName">The name of the indexer to run.</param>
        /// <returns>Redirects to Index with status message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunIndexer(string indexerName)
        {
            if (!await this.featureManager.IsEnabledAsync(FeatureFlags.AzureSearch))
            {
                return this.NotFound();
            }

            if (string.IsNullOrEmpty(indexerName))
            {
                return this.BadRequest("Indexer name is required.");
            }

            var success = await this.azureSearchAdminService.RunIndexerAsync(indexerName);

            this.TempData["Message"] = success
                ? $"Indexer '{indexerName}' has been triggered successfully."
                : $"Failed to trigger indexer '{indexerName}'.";
            this.TempData["IsError"] = !success;

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Resets an indexer.
        /// </summary>
        /// <param name="indexerName">The name of the indexer to reset.</param>
        /// <returns>Redirects to Index with status message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetIndexer(string indexerName)
        {
            if (!await this.featureManager.IsEnabledAsync(FeatureFlags.AzureSearch))
            {
                return this.NotFound();
            }

            if (string.IsNullOrEmpty(indexerName))
            {
                return this.BadRequest("Indexer name is required.");
            }

            var success = await this.azureSearchAdminService.ResetIndexerAsync(indexerName);

            this.TempData["Message"] = success
                ? $"Indexer '{indexerName}' has been reset successfully. You may now run it to perform a full re-index."
                : $"Failed to reset indexer '{indexerName}'.";
            this.TempData["IsError"] = !success;

            return this.RedirectToAction("Index");
        }
    }
}