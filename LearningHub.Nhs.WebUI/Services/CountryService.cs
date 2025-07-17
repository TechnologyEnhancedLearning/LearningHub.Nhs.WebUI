namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Services;
    using LearningHub.Nhs.WebUI.Shared.Models;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="CountryService" />.
    /// </summary>
    public class CountryService : BaseService<CountryService>, ICountryService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="userApiHttpClient">User api http client.</param>
        /// <param name="logger">Logger.</param>
        public CountryService(
            ILearningHubHttpClient learningHubHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<CountryService> logger)
        : base(learningHubHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Country}"/>.</returns>
        public async Task<Country> GetByIdAsync(int id)
        {
            Country viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Country/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<Country>(result);
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
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="T:Task{List{GenericListViewModel}}"/>.</returns>
        public async Task<List<GenericListViewModel>> GetFilteredAsync(string filter)
        {
            List<GenericListViewModel> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Country/GetFiltered/{filter}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<GenericListViewModel>>(result);
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
        /// The GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<Country>> GetAllAsync()
        {
            List<Country> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Country/GetAll";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<Country>>(result);
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
