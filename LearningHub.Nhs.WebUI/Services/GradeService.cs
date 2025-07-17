namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Services;
    using LearningHub.Nhs.WebUI.Shared.Models;
    /// <summary>
    /// Defines the <see cref="GradeService" />.
    /// </summary>
    public class GradeService : BaseService<GradeService>, IGradeService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="userApiHttpClient">User api http client.</param>
        /// <param name="logger">Logger.</param>
        public GradeService(
            ILearningHubHttpClient learningHubHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<GradeService> logger)
        : base(learningHubHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetGradesForJobRoleAsync.
        /// </summary>
        /// <param name="jobRoleId">The jobRoleId<see cref="int"/>.</param>
        /// <returns>The <see cref="T:Task{List{GenericListViewModel}}"/>.</returns>
        public async Task<List<GenericListViewModel>> GetGradesForJobRoleAsync(int jobRoleId)
        {
            List<GenericListViewModel> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"Grade/GetByJobRole/{jobRoleId}";
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
        /// The GetPagedGradesForJobRoleAsync.
        /// </summary>
        /// <param name="jobRoleId">The jobRoleId<see cref="int"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>The <see cref="T:Task{List{GenericListViewModel}}"/>.</returns>
        public async Task<Tuple<int, List<GenericListViewModel>>> GetPagedGradesForJobRoleAsync(int jobRoleId, int page, int pageSize)
        {
            Tuple<int, List<GenericListViewModel>> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();
            var request = $"Grade/GetPagedByJobRole/{jobRoleId}/{page}/{pageSize}";
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
