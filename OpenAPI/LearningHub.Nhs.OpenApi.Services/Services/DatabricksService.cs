using LearningHub.Nhs.Models.Bookmark;
using LearningHub.Nhs.Models.Entities.Reporting;
using LearningHub.Nhs.OpenApi.Models.Configuration;
using LearningHub.Nhs.OpenApi.Services.HttpClients;
using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
using LearningHub.Nhs.OpenApi.Services.Interface.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Services
{
    /// <summary>
    /// DatabricksService
    /// </summary>
    public class DatabricksService : IDatabricksService
    {
        private readonly IOptions<DatabricksConfig> databricksConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabricksService"/> class.
        /// </summary>
        /// <param name="databricksConfig">databricksConfig.</param>
        public DatabricksService(IOptions<DatabricksConfig> databricksConfig)
        {
            this.databricksConfig = databricksConfig;
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserReporter(int userId)
        {
            DatabricksApiHttpClient databricksInstance = new DatabricksApiHttpClient(this.databricksConfig);

            var sqlText = $"CALL {this.databricksConfig.Value.UserPermissionEndpoint}({userId});";
            const string requestUrl = "/api/2.0/sql/statements";

            var requestPayload = new
            {
                warehouse_id = this.databricksConfig.Value.WarehouseId,
                statement = sqlText,
                wait_timeout = "60s",
                on_wait_timeout = "CANCEL"
            };

            var jsonBody = JsonConvert.SerializeObject(requestPayload);
            using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await databricksInstance.GetClient().PostAsync(requestUrl, content);

            var databricksResponse = await databricksInstance.GetClient().PostAsync(requestUrl, content);
            if (databricksResponse.StatusCode is not HttpStatusCode.OK)
            {
                //log failure
                return false;
            }
            var responseResult = await databricksResponse.Content.ReadAsStringAsync();

            responseResult = responseResult.Trim();

            return JsonConvert.DeserializeObject<bool>(responseResult);
        }
    }

}
