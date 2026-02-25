using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Helpers
{
    /// <summary>
    /// The DatabricksOAuth.
    /// </summary>
    /// <remarks>
    /// DatabricksOAuthTokenService.
    /// </remarks>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="workspaceUrl"></param>
    public class DatabricksOAuthTokenService(string clientId, string clientSecret, string workspaceUrl)
    {
        private readonly string clientId = clientId;
        private readonly string clientSecret = clientSecret;
        private readonly string workspaceUrl = workspaceUrl.TrimEnd('/');

        private string? cachedToken;
        private DateTime tokenExpiresAt;

        /// <summary>
        /// The GetToken.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessTokenAsync()
        {
            if (cachedToken != null && DateTime.UtcNow < tokenExpiresAt)
                return cachedToken;

            using var http = new HttpClient();

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["scope"] = "all-apis"
            };

            var response = await http.PostAsync(
                $"{workspaceUrl}/oidc/v1/token",
                new FormUrlEncodedContent(form));

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(json);

            cachedToken = (string)obj.access_token;
            int expiresIn = (int)obj.expires_in;

            tokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 60);

            return cachedToken;
        }

    }

}
