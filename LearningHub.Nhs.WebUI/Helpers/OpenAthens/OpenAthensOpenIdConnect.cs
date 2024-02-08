namespace LearningHub.Nhs.WebUI.Helpers.OpenAthens
{
    using System;
    using System.Web;

    using LearningHub.Nhs.Models.OpenAthens;
    using LearningHub.Nhs.WebUI.Configuration;

    using Newtonsoft.Json;

    /// <summary>
    /// The open athens open id connect.
    /// </summary>
    internal static class OpenAthensOpenIdConnect
    {
        /// <summary>
        /// The deserialise auth payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>The <see cref="OpenAthensAuthServerPayload"/>.</returns>
        internal static OpenAthensAuthServerPayload DeserialiseAuthPayload(string payload)
        {
            var internalPayload = HttpUtility.UrlDecode(payload);
            var rtn = JsonConvert.DeserializeObject<OpenAthensAuthServerPayload>(
                internalPayload,
                new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    });

            return rtn;
        }

        /// <summary>
        /// The get auth server uri.
        /// </summary>
        /// <param name="authConfig">The auth config.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>The <see cref="Uri"/>.</returns>
        internal static Uri GetAuthServerUri(LearningHubAuthServiceConfig authConfig, Settings settings, string returnUrl)
        {
            var url = authConfig.Authority + @"/openathens/login";
            var uri = new Uri(url);
            var clientId = authConfig.OpenAthens.ClientId;
            var origin = settings.LearningHubWebUiUrl;

            var queryCollection = HttpUtility.ParseQueryString(uri.Query);

            queryCollection.Remove("clientId");
            queryCollection.Remove("origin");
            queryCollection.Add("clientId", clientId);
            queryCollection.Add("origin", origin);
            queryCollection.Add("returnUrl", returnUrl);

            var ub = new UriBuilder(uri) { Query = queryCollection.ToString() };

            return ub.Uri;
        }
    }
}
