// <copyright file="RegionService.cs" company="NHS England">
// Copyright (c) NHS England.
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
    /// Defines the <see cref="RegionService" />.
    /// </summary>
    public class RegionService : BaseService<RegionService>, IRegionService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="userApiHttpClient">User Api http client.</param>
        public RegionService(ILearningHubHttpClient learningHubHttpClient, ILogger<RegionService> logger, IUserApiHttpClient userApiHttpClient)
        : base(learningHubHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetAllAsync.
        /// </summary>
        /// <returns>The <see cref="T:Task{List{GenericListViewModel}}"/>.</returns>
        public async Task<List<GenericListViewModel>> GetAllAsync()
        {
            List<GenericListViewModel> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Region/GetAll";
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
        /// The GetAllPagedAsync.
        /// </summary>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>The <see cref="T:Task{List{GenericListViewModel}}"/>.</returns>
        public async Task<Tuple<int, List<GenericListViewModel>>> GetAllPagedAsync(int page, int pageSize)
        {
            Tuple<int, List<GenericListViewModel>> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Region/GetAllPaged/{page}/{pageSize}";
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

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Region}"/>.</returns>
        public async Task<GenericListViewModel> GetByIdAsync(int id)
        {
            GenericListViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Region/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<GenericListViewModel>(result);
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
