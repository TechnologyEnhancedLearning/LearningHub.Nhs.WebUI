// <copyright file="ExternalSystemService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.External;
    using LearningHub.Nhs.Models.Paging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ExternalSystemService" />.
    /// </summary>
    public class ExternalSystemService : BaseService, IExternalSystemService
    {
        private readonly IUserApiHttpClient userApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalSystemService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="userApiHttpClient">The userApiHttpClient <see cref="IUserApiHttpClient"/>.</param>
        public ExternalSystemService(ILearningHubHttpClient learningHubHttpClient, IUserApiHttpClient userApiHttpClient)
        : base(learningHubHttpClient)
        {
            this.userApiHttpClient = userApiHttpClient;
        }

        /// <summary>
        /// The GetIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ExternalSystem}"/>.</returns>
        public async Task<ExternalSystem> GetIdAsync(int id)
        {
            ExternalSystem viewmodel = null;

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ExternalSystem/GetExternalSystemById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ExternalSystem>(result);
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
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{ExternalSystem}"/>.</returns>
        public async Task<PagedResultSet<ExternalSystem>> GetExternalSystems(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<ExternalSystem> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ExternalSystem/GetFilteredPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{Uri.EscapeUriString(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<ExternalSystem>>(result);
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
        /// Deletes a externalSystem.
        /// </summary>
        /// <param name="id">The externalSystem id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int id)
        {
            var client = await this.userApiHttpClient.GetClientAsync();

            var response = await client.DeleteAsync($"ExternalSystem/delete/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="externalSystem">The externalSystem<see cref="ExternalSystem"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<string> Create(ExternalSystem externalSystem)
        {
            int createId = 0;
            var json = JsonConvert.SerializeObject(externalSystem);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ExternalSystem/PostAsync";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            var result = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                if (!apiResponse.Success)
                {
                    return "save failed!";
                }

                createId = apiResponse.ValidationResult.CreatedId ?? 0;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return "Access Denied";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return JsonConvert.DeserializeObject<ApiResponse>(result).ValidationResult.Details[0];
            }

            return createId.ToString();
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="externalSystem">The externalSystem<see cref="ExternalSystem"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<string> Edit(ExternalSystem externalSystem)
        {
            externalSystem.CreateUser = null;
            externalSystem.AmendUser = null;

            var json = JsonConvert.SerializeObject(externalSystem);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.userApiHttpClient.GetClientAsync();

            var request = $"ExternalSystem/PutAsync";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            var result = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                if (!apiResponse.Success)
                {
                    return "Save failed!";
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return "Access Denied";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return JsonConvert.DeserializeObject<ApiResponse>(result).ValidationResult.Details[0];
            }

            return null;
        }
    }
}
