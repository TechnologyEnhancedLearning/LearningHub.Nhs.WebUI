namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="LocationService" />.
    /// </summary>
    public class LocationService : BaseService<LocationService>, ILocationService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="userApiHttpClient">User api http client.</param>
        /// <param name="logger">Logger.</param>
        public LocationService(
            ILearningHubHttpClient learningHubHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<LocationService> logger)
        : base(learningHubHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LocationBasicViewModel}"/>.</returns>
        public async Task<LocationBasicViewModel> GetByIdAsync(int id)
        {
            LocationBasicViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Location/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<LocationBasicViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetFilteredAsync.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The <see cref="T:Task{List{LocationBasicViewModel}}"/>.</returns>
        public async Task<List<LocationBasicViewModel>> GetFilteredAsync(string criteria)
        {
            List<LocationBasicViewModel> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Location/GetBySearchCriteria/{criteria}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<LocationBasicViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetPagedFilteredAsync.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>The <see cref="T:Task{List{LocationBasicViewModel}}"/>.</returns>
        public async Task<Tuple<int, List<LocationBasicViewModel>>> GetPagedFilteredAsync(string criteria, int page, int pageSize)
        {
            Tuple<int, List<LocationBasicViewModel>> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Location/GetPagedBySearchCriteria/{criteria}/{page}/{pageSize}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<Tuple<int, List<LocationBasicViewModel>>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
