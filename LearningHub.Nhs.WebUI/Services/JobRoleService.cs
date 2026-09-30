namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Account;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using NuGet.Common;

    /// <summary>
    /// Defines the <see cref="JobRoleService" />.
    /// </summary>
    public class JobRoleService : BaseService<JobRoleService>, IJobRoleService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRoleService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="userApiHttpClient">User api http client.</param>
        /// <param name="logger">Logger.</param>
        public JobRoleService(
            ILearningHubHttpClient learningHubHttpClient,
            IOpenApiHttpClient openApiHttpClient,
            IUserApiHttpClient userApiHttpClient,
            ILogger<JobRoleService> logger)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{JobRoleBasicViewModel}"/>.</returns>
        public async Task<JobRoleBasicViewModel> GetByIdAsync(int id)
        {
            JobRoleBasicViewModel viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"JobRole/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<JobRoleBasicViewModel>(result);
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
        /// <returns>The <see cref="T:Task{List{JobRoleBasicViewModel}}"/>.</returns>
        public async Task<List<JobRoleBasicViewModel>> GetFilteredAsync(string filter)
        {
            List<JobRoleBasicViewModel> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"JobRole/GetFilteredWithStaffGroup/{filter}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<List<JobRoleBasicViewModel>>(result);
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
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <returns>The <see cref="T:Task{List{JobRoleBasicViewModel}}"/>.</returns>
        public async Task<Tuple<int, List<JobRoleBasicViewModel>>> GetPagedFilteredAsync(string filter, int page, int pageSize)
        {
            Tuple<int, List<JobRoleBasicViewModel>> viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"JobRole/GetPagedFilteredWithStaffGroup/{Uri.EscapeDataString(filter.EncodeParameter())}/{page}/{pageSize}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<Tuple<int, List<JobRoleBasicViewModel>>>(result);
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
        /// The ValidateMedicalCouncilNumber.
        /// </summary>
        /// <param name="lastName">The lastName.</param>
        /// <param name="medicalCouncilId">The medicalCouncilId<see cref="int"/>.</param>
        /// <param name="medicalCouncilNumber">The medicalCouncilNumber.</param>
        /// <returns>The <see cref="T:Task{string}"/>.</returns>
        public async Task<string> ValidateMedicalCouncilNumber(string lastName, int medicalCouncilId, string medicalCouncilNumber)
        {
            string viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = string.Empty;
            switch (medicalCouncilId)
            {
                case 1:
                    request = $"MedicalCouncil/ValidateGMCNumber/{lastName}/{medicalCouncilNumber}";
                    break;
                case 2:
                    request = $"MedicalCouncil/ValidateNMCNumber/{medicalCouncilNumber}";
                    break;
                case 3:
                    request = $"MedicalCouncil/ValidateGDCNumber/{lastName}/{medicalCouncilNumber}";
                    break;
                default:
                    break;
            }

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                viewmodel = JsonConvert.DeserializeObject<string>(result);
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
