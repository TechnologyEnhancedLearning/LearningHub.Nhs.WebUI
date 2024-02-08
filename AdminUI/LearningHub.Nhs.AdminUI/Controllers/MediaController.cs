namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Threading.Tasks;
    using Azure.Storage.Blobs.Models;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="MediaController" />.
    /// </summary>
    public class MediaController : BaseController
    {
        private IAzureMediaService azureMediaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="azureMediaService">Azure media services.</param>
        public MediaController(IWebHostEnvironment hostingEnvironment, IAzureMediaService azureMediaService)
            : base(hostingEnvironment)
        {
            this.azureMediaService = azureMediaService;
        }

        /// <summary>
        /// The MediaManifest.
        /// </summary>
        /// <param name="playBackUrl">The playBackUrl<see cref="string"/>.</param>
        /// <param name="token">The token<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Media/MediaManifest")]
        public IActionResult MediaManifest(string playBackUrl, string token)
        {
            var hostPortion = this.Request.Host;
            var manifestProxyUrl = string.Format("https://{0}/api/MediaManifestProxy", hostPortion);
            var modifiedTopLeveLManifest = this.azureMediaService.GetTopLevelManifestForToken(manifestProxyUrl, playBackUrl, token);

            var response = new ContentResult();
            response.Content = modifiedTopLeveLManifest;
            response.ContentType = @"application/vnd.apple.mpegurl";
            this.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            this.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            return response;
        }

        /// <summary>
        /// Downloads the original input media file for a video/audio resource.
        /// </summary>
        /// <param name="inputAssetName">The AMS input asset name for the video/audio resource, e.g. "input-c0fd93d9-6981".</param>
        /// <param name="fileName">The file name of the original input media file.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Media/DownloadInputAsset/{inputAssetName}/{fileName}")]
        public async Task<IActionResult> DownloadInputAsset(string inputAssetName, string fileName)
        {
            BlobDownloadResult result = await this.azureMediaService.DownloadMediaInputAsset(inputAssetName, fileName);

            if (result != null)
            {
                return this.File(result.Content.ToArray(), result.Details.ContentType);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }
    }
}
