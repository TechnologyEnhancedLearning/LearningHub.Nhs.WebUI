// <copyright file="UrlChecker.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Helpers
{
    using System;
    using System.Net;

    /// <summary>
    /// This class provides the ability to check whether a URL is live and returns a HTTP 200 response.
    /// </summary>
    public class UrlChecker
    {
        /// <summary>
        /// Checks if a URL exists or not (returns HTTP 200).
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <param name="requestTimeoutInSeconds">The timeout for each request sent.</param>
        /// <param name="error">Returns any error details if one occurs.</param>
        /// <returns>True if the URL exists, otherwise false.</returns>
        public bool DoesUrlExist(string url, int requestTimeoutInSeconds, out string error)
        {
            // Remove any trailing bookmark section from Url
            int idx = url.LastIndexOf('#');
            if (idx > -1)
            {
                url = url.Substring(0, idx);
            }

            // Try a HEAD request, but some web servers don't support it. Then try GET request.
            bool exists = this.CheckUrl(url, "HEAD", requestTimeoutInSeconds, out error) || this.CheckUrl(url, "GET", requestTimeoutInSeconds, out error);
            return exists;
        }

        private bool CheckUrl(string url, string httpMethod, int requestTimeoutInSeconds, out string error)
        {
            error = string.Empty;
            if (url.StartsWith("mailto") || url.StartsWith("data"))
            {
                return true;
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = httpMethod;
                request.Timeout = requestTimeoutInSeconds * 1000;

                // Some web servers return error code if useragent is null. We're using a realistic looking one, but anything non-null seems to fool them.
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36";

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    var statusCode = (response != null) ? (int)response.StatusCode : 0;
                    request.Abort();  // call ASAP to kill connection
                    response.Close();

                    if (response != null && statusCode >= 200 && statusCode <= 399)
                    {
                        return true;
                    }
                    else
                    {
                        if (response == null)
                        {
                            error = "HttpWebResponse was null.";
                        }
                        else
                        {
                            error = $"HttpWebResponse.StatusCode: {statusCode}";
                        }

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error = $"Exception: {ex.Message}.";
                return false;
            }
        }
    }
}
