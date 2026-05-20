using LearningHub.Nhs.AdminUI.Interfaces;
using LearningHub.Nhs.Models.Common;
using LearningHub.Nhs.Models.Paging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using LearningHub.Nhs.Models.GovNotifyMessaging;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace LearningHub.Nhs.AdminUI.Services
{
    /// <summary>
    /// Defines the <see cref="GovNotifyDashboardService" />.
    /// </summary>
    public class GovNotifyDashboardService: BaseService, IGovNotifyDashboardService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GovNotifyDashboardService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient"></param>
        /// <param name="openApiHttpClient"></param>
        public GovNotifyDashboardService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient)
            :base(learningHubHttpClient, openApiHttpClient)
        {
        }

        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{MessageRequestViewModel}"/>.</returns>
        public async Task<PagedResultSet<MessageRequestViewModel>> GetPagedAsync(PagingRequestModel pagingRequestModel)
        {
            try
            {
                PagedResultSet<MessageRequestViewModel> viewmodel = null;
                var sortDirection = " ";

                if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
                {
                    pagingRequestModel.SortColumn = " ";
                }

                if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
                {
                    pagingRequestModel.SortDirection = " ";
                }
                else
                {
                    sortDirection = pagingRequestModel.SortDirection == "A" ? "ASC" :
                        pagingRequestModel.SortDirection == "D" ? "DESC" : "ASC";
                }

                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>
                        {
                            new StringEnumConverter()
                        }
                };

                var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter, settings);
                var client = await this.OpenApiHttpClient.GetClientAsync();

                var request = $"GovNotifyMessage/GetMessageRequests"
                    + $"/{pagingRequestModel.Page}"
                    + $"/{pagingRequestModel.PageSize}"
                    + $"/{pagingRequestModel.SortColumn}"
                    + $"/{sortDirection}"
                    +$"/{Uri.EscapeDataString(filter)}";

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<PagedResultSet<MessageRequestViewModel>>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                            ||
                         response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get message request details by id.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MessageRequestViewModel> GetMessageRequestById(int id)
        {
            try
            {
                MessageRequestViewModel viewmodel = null;
                var client = await this.OpenApiHttpClient.GetClientAsync();

                var request = $"GovNotifyMessage/GetMessageRequestById/{id}";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<MessageRequestViewModel>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                            ||
                         response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
