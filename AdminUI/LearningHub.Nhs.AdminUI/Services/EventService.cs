namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="EventService" />.
    /// </summary>
    public class EventService : BaseService, IEventService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        public EventService(ILearningHubHttpClient learningHubHttpClient)
            : base(learningHubHttpClient)
        {
        }

        /// <inheritdoc />
        public async Task<int> Create(Event eventEntity)
        {
            int createId = 0;
            var json = JsonConvert.SerializeObject(eventEntity);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Event/Create";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                createId = apiResponse.ValidationResult.CreatedId ?? 0;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return createId;
        }
    }
}
