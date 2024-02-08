namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Maintenance;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The InternalSystemService.
    /// </summary>
    public class InternalSystemService : BaseService<InternalSystemService>, IInternalSystemService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSystemService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="logger">Logger.</param>
        public InternalSystemService(ILearningHubHttpClient learningHubHttpClient, ILogger<InternalSystemService> logger)
        : base(learningHubHttpClient, logger)
        {
        }

        /// <summary>
        /// Gets InternalSystem by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task<InternalSystemViewModel> GetByIdAsync(int id)
        {
            InternalSystemViewModel internalSystemViewModel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"InternalSystem/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                internalSystemViewModel = JsonConvert.DeserializeObject<InternalSystemViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return internalSystemViewModel;
        }
    }
}
