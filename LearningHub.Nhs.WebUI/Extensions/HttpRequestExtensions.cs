// <copyright file="HttpRequestExtensions.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Extensions
{
    using System;
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;

    /// <summary>
    /// The HttpRequestExtensions.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Initializes static members of the <see cref="HttpRequestExtensions"/> class.
        /// The GetUserIPAddress.
        /// </summary>
        /// <param name="httpRequest">httpRequest.</param>
        /// <returns>User IP Address.</returns>
        public static string GetUserIPAddress(this HttpRequest httpRequest)
        {
            var ipAddress = httpRequest.Headers.FirstOrDefault(x => x.Key == "X-Forwarded-For").Value.ToString();

            if (ipAddress?.IndexOf(':') > 0)
            {
                ipAddress = IPAddress.Parse(ipAddress.Remove(ipAddress.IndexOf(':'))).ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = httpRequest.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = Convert.ToString(httpRequest.HttpContext.Connection.RemoteIpAddress);
            }

            return ipAddress;
        }
    }
}
