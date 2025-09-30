namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The MyLearningService.
    /// </summary>
    public class MyLearningService : BaseService<MyLearningService>, IMyLearningService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">The logger.</param>
        public MyLearningService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<MyLearningService> logger)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
        }

        /// <summary>
        /// Gets the activity records for the detailed activity tab of My Learning screen.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningDetailedViewModel> GetActivityDetailed(MyLearningRequestModel requestModel)
        {
            MyLearningDetailedViewModel viewModel = null;

            var json = JsonConvert.SerializeObject(requestModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"MyLearning/GetActivityDetailed";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewModel = JsonConvert.DeserializeObject<MyLearningDetailedViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewModel;
        }

        /// <summary>
        /// Gets the played segment data for the progress modal in My Learning screen.
        /// </summary>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<PlayedSegmentViewModel>> GetPlayedSegments(int resourceId, int majorVersion)
        {
            List<PlayedSegmentViewModel> viewModel = null;
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"MyLearning/GetPlayedSegments/{resourceId}/{majorVersion}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewModel = JsonConvert.DeserializeObject<List<PlayedSegmentViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewModel;
        }

        /// <summary>
        /// Gets the resource certificate details of a resource reference.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <param name="minorVersion">The minorVersion.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Tuple<int, MyLearningDetailedItemViewModel>> GetResourceCertificateDetails(int resourceReferenceId, int? majorVersion = 0, int? minorVersion = 0, int? userId = 0)
        {
            Tuple<int, MyLearningDetailedItemViewModel> viewModel = null;
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"MyLearning/GetResourceCertificateDetails/{resourceReferenceId}/{majorVersion}/{minorVersion}/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewModel = JsonConvert.DeserializeObject<Tuple<int, MyLearningDetailedItemViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewModel;
        }
    }
}
