namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Cache;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="MediaManifestProxyController" />.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MediaManifestProxyController : ControllerBase
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaManifestProxyController"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public MediaManifestProxyController(ILogger<MediaManifestProxyController> logger)
            : base()
        {
            this.logger = logger;
        }

        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="playBackUrl">The playBackUrl.</param>
        /// <param name="token">The token.</param>
        /// <returns>The MediaManifestProxy string.</returns>
        public string Get(string playBackUrl, string token)
        {
            this.logger.LogDebug($"playBackUrl={playBackUrl} token={token}");
            var httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(playBackUrl));
            httpRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            httpRequest.Timeout = 30000;

            var httpResponse = httpRequest.GetResponse();

            try
            {
                this.logger.LogDebug($"Calling httpResponse.GetResponseStream(): playBackUrl={playBackUrl} ");
                var stream = httpResponse.GetResponseStream();
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string qualityLevelRegex = @"(QualityLevels\(\d+\))";
                        qualityLevelRegex = @"(|)([^""\s]+\.m3u8\(encryption=cbc\))";
                        const string fragmentsRegex = @"(Fragments\([\w\d=-]+,[\w\d=-]+\))";
                        string urlRegex = @"("")(https?:\/\/[\da-z\.-]+\.[a-z\.]{2,6}[\/\w \.-]*\/?[\?&][^&=]+=[^&=#]*)("")";
                        urlRegex = @"(https?:\/\/[\da-z\.-]+\.[a-z\.]{2,6}[\/\w \.-]*)([\?&][^&=]+=[^&=#]*)?";
                        urlRegex = @"(https?:\/\/[\da-z\.-]+\.[a-z\.]{2,6}[\/\w \.-]*\?[^,\s""]*)";

                        var baseUrl = playBackUrl.Substring(0, playBackUrl.IndexOf(".ism", System.StringComparison.OrdinalIgnoreCase)) + ".ism";
                        this.logger.LogDebug($"baseUrl={baseUrl}");

                        var content = reader.ReadToEnd();

                        var newContent = Regex.Replace(content, urlRegex, match =>
                        {
                            string baseUrlWithQuery = match.Groups[1].Value;  // URL including the query string

                            // Append the token correctly without modifying surrounding characters
                            string newUrl = baseUrlWithQuery.Contains("?") ?
                                            $"{baseUrlWithQuery}&token={token}" :
                                            $"{baseUrlWithQuery}?token={token}";

                            return newUrl;
                        });

                        // var newContent = Regex.Replace(content, urlRegex, string.Format(CultureInfo.InvariantCulture, "$1$2$3&token={0}", token));
                        this.logger.LogDebug($"newContent={newContent}");

                        var match = Regex.Match(playBackUrl, qualityLevelRegex);
                        if (match.Success)
                        {
                            this.logger.LogDebug($"match.Success");
                            var qualityLevel = match.Groups[0].Value;
                            newContent = Regex.Replace(newContent, fragmentsRegex, m => string.Format(CultureInfo.InvariantCulture, baseUrl + "/" + qualityLevel + "/" + m.Value));
                            this.logger.LogDebug($"Updated newContent={newContent}");
                        }

                        return newContent;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
            }
            finally
            {
                httpResponse.Close();
            }

            return null;
        }
    }
}
