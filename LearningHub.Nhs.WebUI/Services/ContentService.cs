namespace LearningHub.Nhs.Shared.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Content;
    using LearningHub.Nhs.Shared.Interfaces;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ContentService" />.
    /// </summary>
    public class ContentService : BaseService<ContentService>, IContentService
    {
        private readonly IAzureMediaService azureMediaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="azureMediaService">azureMediaService.</param>
        public ContentService(ILearningHubHttpClient learningHubHttpClient, ILogger<ContentService> logger, IAzureMediaService azureMediaService)
        : base(learningHubHttpClient, logger)
        {
            this.azureMediaService = azureMediaService;
        }

        /// <summary>
        /// The GetPageByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="preview">The preview<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        public async Task<PageViewModel> GetPageByIdAsync(int id, bool preview)
        {
            PageViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var response = await client.GetAsync($"content/page/{id}?publishedOnly={!preview}&preview={preview}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PageViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetPageSectionDetailVideoAssetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PageSectionDetailViewModel> GetPageSectionDetailVideoAssetByIdAsync(int id)
        {
            PageSectionDetailViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var response = await client.GetAsync($"content/page-section-detail-video/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PageSectionDetailViewModel>(result);
                if (viewmodel?.VideoAsset?.AzureMediaAssetId > 0)
                {
                    viewmodel.VideoAsset.AzureMediaAsset.AuthenticationToken =
                        await this.azureMediaService.GetContentAuthenticationTokenAsync(viewmodel.VideoAsset.AzureMediaAsset.FilePath);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}