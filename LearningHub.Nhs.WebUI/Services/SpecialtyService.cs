// <copyright file="SpecialtyService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

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
    /// Defines the <see cref="SpecialtyService" />.
    /// </summary>
    public class SpecialtyService : BaseService<SpecialtyService>, ISpecialtyService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialtyService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="userApiHttpClient">User api http client.</param>
        /// <param name="logger">.</param>
        public SpecialtyService(
            ILearningHubHttpClient learningHubHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<SpecialtyService> logger)
        : base(learningHubHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetSpecialtiesAsync.
        /// </summary>
        /// <returns>The <see cref="T:Task{List{GenericListViewModel}}"/>.</returns>
        public async Task<List<GenericListViewModel>> GetSpecialtiesAsync()
        {
            List<GenericListViewModel> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Specialty/GetAll";
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
        /// The GetPagedSpecialtiesAsync.
        /// </summary>
        /// <param name="filter">The filter<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>The <see cref="T:Task{List{GenericListViewModel}}"/>.</returns>
        public async Task<Tuple<int, List<GenericListViewModel>>> GetPagedSpecialtiesAsync(string filter, int page, int pageSize)
        {
            Tuple<int, List<GenericListViewModel>> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Specialty/GetAllPaged/{filter}/{page}/{pageSize}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<Tuple<int, List<GenericListViewModel>>>(result);
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
