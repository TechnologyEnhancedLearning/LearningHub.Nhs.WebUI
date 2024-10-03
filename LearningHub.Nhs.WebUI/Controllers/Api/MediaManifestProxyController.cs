namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
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
            httpRequest.Headers.Add("Access-Control-Request-Headers", "authorization");
            httpRequest.Headers.Add("Access-Control-Request-Method", "GET");

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

                        content = ReplaceUrisWithProxy(content, baseUrl);

                        // content = ReplaceMediaUriWithProxy(content, baseUrl);
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

                        // response.Content = new StringContent(newContent, Encoding.Default, "application/vnd.apple.mpegurl");
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

        private static string ReplaceUrisWithProxy(string playlistContent, string proxyUrl)
        {
            // Split the playlist content into lines
            var lines = playlistContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // Process each line to replace media or map URIs
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#EXT-X-MAP:URI=", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the URI from the current line for EXT-X-MAP
                    var existingUri = lines[i].Substring(lines[i].IndexOf('=') + 1).Trim('"');
                    var newUri = $"{proxyUrl}/{existingUri}";
                    lines[i] = lines[i].Replace(existingUri, newUri);
                }
                else if (lines[i].StartsWith("#EXTINF:", StringComparison.OrdinalIgnoreCase) && i + 1 < lines.Length)
                {
                    // Get the URI from the next line for EXTINF
                    var existingUri = lines[i + 1].Trim();
                    var newUri = $"{proxyUrl}/{existingUri}";
                    lines[i + 1] = newUri;
                }
            }

            // Join the modified lines back into a single string
            return string.Join("\r\n", lines);
        }

        private static string ReplaceMediaUriWithProxy(string playlistContent, string proxyUrl)
        {
            // Split the playlist content into lines
            var lines = playlistContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // Process each line to replace media URIs
            for (int i = 0; i < lines.Length; i++)
            {
                // Check if the line starts with #EXTINF
                if (lines[i].StartsWith("#EXTINF:", StringComparison.OrdinalIgnoreCase))
                {
                    // Check if there is a next line (media URI)
                    if (i + 1 < lines.Length)
                    {
                        // Get the media URI line
                        var mediaUriLine = lines[i + 1];

                        // Extract the existing URI (removing any quotes)
                        var existingUri = mediaUriLine.Trim();

                        // Create the new URI with the proxy URL
                        var newUri = $"{proxyUrl}/{existingUri}";

                        // Replace the media URI line with the new proxy URI
                        lines[i + 1] = newUri;
                    }
                }
            }

            // Join the modified lines back into a single string
            return string.Join("\r\n", lines);
        }

        private static string ReplaceMapUriWithProxy(string playlistContent, string proxyUrl)
        {
            // Split the content into lines
            var lines = playlistContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // Process each line and replace the relevant URIs
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#EXT-X-MAP:URI=", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the existing URI
                    var existingUri = lines[i].Substring(lines[i].IndexOf('=') + 1).Trim('"');

                    // Create the new URI with the proxy URL
                    // Assuming the proxy URL should be the base URL for the video segments
                    var newUri = $"{proxyUrl}/{existingUri}";

                    // Replace the line with the new URI
                    lines[i] = lines[i].Replace(existingUri, newUri);
                }
            }

            // Join the modified lines back together
            return string.Join("\r\n", lines);
        }

        private static string ReplaceMediaAndMapUrisWithProxy(string playlistContent, string proxyUrl)
        {
            // Split the content into lines
            var lines = playlistContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // Process each line and replace relevant URIs
            for (int i = 0; i < lines.Length; i++)
            {
                // Check for media URI line
                if (lines[i].StartsWith("#EXTINF:", StringComparison.OrdinalIgnoreCase) && i + 1 < lines.Length)
                {
                    // Replace the media URI
                    lines[i + 1] = $"{proxyUrl}/{lines[i + 1].Trim()}";
                }

                // Check for map URI line
                if (lines[i].StartsWith("#EXT-X-MAP:URI=", StringComparison.OrdinalIgnoreCase))
                {
                    // Replace the map URI
                    lines[i] = lines[i].Replace(lines[i].Substring(lines[i].IndexOf('=') + 1).Trim('"'), $"{proxyUrl}/");
                }
            }

            // Join the modified lines back together
            return string.Join("\r\n", lines);
        }
    }
}
